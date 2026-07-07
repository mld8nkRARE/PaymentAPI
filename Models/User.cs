using PaymentAPI.Primitives;

namespace PaymentAPI.Models
{
    public class User
    {
        public UserId UserId { get; private init; }
        public string FullName { get; private init; } = null!;
        public string Email { get; private set; } = null!;
        public string PhoneNumber { get; private set; } = null!;
        public string PasswordHash { get; private init; } = null!;

        private readonly List<Order> _orders  = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        protected User() { }
        public User(string fullName, string email, string phoneNumber, string passwordHash)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(fullName, nameof(fullName));
            ArgumentNullException.ThrowIfNullOrEmpty(email, nameof(email));
            ArgumentNullException.ThrowIfNullOrEmpty(phoneNumber, nameof(phoneNumber));
            ArgumentNullException.ThrowIfNullOrEmpty(passwordHash, nameof(passwordHash));

            UserId = new UserId();
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            PasswordHash = passwordHash;
        }
        public void AddOrder(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);
            _orders.Add(order);
        }
    }
}
