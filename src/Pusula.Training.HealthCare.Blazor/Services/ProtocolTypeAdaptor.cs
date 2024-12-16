using Pusula.Training.HealthCare.ProtocolTypes;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System.Threading.Tasks;
using System;
using Volo.Abp;
using System.Text.Json;
using System.Collections.Generic;


namespace Pusula.Training.HealthCare.Blazor.Services
{
    public class ProtocolTypeAdaptor(IProtocolTypesAppService protocolTypesAppService) :DataAdaptor
    {

        public override async Task<object> ReadAsync(DataManagerRequest dataManagerRequest, 
            string? additionalParam = null)
        {
            try
            {

                var filterJson = dataManagerRequest?.Params?["Filter"]?.ToString();

                var filter = string.IsNullOrEmpty(filterJson)
                    ? new GetProtocolTypeInput { MaxResultCount = dataManagerRequest!.Take, SkipCount = dataManagerRequest!.Skip }
                    : JsonSerializer.Deserialize<GetProtocolTypeInput>(filterJson);

                filter!.Sorting = dataManagerRequest!.Sorted is { Count: > 0 }
                    ? $"{dataManagerRequest.Sorted[0].Name} {dataManagerRequest.Sorted[0].Direction}"
                    : null;

                filter!.SkipCount = dataManagerRequest!.Skip;
                filter.MaxResultCount = dataManagerRequest.Take;
                var result = await protocolTypesAppService.GetListAsync(filter!);

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
}