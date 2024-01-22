using BizManager.Services.Customers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedProject.Dtos;

namespace BizManager.Pages.Customers
{
    // Background logic for AllCustomers component
    public partial class AllCustomers
    {
        // Inject ICustomerService
        [Inject]
        private ICustomerService CustomerService { get; set; } = default!;

        // Inject NavigationManager
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // Property that represents collection of CustomerDto objects
        private Pagination<CustomerDto>? CustomerList {  get; set; }

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
        // CustomerList by invoking CustomerService's
        // GetCustomersAsync method
        protected override async Task OnInitializedAsync()
        {
            CustomerList = await CustomerService.GetCustomersAsync(searchText, pageIndex, pageSize);
        }

        // Method for handling button click event in SearchComponent
        private async Task HandleSearchClickAsync(string strValue)
        {
            // Set searchText field value to the value of strValue
            searchText = strValue;
            // Reset pageIndex value
            pageIndex = default;
            // Fill the CustomerList
            CustomerList = await CustomerService.GetCustomersAsync(searchText, pageIndex, pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task HandlePageChangedClickAsync(int pageNumber)
        {
            // Set pageIndex field value to the value of pageNumber
            pageIndex = pageNumber;
            // Fill the CustomerList
            CustomerList = await CustomerService.GetCustomersAsync(searchText, pageIndex, pageSize);
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
            // Fill the CustomerList
            CustomerList = await CustomerService.GetCustomersAsync(searchText, pageIndex, pageSize);
        }

        // Method for navigating to Page for creating new Customer
        private void NavigateToCreatePage()
        {
            NavigationManager.NavigateTo("/customer");
        }

        // Method for navigating to Page for updating existing Customer
        private void NavigateToEditPage(int id)
        {
            NavigationManager.NavigateTo($"/customer/{id}");
        }
    }
}
