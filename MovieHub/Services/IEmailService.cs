using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using MovieHub.Services.Abstractions;

namespace MovieHub.Services
{
    public class MailService : IEmailSender 
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("MovieHub", "moviehuberofficial@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            
            emailMessage.Body = new TextPart("plain") { Text = message };
            
            using var client = new SmtpClient();
         
            await client.ConnectAsync("smtp.gmail.com", 465, true);
            await client.AuthenticateAsync("moviehuberofficial@gmail.com", "testpass853504");
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}