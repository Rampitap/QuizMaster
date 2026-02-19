using Identity.API.Contracts;
using Identity.API.Interfaces;
using Identity.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Conteollers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var origin = $"{Request.Scheme}://{Request.Host}";
        var result = await _identityService.RegisterAsync(request, origin); 

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code) 
    {
        Console.WriteLine($"==> Confirming email for User: {userId}");

        var result = await _identityService.ConfirmEmailAsync(userId, code);

        if (result.Success)
        {
            return Content("<h1>Email confirmed!</h1><p>You can now go back to Postman and Login.</p>", "text/html");
        }

        return BadRequest(result.Message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _identityService.LoginAsync(request);

        return result.Success ? Ok(result) : Unauthorized(result);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var result = await _identityService.UpdateProfileAsync(request);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request) 
    {
        var result = await _identityService.ChangePasswordasync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpDelete("account")]
    public async Task<IActionResult> DeleteAccount() 
    {
        var result = await _identityService.DeleteProfileAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

}
