using BizManager.Services.Customers;
using BizManager.Services.Orders;
using BizManager.Services.Products;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SharedProject.Dtos;

namespace BizManager.Pages.Orders
{
    // Background logic for SingleOrder component
    public partial class SingleOrder:IDisposable
    {
        // Inject IOrderService
        [Inject]
        private IOrderService OrderService { get; set; } = default!;

        // Inject ICustomerService
        [Inject]
        private ICustomerService CustomerService { get; set; } = default!;

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

        // Field that represents total order value
        double orderTotal = 0;

        // Field that represents Customer Id
        int customerId = 0;

        // Field that represents Product Id
        int productId = 0;

        // Field that represents Product quantity
        int productQty = 0;

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

        // Property that represents list of Customers
        private List<CustomerDto>? CustomerList { get; set; }

        // Property that represents list of Products
        private List<ProductDto>? ProductList { get; set; }

        // Property that represents form's model
        private OrderDto? OrderModel { get; set; }

        // Method for manual activating model validation
        private void HandleValidationRequested(object? sender, ValidationRequestedEventArgs args)
        {
            messageStore?.Clear();

            if (errors!.Any())
            {
                foreach (var error in errors!)
                {
                    FieldIdentifier modelField = new(OrderModel!, error.Key);
                    messageStore!.Add(modelField, error.Value);
                }

                errors!.Clear();
            }
        }

        // Method which is invoked when component is loaded
        // and in it we initialize all needed properties and fields
        protected override async Task OnInitializedAsync()
        {
            OrderModel = new();
            Context = new(new object());
            errors = new();
            CustomerList = await CustomerService.GetAllCustomersAsync();
            ProductList = await ProductService.GetAllProductsAsync();
        }

        // Method which is invoked when component parameters are set
        protected override async Task OnParametersSetAsync()
        {
            // If Id is greater than 0, it means that Order is used
            // in Edit operation, so we find Order by Id, and show Delete button
            if (Id > 0)
            {
                OrderModel = await OrderService.GetSingleOrderAsync(Id);

                hidden = false;
            }

            Context = new(OrderModel!);
            messageStore = new(Context);
            Context.OnValidationRequested += HandleValidationRequested;

            orderTotal = CalculateOrderTotal();
        }

        // Private method for calculating Order's total value
        private double CalculateOrderTotal()
        {
            double total = 0;

            // Iterate through OrderDetailDtos list and calculate
            // order value by multiplying Product's quantity and
            // Product's price and addind that value to total
            if (OrderModel!.OrderDetailDtos.Count > 0)
            {
                foreach (var orderDetailDto in OrderModel.OrderDetailDtos)
                {
                    total += orderDetailDto.Quantity * orderDetailDto.ProductPrice;
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
                // Invoke method for creating new Order
                response = await OrderService.CreateNewOrderAsync(OrderModel!);
            }
            // Otherwise we have Edit operation
            else
            {
                // Invoke method for editing selected Order
                response = await OrderService.EditOrderAsync(OrderModel!);
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
            // and navigate to /orders page
            else
            {
                Context!.Validate();
                NavigationManager.NavigateTo("/orders");
            }
        }

        // Method which is invoked in response
        // to Delete button click
        private async Task DeleteAsync()
        {
            // Prompt the user for confirmation
            // to delete Order
            bool confirmed = await  JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this order?");

            // If confirmed is true, then perform deletion of Order
            if (confirmed)
            {
                await OrderService.DeleteOrderAsync(Id);
                NavigationManager.NavigateTo("/orders");
            }
        }

        public void Dispose()
        {
            Context!.OnValidationRequested -= HandleValidationRequested;
        }

        // Method for handling onchange event on Product drop down list
        private void HandleProductChanged(ChangeEventArgs args)
        {
            productId = int.Parse(args.Value!.ToString()!);
        }

        // Method for handling onchange event on Product quantity input
        private void HandleProductQtyChanged(ChangeEventArgs args)
        {
            productQty = int.Parse(args.Value!.ToString()!);
        }

        // Method for handling Add button click event
        private void HandleAddProductClick()
        {
            OrderDetailDto orderDetailDto = new();

            orderDetailDto.ProductId = productId;
            orderDetailDto.Quantity = productQty;
            orderDetailDto.ProductName = ProductList!.FirstOrDefault(e => e.Id == productId)!.Name;
            orderDetailDto.ProductPrice = ProductList!.FirstOrDefault(e => e.Id == productId)!.Price;

            bool isNotValid = ValidateProductAdded();

            if (isNotValid == false)
            {
                OrderModel!.OrderDetailDtos.Add(orderDetailDto);
                errorMessage = string.Empty;
                orderTotal = CalculateOrderTotal();
            }
        }

        // Method for handling Remove button click event
        private void HandleRemoveProductClick(int id)
        {
            OrderDetailDto orderDetailDto = OrderModel!.OrderDetailDtos.FirstOrDefault(e => e.ProductId == id)!;

            OrderModel!.OrderDetailDtos.Remove(orderDetailDto);

            orderTotal = CalculateOrderTotal();

            errorMessage = string.Empty;
        }

        // Method for checking if selected Product
        // is already added to Order's OrderDetailDtos collection
        // and that Product quantity is greater than 0 (zero)
        private bool ValidateProductAdded()
        {
            bool isNotValid = OrderModel!.OrderDetailDtos.Select(e => e.ProductId).Contains(productId) || productQty <= 0;

            if (isNotValid == true)
            {
                errorMessage= "Error! Please verify that selected Product is not already added to list" +
                    " and that Product quantity is not less than or equal to 0 (zero)!";
            }

            return isNotValid;
        }
    }
}
