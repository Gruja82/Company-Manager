using BizManager.Services.Categories;
using BizManager.Services.Products;
using Microsoft.AspNetCore.Components;
using SharedProject.Dtos;

namespace BizManager.Pages.Products
{
    // Background logic for AllProducts component
    public partial class AllProducts
    {
        // Inject IProductService
        [Inject]
        private IProductService ProductService { get; set; } = default!;

        // Inject ICategoryService
        [Inject]
        private ICategoryService CategoryService { get; set; } = default!;

        // Inject NavigationManager
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // Property that represents collection of ProductDto objects
        private Pagination<ProductDto>? ProductList { get; set; }

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

        // Field that represents product quantity
        // for filtering ProductList
        private int minimum;

        // When component is loaded first time, fill the
        // ProductList and CategoryList
        protected override async Task OnInitializedAsync()
        {
            ProductList = await ProductService.GetProductsAsync(searchText, categoryId, minimum, pageIndex, pageSize);

            CategoryList = await CategoryService.GetAllCategoriesAsync();
        }

        // Method for handling button click event in SearchComponent
        private async Task HandleSearchClickAsync(string strValue)
        {
            // Set searchText field value to the value of strValue
            searchText = strValue;
            // Reset pageIndex value
            pageIndex = default;
            // Fill the ProductList
            ProductList = await ProductService.GetProductsAsync(searchText, categoryId, minimum, pageIndex, pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task HandlePageChangedClickAsync(int pageNumber)
        {
            // Set pageIndex field value to the value of pageNumber
            pageIndex = pageNumber;
            // Fill the ProductList
            ProductList = await ProductService.GetProductsAsync(searchText, categoryId, minimum, pageIndex, pageSize);
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
            // Fill the ProductList
            ProductList = await ProductService.GetProductsAsync(searchText, categoryId, minimum, pageIndex, pageSize);
        }

        // Method for handling category's onchange event
        private async Task HandleCategoryChangedAsync(ChangeEventArgs args)
        {
            categoryId = int.Parse(args.Value!.ToString()!);
            pageIndex = default;
            // Fill the ProductList
            ProductList = await ProductService.GetProductsAsync(searchText, categoryId, minimum, pageIndex, pageSize);
        }

        // Method for handling Product's quantity onchange event
        private async Task HandleQuantityChangedAsync(ChangeEventArgs args)
        {
            minimum = int.Parse(args.Value!.ToString()!);
            pageIndex = default;
            // Fill the ProductList
            ProductList = await ProductService.GetProductsAsync(searchText, categoryId, minimum, pageIndex, pageSize);
        }

        // Method for navigating to Page for creating new Product
        private void NavigateToCreatePage()
        {
            NavigationManager.NavigateTo("/product");
        }

        // Method for navigating to Page for updating existing Product
        private void NavigateToEditPage(int id)
        {
            NavigationManager.NavigateTo($"/product/{id}");
        }
    }
}
