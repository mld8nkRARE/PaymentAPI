using PaymentAPI.Primitives;
using System.Security.Claims;

namespace PaymentAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool TryGetUserId(this ClaimsPrincipal user, out UserId userId)
        {
            userId = default;
            var sub = user.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var id))
                return false;

            userId = new UserId(id);
            return true;
        }
    }
}
