using Xunit;

namespace Pusula.Training.HealthCare.EntityFrameworkCore;

[CollectionDefinition(HealthCareTestConsts.CollectionDefinitionName)]
public class HealthCareEntityFrameworkCoreCollection : ICollectionFixture<HealthCareEntityFrameworkCoreFixture>
{

}
