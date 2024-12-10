using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.MultiTenancy;

namespace Pusula.Training.HealthCare.Emails;

public class NullMailSender(
    ICurrentTenant currentTenant,
    IEmailSenderConfiguration configuration,
    IBackgroundJobManager backgroundJobManager)
    : EmailSenderBase(currentTenant, configuration, backgroundJobManager)
{
    private new ILogger<NullEmailSender> Logger { get; } = NullLogger<NullEmailSender>.Instance;

    protected override Task SendEmailAsync(MailMessage mail)
    {
        Logger.LogWarning("Using Null Mail Service");
        Logger.LogDebug("Sent Mail....");
        LogMail(mail);
        return Task.CompletedTask;
    }

    private void LogMail(MailMessage mail)
    {
        Logger.LogDebug(mail.From?.ToString());
        Logger.LogDebug(mail.CC?.ToString());
        Logger.LogDebug(mail.To?.ToString());
    }
}