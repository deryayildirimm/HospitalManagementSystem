using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Treatment.Examinations;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Pusula.Training.HealthCare.Blazor.Models;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Treatment.Icds;


namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Treatment;

public partial class Examination
{

    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
    private bool CanCreateExamination { get; set; }
    private bool CanEditExamination { get; set; }
    private ExaminationCreateDto NewExamination { get; set; }
    private ExaminationUpdateDto EditingExamination { get; set; }
    private ExaminationDto? CurrentExamination { get; set; }
    private Guid ProtocolId { get; set; }

    private Guid SelectedIcdId { get; set; }
    private IReadOnlyList<IcdDto> IcdList { get; set; }
    private List<IcdDto> SelectedIcds { get; set; } = new();
    private PatientDto Patient { get; set; }

    private string PatientGender = "MALE";
    private bool VisibleProperty { get; set; } = true;
        
    public Examination()
    {
        NewExamination = new ExaminationCreateDto();
        EditingExamination = new ExaminationUpdateDto();
        IcdList = new List<IcdDto>();
    }
    
    protected override async Task OnInitializedAsync()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var getIcdsInput = new GetIcdsInput()
        {
            MaxResultCount = 20
        };
        var icds = await IcdsAppService.GetListAsync(getIcdsInput);
        IcdList = icds.Items;
        
        if (EditingExamination.ExaminationIcds != null)
        {
            SelectedIcds = EditingExamination.ExaminationIcds
                .Select(e => new IcdDto { Id = e.IcdId, CodeNumber = e.Icd.CodeNumber, Detail = e.Icd.Detail })
                .ToList();
        }
        
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("protocolId", out var protocolIdValue))
        {
            ProtocolId = Guid.Parse(protocolIdValue!);
        }
        await SetPermissionsAsync();
        await GetPatientAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetBreadcrumbItemsAsync();
            await GetOrCreateExaminationAsync();
            await InvokeAsync(StateHasChanged);
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Examinations"]));
        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateExamination = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Examinations.Create);
        CanEditExamination = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Examinations.Edit);
    }
    
    private string GetAvatarUrl(string gender)
    {
        return gender == "FEMALE"
            ? "/images/avatar_femalee.jpg"
            : "/images//avatar_male.jpg";
    }
    
    private async Task GetPatientAsync()
    {
        try
        {
            var Protocol = await ProtocolsAppService.GetAsync(ProtocolId);
            Patient = Protocol.Patient;
            PatientGender = Patient.Gender.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        GetAvatarUrl(PatientGender);
        VisibleProperty = false;
    }
    
    protected virtual async Task GetOrCreateExaminationAsync()
    {
        try
        {
            var existingExamination = await ExaminationsAppService.GetByProtocolIdAsync(ProtocolId);

            if (existingExamination != null)
            {
                CurrentExamination = existingExamination;
                EditingExamination = ObjectMapper.Map<ExaminationDto, ExaminationUpdateDto>(existingExamination);
            }
            else
            {
                CurrentExamination = null;
                NewExamination.ProtocolId = ProtocolId;
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateExaminationAsync()
    {
        try
        {
            NewExamination.IcdIds = SelectedIcds.Select(x => x.Id).ToList();
            var createdExamination = await ExaminationsAppService.CreateAsync(NewExamination);
            EditingExamination = ObjectMapper.Map<ExaminationDto, ExaminationUpdateDto>(createdExamination);
            await UiMessageService.Success(L["ExaminationCreated"]);
            NavigationManager.NavigateTo("/my-protocols");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task UpdateExaminationAsync()
    {
        try
        {
            EditingExamination.IcdIds = SelectedIcds.Select(x => x.Id).ToList();
            await ExaminationsAppService.UpdateAsync(EditingExamination);
            await UiMessageService.Success(L["ExaminationCreated"]);
            NavigationManager.NavigateTo("/my-protocols");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    
    private async Task CancelAsync()
    {
        await UiMessageService.Warn(L["NothingIsChanged"]);
        NavigationManager.NavigateTo("/my-protocols");
    }

    private void AddIcdToExamination()
    {
        var icd = IcdList.FirstOrDefault(i => i.Id == SelectedIcdId);
        if (icd != null && !SelectedIcds.Any(i => i.Id == icd.Id))
        {
            SelectedIcds.Add(icd);
        }
    }

    private void RemoveIcd(Guid icdId)
    {
        var icd = SelectedIcds.FirstOrDefault(i => i.Id == icdId);
        if (icd != null)
        {
            SelectedIcds.Remove(icd);
        }
    }
}