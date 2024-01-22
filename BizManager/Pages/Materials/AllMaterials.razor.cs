using BizManager.Services.Categories;
using BizManager.Services.Materials;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedProject.Dtos;

namespace BizManager.Pages.Materials
{
    // Background logic for AllMaterials component
    public partial class AllMaterials
    {
        // Inject IMaterialService
        [Inject]
        private IMaterialService MaterialService { get; set; } = default!;

        // Inject ICategoryService
        [Inject]
        private ICategoryService CategoryService { get; set; } = default!;

        // Inject NavigationManager
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // Property that represents collection of MaterialDto objects
        private Pagination<MaterialDto>? MaterialList { get; set; }

        // Property that represents collection of CategoryDto objects
        private List<CategoryDto>? CategoryList { get; set; }

        // Field that represents Search term
        // used for SearchComponent
        private string? searchText;

        // Field that represents Page number
        // used for PaginationComponent
        private int pageIndex;

        // Field that represents Page size
        // used for PaginationComponent
        private int pageSize;

        // Field that represents Category Id
        private int categoryId;

        // When component is loaded first time, fill the
        // MaterialList by invoking MaterialService's
        // GetMaterialsAsync method
        protected override async Task OnInitializedAsync()
        {
            MaterialList = await MaterialService.GetMaterialsAsync(searchText, categoryId, pageIndex, pageSize);

            CategoryList = await CategoryService.GetAllCategoriesAsync();
        }

        // Method for handling button click event in SearchComponent
        private async Task HandleSearchClickAsync(string strValue)
        {
            // Set searchText field value to the value of strValue
            searchText = strValue;
            // Reset pageIndex value
            pageIndex = default;
            // Fill the MaterialList
            MaterialList = await MaterialService.GetMaterialsAsync(searchText, categoryId, pageIndex, pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task HandlePageChangedClickAsync(int pageNumber)
        {
            // Set pageIndex field value to the value of pageNumber
            pageIndex = pageNumber;
            // Fill the MaterialList
            MaterialList = await MaterialService.GetMaterialsAsync(searchText, categoryId, pageIndex, pageSize);
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
            // Fill the MaterialList
            MaterialList = await MaterialService.GetMaterialsAsync(searchText, categoryId, pageIndex, pageSize);
        }

        // Method for navigating to Page for creating new Material
        private void NavigateToCreatePage()
        {
            NavigationManager.NavigateTo("/material");
        }

        // Method for navigating to Page for updating existing Material
        private void NavigateToEditPage(int id)
        {
            NavigationManager.NavigateTo($"/material/{id}");
        }

        // Method for handling category's onchange event
        private async Task HandleCategoryChangedAsync(ChangeEventArgs args)
        {
            categoryId = int.Parse(args.Value!.ToString()!);
            pageIndex = default;
            // Fill the MaterialList
            MaterialList = await MaterialService.GetMaterialsAsync(searchText, categoryId, pageIndex, pageSize);
        }
    }
}
