namespace PaymentAPI.Primitives
{
    public enum OrderStatus
    {
        Pending = 0,
        WaitingForCapture = 1,
        Paid = 2,
        Cancelled = 3,
        PartiallyRefunded = 4,
        Refunded = 5
    }
}
