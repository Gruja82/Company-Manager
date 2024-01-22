using BizManager.Services.Categories;
using BizManager.Services.Materials;
using BizManager.Services.Products;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SharedProject.Dtos;
using System.Net.Http.Headers;

namespace BizManager.Pages.Products
{
    // Background logic for SingleProduct component
    public partial class SingleProduct:IDisposable
    {
        // Inject IProductService
        [Inject]
        private IProductService ProductService { get; set; } = default!;

        // Inject IMaterialService
        [Inject]
        private IMaterialService MaterialService { get; set; } = default!;

        // Inject ICategoryService
        [Inject]
        private ICategoryService CategoryService { get; set; } = default!;

        // Inject NavigationManager
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // Inject IJSRuntime
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        // Field that determines if Delete button
        // is visible or not
        private bool hidden = true;

        // Field that will hold production cost
        private double productionCost;

        // Field that accepts value from material drop down list
        private int materialId;

        // Field that accepts value from material quantity input
        private double materialQty;

        // Filed that represents error message
        private string errorMessage = string.Empty;

        // Form's EditContext
        private EditContext? Context { get; set; }

        // Field that holds validation messages
        private ValidationMessageStore? messageStore;

        // Field that holds validation errors
        private Dictionary<string, string>? errors;

        // Field that represents image file
        private IBrowserFile? file;

        // Route parameter
        [Parameter]
        public int Id { get; set; }

        // Property that represents list of CategoryDto objects
        private List<CategoryDto>? CategoryList { get; set; }

        // Property that represents list of MaterialDto objects
        private List<MaterialDto>? MaterialList { get; set; } = new();

        // Property that represents form's model
        private ProductDto? ProductModel { get; set; }

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
                    FieldIdentifier modelField = new(ProductModel!, error.Key);
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
            materialId = 0;
            materialQty = 0;
            productionCost = 0;
            ProductModel = new();
            Context = new(new object());
            errors = new();
            CategoryList = await CategoryService.GetAllCategoriesAsync();
            MaterialList = await MaterialService.GetAllMaterialsAsync();
        }

        // Method which is invoked when component parameters are set
        protected override async Task OnParametersSetAsync()
        {
            // If Id is larger than 0, then component shows
            // ProductDto in edit mode
            if (Id > 0)
            {
                // Set ProductModel value
                ProductModel = await ProductService.GetSingleProductAsync(Id);
                // Show Delete button
                hidden = false;
            }

            Context = new(ProductModel!);
            messageStore = new(Context);
            Context.OnValidationRequested += HandleValidationRequested;

            productionCost = CalculateProductionCost();
        }

        // Private method for calculating Product's production cost
        private double CalculateProductionCost()
        {
            double productionCost = 0;

            if (ProductModel!.ProductDetailDtos.Count > 0)
            {
                foreach (var productDetailDto in ProductModel.ProductDetailDtos)
                {
                    productionCost += productDetailDto.MaterialPrice * productDetailDto.MaterialQty;
                }
            }

            return productionCost;
        }

        // Method which is invoked when form is submitted
        private async Task SubmitAsync()
        {
            var multipartContent = new MultipartFormDataContent();

            // If Id is greater than 0, then we have Edit operation
            if (Id > 0)
            {
                var id = ProductModel!.Id.ToString();
                multipartContent.Add(new StringContent(id), "Id");
            }

            multipartContent.Add(new StringContent(ProductModel!.Code), "Code");
            multipartContent.Add(new StringContent(ProductModel!.Name), "Name");
            multipartContent.Add(new StringContent(ProductModel!.CategoryId.ToString()), "CategoryId");
            multipartContent.Add(new StringContent(ProductModel!.Unit), "Unit");
            multipartContent.Add(new StringContent(ProductModel!.Price.ToString()), "Price");

            var serializedMaterialList = JsonConvert.SerializeObject(ProductModel!.ProductDetailDtos);

            multipartContent.Add(new StringContent(serializedMaterialList), "ProductDetailDtos");

            if (file != null)
            {
                var fileStreamContent = new StreamContent(file.OpenReadStream(50000000));
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                multipartContent.Add(fileStreamContent, name: "Image", fileName: file.Name);
            }

            object response;

            // If Id is equal to 0, then we have Create operation
            if (Id == 0)
            {
                response = await ProductService.CreateNewProductAsync(multipartContent);
            }
            // Otherwise we have Edit operation
            else
            {
                response = await ProductService.EditProductAsync(multipartContent);
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
            // and navigate to /products page
            else
            {
                Context!.Validate();
                NavigationManager.NavigateTo("/products");
            }
        }

        // Method that is invoked in response
        // to Choose file button click
        private async Task LoadFileAsync(InputFileChangeEventArgs args)
        {
            file = args.File;
            await JSRuntime.InvokeVoidAsync("setSrc");
        }

        // Method which is invoked in response
        // to Delete button click
        private async Task DeleteAsync()
        {
            // Prompt the user for confirmation
            // to delete Product
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this Product?");

            // If confirmed is true, then perform deletion of Product
            if (confirmed)
            {
                await ProductService.DeleteProductAsync(Id);
                NavigationManager.NavigateTo("/products");
            }
        }

        public void Dispose()
        {
            Context!.OnValidationRequested -= HandleValidationRequested;
        }

        // Method for handling OnChange event on Material drop down list
        private void HandleMaterialChanged(ChangeEventArgs args)
        {

            materialId = int.Parse(args.Value!.ToString()!);
        }

        // Method for handling OnChange event on Material quantity input
        private void HandleMaterialQtyChanged(ChangeEventArgs args)
        {
            materialQty = int.Parse(args.Value!.ToString()!);
        }

        // Method for handling Add button click event
        private void HandleAddMaterialClick()
        {
            ProductDetailDto productDetailDto = new();

            productDetailDto.MaterialId = materialId;
            productDetailDto.MaterialQty = materialQty;
            productDetailDto.MaterialName = MaterialList!.FirstOrDefault(e => e.Id == materialId)!.Name;
            productDetailDto.MaterialPrice = MaterialList!.FirstOrDefault(e => e.Id == materialId)!.Price;

            bool isNotValid = ValidateMaterialAdded();

            if (isNotValid == false)
            {
                ProductModel!.ProductDetailDtos.Add(productDetailDto);
                errorMessage = string.Empty;
                productionCost = CalculateProductionCost();
            }
        }

        // Method for handling Remove button click event
        private void HandleRemoveMaterialClick(int id)
        {
            ProductDetailDto productDetailDto = ProductModel!.ProductDetailDtos.FirstOrDefault(e => e.MaterialId == id)!;

            ProductModel.ProductDetailDtos.Remove(productDetailDto);

            productionCost = CalculateProductionCost();
        }

        // Method for checking if selected Material
        // is already added to Product's ProductDetailDtos collection
        // and that Material quantity is greater than 0 (zero)
        private bool ValidateMaterialAdded()
        {
            bool isNotValid = ProductModel!.ProductDetailDtos.Select(e => e.MaterialId).Contains(materialId) || materialQty <= 0;

            if (isNotValid == true)
            {
                errorMessage = "Error! Please verify that selected Material is not already added to list" +
                    " and that Material quantity is not less than or equal to 0 (zero)!";
            }

            return isNotValid;
        }
    }
}
