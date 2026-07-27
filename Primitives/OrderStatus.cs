namespace PaymentAPI.Primitives
{
    public enum OrderStatus
    {
        Pending = 0,
        Paid = 1,
        Cancelled = 2,
        PartiallyRefunded = 3,
        Refunded = 4
    }
}
