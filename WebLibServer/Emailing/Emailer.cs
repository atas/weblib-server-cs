using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace WebLibServer.Emailing;

public class Emailer<TTplModel>(EmailerCx emailerCx, ILogger<Emailer<TTplModel>> logger) : IDisposable
{
    private MailMessage _mailMessage;

    private string To { get; set; }

    public void Dispose()
    {
        _mailMessage?.Dispose();
    }

    /// <summary>
    ///     Sends an email to the specified email address
    ///     Does not support concurrent runs, create separate instances for separate concurrent
    /// </summary>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="templateModel"></param>
    public async Task Send(string to, string subject, TTplModel templateModel)
    {
        To = to;
        var emailBody = await emailerCx.ViewRenderService.RenderToStringAsync(GetEmailTemplate(), templateModel);

        logger.LogInformation("Sending email to {To} with subject {Subject}", to, subject);

        _mailMessage = new MailMessage
        {
            From = new MailAddress(emailerCx.Config.SiteEmail, emailerCx.Config.SiteName),
            To = { to },
            Body = emailBody,
            IsBodyHtml = true,
            Subject = subject
        };

        if (emailerCx.HasListUnsubscribeHeader)
            AddListUnsubscribeMailHeader();

        using (var client = GetClient())
        {
            client.Send(_mailMessage);
        }

        emailerCx.MetricsSvc.Collect("EmailSent", 1);
    }

    /// <summary>
    ///     Adds a List-Unsubscribe header to the standard endpoints: /api/email/unsubscribe/type(s)?email=...&secret=...
    /// </summary>
    protected virtual void AddListUnsubscribeMailHeader()
    {
        _mailMessage.Headers.Add("List-Unsubscribe",
            $"<{GetUnsubscribeUrl()}>");

        _mailMessage.Headers.Add("List-Unsubscribe-Post", "List-Unsubscribe=One-Click");
    }

    protected virtual string GetUnsubscribeUrl()
    {
        return $"https://{emailerCx.Config.Hostname}/api/email/unsubscribe/{emailerCx.EmailType}s?email={To}&secret={emailerCx.UserSecret}";
    }

    protected virtual string GetEmailTemplate()
    {
        return $"EmailTemplates/{emailerCx.EmailType}Email";
    }

    private SmtpClient GetClient()
    {
        return new SmtpClient("localhost", emailerCx.Config.SmtpPort)
        {
            EnableSsl = emailerCx.Config.SmtpEncryption,
            UseDefaultCredentials = true
        };
    }
}