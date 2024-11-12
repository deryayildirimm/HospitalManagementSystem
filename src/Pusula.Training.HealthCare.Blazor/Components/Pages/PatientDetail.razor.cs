using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pusula.Training.HealthCare.Countries;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using AutoMapper.Internal.Mappers;
using Microsoft.Extensions.Localization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class PatientDetail(IMapper mapper) : ComponentBase
{
    protected IStringLocalizer L { get; }

    private IEnumerable<KeyValuePair<int, string>> PationTypes { get; set; } = new List<KeyValuePair<int, string>>();
    private IEnumerable<KeyValuePair<int, string>> Genders { get; set; } = new List<KeyValuePair<int, string>>();
    private IEnumerable<KeyValuePair<int, string>> Relatives { get; set; } = new List<KeyValuePair<int, string>>();
    private IEnumerable<KeyValuePair<int, string>> InsuranceTypes { get; set; } = new List<KeyValuePair<int, string>>();
    private IEnumerable<KeyValuePair<int, string>> DiscountGroups { get; set; } = new List<KeyValuePair<int, string>>();

    [Parameter]
    public Guid Id { get; set; }

    private PatientDto patient;
    
    private Modal ViewPatientModal { get; set; }

    private async Task OpenPatientModal()
    {
        await ViewPatientModal.Show();
    }

    private async Task ClosePatientModal()
    {
        await ViewPatientModal.Hide();
    }

    
    protected override async Task OnInitializedAsync()
    {
        patient = await PatientsAppService.GetAsync(Id);



    }

    
   
  
    
}
            