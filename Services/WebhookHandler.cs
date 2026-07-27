using PaymentAPI.DTO;
using PaymentAPI.Infrastructure;
using PaymentAPI.Interfaces;
using PaymentAPI.Models;
using PaymentAPI.Primitives;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace PaymentAPI.Services
{
    public class WebhookHandler
    {
        private readonly ApplicationDbContext _db;
        private readonly Dictionary<string, IPaymentGateway> _gateways;
        private readonly Dictionary<string, IRefundGateway> _refundGateways;
        private readonly RefundHandler _refundHandler;

        public WebhookHandler(
            ApplicationDbContext db,
            IEnumerable<IPaymentGateway> gateways,
            IEnumerable<IRefundGateway> refundGateways,
            RefundHandler refundHandler)
        {
            _db = db;
            _gateways = gateways.ToDictionary(g => g.ProviderName, StringComparer.OrdinalIgnoreCase);
            _refundGateways = refundGateways.ToDictionary(g => g.ProviderName, StringComparer.OrdinalIgnoreCase);
            _refundHandler = refundHandler;
        }

        public async Task HandleAsync(string provider, JsonElement webhookBody)
        {
            if (webhookBody.TryGetProperty("event", out var eventElement))
            {
                var eventType = eventElement.GetString();
                if (eventType?.StartsWith("refund.") == true)
                {
                    await HandleRefundAsync(provider, webhookBody);
                    return;
                }
            }

            await HandlePaymentAsync(provider, webhookBody);
        }

        private async Task HandlePaymentAsync(string provider, JsonElement webhookBody)
        {
            if (!_gateways.TryGetValue(provider, out var gateway))
                throw new NotSupportedException($"Провайдер {provider} не поддерживается");

            var result = await gateway.HandleWebhookAsync(webhookBody);

            var payment = await _db.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.ExternalPaymentId != null
                    && p.ExternalPaymentId == new ExternalPaymentId(result.ExternalPaymentId))
                ?? throw new InvalidOperationException($"Платёж {result.ExternalPaymentId} не найден в БД");

            if (payment.Status == result.Status)
                return;

            payment.ChangeStatus(result.Status);

            if (result.Status == PaymentStatus.Succeeded)
                payment.Order.ChangeOrderStatus(OrderStatus.Paid);

            await _db.SaveChangesAsync();
        }

        private async Task HandleRefundAsync(string provider, JsonElement webhookBody)
        {
            if (!_refundGateways.TryGetValue(provider, out var refundGateway))
                throw new NotSupportedException($"Провайдер {provider} не поддерживает возвраты");

            var result = await refundGateway.HandleRefundWebhookAsync(webhookBody);
            await _refundHandler.HandleRefundWebhookAsync(result.ExternalRefundId, result.Status);
        }
    }
}
