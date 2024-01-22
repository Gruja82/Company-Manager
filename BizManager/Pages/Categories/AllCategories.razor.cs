using BizManager.Services.Categories;
using Microsoft.AspNetCore.Components;
using SharedProject.Dtos;

namespace BizManager.Pages.Categories
{
    // Background logic for AllCategories component
    public partial class AllCategories
    {
        // Inject ICategoryService
        [Inject]
        private ICategoryService CategoryService { get; set; } = default!;

        // Inject NavigationManager
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // Property that represents collection of CategoryDto objects
        private Pagination<CategoryDto>? CategoryList {  get; set; }

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
        // CategoryList by invoking CategoryService's
        // GetCategoriesAsync method
        protected override async Task OnInitializedAsync()
        {
            CategoryList = await CategoryService.GetCategoriesAsync(searchText, pageIndex, pageSize);
        }

        // Method for handling button click event in SearchComponent
        private async Task HandleSearchClickAsync(string strValue)
        {
            // Set searchText field value to the value of strValue
            searchText = strValue;
            // Reset pageIndex value
            pageIndex = default;
            // Fill the CategoryList
            CategoryList = await CategoryService.GetCategoriesAsync(searchText, pageIndex, pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task HandlePageChangedClickAsync(int pageNumber)
        {
            // Set pageIndex field value to the value of pageNumber
            pageIndex = pageNumber;
            // Fill the CategoryList
            CategoryList = await CategoryService.GetCategoriesAsync(searchText, pageIndex, pageSize);
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
            // Fill the CategoryList
            CategoryList = await CategoryService.GetCategoriesAsync(searchText, pageIndex, pageSize);
        }

        // Method for navigating to Page for creating new Category
        private void NavigateToCreatePage()
        {
            NavigationManager.NavigateTo("/category");
        }

        // Method for navigating to Page for updating existing Category
        private void NavigateToEditPage(int id)
        {
            NavigationManager.NavigateTo($"/category/{id}");
        }
    }
}
