using PaymentAPI.Infrastructure;
using System.Text.Json;
using PaymentAPI.Domain.Primitives;
using PaymentAPI.Providers.Interfaces;
using PaymentAPI.Application.Refunds;
using PaymentAPI.Application.Payments;

namespace PaymentAPI.Application.Webhook
{
    public class WebhookHandler
    {
        private readonly ApplicationDbContext _db;
        private readonly PaymentRepository _paymentRepository;
        private readonly RefundRepository _refundRepository;

        private readonly Dictionary<string, IPaymentWebhookHandler> _paymentWebhookHandlers;
        private readonly Dictionary<string, IRefundWebhookHandler> _refundWebhookHandlers;
        private readonly Dictionary<string, IWebhookClassifier> _webhookClassifier;

        public WebhookHandler (ApplicationDbContext db, IEnumerable<IPaymentWebhookHandler> paymentGateways,
            IEnumerable<IRefundWebhookHandler> refundGateways, IEnumerable<IWebhookClassifier> classifiers
            )
        {
            _db = db;
            _paymentWebhookHandlers = paymentGateways.ToDictionary(g => g.ProviderName, StringComparer.OrdinalIgnoreCase);
            _refundWebhookHandlers = refundGateways.ToDictionary(g => g.ProviderName, StringComparer.OrdinalIgnoreCase);
            _webhookClassifier = classifiers.ToDictionary(c => c.ProviderName, StringComparer.OrdinalIgnoreCase);
            _paymentRepository = new PaymentRepository(db);
            _refundRepository = new RefundRepository(db);
        }

        public async Task HandleWebhokAsync(string provider, JsonElement webhookBody)
        {
            if(!_webhookClassifier.TryGetValue(provider, out var webhookClassifier))
                throw new NotSupportedException($"Провайдер {provider} не поддерживается");

            var  webhookType = webhookClassifier.GetWebhookType(webhookBody);

            switch(webhookType)
            {
                case WebhookType.Payment:
                    await HandlePaymentAsync(provider, webhookBody);
                    break;
                case WebhookType.Refund:
                    await HandleRefundAsync(provider, webhookBody);
                    break;
                default:
                    throw new NotSupportedException($"Тип уведомления {webhookType} не поддерживается");
            };
            
        }

        private async Task HandlePaymentAsync(string provider, JsonElement webhookBody)
        {
            if (!_paymentWebhookHandlers.TryGetValue(provider, out var paymentWebhookHanlder))
                throw new NotSupportedException($"Обработка уведомлений об оплате для провайдера {provider} не поддерживается");

            var gatewayResponse = await paymentWebhookHanlder.HandlePaymentWebhookAsync(webhookBody);


            var payment = await _paymentRepository.GetPaymentByExternalIdAsync(gatewayResponse.ExternalPaymentId)
                ?? throw new InvalidOperationException($"Платёж {gatewayResponse.ExternalPaymentId} не найден в БД");

            payment.ApplyGatewayResult(gatewayResponse.Status);
            

            await _db.SaveChangesAsync();
        }

        private async Task HandleRefundAsync(string provider, JsonElement webhookBody)
        {
            if (!_refundWebhookHandlers.TryGetValue(provider, out var refundWebhookHandler))
                throw new NotSupportedException($"Обработка уведомлений о возврате для провайдера {provider} не поддерживается");

            var gatewayResponse = await refundWebhookHandler.HandleRefundWebhookAsync(webhookBody);

            var refund = await _refundRepository.GetRefundByExternalId(gatewayResponse.ExternalRefundId)
                ?? throw new InvalidOperationException($"Возврат {gatewayResponse.ExternalRefundId} не найден в БД");

            if (refund.Status == gatewayResponse.Status || refund.Status != RefundStatus.Pending)
                return;

            refund.ApplyGatewayResult(gatewayResponse.ExternalRefundId, gatewayResponse.Status);

            await _db.SaveChangesAsync();
        }
    }
}
