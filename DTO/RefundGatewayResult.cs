namespace PaymentAPI.DTO
{
    public record RefundResult(
        string ExternalRefundId,
        decimal Amount,
        string Currency,
        string Status,
        string? CancellationParty = null,
        string? CancellationReason = null);

    public record RefundWebhookResult(string ExternalRefundId, string Status);
}