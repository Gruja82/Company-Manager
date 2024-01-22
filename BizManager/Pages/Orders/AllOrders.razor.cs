using BizManager.Services.Customers;
using BizManager.Services.Orders;
using BizManager.Services.Products;
using Microsoft.AspNetCore.Components;
using SharedProject.Dtos;

namespace BizManager.Pages.Orders
{
    // Background logic for AllCOrders component
    public partial class AllOrders
    {
        // Inject IOrderService
        [Inject]
        private IOrderService OrderService { get; set; } = default!;

        // Inject IProductService
        [Inject]
        private IProductService ProductService { get; set; } = default!;

        // Inject ICustomerService
        [Inject]
        private ICustomerService CustomerService { get; set; } = default!;

        // Inject NavigationManager
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // Property that represents collection of OrderDto objects
        private Pagination<OrderDto>? OrderList { get; set; }

        // Property that represents list of Order dates
        private List<string>? OrderDates { get; set; }

        // Property that represents list of Products
        private List<ProductDto>? ProductList { get; set; }

        // Property that represents list of Customers
        private List<CustomerDto>? CustomerList { get; set; }

        // Field that represents Search term
        // used for SearchComponent
        private string? searchText;

        // Field that represents Order Date
        private string? orderDate;

        // Field that represents Customer Id
        private int customerId;

        // Field that represents Product Id
        private int productId;

        // Field that represents Page number
        // used for PaginationComponent
        private int pageIndex;

        // Field that represents Page size
        // used for PaginationComponent
        private int pageSize;

        // When component is loaded first time, fill the
        // OrderList, OrderDates, ProductList and CustomerList
        protected override async Task OnInitializedAsync()
        {
            OrderList = await OrderService.GetOrdersAsync(searchText, orderDate, customerId, productId, pageIndex, pageSize);
            OrderDates = await OrderService.GetOrderDatesAsync();
            ProductList = await ProductService.GetAllProductsAsync();
            CustomerList = await CustomerService.GetAllCustomersAsync();
        }

        // Method for handling button click event in SearchComponent
        private async Task HandleSearchClickAsync(string strValue)
        {
            // Set searchText field value to the value of strValue
            searchText = strValue;
            // Reset pageIndex value
            pageIndex = default;
            // Fill the OrderList
            OrderList = await OrderService.GetOrdersAsync(searchText, orderDate,customerId, productId, pageIndex, pageSize);
        }

        // Method for handling date's onchange event
        private async Task HandleDateChangedAsync(ChangeEventArgs args)
        {
            orderDate = args.Value!.ToString()!;
            // Reset pageIndex value
            pageIndex = default;
            // Fill the OrderList
            OrderList = await OrderService.GetOrdersAsync(searchText, orderDate, customerId, productId, pageIndex, pageSize);
        }

        // Method for handling product's onchange event
        private async Task HandleProductChangedAsync(ChangeEventArgs args)
        {
            productId = int.Parse(args.Value!.ToString()!);
            // Reset pageIndex value
            pageIndex = default;
            // Fill the OrderList
            OrderList = await OrderService.GetOrdersAsync(searchText, orderDate, customerId, productId, pageIndex, pageSize);
        }

        // Method for handling customer's onchange event
        private async Task HandleCustomerChangedAsync(ChangeEventArgs args)
        {
            customerId = int.Parse(args.Value!.ToString()!);
            // Reset pageIndex value
            pageIndex = default;
            // Fill the OrderList
            OrderList = await OrderService.GetOrdersAsync(searchText, orderDate, customerId, productId, pageIndex, pageSize);
        }

        // Method for handling PaginationComponent's page number
        // button click event
        private async Task HandlePageChangedClickAsync(int pageNumber)
        {
            // Set pageIndex field value to the value of pageNumber
            pageIndex = pageNumber;
            // Fill the OrderList
            OrderList = await OrderService.GetOrdersAsync(searchText, orderDate, customerId, productId, pageIndex, pageSize);
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
            // Fill the OrderList
            OrderList = await OrderService.GetOrdersAsync(searchText, orderDate, customerId, productId, pageIndex, pageSize);
        }

        // Method for navigating to Page for creating new Order
        private void NavigateToCreatePage()
        {
            NavigationManager.NavigateTo("/order");
        }

        // Method for navigating to Page for updating existing Order
        private void NavigateToEditPage(int id)
        {
            NavigationManager.NavigateTo($"/order/{id}");
        }
    }
}
