namespace Identity.API.Interfaces;

public interface IUserContext
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}
