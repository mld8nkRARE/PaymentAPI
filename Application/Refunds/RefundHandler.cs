using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.Payments;
using PaymentAPI.Domain.Refunds;
using PaymentAPI.DTO.refund;
using PaymentAPI.Infrastructure;
using PaymentAPI.Primitives;
using PaymentAPI.Providers.Interfaces;

namespace PaymentAPI.Application.Refunds
{
    public class RefundHandler
    {
        private readonly ApplicationDbContext _db;
        private readonly IServiceProvider _serviceProvider;
        private readonly PaymentRepository _paymentRepository;


        public RefundHandler(ApplicationDbContext db, IServiceProvider serviceProvider)
        {
            _db = db;
            _serviceProvider = serviceProvider;
            _paymentRepository = new PaymentRepository(_db);
        }

        public async Task<RefundResponse> CreateRefundAsync(RefundCreateRequest request,
            UserId userId, string idempotenceKey)
        {
            var payment = await _paymentRepository.GetPaymentByExternalIdAsync(request.ExternalPaymentId);
            var refund = payment.RequestRefund(request.Amount, request.Currency, request.Description, userId);

            var cmd = request.ToCommand();
            var refundGateway = ResolveGateway(cmd);
            
            RefundResult gatewayResult = await ((dynamic)refundGateway).CreateRefundAsync((dynamic)cmd, idempotenceKey);

            refund.ApplyGatewayResult(
                gatewayResult.ExternalRefundId,
                gatewayResult.Status,
                gatewayResult.CancellationParty,
                gatewayResult.CancellationReason);

            await _db.SaveChangesAsync();

            return new RefundResponse(
                refund.Id,
                refund.PaymentId,
                refund.Amount,
                refund.Currency,
                refund.Status.ToString(),
                refund.CreatedAt);
        }
        private object ResolveGateway(RefundCreateCommand cmd)
        {
            var RefundProviderName = typeof(IRefundGateway<>).MakeGenericType(cmd.GetType());
            var gateway = _serviceProvider.GetService(RefundProviderName);
            if (gateway is null)
                throw new NotSupportedException($"Провайдер для {cmd.GetType().Name} не поддерживается");
            return gateway;
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

       
    }
}