using PaymentAPI.DTO;
using PaymentAPI.Infrastructure;
using PaymentAPI.Interfaces;
using PaymentAPI.Models;
using PaymentAPI.Primitives;
using Microsoft.EntityFrameworkCore;

namespace PaymentAPI.Services
{
    public class RefundHandler
    {
        private readonly ApplicationDbContext _db;
        private readonly Dictionary<string, IRefundGateway> _refundGateways;
        private readonly RefundValidator _refundValidator;

        public RefundHandler(ApplicationDbContext db, IEnumerable<IRefundGateway> refundGateways, RefundValidator refundValidator)
        {
            _db = db;
            _refundGateways = refundGateways.ToDictionary(g => g.ProviderName, StringComparer.OrdinalIgnoreCase);
            _refundValidator = refundValidator;
        }

        public async Task<RefundResponse> CreateRefundAsync(RefundCreateRequest request, UserId userId, string provider, string idempotenceKey)
        {
            if (!_refundGateways.TryGetValue(provider, out var refundGateway))
                throw new NotSupportedException($"Провайдер {provider} не поддерживается");

            var payment = await _db.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == request.PaymentId && p.UserId == userId)
                ?? throw new InvalidOperationException($"Платёж {request.PaymentId} не найден");

            await _refundValidator.ValidateAsync(payment, request.Amount, userId);

            var gatewayResult = await refundGateway.CreateRefundAsync(
                payment.ExternalPaymentId!.ToString(), request.Amount, request.Currency, idempotenceKey);

            var refund = new Refund(request.PaymentId, request.Amount, request.Currency, request.Description);

            refund.ApplyGatewayResult(
                gatewayResult.ExternalRefundId,
                gatewayResult.Status,
                gatewayResult.CancellationParty,
                gatewayResult.CancellationReason);

            _db.Refunds.Add(refund);

            if (refund.Status == RefundStatus.Succeeded)
                await OnRefundSucceeded(refund);

            await _db.SaveChangesAsync();

            return new RefundResponse(
                refund.Id,
                refund.PaymentId,
                refund.Amount,
                refund.Currency,
                refund.Status.ToString(),
                refund.CreatedAt);
        }

        public async Task HandleRefundWebhookAsync(string externalRefundId, string status)
        {
            var refund = await _db.Refunds
                .FirstOrDefaultAsync(r => r.ExternalRefundId == externalRefundId)
                ?? throw new InvalidOperationException($"Возврат {externalRefundId} не найден");

            if (refund.Status == RefundStatus.Succeeded || refund.Status == RefundStatus.Canceled)
                return;

            if (status == "succeeded")
            {
                refund.SetSucceeded();
                await OnRefundSucceeded(refund);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<RefundResponse?> GetRefundAsync(RefundId refundId, UserId userId)
        {
            var refund = await _db.Refunds
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.Id == refundId && r.Payment.UserId == userId);

            if (refund is null)
                return null;

            return new RefundResponse(
                refund.Id,
                refund.PaymentId,
                refund.Amount,
                refund.Currency,
                refund.Status.ToString(),
                refund.CreatedAt);
        }

        public async Task<List<RefundResponse>> GetRefundsByPaymentAsync(PaymentId paymentId, UserId userId)
        {
            var payment = await _db.Payments
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId)
                ?? throw new InvalidOperationException($"Платёж {paymentId} не найден");

            var refunds = await _db.Refunds
                .Where(r => r.PaymentId == paymentId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return refunds.Select(r => new RefundResponse(
                r.Id,
                r.PaymentId,
                r.Amount,
                r.Currency,
                r.Status.ToString(),
                r.CreatedAt
            )).ToList();
        }

        public async Task OnRefundSucceeded(Refund refund)
        {
            var payment = await _db.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.OrderItems)
                .FirstAsync(p => p.Id == refund.PaymentId);

            var order = payment.Order;

            var totalRefunded = await _db.Refunds
                .Where(r => r.PaymentId == refund.PaymentId && r.Status == RefundStatus.Succeeded)
                .SumAsync(r => r.Amount);

            if (totalRefunded >= payment.Amount)
            {
                order.ChangeOrderStatus(OrderStatus.Refunded);
                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _db.Products.FindAsync(orderItem.ProductId);
                    if (product is not null)
                        product.AddToStock(orderItem.Quantity);
                }
            }
            else
            {
                order.ChangeOrderStatus(OrderStatus.PartiallyRefunded);
            }
        }
    }
}