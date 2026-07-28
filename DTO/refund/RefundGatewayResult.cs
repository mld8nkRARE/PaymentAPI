namespace PaymentAPI.DTO.refund
{
    public record RefundResult(
        string ExternalRefundId,
        decimal Amount,
        string Currency,
        string Status,
        string? CancellationParty = null,
        string? CancellationReason = null);

}