using System.Security.Claims;

namespace backend.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            if (user == null)
                return null;

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return null;

            if (int.TryParse(userIdClaim.Value, out int userId))
                return userId;

            return null;
        }

        public static string? GetUserRole(this ClaimsPrincipal user)
        {
            if (user == null)
                return null;

            return user.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
