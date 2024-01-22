using SharedProject.Dtos;

namespace BizManager.Services.Productions
{
    // Interface that defines methods for communicating with Server
    public interface IProductionService
    {
        // Return paginated filtered list of ProductionDto objects
        Task<Pagination<ProductionDto>> GetProductionsAsync(string? searchText, string? productionDate, int productId, int pageIndex, int pageSize);
        // Return single ProductionDto object
        Task<ProductionDto> GetSingleProductionAsync(int id);
        // Create new Production
        Task<object> CreateNewProductionAsync(ProductionDto productionDto);
        // Edit selected Production
        Task<object> EditProductionAsync(ProductionDto productionDto);
        // Delete selected Production
        Task<int> DeleteProductionAsync(int id);
        // Return list of all production dates
        Task<List<string>> GetAllDatesAsync();
    }
}
