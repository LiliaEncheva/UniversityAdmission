using Microsoft.AspNetCore.Identity.UI.Services;


namespace UniversityAdmission.Services
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Не прави нищо - просто връща успешна задача
            return Task.CompletedTask;
        }
    }
}
