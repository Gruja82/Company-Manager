using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using SharedProject.Dtos;
using System.Net.Http.Json;

namespace BizManager.Services.Orders
{
    // Implementation class for IOrderService
    public class OrderService:IOrderService
    {
        private readonly HttpClient httpClient;

        public OrderService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // Create new Order
        public async Task<object> CreateNewOrderAsync(OrderDto orderDto)
        {
            // Invoke API method for creating new Order
            var response = await httpClient.PostAsJsonAsync<OrderDto>("api/orders/create", orderDto);

            // If returned result is not null,
            if (response != null)
            {
                // If returned status code is 201 Created, then return
                // simple string 
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return "Created";
                }
                // Otherwise return Dictionary containg errors
                else
                {
                    var errors = await response.Content.ReadFromJsonAsync<IDictionary<string, string>>();
                    return errors ?? new Dictionary<string, string>();
                }
            }
            // Otherwise throw exception
            else
            {
                throw new Exception("Unexpected error!");
            }
        }

        // Delete selected Order
        public async Task<int> DeleteOrderAsync(int id)
        {
            // Invoke API method for deleting selected Order
            var response = await httpClient.DeleteAsync($"api/orders/delete/{id}");

            // If returned status code marks success
            if (response.IsSuccessStatusCode)
            {
                // Return status code 204 - No Content
                return StatusCodes.Status204NoContent;
            }
            // Otherwise - return status code 400 BadRequest
            else
            {
                return StatusCodes.Status400BadRequest;
            }
        }

        // Edit selected Order
        public async Task<object> EditOrderAsync(OrderDto orderDto)
        {
            // Invoke API method for editing selected Order
            var response = await httpClient.PatchAsJsonAsync("api/orders/patch", orderDto);

            // If returned result is not null,
            if (response != null)
            {
                // If returned status code is 204 NoContent, then return
                // simple string 
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return "Created";
                }
                // Otherwise return Dictionary containg errors
                else
                {
                    var errors = await response.Content.ReadFromJsonAsync<IDictionary<string, string>>();
                    return errors ?? new Dictionary<string, string>();
                }
            }
            // Otherwise throw exception
            else
            {
                throw new Exception("Unexpected error!");
            }
        }

        // Return paginated filtered list of OrderDto objects
        public async Task<Pagination<OrderDto>> GetOrdersAsync(string? searchText, string? orderDate,int customerId, int productId, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["orderDate"] = orderDate ?? string.Empty;
            queryParams["customerId"] = customerId.ToString();
            queryParams["productId"] = productId.ToString();
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/orders";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            // Invoke API method for returning paginated filtered
            // list of CategoryDto objects
            var response = await httpClient.GetAsync(fullUrl);

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    var orders = await response.Content.ReadFromJsonAsync<Pagination<OrderDto>>();

                    // If orders is null, then return new
                    // Pagination<OrderDto>, otherwise
                    // return orders
                    return orders ?? new Pagination<OrderDto>();
                }
                // Otherwise return new Pagination<OrderDto>
                else
                {
                    return new Pagination<OrderDto>();
                }
            }
            // Otherwise return new Pagination<OrderDto>
            else
            {
                return new Pagination<OrderDto>();
            }
        }

        // Return single OrderDto object
        public async Task<OrderDto> GetSingleOrderAsync(int id)
        {
            // Invoke API method for returning single OrderDto object
            var response = await httpClient.GetAsync($"api/orders/{id}");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    OrderDto? orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();

                    // If orderDto is null, then return new
                    // OrderDto, otherwise
                    // return orderDto
                    return orderDto ?? new OrderDto();
                }
                // Otherwise return new OrderDto
                else
                {
                    return new OrderDto();
                }
            }
            // Otherwise return new OrderDto
            else
            {
                return new OrderDto();
            }
        }

        // Return list of Order dates
        public async Task<List<string>> GetOrderDatesAsync()
        {
            // Invoke API method for returning list of Order dates
            var response = await httpClient.GetAsync("api/orders/getorderdates");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    List<string>? orderDates = await response.Content.ReadFromJsonAsync<List<string>>();

                    // If orderDates is null, then return new
                    // List<string>, otherwise
                    // return orderDates
                    return orderDates ?? new List<string>();
                }
                // Otherwise return new List<string>
                else
                {
                    return new List<string>();
                }
            }
            // Otherwise return new List<string>
            else
            {
                return new List<string>();
            }
        }
    }
}
