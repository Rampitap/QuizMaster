namespace Quiz.API.DataBase.Intefaces;

public interface IUserContext
{
    Guid UserId { get; }
    string UserEmail { get; }
    string FirstName { get; }
    string LastName { get; }
    bool IsAuthenticated { get; }
}
