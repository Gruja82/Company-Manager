using BizManager.Services.Suppliers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedProject.Dtos;

namespace BizManager.Pages.Suppliers
{
    // Background logic for AllCustomers component
    public partial class AllSuppliers
    {
        // Inject ISupplierService
        [Inject]
        private ISupplierService SupplierService { get; set; } = default!;

        // Inject Navigation Manager
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // Inject IJSRuntime
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        // Property that represents collection of SupplierDto objects
        private Pagination<SupplierDto>? SupplierList { get; set; }

        // Field that represents Search term
        // used for SearchComponent
        private string? searchText;

        // Field that represents Page number
        // used for PaginationComponent
        private int pageIndex;

        // Field that represents Page size
        // used for PaginationComponent
        private int pageSize;

        // When component is loaded first time, fill the
        // SupplierList by invoking SupplierService's
        // GetSuppliersAsync method
        protected override async Task OnInitializedAsync()
        {
            SupplierList = await SupplierService.GetSuppliersAsync(searchText, pageIndex, pageSize);
        }

        // Method for handling button click event in SearchComponent
        private async Task HandleSearchClickAsync(string strValue)
        {
            // Set searchText field value to the value of strValue
            searchText = strValue;
            // Reset pageIndex value
            pageIndex = default;
            // Fill the SupplierList
            SupplierList = await SupplierService.GetSuppliersAsync(searchText, pageIndex, pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task HandlePageChangedClickAsync(int pageNumber)
        {
            // Reset pageIndex value
            pageIndex = pageNumber;
            // Fill the SupplierList
            SupplierList = await SupplierService.GetSuppliersAsync(searchText, pageIndex, pageSize);
        }

        // Method for handling PaginationComponent's page size
        // button click event
        private async Task HandlePageSizeChangedAsync(int pageSizeValue)
        {
            // Reset pageIndex value
            pageIndex = default;
            // If pageSize value is larger than 0 (zero),
            // then set pageSize field value to the value of pageSizeValue.
            // Otherwise, set pageSize field value to 4
            int pageValue = pageSizeValue > 0 ? pageSizeValue : 4;
            pageSize = pageValue;
            // Fill the SupplierList
            SupplierList = await SupplierService.GetSuppliersAsync(searchText, pageIndex, pageSize);
        }

        // Method for navigating to Page for creating new Supplier
        private void NavigateToCreatePage()
        {
            NavigationManager.NavigateTo("/supplier");
        }

        // Method for navigating to Page for editing selected Supplier
        private void NavigateToEditPage(int id)
        {
            NavigationManager.NavigateTo($"/supplier/{id}");
        }
    }
}
