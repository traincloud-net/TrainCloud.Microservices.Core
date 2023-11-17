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
        await Task.Delay(0);
        string hostName = Configuration.GetSection("Services").GetSection("System").GetSection("EMailSenderService").GetValue<string>("HostName")!;
        int port = Configuration.GetSection("Services").GetSection("System").GetSection("EMailSenderService").GetValue<int>("Port");
        string userName = Configuration.GetSection("EMail").GetValue<string>("UserName")!;
        string password = Configuration.GetSection("EMail").GetValue<string>("Password")!;

        var client = new SmtpClient(hostName, port)
        {
            Credentials = new NetworkCredential(userName, password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage(userName, email, subject, htmlMessage)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(mailMessage);
    }
}

