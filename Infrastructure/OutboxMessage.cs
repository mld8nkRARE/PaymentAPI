namespace PaymentAPI.Infrastructure
{
    public class OutboxMessage
    {
        public Guid Id { get; init; }
        public string Type { get; init; } = null!;
        public string Content { get; init; } = null!;
        public DateTime OccurredOn { get; init; }
        public DateTime? ProcessedOn { get; private set; }
        public string? Error { get; private set; }
        public int Attempts { get; private set; }

        public void Processed() => ProcessedOn = DateTime.UtcNow;
        public void SetError(string error) => Error = error;
        public void IncrementAttempts() => Attempts++;
    }
}
