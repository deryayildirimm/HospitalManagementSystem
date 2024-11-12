using Volo.Abp.Settings;

namespace Pusula.Training.HealthCare.Emails;

public class EmailSettingsProvider : SettingDefinitionProvider
{
    private readonly ISettingEncryptionService encryptionService;

    public EmailSettingsProvider(ISettingEncryptionService encryptionService)
    {
        this.encryptionService = encryptionService;
    }

    public override void Define(ISettingDefinitionContext context)
    {
        var passSetting = context.GetOrNull("Abp.Mailing.Smtp.Password");
        if(passSetting!=null)
        {
            string? debug = encryptionService.Encrypt(passSetting,"1q2w3e$R");
        }
    }
}