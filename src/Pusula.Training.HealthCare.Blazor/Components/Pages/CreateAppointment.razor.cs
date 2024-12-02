using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Shared;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class CreateAppointment
{
        private IReadOnlyList<LookupDto<Guid>> AppointmentTypesCollection { get; set; } = [];
        private int TypePageSize { get; } = 50;


        public CreateAppointment()
        {
        }
        
        protected override async Task OnInitializedAsync()
        {
                await GetAppointmentTypes();
        }
        
        private async Task GetAppointmentTypes(string? newValue = null)
        {
                try
                {
                        AppointmentTypesCollection =
                                (await AppointmentAppService.GetAppointmentTypeLookupAsync(new LookupRequestDto
                                        { Filter = newValue, MaxResultCount = TypePageSize }))
                                .Items;

                        StateHasChanged();
                }
                catch (Exception e)
                {
                        AppointmentTypesCollection = [];
                        await UiMessageService.Error(e.Message);
                }
        }
}