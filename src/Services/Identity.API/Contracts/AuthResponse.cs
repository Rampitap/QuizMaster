namespace Identity.API.Contracts;

public record AuthResponse
(

    bool Success,
    string Message,
    string? Token = null,
    UserDto? User = null
);
