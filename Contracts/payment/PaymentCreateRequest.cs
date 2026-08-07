using PaymentAPI.Domain.Payments;
using PaymentAPI.Primitives;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentAPI.DTO.payment
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "ProviderName")]
    [JsonDerivedType(typeof(PaymentCreateYookassaRequest), typeDiscriminator: "yookassa")]
    public abstract record PaymentCreateRequest
    {
        [Required]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency must be 3 uppercase letters")]
        public required string Currency { get; init; }

        [Required]
        public required OrderId OrderId { get; init; }

        [StringLength(500)]
        public string? Description { get; init; }
        abstract public PaymentCreateCommand ToCommand(decimal amount);
    };
}