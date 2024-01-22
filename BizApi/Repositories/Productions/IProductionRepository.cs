using SharedProject.Dtos;

namespace BizApi.Repositories.Productions
{
    // Interface that declares operations that can be applied
    // on ProductionDto objects
    public interface IProductionRepository
    {
        // Return collection of ProductionDto objects
        Task<Pagination<ProductionDto>> GetProductionsCollectionAsync(string? searchText, string? productionDate, int productId, int pageIndex, int pageSize);
        // Return single ProductionDto object
        Task<ProductionDto> GetSingleProductionAsync(int id);
        // Create new Production
        Task CreateNewProductionAsync(ProductionDto productionDto);
        // Edit selected Production
        Task EditProductionAsync(ProductionDto productionDto);
        // Delete selected Production
        Task DeleteProductionAsync(int id);
        // Custom validation
        Task<Dictionary<string, string>> ValidateProductionAsync(ProductionDto productionDto);
        // Return all Production dates
        Task<List<string>> GetAllDatesAsync();
    }
}
