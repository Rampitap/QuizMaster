namespace Identity.API.Contracts;

public record UserDto
(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber
);
