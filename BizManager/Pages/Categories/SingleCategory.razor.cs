using BizManager.Services.Categories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SharedProject.Dtos;

namespace BizManager.Pages.Categories
{
    // Background logic for SingleCategory component
    public partial class SingleCategory:IDisposable
    {
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

        // Route parameter
        [Parameter]
        public int Id { get; set; }

        // Property that represents form's model
        private CategoryDto? CategoryModel { get; set; }

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
                    FieldIdentifier modelField = new(CategoryModel!, error.Key);
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
            CategoryModel = new();
            Context = new(new object());
            errors = new();
        }

        // Method which is invoked when component parameters are set
        protected override async Task OnParametersSetAsync()
        {
            // If Id is larger than 0, then component shows
            // CategoryDto in edit mode
            if (Id > 0)
            {
                // Set CategoryModel value
                CategoryModel = await CategoryService.GetSingleCategoryAsync(Id);
                // Show Delete button
                hidden = false;
            }

            Context = new(CategoryModel!);
            messageStore = new(Context);
            Context.OnValidationRequested += HandleValidationRequested;
        }

        // Method which is invoked when form is submitted
        private async Task SubmitAsync()
        {
            // If Id is equal to 0, then we have Create operation
            if (Id == 0)
            {
                // Invoke method for creating new Category
                var response = await CategoryService.CreateNewCategoryAsync(CategoryModel!);

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
                // and finally we are redirecting user to "/categories" page
                else
                {
                    Context!.Validate();
                    NavigationManager.NavigateTo("/categories");
                }
            }
            // Otherwise we have Edit operation
            else
            {
                // Invoke method for editing selected Category
                var response = await CategoryService.EditCategoryAsync(CategoryModel!);

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
                // and finally we are redirecting user to "/categories" page
                else
                {
                    Context!.Validate();
                    NavigationManager.NavigateTo("/categories");
                }
            }
        }

        // Method which handles Delete button click event
        private async Task DeleteAsync()
        {
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this Category?");

            if (confirmed)
            {
                await CategoryService.DeleteCategoryAsync(Id);
                NavigationManager.NavigateTo("/categories");
            }
        }

        public void Dispose()
        {
            if (Context is not null)
            {
                Context.OnValidationRequested -= HandleValidationRequested;
            }
        }
    }
}
