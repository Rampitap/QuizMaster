using Identity.API.Contracts;

namespace Identity.API.Interfaces;

public interface IIdentityService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, string origin);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> ConfirmEmailAsync(string userId, string code);
    Task<AuthResponse> UpdateProfileAsync(UpdateProfileRequest request);
    Task<AuthResponse> DeleteProfileAsync();
    Task<AuthResponse> ChangePasswordasync(ChangePasswordRequest request);
}
