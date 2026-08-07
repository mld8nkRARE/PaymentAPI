using Microsoft.Extensions.Options;
using PaymentAPI.DTO.payment;
using System.Text.Json;
using Yandex.Checkout.V3;
using PaymentAPI.DTO;
using PaymentAPI.Primitives;
using PaymentAPI.Domain.Payments;
using PaymentAPI.Providers.Interfaces;

namespace PaymentAPI.Providers.Yookassa
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
                Capture = true,
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
                Status = payment.Status.ToString(),
                ExternalPaymentId = new ExternalPaymentId(payment.Id),
                Amount = payment.Amount.Value,
                Currency = payment.Amount.Currency,
                ConfirmationUrl = payment.Confirmation.ConfirmationUrl,
                CreatedAt = payment.CreatedAt
            };
        }

        public Task<PaymentResult> CreatePaymentAsync(PaymentCreateCommand cmd, string idempotenceKey)
        {
            if (cmd is not PaymentCreateYookassaCommand yookassaCmd)
                throw new ArgumentException("Команда должна быть типа PaymentCreateYookassaCommand", nameof(cmd));

            return CreatePaymentAsync(yookassaCmd, idempotenceKey);
        }
    }
}
