using BizManager.Services.Customers;
using BizManager.Services.Suppliers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SharedProject.Dtos;
using System.Net.Http.Headers;

namespace BizManager.Pages.Suppliers
{
    // Background logic for SingleSupplier component
    public partial class SingleSupplier:IDisposable
    {
        // Inject ISupplierService
        [Inject]
        private ISupplierService SupplierService { get; set; } = default!;

        // Inject Navigation Manager
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

        // Property that represents form's model
        private SupplierDto? SupplierModel { get; set; }

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
                    FieldIdentifier modelField = new(SupplierModel!, error.Key);
                    messageStore!.Add(modelField, error.Value);
                }

                // Clear errors Dictionary
                errors!.Clear();
            }
        }

        // Method which is invoked when component is loaded
        // and in it we initialize all needed properties and fields
        protected override void OnInitialized()
        {
            SupplierModel = new();
            Context = new(new object());
            errors = new();
        }

        // Method which is invoked when component parameters are set
        protected override async Task OnParametersSetAsync()
        {
            // If Id is larger than 0, then component shows
            // SupplierDto in edit mode
            if (Id > 0)
            {
                // Set SupplierModel value
                SupplierModel = await SupplierService.GetSingleSupplierAsync(Id);
                // Show Delete button
                hidden = false;
            }

            Context = new(SupplierModel!);
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
                var id = SupplierModel!.Id.ToString();
                multipartContent.Add(new StringContent(id), "Id");
            }

            multipartContent.Add(new StringContent(SupplierModel!.Code), "Code");
            multipartContent.Add(new StringContent(SupplierModel!.Name), "Name");
            multipartContent.Add(new StringContent(SupplierModel!.Contact), "Contact");
            multipartContent.Add(new StringContent(SupplierModel!.Address), "Address");
            multipartContent.Add(new StringContent(SupplierModel!.City), "City");
            multipartContent.Add(new StringContent(SupplierModel!.Postal), "Postal");
            multipartContent.Add(new StringContent(SupplierModel!.Phone), "Phone");
            multipartContent.Add(new StringContent(SupplierModel!.Email), "Email");
            multipartContent.Add(new StringContent(SupplierModel!.Web ?? string.Empty), "Web");

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
                response = await SupplierService.CreateNewSupplierAsync(multipartContent);
            }
            // Otherwise we have Edit operation
            else
            {
                response = await SupplierService.EditSupplierAsync(multipartContent);
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
            // and navigate to /suppliers page
            else
            {
                Context!.Validate();
                NavigationManager.NavigateTo("/suppliers");
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
            // to delete Supplier
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this Supplier?");

            // If confirmed is true, then perform deletion of Supplier
            if (confirmed)
            {
                await SupplierService.DeleteSupplierAsync(Id);
                NavigationManager.NavigateTo("/suppliers");
            }
        }

        public void Dispose()
        {
            Context!.OnValidationRequested -= HandleValidationRequested;
        }
    }
}
