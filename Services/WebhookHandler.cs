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

        public WebhookHandler(ApplicationDbContext db, IEnumerable<IPaymentGateway> gateways)
        {
            _db = db;
            _gateways = gateways.ToDictionary(g => g.ProviderName, StringComparer.OrdinalIgnoreCase);
        }

        public async Task HandleAsync(string provider, JsonElement webhookBody)
        {
            if (!_gateways.TryGetValue(provider, out var gateway))
                throw new NotSupportedException($"Провайдер {provider} не поддерживается");

            var result = await gateway.HandleWebhookAsync(webhookBody);

            var payment = await _db.Payments
                .FirstOrDefaultAsync(p => p.ExternalPaymentId != null
                    && p.ExternalPaymentId == new ExternalPaymentId(result.ExternalPaymentId))
                ?? throw new InvalidOperationException($"Платёж {result.ExternalPaymentId} не найден в БД");

            if (payment.Status == result.Status)
                return;

            payment.ChangeStatus(result.Status);
            await _db.SaveChangesAsync();
        }
    }
}
