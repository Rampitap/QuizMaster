using Identity.API.Contracts;
using Identity.API.Interfaces;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Serilog.Core;

namespace Identity.API.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<IdentityService> _logger;
    private readonly ITokenService _tokenService;
    private readonly IEmailSender<ApplicationUser> _sender;
    private readonly IUserContext _userContext;

    public IdentityService(UserManager<ApplicationUser> userManager, ILogger<IdentityService> logger, ITokenService tokenService, IEmailSender<ApplicationUser> sender, IUserContext userContext)
    {
        _userManager = userManager;
        _logger = logger;
        _tokenService = tokenService;
        _sender = sender;
        _userContext = userContext;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string origin)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return new AuthResponse(false, "User already exists");

        var user = new ApplicationUser 
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return new AuthResponse(false, string.Join(", ", result.Errors.Select(e => e.Description)));

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var callBackUrl = $"{origin}/api/auth/confirm-email?userId={user.Id}&code={Uri.EscapeDataString(code)}";

        await _sender.SendConfirmationLinkAsync(user, user.Email!, callBackUrl);

        return new AuthResponse(true, "Please confirm your email to activate acount");
    }
   
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return new AuthResponse(false, "Invalid email or password");

        if(!user.EmailConfirmed)
            return new AuthResponse(false, "Please confirm your email to activate acount");

        var token = _tokenService.CreateToken(user);
        var userDto = new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, user.PhoneNumber);

        return new AuthResponse(true, "Login successful", token, userDto);  
    }

    public async Task<AuthResponse> ConfirmEmailAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new AuthResponse(false, "User not found");

        var result = await _userManager.ConfirmEmailAsync(user, code);

        return result.Succeeded ? new AuthResponse(true, "Email confirmed") : new AuthResponse(false, "Failed" +
            "86");
    }

    public async Task<AuthResponse> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(_userContext.UserId.ToString());
        if (user == null)
            return new AuthResponse(false, "User not found");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded 
            ? new AuthResponse(true, "Profile updated") 
            : new AuthResponse(false, "Update failed");
    }

    public async Task<AuthResponse> DeleteProfileAsync()
    {
        var user = await _userManager.FindByIdAsync(_userContext.UserId.ToString());
        if (user == null)
            return new AuthResponse(false, "User not found");

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded 
            ? new AuthResponse(true, "Profile deleted") 
            : new AuthResponse(false, "Delete failed");
    }

    public async Task<AuthResponse> ChangePasswordasync(ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(_userContext.UserId.ToString());
        if (user == null)
            return new AuthResponse(false, "User not found");

        if (request.OldPassword == request.NewPassword)
            return new AuthResponse(false, "New password cannot be the same as old password");

        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponse(false, errors);
        }

        _logger.LogInformation("User {UserId} successfully changed their password.", user.Id);
        return new AuthResponse(true, "Password changed successfully");
    }
}
