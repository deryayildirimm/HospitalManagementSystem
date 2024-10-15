using Pusula.Training.HealthCare.Samples;
using Xunit;

namespace Pusula.Training.HealthCare.EntityFrameworkCore.Applications;

[Collection(HealthCareTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<HealthCareEntityFrameworkCoreTestModule>
{

}
