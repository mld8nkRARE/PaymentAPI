﻿using StronglyTypedIds;
namespace PaymentAPI.Primitives
{
    [StronglyTypedId(Template.Guid, "guid-efcore")]
    public readonly partial struct UserId;
    [StronglyTypedId(Template.Guid, "guid-efcore")]
    public readonly partial struct PaymentId;
    [StronglyTypedId(Template.String, "string-efcore")]
    public readonly partial struct ExternalPaymentId;
    [StronglyTypedId(Template.Guid, "guid-efcore")]
    public readonly partial struct OrderId;
    [StronglyTypedId(Template.Guid, "guid-efcore")]
    public readonly partial struct ProductId;
    [StronglyTypedId(Template.Guid, "guid-efcore")]
    public readonly partial struct OrderItemId;
    [StronglyTypedId(Template.Guid, "guid-efcore")]
    public readonly partial struct ReceiptId;
    public readonly partial struct RefreshTokenId;
}
