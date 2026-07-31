namespace PaymentAPI.Domain.Primitives
{
    public enum WebhookType
    {
        Payment = 0,
        Refund = 1,
        Payout = 2,
        Deal = 3
    }
}
