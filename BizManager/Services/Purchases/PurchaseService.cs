using SharedProject.Dtos;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace BizManager.Services.Purchases
{
    // Implementation class for IPurchaseService
    public class PurchaseService:IPurchaseService
    {
        private readonly HttpClient httpClient;

        public PurchaseService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // Create new Purchase
        public async Task<object> CreateNewPurchaseAsync(PurchaseDto purchaseDto)
        {
            // Invoke API method for creating new Purchase
            var response = await httpClient.PostAsJsonAsync<PurchaseDto>("api/purchases/create", purchaseDto);

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

        // Delete selected Purchase
        public async Task<int> DeletePurchaseAsync(int id)
        {
            // Invoke API method for deleting selected Purchase
            var response = await httpClient.DeleteAsync($"api/purchases/delete/{id}");

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

        // Edit selected Purchase
        public async Task<object> EditPurchaseAsync(PurchaseDto purchaseDto)
        {
            // Invoke API method for editing selected Purchase
            var response = await httpClient.PatchAsJsonAsync<PurchaseDto>("api/purchases/create", purchaseDto);

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

        // Return list of Purchase dates
        public async Task<List<string>> GetPurchaseDatesAsync()
        {
            // Invoke API method for returning list of Purchase dates
            var response = await httpClient.GetAsync("api/purchases/getpurchasedates");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    List<string>? purchaseDates = await response.Content.ReadFromJsonAsync<List<string>>();

                    // If purchaseDates is null, then return new
                    // List<string>, otherwise
                    // return purchaseDates
                    return purchaseDates ?? new List<string>();
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

        // Return paginated filtered list of PurchaseDto objects
        public async Task<Pagination<PurchaseDto>> GetPurchasesAsync(string? searchText, string? purchaseDate, int supplierId, int materialId, int pageIndex, int pageSize)
        {
            // Dictionary that will be used to store query string values
            Dictionary<string, string> queryParams = new();

            // Add query string values to queryParams Dictionary
            queryParams["searchText"] = searchText ?? string.Empty;
            queryParams["orderDate"] = purchaseDate ?? string.Empty;
            queryParams["customerId"] = supplierId.ToString();
            queryParams["productId"] = materialId.ToString();
            queryParams["pageIndex"] = pageIndex == 0 ? 1.ToString() : pageIndex.ToString();
            queryParams["pageSize"] = pageSize == 0 ? 4.ToString() : pageSize.ToString();

            // Base API url
            string baseUrl = "api/purchases";

            // Generate query string values
            var queryBuilder = new QueryBuilder(queryParams);

            // Append queryBuilder to baseUrl
            string fullUrl = baseUrl + queryBuilder;

            // Invoke API method for returning paginated filtered
            // list of PurchaseDto objects
            var response = await httpClient.GetAsync(fullUrl);

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    var purchases = await response.Content.ReadFromJsonAsync<Pagination<PurchaseDto>>();

                    // If purchases is null, then return new
                    // Pagination<PurchaseDto>, otherwise
                    // return purchases
                    return purchases ?? new Pagination<PurchaseDto>();
                }
                // Otherwise return new Pagination<PurchaseDto>
                else
                {
                    return new Pagination<PurchaseDto>(); 
                }
            }
            // Otherwise return new Pagination<PurchaseDto>
            else
            {
                return new Pagination<PurchaseDto>();
            }
        }

        // Return single PurchaseDto object
        public async Task<PurchaseDto> GetSinglePurchaseAsync(int id)
        {
            // Invoke API method for returning single PurchaseDto object
            var response = await httpClient.GetAsync($"api/purchases/{id}");

            // If response is not null
            if (response != null)
            {
                // If returned status code is Success status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response
                    PurchaseDto? purchaseDto = await response.Content.ReadFromJsonAsync<PurchaseDto>();

                    // If purchaseDto is null, then return new
                    // PurchaseDto, otherwise
                    // return purchaseDto
                    return purchaseDto ?? new PurchaseDto();
                }
                // Otherwise return new PurchaseDto
                else
                {
                    return new PurchaseDto();
                }
            }
            // Otherwise return new PurchaseDto
            else
            {
                return new PurchaseDto();
            }
        }
    }
}
