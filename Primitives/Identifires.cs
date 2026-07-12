using StronglyTypedIds;
namespace PaymentAPI.Primitives
{
    [StronglyTypedId(Template.Guid, "guid-efcore")]
    public readonly partial struct UserId;
    [StronglyTypedId(Template.Guid, "guid-efcore")]
    public readonly partial struct PaymentId;
    [StronglyTypedId(Template.Guid, "guid-efcore")]
    public readonly partial struct ExternalPaymentId;
    [StronglyTypedId(Template.Guid, "guid-efcore")]
    public readonly partial struct OrderId;
    [StronglyTypedId(Template.Guid, "guid-efcore")]
    public readonly partial struct ProductId;
    [StronglyTypedId(Template.Guid, "guid-efcore")]
    public readonly partial struct OrderItemId;
}
