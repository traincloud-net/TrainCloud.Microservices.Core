using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace TrainCloud.Microservices.Core.Services.EMailSender;

public class EMailSenderService : AbstractService<EMailSenderService>, IEmailSender
{
    public EMailSenderService(IConfiguration configuration,
                              ILogger<EMailSenderService> logger)
        : base(configuration, logger)
    {

    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var configSection = Configuration.GetSection("Services").GetSection("EMailSenderService");
        string sender = configSection.GetValue<string>("Sender")!;
        string hostName = configSection.GetValue<string>("HostName")!;
        int port = configSection.GetValue<int>("Port");
        string userName = configSection.GetValue<string>("UserName")!;
        string password = configSection.GetValue<string>("Password")!;

        var client = new SmtpClient(hostName, port)
        {
            Credentials = new NetworkCredential(userName, password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage(userName, email, subject, htmlMessage)
        {
            From = new MailAddress(sender),
            IsBodyHtml = true
        };

        await client.SendMailAsync(mailMessage);
    }
}

