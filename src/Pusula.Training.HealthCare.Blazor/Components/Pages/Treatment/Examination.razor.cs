using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Treatment.Examinations;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;


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

        
    public Examination()
    {
        NewExamination = new ExaminationCreateDto();
        EditingExamination = new ExaminationUpdateDto();
    }
    
    protected override async Task OnInitializedAsync()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("protocolId", out var protocolIdValue))
        {
            ProtocolId = Guid.Parse(protocolIdValue!);
        }
        await SetPermissionsAsync();
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

}