using PaymentAPI.DTO;
using PaymentAPI.Infrastructure;
using PaymentAPI.Models;
using PaymentAPI.Primitives;
using Microsoft.EntityFrameworkCore;

namespace PaymentAPI.Services
{
    public class WebhookHandler
    {
        private readonly ApplicationDbContext _db;

        public WebhookHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task HandleAsync(PaymentWebhookRequest request)
        {
            var externalId = request.PaymentObject.GetProperty("id").GetString()
                ?? throw new ArgumentException("Отсутствует id в webhook");

            if (!Guid.TryParse(externalId, out var externalGuid))
                throw new ArgumentException($"Некорректный формат id: {externalId}");

            var payment = await _db.Payments
                .FirstOrDefaultAsync(p => p.ExternalPaymentId != null
                    && p.ExternalPaymentId == new ExternalPaymentId(externalGuid))
                ?? throw new InvalidOperationException($"Платёж {externalId} не найден");

            var status = MapStatus(request.Event);
            payment.ChangeStatus(status);
            await _db.SaveChangesAsync();
        }

        private static PaymentStatus MapStatus(string webhookEvent) => webhookEvent switch
        {
            "payment.waiting_for_capture" => PaymentStatus.WaitingForCapture,
            "payment.succeeded" => PaymentStatus.Succeeded,
            "payment.canceled" => PaymentStatus.Cancelled,
            "payment.refunded" => PaymentStatus.Refunded,
            _ => throw new NotSupportedException($"Неизвестное событие: {webhookEvent}")
        };
    }
}
