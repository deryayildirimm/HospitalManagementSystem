using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.MultiTenancy;

namespace Pusula.Training.HealthCare.Emails;

public class NullMailSender : EmailSenderBase
{
    public ILogger<NullEmailSender> Logger { get; }

    public NullMailSender(ICurrentTenant currentTenant, IEmailSenderConfiguration configuration,
        IBackgroundJobManager backgroundJobManager) : base(currentTenant, configuration, backgroundJobManager)
    {
        Logger = NullLogger<NullEmailSender>.Instance;
    }

    protected override Task SendEmailAsync(MailMessage mail)
    {
        Logger.LogWarning("USING null mail!!");
        Logger.LogDebug("Send Mail....");
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