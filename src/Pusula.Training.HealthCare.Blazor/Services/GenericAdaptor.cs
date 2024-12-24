using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Blazor.Services
{
    public class GenericAdaptor<TService, TInput, TResult> : DataAdaptor
          where TService : class
          where TInput : new()
          where TResult : IEntityDto
    {
        private readonly TService _service;

        public GenericAdaptor(TService service)
        {
            _service = service;
        }

        public override async Task<object> ReadAsync(
       DataManagerRequest dataManagerRequest,
       string? additionalParam = null)
        {
            // Filtreleme
            var filterJson = dataManagerRequest?.Params?["Filter"]?.ToString();
            var filter = string.IsNullOrEmpty(filterJson)
                ? new TInput()
                : JsonSerializer.Deserialize<TInput>(filterJson);

            // MaxResultCount ve SkipCount ayarları
            dynamic dynamicFilter = filter!;
            dynamicFilter.MaxResultCount = dataManagerRequest?.Take;
            dynamicFilter.SkipCount = dataManagerRequest?.Skip;

            // Sıralama
            if (dataManagerRequest?.Sorted is { Count: > 0 })
            {
                dynamicFilter.Sorting = $"{dataManagerRequest.Sorted[0].Name} {dataManagerRequest.Sorted[0].Direction}";
            }

            // Verileri çağır
            var serviceMethod = _service.GetType().GetMethod("GetListAsync");
            if (serviceMethod == null) throw new InvalidOperationException("GetListAsync method not found!");

            var result = await (Task<PagedResultDto<TResult>>)serviceMethod.Invoke(_service, new object[] { dynamicFilter });

            return new DataResult
            {
                Result = result.Items,
                Count = (int)result.TotalCount
            };
        }
    }
}
