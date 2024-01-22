using SharedProject.Dtos;

namespace BizApi.Repositories.Suppliers
{
    // Interface that declares operations that can be applied
    // on SupplierDto objects
    public interface ISupplierRepository
    {
        // Return collection of SupplierDto objects
        Task<Pagination<SupplierDto>> GetSupplierCollectionAsync(string searchText, int pageIndex, int pageSize);
        // Return single SupplierDto object
        Task<SupplierDto> GetSingleSupplierAsync(int id);
        // Create new Supplier
        Task CreateNewSupplierAsync(SupplierDto supplierDto, string imagesFolder);
        // Edit selected Supplier
        Task EditSupplierAsync(SupplierDto supplierDto, string imagesFolder);
        // Delete selected Supplier
        Task DeleteSupplierAsync(int id, string imagesFolder);
        // Custom validation
        Task<Dictionary<string, string>> ValidateSupplierAsync(SupplierDto supplierDto);
        // Return all Supplier records
        Task<List<SupplierDto>> GetAllSuppliersAsync();
    }
}
