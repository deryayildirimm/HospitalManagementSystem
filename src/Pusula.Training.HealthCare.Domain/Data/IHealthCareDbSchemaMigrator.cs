using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Data;

public interface IHealthCareDbSchemaMigrator
{
    Task MigrateAsync();
}
