using Identity.API.Interfaces;
using System.Security.Claims;

namespace Identity.API.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid UserId => Guid.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
        ? id
        : Guid.Empty;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
