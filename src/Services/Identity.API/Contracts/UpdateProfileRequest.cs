namespace Identity.API.Contracts;

public record UpdateProfileRequest
(
    string FirstName,
    string LastName,
    string PhoneNumber
);
