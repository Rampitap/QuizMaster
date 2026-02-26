using Quiz.API.DataBase.Intefaces;
using System.Security.Claims;

namespace Quiz.API.DataBase.Repositories;

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
    public string UserEmail => User?.FindFirstValue(ClaimTypes.Email) ?? "unknown@test.com";
    public string FirstName => User?.FindFirstValue("firstName") ?? "Guest User";
    public string LastName => User?.FindFirstValue("lastName") ?? "Guest User";
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
