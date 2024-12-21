using System;
using System.Threading.Tasks;
using System.Text.Json;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using Volo.Abp;
using Pusula.Training.HealthCare.Treatment.Examinations;
using Pusula.Training.HealthCare.Treatment.Icds;

namespace Pusula.Training.HealthCare.Blazor.Services.Treatment;

public class IcdReportAdaptor(IExaminationsAppService examinaitonsAppService) : DataAdaptor
{
    public override async Task<object> ReadAsync(
        DataManagerRequest dataManagerRequest,
        string? additionalParam = null)
    {
        try
        {
            
            var filterJson = dataManagerRequest?.Params?["Filter"]?.ToString();
            
            var filter = string.IsNullOrEmpty(filterJson)
                ? new GetIcdReportInput { MaxResultCount = dataManagerRequest!.Take, SkipCount = dataManagerRequest!.Skip }
                : JsonSerializer.Deserialize<GetIcdReportInput>(filterJson);

            filter!.Sorting = dataManagerRequest!.Sorted is { Count: > 0 }
                ? $"{dataManagerRequest.Sorted[0].Name} {dataManagerRequest.Sorted[0].Direction}"
                : null;
            
            filter!.SkipCount = dataManagerRequest!.Skip;
            filter.MaxResultCount = dataManagerRequest.Take;
            var result = await examinaitonsAppService.GetIcdReportAsync(filter!);

            var dataResult = new DataResult
            {
                Result = result.Items,
                Count = (int)result.TotalCount
            };
            return dataResult;
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
        
    }
}