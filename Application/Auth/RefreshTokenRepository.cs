using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain;
using PaymentAPI.Infrastructure;
using PaymentAPI.Primitives;
namespace PaymentAPI.Application.Auth
{
    public class RefreshTokenRepository
    {
        private readonly ApplicationDbContext _db;
        public RefreshTokenRepository(ApplicationDbContext db) => _db = db;
        public Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash)
            => _db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        public Task<List<RefreshToken>> GetActiveRefreshTokensByUserIdAsync(UserId userId)
            => _db.RefreshTokens.Where(t => t.UserId == userId && !t.RevokedAt.HasValue && t.ExpireAt > DateTime.UtcNow).ToListAsync();
        public void Add(RefreshToken token) => _db.RefreshTokens.Add(token);
    }
}
