using System.Security.Claims;

namespace ECommerceAPI.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var idValue =
            user.FindFirst("id")?.Value ??
            user.FindFirst("sub")?.Value ??
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(idValue) || !int.TryParse(idValue, out var userId))
            throw new UnauthorizedAccessException("Token inválido: no se encontró el id.");

        return userId;
    }

    public static int? TryGetUserId(this ClaimsPrincipal user)
    {
        var idValue =
            user.FindFirst("id")?.Value ??
            user.FindFirst("sub")?.Value ??
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(idValue) || !int.TryParse(idValue, out var userId))
            return null;

        return userId;
    }
}
