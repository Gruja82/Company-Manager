using BizManager.Services.Categories;
using BizManager.Services.Customers;
using BizManager.Services.Materials;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SharedProject.Dtos;
using System.Net.Http.Headers;

namespace BizManager.Pages.Materials
{
    // Background logic for SingleMaterial component
    public partial class SingleMaterial:IDisposable
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

        // Field that represents image file
        private IBrowserFile? file;

        // Route parameter
        [Parameter]
        public int Id { get; set; }

        // Property that represents list of CategoryDto objects
        private List<CategoryDto>? CategoryList { get; set; }

        // Property that represents form's model
        private MaterialDto? MaterialModel { get; set; }

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
                    FieldIdentifier modelField = new(MaterialModel!, error.Key);
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
            MaterialModel = new();
            Context = new(new object());
            errors = new();
            CategoryList = await CategoryService.GetAllCategoriesAsync();
        }

        // Method which is invoked when component parameters are set
        protected override async Task OnParametersSetAsync()
        {
            // If Id is larger than 0, then component shows
            // MaterialDto in edit mode
            if (Id > 0)
            {
                // Set MaterialModel value
                MaterialModel = await MaterialService.GetSingleMaterialAsync(Id);
                // Show Delete button
                hidden = false;
            }

            Context = new(MaterialModel!);
            messageStore = new(Context);
            Context.OnValidationRequested += HandleValidationRequested;
        }

        // Method which is invoked when form is submitted
        private async Task SubmitAsync()
        {
            var multipartContent = new MultipartFormDataContent();

            // If Id is greater than 0, then we have Edit operation
            if (Id > 0)
            {
                var id = MaterialModel!.Id.ToString();
                multipartContent.Add(new StringContent(id), "Id");
            }

            multipartContent.Add(new StringContent(MaterialModel!.Code), "Code");
            multipartContent.Add(new StringContent(MaterialModel!.Name), "Name");
            multipartContent.Add(new StringContent(MaterialModel!.CategoryId.ToString()), "CategoryId");
            multipartContent.Add(new StringContent(MaterialModel!.Unit), "Unit");
            multipartContent.Add(new StringContent(MaterialModel!.Price.ToString()), "Price");

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
                response = await MaterialService.CreateNewMaterialAsync(multipartContent);
            }
            // Otherwise we have Edit operation
            else
            {
                response = await MaterialService.EditMaterialAsync(multipartContent);
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
            // and navigate to /materials page
            else
            {
                Context!.Validate();
                NavigationManager.NavigateTo("/materials");
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
            // to delete Material
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this Material?");

            // If confirmed is true, then perform deletion of Material
            if (confirmed)
            {
                await MaterialService.DeleteMaterialAsync(Id);
                NavigationManager.NavigateTo("/materials");
            }
        }

        public void Dispose()
        {
            Context!.OnValidationRequested -= HandleValidationRequested;
        }
    }
}
