using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using SharedProject.Dtos;
using System.Net.Http.Json;

namespace BizManager.Services.Productions
{
    // Implementation class for IProductionService
    public class ProductionService:IProductionService
    {
        private readonly HttpClient httpClient;

        public ProductionService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // Create new Production
        public async Task<object> CreateNewProductionAsync(ProductionDto productionDto)
        {
            // Invoke API method for creating new Production
            var response = await httpClient.PostAsJsonAsync<ProductionDto>("api/productions/create", productionDto);

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

        // Delete selected Production
        public async Task<int> DeleteProductionAsync(int id)
        {
            // Invoke API method for deleting selected Production
            var response = await httpClient.DeleteAsync($"api/productions/delete/{id}");

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

        // Edit selected Production
        public async Task<object> EditProductionAsync(ProductionDto productionDto)
        {
            // Invoke API method for editing selected Production
            var response = await httpClient.PatchAsJsonAsync<ProductionDto>("api/productions/patch", productionDto);

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

        // Return paginated filtered list of ProductionDto objects
        public async Task<Pagination<ProductionDto>> GetProductionsAsync(string? searchText, string? productionDate, int productId, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["productionDate"] = productionDate ?? string.Empty;
            queryParams["productId"] = productId.ToString();
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/productions";

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
                    var productions = await response.Content.ReadFromJsonAsync<Pagination<ProductionDto>>();

                    // If productions is null, then return new
                    // Pagination<ProductionDto>, otherwise
                    // return productions
                    return productions ?? new Pagination<ProductionDto>();
                }
                // Otherwise return new Pagination<ProductionDto>
                else
                {
                    return new Pagination<ProductionDto>();
                }
            }
            // Otherwise return new Pagination<ProductionDto>
            else
            {
                return new Pagination<ProductionDto>();
            }
        }

        // Return single ProductionDto object
        public async Task<ProductionDto> GetSingleProductionAsync(int id)
        {
            // Invoke API method for retreiving specific ProductionDto object
            var response = await httpClient.GetAsync($"api/productions/{id}");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the result
                    ProductionDto? productionDto = await response.Content.ReadFromJsonAsync<ProductionDto>();

                    // If productionDto is not null, return productionDto
                    // Otherwise return new ProductionDto object
                    return productionDto ?? new ProductionDto();
                }
                // Otherwise return new ProductionDto
                else
                {
                    return new ProductionDto();
                }
            }
            // Otherwise return new ProductionDto
            else
            {
                return new ProductionDto();
            }
        }

        // Return list of all production dates
        public async Task<List<string>> GetAllDatesAsync()
        {
            // Invoke API method for returning list of all production dates
            var response = await httpClient.GetAsync("api/productions/alldates");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the result
                    List<string>? allDates = await response.Content.ReadFromJsonAsync<List<string>>();

                    // If allDates is not null, return allDates
                    // Otherwise return new List<string> object
                    return allDates ?? new List<string>();
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
