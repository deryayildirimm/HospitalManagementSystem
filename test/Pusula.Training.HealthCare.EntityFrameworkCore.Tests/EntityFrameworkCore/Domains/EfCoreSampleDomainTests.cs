using Pusula.Training.HealthCare.Samples;
using Xunit;

namespace Pusula.Training.HealthCare.EntityFrameworkCore.Domains;

[Collection(HealthCareTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<HealthCareEntityFrameworkCoreTestModule>
{

}
