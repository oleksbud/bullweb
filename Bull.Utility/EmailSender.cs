using Microsoft.AspNetCore.Identity.UI.Services;

namespace Bull.Utility;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Fake implementation of e-mail sender
        return Task.CompletedTask;
    }
}