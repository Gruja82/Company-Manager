using BizManager.Services.Productions;
using BizManager.Services.Products;
using Microsoft.AspNetCore.Components;
using SharedProject.Dtos;

namespace BizManager.Pages.Productions
{
    // Background logic for AllProductions component
    public partial class AllProductions
    {
        // Inject IProductionService
        [Inject]
        private IProductionService ProductionService { get; set; } = default!;

        // Inject IProductService
        [Inject]
        private IProductService ProductService { get; set; } = default!;

        // Inject NavigationManager
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // Property that represents collection of ProductionDto objects
        private Pagination<ProductionDto>? ProductionList { get; set; }

        // Property that represents collection of ProductDto objects
        private List<ProductDto>? ProductList { get; set; }

        // Property that represents collection of all Production dates
        private List<string>? DatesList { get; set; }

        // Field that represents Search term
        // used for SearchComponent
        private string? searchText;

        // Field that represents selected date
        private string? productionDate;

        // Field that represents Page number
        // used for PaginationComponent
        private int pageIndex;

        // Field that represents Page size
        // used for PaginationComponent
        private int pageSize;

        // Field that represents Product Id
        private int productId;

        // When component is loaded first time, fill the
        // ProductList by invoking ProductService's
        // GetProductsAsync method
        protected override async Task OnInitializedAsync()
        {
            ProductionList = await ProductionService.GetProductionsAsync(searchText, productionDate, productId, pageIndex, pageSize);

            ProductList = await ProductService.GetAllProductsAsync();

            DatesList = await ProductionService.GetAllDatesAsync();
        }

        // Method for handling button click event in SearchComponent
        private async Task HandleSearchClickAsync(string strValue)
        {
            // Set searchText field value to the value of strValue
            searchText = strValue;
            // Reset pageIndex value
            pageIndex = default;
            // Fill the ProductionList
            ProductionList = await ProductionService.GetProductionsAsync(searchText, productionDate, productId, pageIndex, pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task HandlePageChangedClickAsync(int pageNumber)
        {
            // Set pageIndex field value to the value of pageNumber
            pageIndex = pageNumber;
            // Fill the ProductionList
            ProductionList = await ProductionService.GetProductionsAsync(searchText, productionDate, productId, pageIndex, pageSize);
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
            // Fill the ProductionList
            ProductionList = await ProductionService.GetProductionsAsync(searchText, productionDate, productId, pageIndex, pageSize);
        }

        // Method for handling product's onchange event
        private async Task HandleProductChangedAsync(ChangeEventArgs args)
        {
            productId = int.Parse(args.Value!.ToString()!);
            pageIndex = default;
            // Fill the ProductionList
            ProductionList = await ProductionService.GetProductionsAsync(searchText, productionDate, productId, pageIndex, pageSize);
        }

        // Method for handling production date's onchange event
        private async Task HandleDateChangedAsync(ChangeEventArgs args)
        {
            productionDate = args.Value!.ToString();
            pageIndex = default;
            // Fill the ProductionList
            ProductionList = await ProductionService.GetProductionsAsync(searchText, productionDate, productId, pageIndex, pageSize);
        }
        // Method for navigating to Page for creating new Production
        private void NavigateToCreatePage()
        {
            NavigationManager.NavigateTo("/production");
        }

        // Method for navigating to Page for updating existing Production
        private void NavigateToEditPage(int id)
        {
            NavigationManager.NavigateTo($"/production/{id}");
        }

    }
}
