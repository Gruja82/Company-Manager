using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using SharedProject.Dtos;
using System.Net.Http.Json;

namespace BizManager.Services.Categories
{
    // Implmentation class for ICategoryService
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient httpClient;

        public CategoryService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // Create new Category
        public async Task<object> CreateNewCategoryAsync(CategoryDto categoryDto)
        {
            // Invoke API method for creating new Category
            var response = await httpClient.PostAsJsonAsync<CategoryDto>("api/categories/create", categoryDto);

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

        // Delete selected Category
        public async Task<int> DeleteCategoryAsync(int id)
        {
            // Invoke API method for deleting selected Category
            var response = await httpClient.DeleteAsync($"api/categories/delete/{id}");

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

        // Edit selected Category
        public async Task<object> EditCategoryAsync(CategoryDto categoryDto)
        {
            // Invoke API method for editing selected Category
            var response = await httpClient.PatchAsJsonAsync<CategoryDto>("api/categories/patch", categoryDto);

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

        // Return paginated filtered list of CategoryDto objects
        public async Task<Pagination<CategoryDto>> GetCategoriesAsync(string? searchText, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/categories";

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
                    var categories = await response.Content.ReadFromJsonAsync<Pagination<CategoryDto>>();

                    // If categories is null, then return new
                    // Pagination<CategoryDto>, otherwise
                    // return categories
                    return categories ?? new Pagination<CategoryDto>();
                }
                // Otherwise return new Pagination<CategoryDto>
                else
                {
                    return new Pagination<CategoryDto>();
                }
            }
            // Otherwise return new Pagination<CategoryDto>
            else
            {
                return new Pagination<CategoryDto>();
            }
        }

        // Return single CategoryDto object
        public async Task<CategoryDto> GetSingleCategoryAsync(int id)
        {
            // Invoke API method for retreiving specific CategoryDto object
            var response = await httpClient.GetAsync($"api/categories/{id}");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the result
                    CategoryDto? categoryDto = await response.Content.ReadFromJsonAsync<CategoryDto>();

                    // If categoryDto is not null, return categoryDto
                    // Otherwise return new CategoryDto object
                    return categoryDto ?? new CategoryDto();
                }
                // Otherwise return new CategoryDto
                else
                {
                    return new CategoryDto();
                }
            }
            // Otherwise return new CategoryDto
            else
            {
                return new CategoryDto();
            }
        }

        // Return all CategoryDto objects
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            // Invoke API method for retreiving list of CategoryDto objects
            var response = await httpClient.GetAsync("api/categories/getall");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();

                    // If categories is null, then return new
                    // List<CategoryDto>, otherwise
                    // return categories
                    return categories ?? new List<CategoryDto>();
                }
                // Otherwise return new List<CategoryDto>
                else
                {
                    return new List<CategoryDto>();
                }
            }
            // Otherwise return new List<CategoryDto>
            else
            {
                return new List<CategoryDto>();
            }
        }
    }
}
