using BizManager.Services.Materials;
using BizManager.Services.Orders;
using BizManager.Services.Purchases;
using BizManager.Services.Suppliers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SharedProject.Dtos;

namespace BizManager.Pages.Purchases
{
    // Background logic for SingleOrder component
    public partial class SinglePurchase:IDisposable
    {
        // Inject IPurchaseService
        [Inject]
        private IPurchaseService PurchaseService { get; set; } = default!;

        // Inject ISupplierService
        [Inject]
        private ISupplierService SupplierService { get; set; } = default!;

        // Inject IMaterialService
        [Inject]
        private IMaterialService MaterialService { get; set; } = default!;

        // Inject Navigation Manager
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // Inject IJSRuntime
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        // Field that determines if Delete button
        // is visible or not
        private bool hidden = true;

        // Field that represents total purchase value
        private double purchaseTotal = 0;

        // Field that represents Supplier Id
        private int supplierId = 0;

        // Field that represents Material Id
        private int materialId = 0;

        // Field that represents Material quantity
        private double materialQty = 0;

        // Field that represents validation error message
        private string errorMessage = string.Empty;

        // Form's EditContext
        private EditContext? Context { get; set; }

        // Field that holds validation messages
        private ValidationMessageStore? messageStore;

        // Field that holds validation errors
        private Dictionary<string, string>? errors;

        // Route parameter
        [Parameter]
        public int Id { get; set; }

        // Property that represents list of Suppliers
        private List<SupplierDto>? SupplierList { get; set; }

        // Property that represents list of Materials
        private List<MaterialDto>? MaterialList { get; set; }

        // Property that represents form's model
        private PurchaseDto? PurchaseModel { get; set; }

        // Method for manual activating model validation
        private void HandleValidationRequested(object? sender, ValidationRequestedEventArgs args)
        {
            messageStore?.Clear();

            if (errors!.Any())
            {
                foreach (var error in errors!)
                {
                    FieldIdentifier modelField = new(PurchaseModel!, error.Key);
                    messageStore!.Add(modelField, error.Value);
                }

                errors!.Clear();
            }
        }

        // Method which is invoked when component is loaded
        // and in it we initialize all needed properties and fields
        protected override async Task OnInitializedAsync()
        {
            PurchaseModel = new();
            Context = new(new object());
            errors = new();
            SupplierList = await SupplierService.GetAllSuppliersAsync();
            MaterialList = await MaterialService.GetAllMaterialsAsync();
        }

        // Method which is invoked when component parameters are set
        protected override async Task OnParametersSetAsync()
        {
            // If Id is greater than 0, it means that Purchase is used
            // in Edit operation, so we find Purchase by Id, and show Delete button
            if (Id > 0)
            {
                PurchaseModel = await PurchaseService.GetSinglePurchaseAsync(Id);

                hidden = false;
            }

            Context = new(PurchaseModel!);
            messageStore = new(Context);
            Context.OnValidationRequested += HandleValidationRequested;

            purchaseTotal = CalculatePurchaseTotal();
        }

        // Private method for calculating Purchase's total value
        private double CalculatePurchaseTotal()
        {
            double total = 0;

            // Iterate through PurchaseDetailDtos list and calculate
            // purchase value by multiplying Material's quantity and
            // Material's price and adding that value to total
            if (PurchaseModel!.PurchaseDetailDtos.Count > 0)
            {
                foreach (var purchaseDetailDto in PurchaseModel.PurchaseDetailDtos)
                {
                    total += purchaseDetailDto.Quantity * purchaseDetailDto.MaterialPrice;
                }
            }

            return total;
        }

        // Method which is invoked when form is submitted
        private async Task SubmitAsync()
        {
            object response;

            // If Id value is equal to 0 (zero), 
            // then we have Create operation
            if (Id == 0)
            {
                // Invoke method for creating new Purchase
                response = await PurchaseService.CreateNewPurchaseAsync(PurchaseModel!);
            }
            // Otherwise we have Edit operation
            else
            {
                response = await PurchaseService.EditPurchaseAsync(PurchaseModel!);
            }

            // If returned result is of type Dictionary<string, string>
            // then perform cast response to Dictionary type
            // and perform Context validation
            if (response.GetType() == typeof(Dictionary<string, string>))
            {
                errors = (Dictionary<string, string>)response;
                Context!.Validate();
            }
            // Otherwise, perform Context validation
            // and navigate to /purchases page
            else
            {
                Context!.Validate();
                NavigationManager.NavigateTo("/purchases");
            }
        }

        // Method which is invoked in response
        // to Delete button click
        private async Task DeleteAsync()
        {
            // Prompt the user for confirmation
            // to delete Purchase
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this Purchase?");

            // If confirmed is true, then perform deletion of Order
            if (confirmed)
            {
                await PurchaseService.DeletePurchaseAsync(Id);
                NavigationManager.NavigateTo("/purchases");
            }
        }

        public void Dispose()
        {
            Context!.OnValidationRequested -= HandleValidationRequested;
        }

        // Method for handling onchange event on Material drop down list
        private void HandleMaterialChanged(ChangeEventArgs args)
        {
            materialId = int.Parse(args.Value!.ToString()!);
        }

        // Method for handling onchange event on Material quantity input
        private void HandleMaterialQtyChanged(ChangeEventArgs args)
        {
            materialQty = int.Parse(args.Value!.ToString()!);
        }

        // Method for handling Add button click event
        private void HandleAddMaterialClick()
        {
            PurchaseDetailDto purchaseDetailDto = new();

            purchaseDetailDto.MaterialId = materialId;
            purchaseDetailDto.Quantity = materialQty;
            purchaseDetailDto.MaterialName = MaterialList!.FirstOrDefault(e => e.Id == materialId)!.Name;
            purchaseDetailDto.MaterialPrice = MaterialList!.FirstOrDefault(e => e.Id == materialId)!.Price;

            bool isNotValid = ValidateMaterialAdded();

            if (isNotValid == false)
            {
                PurchaseModel!.PurchaseDetailDtos.Add(purchaseDetailDto);
                errorMessage = string.Empty;
                purchaseTotal = CalculatePurchaseTotal();
            }
        }

        // Method for handling Remove button click event
        private void HandleRemoveMaterialClick(int id)
        {
            PurchaseDetailDto purchaseDetailDto = PurchaseModel!.PurchaseDetailDtos.FirstOrDefault(e => e.MaterialId == id)!;

            PurchaseModel!.PurchaseDetailDtos.Remove(purchaseDetailDto);

            purchaseTotal = CalculatePurchaseTotal();

            errorMessage = string.Empty;
        }

        // Method for checking if selected Material
        // is already added to Purchase's PurchaseDetailDtos collection
        // and that Material quantity is greater than 0 (zero)
        private bool ValidateMaterialAdded()
        {
            bool isNotValid = PurchaseModel!.PurchaseDetailDtos.Select(e => e.MaterialId).Contains(materialId) || materialQty <= 0;

            if (isNotValid == true)
            {
                errorMessage = "Error! Please verify that selected Material is not already added to list" +
                    " and that Material quantity is not less than or equal to 0 (zero)!";
            }

            return isNotValid;
        }
    }
}
