using System.Threading.Tasks;
using DeviceDetectorNET;
using Pusula.Training.HealthCare.AppointmentTypes;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace Pusula.Training.HealthCare.Blazor.Services;

public class AppointmentTypeAdaptor(IAppointmentTypesAppService appointmentTypesAppService) : DataAdaptor
{
    public override async Task<object> ReadAsync(
        DataManagerRequest dataManagerRequest,
        string? additionalParam = null)
    {
        var filter = new GetAppointmentTypesInput
        {
            MaxResultCount = dataManagerRequest.Take,
            SkipCount = dataManagerRequest.Skip,
            Sorting = dataManagerRequest.Sorted is { Count: > 0 }
                ? dataManagerRequest.Sorted[0].Name + " " + dataManagerRequest.Sorted[0].Direction
                : "Name ASC"
        };

        if (dataManagerRequest.Search != null && dataManagerRequest.Search.Count > 0)
        {
            filter.FilterText = dataManagerRequest.Search[0].Key;
        }
        
        var result = await appointmentTypesAppService.GetListAsync(filter);

        var dataResult = new DataResult()
        {
            Result = result.Items,
            Count = (int)result.TotalCount
        };
        return dataResult;
    }
}