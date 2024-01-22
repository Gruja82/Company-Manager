using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using SharedProject.Dtos;
using System.Net.Http.Json;

namespace BizManager.Services.Suppliers
{
    // Implementation class for ISupplierService
    public class SupplierService:ISupplierService
    {
        private readonly HttpClient httpClient;

        public SupplierService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // Create new Supplier
        public async Task<object> CreateNewSupplierAsync(MultipartFormDataContent formDataContent)
        {
            // Invoke API method for creating new Supplier
            var response = await httpClient.PostAsync("api/suppliers/create", formDataContent);

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

        // Delete selected Supplier
        public async Task<int> DeleteSupplierAsync(int id)
        {
            // Invoke API method for deleting selected Supplier
            var response = await httpClient.DeleteAsync($"api/suppliers/delete/{id}");

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

        // Edit selected Supplier
        public async Task<object> EditSupplierAsync(MultipartFormDataContent formDataContent)
        {
            // Invoke API method for editing selected Supplier
            var response = await httpClient.PatchAsync("api/suppliers/patch", formDataContent);

            // If returned result is not null,
            if (response != null)
            {
                // If returned status code is 204 No Content, then return
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

        // Return all Supplier records
        public async Task<List<SupplierDto>> GetAllSuppliersAsync()
        {
            // Invoke API method for returning all SupplierDto objects
            var response = await httpClient.GetAsync("api/suppliers/getall");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the result
                    List<SupplierDto>? supplierDtos = await response.Content.ReadFromJsonAsync<List<SupplierDto>>();

                    // If supplierDtos is not null, return supplierDtos
                    // Otherwise return new List<SupplierDto> object
                    return supplierDtos ?? new List<SupplierDto>();
                }
                // Otherwise return new List<SupplierDto>
                else
                {
                    return new List<SupplierDto>();
                }
            }
            // Otherwise return new List<SupplierDto>
            else
            {
                return new List<SupplierDto>();
            }
        }

        // Return single SupplierDto object
        public async Task<SupplierDto> GetSingleSupplierAsync(int id)
        {
            // Invoke API method for retreiving specific SupplierDto object
            var response = await httpClient.GetAsync($"api/suppliers/{id}");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the result
                    SupplierDto? supplierDto = await response.Content.ReadFromJsonAsync<SupplierDto>();

                    // If supplierDto is not null, return supplierDto
                    // Otherwise return new SupplierDto object
                    return supplierDto ?? new SupplierDto();
                }
                // Otherwise return new SupplierDto
                else
                {
                    return new SupplierDto();
                }
            }
            // Otherwise return new SupplierDto
            else
            {
                return new SupplierDto();
            }
        }

        // Return paginated filtered list of SupplierDto objects
        public async Task<Pagination<SupplierDto>> GetSuppliersAsync(string? searchText, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/suppliers";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            // Invoke API method for returning paginated filtered
            // list of SupplierDto objects
            var response = await httpClient.GetAsync(fullUrl);

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    var suppliers = await response.Content.ReadFromJsonAsync<Pagination<SupplierDto>>();

                    // If suppliers is null, then return new Pagination<SupplierDto>,
                    // otherwise return suppliers
                    return suppliers ?? new Pagination<SupplierDto>();
                }
                // Otherwise return new Pagination<SupplierDto>
                else
                {
                    return new Pagination<SupplierDto>();
                }
            }
            // Otherwise return new Pagination<SupplierDto>
            else
            {
                return new Pagination<SupplierDto>();
            }
        }
    }
}
