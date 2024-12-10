using Volo.Abp.Settings;

namespace Pusula.Training.HealthCare.Emails;

public class EmailSettingsProvider(ISettingEncryptionService encryptionService) : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
    }
}