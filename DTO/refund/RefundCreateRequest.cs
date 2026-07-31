using PaymentAPI.Primitives;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentAPI.DTO.refund
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "ProviderName")]
    [JsonDerivedType(typeof(RefundCreateCommand),"yookassa")]
    public abstract record RefundCreateRequest
    {
        [Required]
        [Range(0, 1000000, ErrorMessage = "Amount must be between 0 and 1,000,000")]
        public required decimal Amount;

        [Required]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency must be 3 uppercase letters")]
        public required string Currency { get; init; }

        [Required]
        public ExternalPaymentId ExternalPaymentId;

        [Required]
        public required PaymentId PaymentId;

        [Required]
        public required OrderId OrderId;

        [StringLength(500)]
        public string? Description;

        abstract public RefundCreateCommand ToCommand();
    };
}