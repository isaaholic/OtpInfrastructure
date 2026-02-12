using Microsoft.Extensions.Options;
using OtpModule.Abstractions;
using OtpModule.TestApi.Options;
using System.Net;
using System.Net.Mail;

namespace OtpModule.TestApi.Services;

public class EmailOtpSender(IOptions<EmailOptions> options) : IOtpSender
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendAsync(string key, string code)
    {
        var mail = new MailMessage
        {
            From = new MailAddress(_options.From),
            Subject = "OTP Verification",
            Body = $"Your OTP code is: {code}",
            IsBodyHtml = false
        };

        mail.To.Add(key);

        using var client = new SmtpClient(_options.SmtpHost, _options.Port)
        {
            Credentials = new NetworkCredential(
                _options.Username,
                _options.Password
            ),
            EnableSsl = true
        };

        await client.SendMailAsync(mail);
    }
}
