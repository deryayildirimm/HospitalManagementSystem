using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Treatment.Icds;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Treatment;

public partial class IcdReport
{
    private DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);
    private DateTime EndDate { get; set; } = DateTime.Now;
    private List<IcdReportDto>? IcdReportList;

    protected override async Task OnInitializedAsync()
    {
        await FetchReport();
    }
    
    private async Task FetchReport()
    {
        IcdReportList = await ExaminationsAppService.GetIcdReportAsync(StartDate, EndDate);
    }
}