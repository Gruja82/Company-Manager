using BizManager.Services.Materials;
using BizManager.Services.Productions;
using BizManager.Services.Products;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SharedProject.Dtos;

namespace BizManager.Pages.Productions
{
    // Background logic for SingleProduction component
    public partial class SingleProduction:IDisposable
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

        // Inject IJSRuntime
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        // Field that determines if Delete button
        // is visible or not
        private bool hidden = true;

        // Form's EditContext
        private EditContext? Context { get; set; }

        // Field that holds validation messages
        private ValidationMessageStore? messageStore;

        // Field that holds validation errors
        private Dictionary<string, string>? errors;

        // Route parameter
        [Parameter]
        public int Id { get; set; }

        // Property that represents list of ProductDto objects
        private List<ProductDto>? ProductList { get; set; }

        // Property that represents form's model
        private ProductionDto? ProductionModel { get; set; }

        // Method for manual activating model validation
        private void HandleValidationRequested(object? sender, ValidationRequestedEventArgs args)
        {
            // Clear messageStore
            messageStore?.Clear();

            // If there are any validation errors
            // then for each error in error add
            // validation message to message store
            if (errors!.Any())
            {
                foreach (var error in errors!)
                {
                    FieldIdentifier modelField = new(ProductionModel!, error.Key);
                    messageStore!.Add(modelField, error.Value);
                }

                // Clear errors Dictionary
                errors!.Clear();
            }
        }

        // Method which is invoked when component is loaded
        // and in it we initialize all needed properties and fields
        protected override async Task OnInitializedAsync()
        {
            ProductionModel = new();
            Context = new(new object());
            errors = new();
            ProductList = await ProductService.GetAllProductsAsync();
        }

        // Method which is invoked when component parameters are set
        protected override async Task OnParametersSetAsync()
        {
            // If Id is larger than 0, then component shows
            // ProductionDto in edit mode
            if (Id > 0)
            {
                // Set ProductionModel value
                ProductionModel = await ProductionService.GetSingleProductionAsync(Id);
                // Show Delete button
                hidden = false;
            }

            Context = new(ProductionModel!);
            messageStore = new(Context);
            Context.OnValidationRequested += HandleValidationRequested;
        }

        // Method which is invoked when form is submitted
        private async Task SubmitAsync()
        {
            // If Id is equal to 0, then we have Create operation
            if (Id == 0)
            {
                // Invoke method for creating new Production
                var response = await ProductionService.CreateNewProductionAsync(ProductionModel!);

                // If result is of type Dictionary<string,string>
                // then it means we have validation errors, 
                // and we are converting response to Dictionary<string, string>,
                // and invoking method for model validation
                if (response.GetType() == typeof(Dictionary<string, string>))
                {
                    errors = (Dictionary<string, string>)response;
                    Context!.Validate();
                }
                // Otherwise, it means that Create operation was succesfull
                // and we are again invoking method for model validation
                // so that we clear previous error messages
                // and finally we are redirecting user to "/productions" page
                else
                {
                    Context!.Validate();
                    NavigationManager.NavigateTo("/productions");
                }
            }
            // Otherwise we have Edit operation
            else
            {
                // Invoke method for creating new Production
                var response = await ProductionService.EditProductionAsync(ProductionModel!);

                // If result is of type Dictionary<string,string>
                // then it means we have validation errors, 
                // and we are converting response to Dictionary<string, string>,
                // and invoking method for model validation
                if (response.GetType() == typeof(Dictionary<string, string>))
                {
                    errors = (Dictionary<string, string>)response;
                    Context!.Validate();
                }
                // Otherwise, it means that Create operation was succesfull
                // and we are again invoking method for model validation
                // so that we clear previous error messages
                // and finally we are redirecting user to "/productions" page
                else
                {
                    Context!.Validate();
                    NavigationManager.NavigateTo("/productions");
                }
            }
        }

        // Method which handles Delete button click event
        private async Task DeleteAsync()
        {
            // Prompt the user for confirmation
            // to delete Material
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this Production?");

            // If confirmed is true, then perform deletion of Production
            if (confirmed)
            {
                await ProductionService.DeleteProductionAsync(Id);
                NavigationManager.NavigateTo("/productions");
            }
        }

        public void Dispose()
        {
            Context!.OnValidationRequested -= HandleValidationRequested;
        }
    }
}
