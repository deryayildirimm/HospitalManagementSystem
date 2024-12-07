using System.Threading.Tasks;
using Pusula.Training.HealthCare.Patients;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace Pusula.Training.HealthCare.Blazor.Services;

public class PatientAdaptor(IPatientsAppService patientAppService) : DataAdaptor
{
    public override async Task<object> ReadAsync(
        DataManagerRequest dataManagerRequest,
        string? additionalParam = null)
    {
        var filter = dataManagerRequest?.Params?["Filter"] as GetPatientsInput
                     ?? new GetPatientsInput { MaxResultCount = dataManagerRequest!.Take };

        var result = await patientAppService.GetListAsync(filter);

        var dataResult = new DataResult()
        {
            Result = result.Items,
            Count = (int)result.TotalCount
        };
        return dataResult;
    }
}