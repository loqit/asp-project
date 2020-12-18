using System.Threading.Tasks;

namespace MovieHub.Services.Abstractions
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}