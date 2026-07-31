using PaymentAPI.Primitives;

namespace PaymentAPI.Models
{
    public class RefreshToken : Entity
    {
        public RefreshTokenId Id { get; private init; }
        public string Token { get; private init; } = null!;
        public UserId UserId { get; private init; }
        public DateTime ExpireAt { get; private init; }
        public DateTime CreatedAt { get; private init; } 
        public DateTime? RevokedAt { get; private set; } = null;
        public string? ReplacedByToken { get; private set; } 
        public bool IsActive => !RevokedAt.HasValue && ExpireAt > DateTime.UtcNow;
        public User User { get; private set; } = null! ;
        protected RefreshToken() {}

        public RefreshToken(string token, UserId userId, DateTime expireAt)
        {
            Id = RefreshTokenId.New();
            Token = token;
            UserId = userId;
            ExpireAt = expireAt;
            CreatedAt = DateTime.UtcNow;
        }
        public void ReplaceToken(string newToken)
        {
            ReplacedByToken = newToken;
        }
        public void Revoke()
        {
            RevokedAt = DateTime.UtcNow;
        }
    }
}
