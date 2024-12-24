using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Blazor.Models.PatientReport;
using Pusula.Training.HealthCare.BloodTests.Reports;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Syncfusion.Blazor.Grids;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Laboratory
{
    public partial class PatientReports
    {
        [Parameter]
        public int PatientNumber { get; set; }
        private bool CanDefaultReport { get; set; }
        private PatientDto? Patient { get; set; }
        private IEnumerable<BloodTestReportDto> Reports { get; set; }
        public string Width { get; set; }
        public List<LineChartData> ChartData;
        private List<BloodTestSummaryDto> BloodTestSummaries { get; set; } 
        private List<BloodTestResultDetailsDto> BloodTestResultDetails { get; set; }
        private bool IsVisibleModal;
        private bool isLoading;

        public PatientReports()
        {
            isLoading = true;   
            IsVisibleModal = false;
            BloodTestSummaries = [];
            BloodTestResultDetails = [];
            ChartData = [];
            Reports = [];
            Width = "90%";
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            isLoading = true;
            await GetPatient();
            await GetReports();
            isLoading = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task SetPermissionsAsync()
        {
            CanDefaultReport = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.BloodTests.Default);
        }

        private async Task GetPatient()
        {
            var patient = await PatientsAppService.GetPatientByNumberAsync(PatientNumber);
            Patient = patient;
        }

        private async Task GetReports()
        {
            var filteredReports = await bloodTestReportService.GetListByPatientNumberAsync(PatientNumber);

            BloodTestSummaries = filteredReports.Select(report => new BloodTestSummaryDto
            {
                BloodTestId = report.BloodTestId,
                DateCreated = report.BloodTest.DateCreated,
                DoctorName = report.BloodTest.Doctor.FirstName,
                PatientName = report.BloodTest.Patient.FirstName,
                Status = report.BloodTest.Status
            }).ToList();

            BloodTestResultDetails = filteredReports.SelectMany(report => report.Results!.Select(result => new BloodTestResultDetailsDto
            {
                BloodTestId = result.BloodTestReport.BloodTestId,
                Value = result.BloodTestResult.Value,
                BloodResultStatus = result.BloodTestResult.BloodResultStatus,
                TestName = result.BloodTestResult.Test.Name,
                PatientId = result.BloodTestReport.BloodTest.PatientId,
                TestId = result.BloodTestResult.TestId,
                Interval = $"{result.BloodTestResult.Test.MinValue} - {result.BloodTestResult.Test.MaxValue}"
            })).ToList();
            await InvokeAsync(StateHasChanged);
        }

        public async void OnCommandClicked(CommandClickEventArgs<BloodTestResultDetailsDto> args)
        {
            var selectedData = args.RowData;
            await OpenGraphicModal(selectedData);

        }

        private async Task OpenGraphicModal(BloodTestResultDetailsDto data)
        {
            var result = await bloodTestReportService.GetListAsync(new GetBloodTestReportsInput());
            var list = result.Items.ToList();

            var filteredList = list
            .Where(x => x.BloodTest.PatientId == data.PatientId)  
            .SelectMany(report => report.Results!
                .Where(r => r.BloodTestResult.TestId == data.TestId)  
            )
            .ToList();

            ChartData = filteredList.Select(result => new LineChartData
            {
                Period = result.BloodTestReport.BloodTest.DateCreated,
                Value = result.BloodTestResult.Value,
            }).ToList();

            IsVisibleModal = true;
        }

        private Task CloseGraphicModal()
        {
            IsVisibleModal = false;
            return Task.CompletedTask;
        }
    }
}


