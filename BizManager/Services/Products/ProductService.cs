using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using SharedProject.Dtos;
using System.Net.Http.Json;

namespace BizManager.Services.Products
{
    // Implementation class for IProductService
    public class ProductService:IProductService
    {
        private readonly HttpClient httpClient;

        public ProductService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // Create new Product
        public async Task<object> CreateNewProductAsync(MultipartFormDataContent formDataContent)
        {
            // Invoke API method for creating new Product
            var response = await httpClient.PostAsync("api/products/create", formDataContent);

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

        // Delete selected Product
        public async Task<int> DeleteProductAsync(int id)
        {
            // Invoke API method for deleting selected Product
            var response = await httpClient.DeleteAsync($"api/products/delete/{id}");

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

        // Edit selected Product
        public async Task<object> EditProductAsync(MultipartFormDataContent formDataContent)
        {
            // Invoke API method for editing selected Product
            var response = await httpClient.PatchAsync("api/products/patch", formDataContent);

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

        // Return paginated filtered list of ProductDto objects
        public async Task<Pagination<ProductDto>> GetProductsAsync(string? searchText, int categoryId, int minimum, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["categoryId"] = categoryId.ToString();
            queryParams["minimum"] = minimum.ToString();
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/products";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            // Invoke API method for returning paginated filtered
            // list of ProductDto objects
            var response = await httpClient.GetAsync(fullUrl);

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    var products = await response.Content.ReadFromJsonAsync<Pagination<ProductDto>>();

                    // If products is null, then return new Pagination<ProductDto>,
                    // otherwise return products
                    return products ?? new Pagination<ProductDto>();
                }
                // Otherwise return new Pagination<ProductDto> object
                else
                {
                    return new Pagination<ProductDto>();
                }
            }
            // Otherwise return new Pagination<ProductDto> object
            else
            {
                return new Pagination<ProductDto>();
            }
        }

        // Return single ProductDto object
        public async Task<ProductDto> GetSingleProductAsync(int id)
        {
            // Invoke API method for retreiving specific ProductlDto object
            var response = await httpClient.GetAsync($"api/products/{id}");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the result
                    ProductDto? productDto = await response.Content.ReadFromJsonAsync<ProductDto>();

                    // If productDto is not null, return productDto
                    // Otherwise return new ProductDto object
                    return productDto ?? new ProductDto();
                }
                // Otherwise return new ProductDto
                else
                {
                    return new ProductDto();
                }
            }
            // Otherwise return new ProductDto
            else
            {
                return new ProductDto();
            }
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var response = await httpClient.GetAsync("api/products/getall");

            if (response != null)
            {
                if (response.IsSuccessStatusCode)
                {
                    List<ProductDto>? productDtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>();

                    return productDtos ?? new List<ProductDto>();
                }
                else
                {
                    return new List<ProductDto>();
                }
            }
            else
            {
                return new List<ProductDto>();
            }
        }
    }
}
