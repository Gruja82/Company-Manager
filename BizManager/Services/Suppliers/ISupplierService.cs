using SharedProject.Dtos;

namespace BizManager.Services.Suppliers
{
    // Interface that defines methods for communicating with Server
    public interface ISupplierService
    {
        // Return paginated filtered list of SupplierDto objects
        Task<Pagination<SupplierDto>> GetSuppliersAsync(string? searchText, int pageIndex, int pageSize);
        // Return single SupplierDto object
        Task<SupplierDto> GetSingleSupplierAsync(int id);
        // Create new Supplier
        Task<object> CreateNewSupplierAsync(MultipartFormDataContent formDataContent);
        // Edit selected Supplier
        Task<object> EditSupplierAsync(MultipartFormDataContent formDataContent);
        // Delete selected Supplier
        Task<int> DeleteSupplierAsync(int id);
        // Return all Supplier records
        Task<List<SupplierDto>> GetAllSuppliersAsync();
    }
}
