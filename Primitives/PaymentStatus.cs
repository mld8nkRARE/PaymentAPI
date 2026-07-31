namespace PaymentAPI.Primitives
{
    public enum PaymentStatus
    {
        Pending = 0,
        WaitingForCapture = 1,
        Succeeded = 2,
        Canceled = 3,
        PartiallyRefunded = 4,
        Refunded = 5
    }
}
