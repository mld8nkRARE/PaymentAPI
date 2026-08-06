using PaymentAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Primitives;
using PaymentAPI.Providers.Interfaces;
using PaymentAPI.DTO.refund;
using PaymentAPI.Domain.Refunds;

namespace PaymentAPI.Application.Refunds
{
    public class RefundPollingService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RefundPollingService> _logger;
        private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(1);

        public RefundPollingService(IServiceScopeFactory scopeFactory, ILogger<RefundPollingService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RefundPollingService запущен");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingRefundsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка в RefundPollingService");
                }

                await Task.Delay(PollInterval, stoppingToken);
            }
        }

        private async Task ProcessPendingRefundsAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var repository = scope.ServiceProvider.GetRequiredService<RefundRepository>();
            var refundGateways = scope.ServiceProvider.GetRequiredService<Dictionary<string, IRefundStatusGateway>>();
            var refundHandler = scope.ServiceProvider.GetRequiredService<RefundHandler>();

            var pendingRefunds = await repository.GetPendingRefundsForPollingServiceAsync(batchSize: 50, cancellationToken);
            if (pendingRefunds.Count == 0)
                return;

            _logger.LogInformation("Найдено {Count} зависших возвратов в статусе Pending", pendingRefunds.Count);

            foreach (var refund in pendingRefunds)
            {
                await ProcessSingleRefundAsync(refund, refundGateways, cancellationToken);
            }

            await db.SaveChangesAsync(cancellationToken);
        }
        private async Task ProcessSingleRefundAsync(Refund refund, Dictionary<string, IRefundStatusGateway> refundGateways,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!refundGateways.TryGetValue(refund.ProviderName, out var gateway))
                {
                    _logger.LogWarning("Не удалось получить статус возврата {RefundId} для провайдера {Provider}",
                        refund.Id, refund.ProviderName);
                    return;
                }
                var result = await gateway.GetRefundAsync(refund.ExternalRefundId!.Value);
                refund.ApplyGatewayResult(result.ExternalRefundId, result.Status, result.CancellationParty, result.CancellationReason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке возврата {RefundId}", refund.Id);
            }
        }
    }
}
