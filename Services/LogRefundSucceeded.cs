using MediatR;
using PaymentAPI.DTO.refund;

namespace PaymentAPI.Services
{
    public class LogRefundSucceeded : INotificationHandler<RefundSucceededEvent>
    {
        private readonly ILogger<LogRefundSucceeded> _logger;
        public LogRefundSucceeded(ILogger<LogRefundSucceeded> logger) => _logger = logger;

        public Task Handle(RefundSucceededEvent notification, CancellationToken ct)
        {
            _logger.LogInformation(
                "Refund {RefundId} succeeded for Payment {PaymentId}, full refund: {IsFull}",
                notification.RefundId, notification.PaymentId, notification.IsFullRefund);
            return Task.CompletedTask;
        }
    }
}
