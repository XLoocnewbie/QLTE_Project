using Backend_QLTE.AuthService.Application.Interfaces.Services;
using Backend_QLTE.AuthService.Application.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Backend_QLTE.AuthService.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailOptions _emailOptions;

        public SmtpEmailService(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailOptions.Name, _emailOptions.Email));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();

            // ✅ Chọn kiểu bảo mật phù hợp
            var secureOption = _emailOptions.SSL
                ? SecureSocketOptions.SslOnConnect   // Port 465
                : SecureSocketOptions.StartTls;      // Port 587

            await client.ConnectAsync(_emailOptions.Host, _emailOptions.Port, secureOption);

            // ✅ Đảm bảo App Password đúng (không phải mật khẩu Gmail thật)
            await client.AuthenticateAsync(_emailOptions.Email, _emailOptions.Pass);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
