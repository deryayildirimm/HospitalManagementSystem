using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Appointments;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace Pusula.Training.HealthCare.Blazor.Services;

public class AppointmentAdaptor(IAppointmentAppService appointmentAppService) : DataAdaptor
{
    public override async Task<object> ReadAsync(
        DataManagerRequest dataManagerRequest,
        string? additionalParam = null)
    {
        var filter = dataManagerRequest?.Params?["Filter"] as GetAppointmentsInput
                     ?? new GetAppointmentsInput { MaxResultCount = dataManagerRequest!.Take };

        var result = await appointmentAppService.GetListAsync(filter);

        var dataResult = new DataResult()
        {
            Result = result.Items,
            Count = (int)result.TotalCount
        };
        return dataResult;
    }
}