using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Infrastructure;
using System.Text.Json;

namespace PaymentAPI.Application.Infrastructure
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxProcessor> _logger;
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);
        private const int BatchSize = 20;
        private const int MaxAttempts = 10;

        public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OutboxProcessor запущен");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOutboxMessagesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обработке Outbox сообщений");
                }

                await Task.Delay(PollInterval, stoppingToken);
            }
        }

        private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            var ids = (await db.OutboxMessages
                .FromSqlInterpolated($@"
                    SELECT * FROM outbox_messages
                    WHERE processed_on IS NULL AND attempts < {MaxAttempts}
                    ORDER BY occurred_on
                    LIMIT {BatchSize}
                    FOR UPDATE SKIP LOCKED")
                .ToListAsync(cancellationToken))
                .Select(m => m.Id)
                .ToList();

            if (ids.Count == 0)
            {
                await transaction.CommitAsync(cancellationToken);
                return;
            }

            foreach (var id in ids)
            {
                var savepoint = $"outbox_{id}";
                await transaction.CreateSavepointAsync(savepoint, cancellationToken);

                try
                {
                    var message = await db.OutboxMessages.SingleAsync(m => m.Id == id, cancellationToken);

                    var type = Type.GetType(message.Type)
                               ?? throw new InvalidOperationException($"Тип события {message.Type} не найден");

                    var domainEvent = JsonSerializer.Deserialize(message.Content, type)
                                      ?? throw new InvalidOperationException($"Ошибка десериализации {message.Type}");

                    await publisher.Publish(domainEvent, cancellationToken);
                    message.Processed();
                    await db.SaveChangesAsync(cancellationToken);

                    await transaction.ReleaseSavepointAsync(savepoint, cancellationToken);
                }
                catch (Exception ex)
                {
                    var attemptsAfterFailure = 0;

                    try
                    {
                        var failed = await db.OutboxMessages.SingleAsync(m => m.Id == id, cancellationToken);
                        attemptsAfterFailure = failed.Attempts + 1;
                    }
                    catch (Exception reloadEx)
                    {
                        _logger.LogWarning(reloadEx, "Не удалось перезагрузить сообщение {Id} для логирования попытки", id);
                    }

                    _logger.LogError(ex, "Ошибка обработки сообщения {Id}, попытка {Attempts}", id, attemptsAfterFailure);

                    await transaction.RollbackToSavepointAsync(savepoint, cancellationToken);
                    db.ChangeTracker.Clear();

                    var message = await db.OutboxMessages.SingleAsync(m => m.Id == id, cancellationToken);
                    message.IncrementAttempts();
                    message.SetError(ex.Message);

                    if (message.Attempts >= MaxAttempts)
                    {
                        _logger.LogError("Сообщение {Id} достигло максимума попыток ({MaxAttempts}) и отправлено в dead-letter", id, MaxAttempts);
                        message.Processed();
                    }

                    await db.SaveChangesAsync(cancellationToken);
                }
            }

            await transaction.CommitAsync(cancellationToken);
        }
    }
}