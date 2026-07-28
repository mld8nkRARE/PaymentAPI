using PaymentAPI.DTO.payment;
using PaymentAPI.DTO.refund;
using PaymentAPI.Interfaces;
using PaymentAPI.Services;
using PaymentAPI.Settings;
using System.Text.Json;

namespace PaymentAPI.Gateways
{
    public class YookassaGateway : IPaymentGateway<PaymentCreateYookassaCommand>, IPaymentWebhookHandler, IRefundGateway,IRefundWebhookHandler
    {
        public string ProviderName => "yookassa"; 
        private readonly YookassaPaymentGateway _yookassaPaymentGateway;
        private readonly YookassaPaymentWebhookHandler _yookassaPaymentWebhookHandler;
        private readonly YookassaRefundGateway _yookassaRefundGateway;
        private readonly YookassaRefundWebhookHandler _yookassaRefundWebhookHandler;
        public YookassaGateway(YookassaPaymentGateway yookassaPaymentGateway, YookassaPaymentWebhookHandler yookassaPaymentWebhookHandler,
            YookassaRefundGateway yookassaRefundGateway, YookassaRefundWebhookHandler yookassaRefundWebhookHandler)
        {
            _yookassaPaymentGateway = yookassaPaymentGateway;
            _yookassaPaymentWebhookHandler = yookassaPaymentWebhookHandler;
            _yookassaRefundGateway = yookassaRefundGateway;
            _yookassaRefundWebhookHandler = yookassaRefundWebhookHandler;
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
    }
}
