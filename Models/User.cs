using Microsoft.AspNetCore.Identity;
using PaymentAPI.Primitives;

namespace PaymentAPI.Models
{
    public class User : IdentityUser<UserId>
    {
        public string? FullName { get; private init; }
        private readonly List<Order> _orders  = new();
        private readonly List<Payment> _payments = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
        protected User() { }
        public User( string email, string? fullName = null, string? phoneNumber = null)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(email, nameof(email));

            Id = UserId.New();
            FullName = fullName;
            UserName = email;
            Email = email;
            PhoneNumber = phoneNumber;
        }
        public void AddOrder(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);
            _orders.Add(order);
        }
    }
}
