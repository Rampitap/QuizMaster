using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Resend;
using Serilog.Core;

namespace Identity.API.Services;

public class ResendEmailSender : IEmailSender<ApplicationUser>
{
    private readonly IResend _resendProvider;
    private readonly ILogger<ResendEmailSender> _logger;

    public ResendEmailSender(IResend resendProvider, ILogger<ResendEmailSender> logger)
    {
        _resendProvider = resendProvider;
        _logger = logger;
    }

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        var message = new EmailMessage();

        message.From = "onborarding@resend.dev";
        message.To.Add(email);
        message.Subject = "Confirm your QuizMaster account"; 

        message.HtmlBody = $"""
            <div style="font-family: sans-serif; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;">
                <h2 style="color: #2563eb;">Welcome to QuizMaster, {user.FirstName}!</h2>
                <p>We're excited to have you on board. To start taking quizzes and earning certificates, please confirm your email address.</p>
                <div style="margin: 30px 0;">
                    <a href='{confirmationLink}' 
                       style="background-color: #2563eb; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;">
                       Confirm Email Address
                    </a>
                </div>
                <p style="font-size: 0.8em; color: #666;">If the button doesn't work, copy and paste this link into your browser:</p>
                <p style="font-size: 0.8em; color: #2563eb;">{confirmationLink}</p>
                <hr style="border: 0; border-top: 1px solid #eee; margin: 20px 0;">
                <p style="font-size: 0.8em; color: #999;">If you didn't create this account, you can safely ignore this email.</p>
            </div>
            """;

        try 
        {
            await _resendProvider.EmailSendAsync(message);
            _logger.LogInformation("Confirmation email successfuly sent to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to {Email}. Error: {Message}", email, ex.Message);
        }
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }
}
