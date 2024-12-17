using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.DoctorLeaves;
using Pusula.Training.HealthCare.Lookups;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class DoctorLeaves : HealthCareComponentBase
{
    private Query FilterQuery { get; set; }
    private SfGrid<DoctorLeaveDto> Grid { get; set; }
    private string[] ToolbarItems { get; set; }
    private int PageSize { get; } = 20;
    private int LookupPageSize { get; } = 100;
    private int CurrentPage { get; set; } = 1;
    private bool IsEditDialogVisible { get; set; }
    private GetDoctorLeaveInput InputFilter { get; set; }

    //Collections
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> MedicalServiceCollection { get; set; }
    private List<KeyValuePair<string, EnumLeaveType>> LeaveTypeCollection { get; set; }


    public DoctorLeaves()
    {
        ToolbarItems = ["Add", "Delete", "Edit", "PdfExport", "ExcelExport"];
        Grid = new SfGrid<DoctorLeaveDto>();
        InputFilter = new GetDoctorLeaveInput();
        FilterQuery = new Query();
        IsEditDialogVisible = false;

        DepartmentsCollection = [];
        MedicalServiceCollection = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await SetLookupsAsync();
        SetLeaveTypes();
    }

    private async Task DeleteTypeAsync(DoctorLeaveDto input)
    {
        try
        {
            var confirmed = await UiMessageService.Confirm(@L["DeleteConfirmationMessage"]);
            if (!confirmed)
            {
                return;
            }

            await DoctorLeaveAppService.DeleteAsync(input.Id);
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
        finally
        {
            Refresh();
        }
    }

    private async Task SetLookupsAsync()
    {
        try
        {
            DepartmentsCollection =
                (await LookupAppService.GetDepartmentLookupAsync(new LookupRequestDto
                    { MaxResultCount = LookupPageSize }))
                .Items;

            MedicalServiceCollection = (await LookupAppService.GetMedicalServiceLookupAsync(new LookupRequestDto
                    { MaxResultCount = LookupPageSize }))
                .Items;
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task GetAppointmentsAsync()
    {
        InputFilter.MaxResultCount = PageSize;
        InputFilter.SkipCount = (CurrentPage - 1) * PageSize;

        SetFilters();
        await Refresh();
    }

    private void SetLeaveTypes()
    {
        LeaveTypeCollection = Enum.GetValues(typeof(EnumLeaveType))
            .Cast<EnumLeaveType>()
            .Select(e => new KeyValuePair<string, EnumLeaveType>(e.ToString(), e))
            .ToList();
    }

    private void SetFilters()
    {
        FilterQuery.Queries.Params = new Dictionary<string, object>();
        FilterQuery.Queries.Params.Add("Filter", FilterQuery);
    }


    private async Task ClearFilters()
    {
        InputFilter = new GetDoctorLeaveInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize
        };

        StateHasChanged();
        SetFilters();
        await Refresh();
    }

    private async Task Refresh()
    {
        await Grid.Refresh();
    }

    private void OpenEditDialog(DoctorLeaveDto input)
    {
        IsEditDialogVisible = true;
    }
}