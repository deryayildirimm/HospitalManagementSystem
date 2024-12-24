using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Extensions;
using Pusula.Training.HealthCare.Insurances;
using Pusula.Training.HealthCare.Permissions;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using SortDirection = Blazorise.SortDirection;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Patient
{
    public partial class Insurances
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<InsuranceDto> InsuranceList { get; set; } = new List<InsuranceDto>();
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private int TotalCount { get; set; }
        private string CurrentSorting { get; set; } = string.Empty;
        private bool AllInsurancesSelected { get; set; }
        private bool CanCreateInsurance { get; set; }
        private bool CanEditInsurance { get; set; }
        private bool CanDeleteInsurance { get; set; }
        private Guid EditingInsuranceId { get; set; }
        private InsuranceCreateDto? NewInsurance {  get; set; }
        private InsuranceUpdateDto? EditInsurance {  get; set; }
        private GetInsurancesInput Filter { get; set; }
        private List<InsuranceDto> SelectedInsurances { get; set; } = [];
        private List<KeyValuePair<EnumInsuranceCompanyName, string>> InsuranceCompanyNameList { get; set; } = [];
        private bool isLoading { get; set; }
        private bool IsVisibleCreate { get; set; }
        private bool IsVisibleEdit { get; set; }

        public Insurances()
        {
            isLoading = true;
            IsVisibleCreate = false;
            IsVisibleEdit = false;
            Filter = new GetInsurancesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            InsuranceCompanyNameList = new();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            isLoading = true;
            await SearchAsync();
            await GetInsuranceCompanyNameList();
            await GetInsurances();
            isLoading = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Insurances"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["CreateInsurance"], OpenCreateInsuranceModal, IconName.Add, requiredPolicyName: HealthCarePermissions.Insurances.Create);
            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateInsurance = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Insurances.Create);
            CanEditInsurance = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Insurances.Edit);
            CanDeleteInsurance = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Insurances.Delete);
        }
        public async Task PageChangingHandler(GridPageChangingEventArgs args)
        {
            CurrentPage = args.CurrentPage;
            await GetInsurances();
        }

        public async Task PageChangedHandler(GridPageChangedEventArgs args)
        {
            CurrentPage = args.CurrentPage;
            await GetInsurances();

        }
        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetInsurances();
            await InvokeAsync(StateHasChanged);
        }

        private Task OpenCreateInsuranceModal()
        {
            IsVisibleCreate = true;
            NewInsurance = new InsuranceCreateDto();
            return Task.CompletedTask;
        }
        private Task OpenEditModal(InsuranceDto insurance)
        {
            EditingInsuranceId = insurance.Id;
            EditInsurance = new InsuranceUpdateDto
            {
                PolicyNumber = insurance.PolicyNumber,
                PremiumAmount = insurance.PremiumAmount,
                CoverageAmount = insurance.CoverageAmount,
                StartDate = insurance.StartDate,
                EndDate = insurance.EndDate,
                InsuranceCompanyName = insurance.InsuranceCompanyName,
                Description = insurance.Description,
            };
            IsVisibleEdit = true;
            return Task.CompletedTask;
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<InsuranceDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetInsurances();
            await InvokeAsync(StateHasChanged);
        }
        private async Task GetInsurances()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var insuranceList = await insuranceAppService.GetListAsync(Filter);
            InsuranceList = insuranceList.Items;
            TotalCount = (int)insuranceList.TotalCount;

            await ClearSelection();
        }
        private Task SelectAllItems()
        {
            AllInsurancesSelected = true;

            return Task.CompletedTask;
        }
        private Task ClearSelection()
        {
            SelectedInsurances.Clear();
            AllInsurancesSelected = false;
            return Task.CompletedTask;
        }
        private Task SelectedInsurancesRowsChanged()
        {
            if (SelectedInsurances.Count != PageSize)
            {
                AllInsurancesSelected = false;
            }

            return Task.CompletedTask;
        }
        private async Task DeleteInsuranceAsync(InsuranceDto input)
        {
            var confirmed = await UiMessageService.Confirm($"Are you sure you want to delete {input.InsuranceCompanyName} ?");
            if (!confirmed) return;

            await insuranceAppService.DeleteAsync(input.Id);
            await GetInsurances();
        }

        private async Task DeleteSelectedInsurancesAsync()
        {
            var message = AllInsurancesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedInsurances.Count].Value;

            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllInsurancesSelected)
            {
                await insuranceAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await insuranceAppService.DeleteByIdsAsync(SelectedInsurances.Select(x => x.Id).ToList());
            }

            SelectedInsurances.Clear();
            AllInsurancesSelected = false;

            await GetInsurances();
        }

        private async Task CreateInsuranceAsync()
        {
            try
            {
                await insuranceAppService.CreateAsync(NewInsurance!);
                await GetInsurances();
                CloseCreateInsuranceModal();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task UpdateInsuranceAsync()
        {
            try
            {
                await insuranceAppService.UpdateAsync(EditingInsuranceId,EditInsurance!);
                await GetInsurances();
                CloseEditInsuranceModal();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void CloseCreateInsuranceModal()
        {
            IsVisibleCreate = false;
        }

        private void CloseEditInsuranceModal()
        {
            IsVisibleEdit = false;
        }

        private Task GetInsuranceCompanyNameList()
        {
            InsuranceCompanyNameList = Enum.GetValues(typeof(EnumInsuranceCompanyName))
                .Cast<EnumInsuranceCompanyName>()
                .Select(s => new { Key = s, Value = s.GetDisplayName().ToString() })
                .ToDictionary(d => d.Key, d => d.Value).ToList();
            return Task.CompletedTask;
        }
    }
}