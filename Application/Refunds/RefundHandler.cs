using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.Payments;
using PaymentAPI.Domain.Refunds;
using PaymentAPI.DTO.refund;
using PaymentAPI.Infrastructure;
using PaymentAPI.Primitives;
using PaymentAPI.Providers.Interfaces;
using PaymentAPI.Domain;

namespace PaymentAPI.Application.Refunds
{
    public class RefundHandler
    {
        private readonly ApplicationDbContext _db;
        private readonly IEnumerable<IRefundGateway> _gateways;
        private readonly PaymentRepository _paymentRepository;


        public RefundHandler(ApplicationDbContext db, IEnumerable<IRefundGateway> gateways)
        {
            _db = db;
            _gateways = gateways;
            _paymentRepository = new PaymentRepository(_db);
        }

        public async Task<RefundResponse> CreateRefundAsync(RefundCreateRequest request,
            UserId userId, string idempotenceKey)
        {
            var cmd = request.ToCommand();
            
            var refundGateway = _gateways.FirstOrDefault(g => g.ProviderName.Equals(cmd.ProviderName, StringComparison.OrdinalIgnoreCase))
                ?? throw new NotSupportedException($"Провайдер {cmd.ProviderName} не поддерживается");

            var payment = await _paymentRepository.GetPaymentByExternalIdAsync(request.ExternalPaymentId)
                ?? throw new InvalidOperationException($"Платёж {request.ExternalPaymentId} не найден");

            // 1. Валидация перед вызовом внешнего шлюза
            payment.ValidateRefund(request.Amount, request.Currency, userId);

            // 2. Определяем позиции, возвращаемые на склад
            var refundItems = await ResolveRefundItemsAsync(payment.Order, request.Items);

            var refundItemsTotal = refundItems.Sum(i => i.UnitPrice * i.Quantity);
            if (refundItemsTotal != request.Amount)
                throw new InvalidOperationException(
                    $"Сумма позиций возврата ({refundItemsTotal}) не совпадает с суммой возврата ({request.Amount})");

            // 3. Запрос во внешний шлюз
            RefundResult gatewayResult = await refundGateway.CreateRefundAsync(cmd, idempotenceKey);

            // 4. Создание сущности в домене и привязка к БД только после успешного ответа шлюза
            var refund = payment.RequestRefund(request.Amount, request.Currency, request.Description, userId);

            foreach (var item in refundItems)
                refund.AddItem(item.Product, item.Quantity, item.UnitPrice);

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

        private async Task<List<(Product Product, int Quantity, decimal UnitPrice)>> ResolveRefundItemsAsync(Order order, List<RefundItemCreateRequest> requestedItems)
        {
            var orderItems = await _db.OrderItems
                .Where(i => i.OrderId == order.Id)
                .Include(i => i.Product)
                .ToListAsync();

            var result = new List<(Product, int, decimal)>();

            if (requestedItems is null || requestedItems.Count == 0)
            {
                foreach (var orderItem in orderItems)
                    result.Add((orderItem.Product, orderItem.Quantity, orderItem.UnitPrice));
                return result;
            }

            foreach (var requested in requestedItems)
            {
                var orderItem = orderItems.FirstOrDefault(i => i.ProductId == requested.ProductId)
                    ?? throw new InvalidOperationException($"Товар {requested.ProductId} отсутствует в заказе");

                if (requested.Quantity > orderItem.Quantity)
                    throw new InvalidOperationException(
                        $"Количество {requested.Quantity} превышает заказанное ({orderItem.Quantity}) для товара {requested.ProductId}");

                result.Add((orderItem.Product, requested.Quantity, orderItem.UnitPrice));
            }

            return result;
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