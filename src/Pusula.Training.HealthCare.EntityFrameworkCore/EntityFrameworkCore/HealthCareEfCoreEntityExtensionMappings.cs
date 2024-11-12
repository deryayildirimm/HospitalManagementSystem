using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.Users;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Pusula.Training.HealthCare.EntityFrameworkCore;

public static class HealthCareEfCoreEntityExtensionMappings
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        HealthCareGlobalFeatureConfigurator.Configure();
        HealthCareModuleExtensionConfigurator.Configure();

        OneTimeRunner.Run(() =>
        {
            /* You can configure extra properties for the
             * entities defined in the modules used by your application.
             *
             * This class can be used to map these extra properties to table fields in the database.
             *
             * USE THIS CLASS ONLY TO CONFIGURE EF CORE RELATED MAPPING.
             * USE HealthCareModuleExtensionConfigurator CLASS (in the Domain.Shared project)
             * FOR A HIGH LEVEL API TO DEFINE EXTRA PROPERTIES TO ENTITIES OF THE USED MODULES
             *
             * Example: Map a property to a table field:
             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Customizing-Application-Modules-Extending-Entities
             */

            ObjectExtensionManager.Instance
                .MapEfCoreProperty<IdentityUser, string>(
                    UserConsts.IdPropertyName,
                    (_, propertyBuilder) =>
                    {
                        propertyBuilder.HasDefaultValue("");
                        propertyBuilder.HasMaxLength(UserConsts.MaxIdLength);
                        propertyBuilder.IsRequired(true);
                    }
                );
        });
    }
}