using MediatR;
using PaymentAPI.Primitives;
using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.Domain
{
    public class RefreshToken : Entity
    {
        public RefreshTokenId Id { get; private init; }
        public string TokenHash { get; private init; } = null!;
        public UserId UserId { get; private init; }
        public DateTime ExpireAt { get; private init; }
        public DateTime CreatedAt { get; private init; } 
        public DateTime? RevokedAt { get; private set; } = null;
        public RefreshTokenId? ReplacedByToken { get; private set; } 
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsExpire => ExpireAt < DateTime.UtcNow;
        public bool IsActive => !IsRevoked && !IsExpire;
        [Timestamp]
        public uint Xmin { get; private set; }
        public User User { get; private set; } = null! ;
        protected RefreshToken() {}

        public RefreshToken(string tokenHash, UserId userId, DateTime expireAt)
        {
            Id = RefreshTokenId.New();
            TokenHash = tokenHash;
            UserId = userId;
            ExpireAt = expireAt;
            CreatedAt = DateTime.UtcNow;
        }
        public void RotateTo(RefreshTokenId newToken)
        {
            ReplacedByToken = newToken;
            Revoke();
        }
        public void Revoke()
        {
            RevokedAt = DateTime.UtcNow;
        }
    }
}
