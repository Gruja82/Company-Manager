using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using SharedProject.Dtos;
using System.Net.Http.Json;

namespace BizManager.Services.Customers
{
    // Implmentation class for ICustomerService
    public class CustomerService:ICustomerService
    {
        private readonly HttpClient httpClient;

        public CustomerService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // Create new Customer
        public async Task<object> CreateNewCustomerAsync(MultipartFormDataContent formDataContent)
        {
            // Invoke API method for creating new Customer
            var response = await httpClient.PostAsync("api/customers/create", formDataContent);

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

        // Delete selected Customer
        public async Task<int> DeleteCustomerAsync(int id)
        {
            // Invoke API method for deleting selected Customer
            var response = await httpClient.DeleteAsync($"api/customers/delete/{id}");

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

        // Edit selected Customer
        public async Task<object> EditCustomerAsync(MultipartFormDataContent formDataContent)
        {
            // Invoke API method for editing selected Customer
            var response = await httpClient.PatchAsync("api/customers/patch", formDataContent);

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

        // Return paginated filtered list of CustomerDto objects
        public async Task<Pagination<CustomerDto>> GetCustomersAsync(string? searchText, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/customers";

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
                    var customers = await response.Content.ReadFromJsonAsync<Pagination<CustomerDto>>();

                    // If customers is null, then return new Pagination<CustomerDto>,
                    // otherwise return customers
                    return customers ?? new Pagination<CustomerDto>();
                }
                // Otherwise return new Pagination<CustomerDto>
                else
                {
                    return new Pagination<CustomerDto>();
                }
            }
            // Otherwise return new Pagination<CustomerDto>
            else
            {
                return new Pagination<CustomerDto>();
            }
        }

        // Return single CustomerDto object
        public async Task<CustomerDto> GetSingleCustomerAsync(int id)
        {
            // Invoke API method for retreiving specific CustomerDto object
            var response = await httpClient.GetAsync($"api/customers/{id}");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the result
                    CustomerDto? customerDto = await response.Content.ReadFromJsonAsync<CustomerDto>();

                    // If customerDto is not null, return customerDto
                    // Otherwise return new CustomerDto object
                    return customerDto ?? new CustomerDto();
                }
                // Otherwise return new CustomerDto
                else
                {
                    return new CustomerDto();
                }
            }
            // Otherwise return new CustomerDto
            else
            {
                return new CustomerDto();
            }
        }

        // Return all Customer records
        public async Task<List<CustomerDto>> GetAllCustomersAsync()
        {
            // Invoke API method for returning all CustomerDto objects
            var response = await httpClient.GetAsync("api/customers/getall");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the result
                    List<CustomerDto>? customerDtos = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();

                    // If customerDtos is not null, return customerDtos
                    // Otherwise return new List<CustomerDto> object
                    return customerDtos ?? new List<CustomerDto>();
                }
                // Otherwise return new List<CustomerDto>
                else
                {
                    return new List<CustomerDto>();
                }
            }
            // Otherwise return new List<CustomerDto>
            else
            {
                return new List<CustomerDto>();
            }
        }
    }
}
