using PaymentAPI.Domain.Payment;
using PaymentAPI.Domain.Primitives;
using PaymentAPI.Domain.Refund;
using PaymentAPI.DTO.payment;
using PaymentAPI.DTO.refund;
using PaymentAPI.Providers.Interfaces;
using PaymentAPI.Settings;
using System.Text.Json;

namespace PaymentAPI.Providers.Yookassa
{
    public class YookassaGateway : IPaymentGateway<PaymentCreateYookassaCommand>, IPaymentWebhookHandler,
        IRefundGateway<RefundCreateYookassaCommand>, IRefundWebhookHandler, IWebhookClassifier
    {
        public string ProviderName => "yookassa"; 
        private readonly YookassaPaymentGateway _yookassaPaymentGateway;
        private readonly YookassaPaymentWebhookHandler _yookassaPaymentWebhookHandler;
        private readonly YookassaRefundGateway _yookassaRefundGateway;
        private readonly YookassaRefundWebhookHandler _yookassaRefundWebhookHandler;
        private readonly YookassaWebhookClassifier _yookassaWebhookClassifier;
        public YookassaGateway(YookassaPaymentGateway yookassaPaymentGateway,
            YookassaPaymentWebhookHandler yookassaPaymentWebhookHandler,YookassaRefundGateway yookassaRefundGateway,
            YookassaRefundWebhookHandler yookassaRefundWebhookHandler, YookassaWebhookClassifier yookassaWebhookClassifier)
        {
            _yookassaPaymentGateway = yookassaPaymentGateway;
            _yookassaPaymentWebhookHandler = yookassaPaymentWebhookHandler;
            _yookassaRefundGateway = yookassaRefundGateway;
            _yookassaRefundWebhookHandler = yookassaRefundWebhookHandler;
            _yookassaWebhookClassifier = yookassaWebhookClassifier;
        }
        public Task<PaymentResult> CreatePaymentAsync(PaymentCreateYookassaCommand cmd, string idempotenceKey)
            => _yookassaPaymentGateway.CreatePaymentAsync(cmd, idempotenceKey);
        public Task<PaymentWebhookResult> HandlePaymentWebhookAsync(JsonElement webhookBody)
            => _yookassaPaymentWebhookHandler.HandlePaymentWebhookAsync(webhookBody);
        public Task<RefundResult> CreateRefundAsync(RefundCreateYookassaCommand cmd, string idempotenceKey)
            => _yookassaRefundGateway.CreateRefundAsync(cmd, idempotenceKey);
        public Task<RefundWebhookResult> HandleRefundWebhookAsync(JsonElement webhookBody)
            => _yookassaRefundWebhookHandler.HandleRefundWebhookAsync(webhookBody);
        public Task<RefundResult> GetRefundAsync(string refundId)
            => _yookassaRefundGateway.GetRefundAsync(refundId);
        public WebhookType GetWebhookType(JsonElement webhookBody)
            => _yookassaWebhookClassifier.GetWebhookType(webhookBody);
    }
}
