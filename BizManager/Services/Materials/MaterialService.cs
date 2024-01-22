using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using SharedProject.Dtos;
using System.Net.Http.Json;

namespace BizManager.Services.Materials
{
    // Implementation class for IMaterialService
    public class MaterialService:IMaterialService
    {
        private readonly HttpClient httpClient;

        public MaterialService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // Create new Material
        public async Task<object> CreateNewMaterialAsync(MultipartFormDataContent formDataContent)
        {
            // Invoke API method for creating new Material
            var response = await httpClient.PostAsync("api/materials/create", formDataContent);

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

        // Delete selected Material
        public async Task<int> DeleteMaterialAsync(int id)
        {
            // Invoke API method for deleting selected Material
            var response = await httpClient.DeleteAsync($"api/materials/delete/{id}");

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

        // Edit selected Material
        public async Task<object> EditMaterialAsync(MultipartFormDataContent formDataContent)
        {
            // Invoke API method for editing selected Material
            var response = await httpClient.PatchAsync("api/materials/patch", formDataContent);

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

        // Return paginated filtered list of MaterialDto objects
        public async Task<Pagination<MaterialDto>> GetMaterialsAsync(string? searchText, int categoryId, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["categoryId"] = categoryId.ToString();
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/materials";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            // Invoke API method for returning paginated filtered
            // list of MaterialDto objects
            var response = await httpClient.GetAsync(fullUrl);

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    var materials = await response.Content.ReadFromJsonAsync<Pagination<MaterialDto>>();

                    // If materials is null, then return new Pagination<MaterialDto>,
                    // otherwise return materials
                    return materials ?? new Pagination<MaterialDto>();
                }
                // Otherwise return new Pagination<MaterialDto> object
                else
                {
                    return new Pagination<MaterialDto>();
                }
            }
            // Otherwise return new Pagination<MaterialDto> object
            else
            {
                return new Pagination<MaterialDto>();
            }
        }

        // Return single MaterialDto object
        public async Task<MaterialDto> GetSingleMaterialAsync(int id)
        {
            // Invoke API method for retreiving specific MaterialDto object
            var response = await httpClient.GetAsync($"api/materials/{id}");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the result
                    MaterialDto? materialDto = await response.Content.ReadFromJsonAsync<MaterialDto>();

                    // If materialDto is not null, return materialDto
                    // Otherwise return new MaterialDto object
                    return materialDto ?? new MaterialDto();
                }
                // Otherwise return new MaterialDto
                else
                {
                    return new MaterialDto();
                }
            }
            // Otherwise return new MaterialDto
            else
            {
                return new MaterialDto();
            }
        }

        // Return all Material records
        public async Task<List<MaterialDto>> GetAllMaterialsAsync()
        {
            // Invoke API method for returning list
            // of all Material records
            var response = await httpClient.GetAsync("api/materials/getall");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    var materials = await response.Content.ReadFromJsonAsync<List<MaterialDto>>();

                    // If materials is null, then return new List<MaterialDto>,
                    // otherwise return materials
                    return materials ?? new List<MaterialDto>();
                }
                // Otherwise return new List<MaterialDto>
                else
                {
                    return new List<MaterialDto>();
                }
            }
            // Otherwise return new List<MaterialDto>
            else
            {
                return new List<MaterialDto>();
            }
        }
    }
}
