using PaymentAPI.Domain.Payments;
using PaymentAPI.Domain.Primitives;
using PaymentAPI.Domain.Refunds;
using PaymentAPI.DTO.payment;
using PaymentAPI.DTO.refund;
using PaymentAPI.Primitives;
using PaymentAPI.Providers.Interfaces;
using System.Text.Json;

namespace PaymentAPI.Providers.Yookassa
{
    public class YookassaGateway : IPaymentGateway<PaymentCreateYookassaCommand>, IPaymentWebhookHandler,
        IRefundGateway<RefundCreateYookassaCommand>, IRefundWebhookHandler, IWebhookClassifier, IRefundStatusGateway
    {
        public string ProviderName => "yookassa"; 
        private readonly YookassaPaymentGateway _yookassaPaymentGateway;
        private readonly YookassaPaymentWebhookHandler _yookassaPaymentWebhookHandler;
        private readonly YookassaRefundGateway _yookassaRefundGateway;
        private readonly YookassaRefundWebhookHandler _yookassaRefundWebhookHandler;
        private readonly YookassaWebhookClassifier _yookassaWebhookClassifier;
        private readonly YookassaRefundStatusGateway _yookassaRefundStatusGateway;
        public YookassaGateway(YookassaPaymentGateway yookassaPaymentGateway,
            YookassaPaymentWebhookHandler yookassaPaymentWebhookHandler,YookassaRefundGateway yookassaRefundGateway,
            YookassaRefundWebhookHandler yookassaRefundWebhookHandler, YookassaWebhookClassifier yookassaWebhookClassifier,
            YookassaRefundStatusGateway yookassaRefundStatusGateway )
        {
            _yookassaPaymentGateway = yookassaPaymentGateway;
            _yookassaPaymentWebhookHandler = yookassaPaymentWebhookHandler;
            _yookassaRefundGateway = yookassaRefundGateway;
            _yookassaRefundWebhookHandler = yookassaRefundWebhookHandler;
            _yookassaWebhookClassifier = yookassaWebhookClassifier;
            _yookassaRefundStatusGateway = yookassaRefundStatusGateway;
        }
        public Task<PaymentResult> CreatePaymentAsync(PaymentCreateYookassaCommand cmd, string idempotenceKey)
            => _yookassaPaymentGateway.CreatePaymentAsync(cmd, idempotenceKey);
        public Task<PaymentWebhookResult> HandlePaymentWebhookAsync(JsonElement webhookBody)
            => _yookassaPaymentWebhookHandler.HandlePaymentWebhookAsync(webhookBody);
        public Task<RefundResult> CreateRefundAsync(RefundCreateYookassaCommand cmd, string idempotenceKey)
            => _yookassaRefundGateway.CreateRefundAsync(cmd, idempotenceKey);
        public Task<RefundWebhookResult> HandleRefundWebhookAsync(JsonElement webhookBody)
            => _yookassaRefundWebhookHandler.HandleRefundWebhookAsync(webhookBody);
        public Task<RefundResult> GetRefundAsync(ExternalRefundId refundId)
            => _yookassaRefundStatusGateway.GetRefundAsync(refundId);
        public WebhookType GetWebhookType(JsonElement webhookBody)
            => _yookassaWebhookClassifier.GetWebhookType(webhookBody);
    }
}
