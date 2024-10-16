using KoiShipping.API;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace KoiShipping.API
{
    public class EmailService : IEmailService
    {
        private readonly string _email; // Your email
        private readonly string _password; // Your email password

        public EmailService(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(_email, _password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_email),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(to);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}