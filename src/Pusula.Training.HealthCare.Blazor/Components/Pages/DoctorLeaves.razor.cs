using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.DoctorLeaves;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class DoctorLeaves : HealthCareComponentBase
{
    private SfGrid<DoctorLeaveDto> Grid { get; set; }
    private string[] ToolbarItems { get; set; }
    private int PageSize { get; } = 20;
    private bool IsEditDialogVisible { get; set; }

    public DoctorLeaves()
    {
        ToolbarItems = ["Add", "Delete", "Edit", "PdfExport", "ExcelExport"];
        Grid = new SfGrid<DoctorLeaveDto>();
        IsEditDialogVisible = false;
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


    private void Refresh()
    {
        Grid.Refresh();
    }
    
    private void OpenEditDialog(DoctorLeaveDto input)
    {
        IsEditDialogVisible = true;
    }
}