using BizManager.Services.Materials;
using BizManager.Services.Purchases;
using BizManager.Services.Suppliers;
using Microsoft.AspNetCore.Components;
using SharedProject.Dtos;

namespace BizManager.Pages.Purchases
{
    // Background logic for AllCOrders component
    public partial class AllPurchases
    {
        // Inject IPUrchaseService
        [Inject]
        private IPurchaseService PurchaseService { get; set; } = default!;

        // Inject IMaterialService
        [Inject]
        private IMaterialService MaterialService { get; set; } = default!;

        // Inject ISupplierService
        [Inject]
        private ISupplierService SupplierService { get; set; } = default!;

        // Inject Navigation Manager
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // Property that represents collection ofPurchaseDto objects
        private Pagination<PurchaseDto>? PurchaseList { get; set; }

        // Property that represents list of Purchase dates
        private List<string>? PurchaseDates { get; set; }

        // Property that represents list of Materials
        private List<MaterialDto>? MaterialList { get; set; }

        // Property that represents list of Suppliers
        private List<SupplierDto>? SupplierList { get; set; }

        // Field that represents Search term
        // used for SearchComponent
        private string? searchText;

        // Field that represents Purchase Date
        private string? purchaseDate;

        // Field that represents Supplier Id
        public int supplierId;

        // Field that represents Material Id
        private int materialId;

        // Field that represents Page number
        // used for PaginationComponent
        private int pageIndex;

        // Field that represents Page size
        // used for PaginationComponent
        private int pageSize;

        // When component is loaded first time, fill the
        // PurchaseList, PurchaseDates, MaterialList and SupplierList
        protected override async Task OnInitializedAsync()
        {
            PurchaseList = await PurchaseService.GetPurchasesAsync(searchText, purchaseDate, supplierId, materialId, pageIndex, pageSize);
            PurchaseDates = await PurchaseService.GetPurchaseDatesAsync();
            MaterialList = await MaterialService.GetAllMaterialsAsync();
            SupplierList = await SupplierService.GetAllSuppliersAsync();
        }

        // Method for handling button click event in SearchComponent
        private async Task HandleSearchClickAsync(string strValue)
        {
            // Set searchText field value to the value of strValue
            searchText = strValue;
            // Reset pageIndex value
            pageIndex = default;
            // Fill the PurchaseList
            PurchaseList = await PurchaseService.GetPurchasesAsync(searchText, purchaseDate, supplierId, materialId, pageIndex, pageSize);
        }

        // Method for handling date's onchange event
        private async Task HandleDateChangedAsync(ChangeEventArgs args)
        {
            purchaseDate = args.Value!.ToString();
            // Reset pageIndex value
            pageIndex = default;
            // Fill the PurchaseList
            PurchaseList = await PurchaseService.GetPurchasesAsync(searchText, purchaseDate, supplierId, materialId, pageIndex, pageSize);
        }

        // Method for handling material's onchange event
        private async Task HandleMaterialChangedAsync(ChangeEventArgs args)
        {
            materialId = int.Parse(args.Value!.ToString()!);
            // Reset pageIndex value
            pageIndex = default;
            // Fill the PurchaseList
            PurchaseList = await PurchaseService.GetPurchasesAsync(searchText, purchaseDate, supplierId, materialId, pageIndex, pageSize);
        }

        // Method for handling supplier's onchange event
        private async Task HandleSupplierChangedAsync(ChangeEventArgs args)
        {
            supplierId = int.Parse(args.Value!.ToString()!);
            // Reset pageIndex value
            pageIndex = default;
            // Fill the PurchaseList
            PurchaseList = await PurchaseService.GetPurchasesAsync(searchText, purchaseDate, supplierId, materialId, pageIndex, pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task HandlePageChangedClickAsync(int pageNumber)
        {
            // Set pageIndex field value to the value of pageNumber
            pageIndex = pageNumber;
            // Fill the PurchaseList
            PurchaseList = await PurchaseService.GetPurchasesAsync(searchText, purchaseDate, supplierId, materialId, pageIndex, pageSize);
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
            // Fill the PurchaseList
            PurchaseList = await PurchaseService.GetPurchasesAsync(searchText, purchaseDate, supplierId, materialId, pageIndex, pageSize);
        }

        // Method for navigating to Page for creating new Purchase
        private void NavigateToCreatePage()
        {
            NavigationManager.NavigateTo("/purchase");
        }

        // Method for navigating to Page for updating existing Purchase
        private void NavigateToEditPage(int id)
        {
            NavigationManager.NavigateTo($"/purchase/{id}");
        }
    }
}
