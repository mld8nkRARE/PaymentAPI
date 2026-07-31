//using PaymentAPI.Infrastructure;
//using Microsoft.EntityFrameworkCore;
//using PaymentAPI.Domain.Primitives;
//using PaymentAPI.Providers.Interfaces;
//using PaymentAPI.DTO.refund;

//namespace PaymentAPI.Application.Refunds
//{
//    public class RefundPollingService : BackgroundService
//    {
//        private readonly IServiceProvider _serviceProvider;
//        private readonly ILogger<RefundPollingService> _logger;

//        public RefundPollingService(IServiceProvider serviceProvider, ILogger<RefundPollingService> logger)
//        {
//            _serviceProvider = serviceProvider;
//            _logger = logger;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            _logger.LogInformation("RefundPollingService запущен");

//            while (!stoppingToken.IsCancellationRequested)
//            {
//                try
//                {
//                    await ProcessPendingRefundsAsync(stoppingToken);
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Ошибка в RefundPollingService");
//                }

//                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
//            }
//        }

//        private async Task ProcessPendingRefundsAsync(CancellationToken cancellationToken)
//        {
//            using var scope = _serviceProvider.CreateScope();
//            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//            var refundGateways = scope.ServiceProvider.GetRequiredService<IEnumerable<IRefundGateway>>()
//                .ToDictionary(g => g.ProviderName, StringComparer.OrdinalIgnoreCase);
//            var refundHandler = scope.ServiceProvider.GetRequiredService<RefundHandler>();

//            var pendingRefunds = await db.Refunds
//                .Where(r => r.Status == RefundStatus.Pending
//                    && r.ExternalRefundId != null
//                    && r.CreatedAt < DateTime.UtcNow.AddMinutes(-1))
//                .ToListAsync(cancellationToken);

//            if (pendingRefunds.Count == 0)
//                return;

//            _logger.LogInformation("Найдено {Count} зависших возвратов в статусе Pending", pendingRefunds.Count);

//            foreach (var refund in pendingRefunds)
//            {
//                try
//                {
//                    RefundResult? result = null;
//                    foreach (var gateway in refundGateways.Values)
//                    {
//                        try
//                        {
//                            result = await gateway.GetRefundAsync(refund.ExternalRefundId!);
//                            break;
//                        }
//                        catch
//                        {

//                        }
//                    }

//                    if (result is null)
//                    {
//                        _logger.LogWarning("Не удалось получить статус возврата {RefundId} ни от одного gateway", refund.ExternalRefundId);
//                        continue;
//                    }

//                    if (result.Status == "succeeded")
//                    {
//                        refund.SetSucceeded();
//                        await refundHandler.OnRefundSucceeded(refund);
//                        _logger.LogInformation("Возврат {RefundId} переведён в Succeeded (polling)", refund.ExternalRefundId);
//                    }
//                    else if (result.Status == "canceled")
//                    {
//                        refund.SetCanceled(result.CancellationParty ?? "unknown", result.CancellationReason ?? "unknown");
//                        _logger.LogInformation("Возврат {RefundId} переведён в Canceled (polling), причина: {Reason}",
//                            refund.ExternalRefundId, result.CancellationReason);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Ошибка при обработке возврата {RefundId}", refund.ExternalRefundId);
//                }
//            }

//            await db.SaveChangesAsync(cancellationToken);
//        }
//    }
//}