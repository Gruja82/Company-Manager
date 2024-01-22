using SharedProject.Dtos;

namespace BizManager.Services.Customers
{
    // Interface that defines methods for communicating with Server
    public interface ICustomerService
    {
        // Return paginated filtered list of CustomerDto objects
        Task<Pagination<CustomerDto>> GetCustomersAsync(string? searchText, int pageIndex, int pageSize);
        // Return single CustomerDto object
        Task<CustomerDto> GetSingleCustomerAsync(int id);
        // Create new Customer
        Task<object> CreateNewCustomerAsync(MultipartFormDataContent formDataContent);
        // Edit selected Customer
        Task<object> EditCustomerAsync(MultipartFormDataContent formDataContent);
        // Delete selected Customer
        Task<int> DeleteCustomerAsync(int id);
        // Return all Customer records
        Task<List<CustomerDto>> GetAllCustomersAsync();
    }
}
