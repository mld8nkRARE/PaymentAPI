namespace PaymentAPI.Primitives
{
    public enum PaymentStatus
    {
        Pending = 0,
        WaitingForCapture = 1,
        Succeeded = 2,
        Cancelled = 3,
        Refunded = 4
    }
}
