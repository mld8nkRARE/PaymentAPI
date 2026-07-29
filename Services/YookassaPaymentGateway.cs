using Microsoft.Extensions.Options;
using PaymentAPI.DTO.payment;
using PaymentAPI.Interfaces;
using PaymentAPI.Settings;
using System.Text.Json;
using Yandex.Checkout.V3;
using PaymentAPI.DTO;

namespace PaymentAPI.Services
{
    public class YookassaPaymentGateway : IPaymentGateway<PaymentCreateYookassaCommand>
    {
        public string ProviderName => "yookassa";
        private readonly YookassaSettings _yookassaSettings;
        private readonly AsyncClient _client;

        public YookassaPaymentGateway(IOptions<YookassaSettings> yookassaSettings, IHttpClientFactory httpClientFactory)
        {
            _yookassaSettings = yookassaSettings.Value;
            var client = new Client(yookassaSettings.Value.ShopId, yookassaSettings.Value.SecretKey);
            var httpClient = httpClientFactory.CreateClient();
            _client = new AsyncClient(httpClient, false, client);
        }

        public async Task<PaymentResult> CreatePaymentAsync(PaymentCreateYookassaCommand paymentData, string idempotenceKey)
        {
            var amount = paymentData.Amount;
            var currency = paymentData.Currency;
            var description = paymentData.Description;

            NewPayment newPayment = new NewPayment()
            {
                Amount = new Amount
                {
                    Value = amount,
                    Currency = currency
                },
                Description = description,
                Confirmation = new Confirmation
                {
                    Type = ConfirmationType.Redirect,
                    ReturnUrl = _yookassaSettings.ReturnUrl
                }
            };

            Yandex.Checkout.V3.Payment payment = await _client.CreatePaymentAsync(newPayment, idempotenceKey);
            var get = await _client.GetPaymentAsync(payment.Id);
            
            return new PaymentResult
            {
                PaymentId = string.Empty,
                Status = payment.Status.ToString(),
                ExternalPaymentId = payment.Id,
                Amount = payment.Amount.Value,
                Currency = payment.Amount.Currency,
                ConfirmationUrl = payment.Confirmation.ConfirmationUrl,
                CreatedAt = payment.CreatedAt
            };
        }

       
    }
}
