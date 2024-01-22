using SharedProject.Dtos;

namespace BizApi.Repositories.Materials
{
    // Interface that declares operations that can be applied
    // on MaterialDto objects
    public interface IMaterialRepository
    {
        // Return collection of MaterialDto objects
        Task<Pagination<MaterialDto>> GetMaterialsCollectionAsync(string? searchText, int categoryId, int pageIndex, int pageSize);
        // Return single MaterialDto object
        Task<MaterialDto> GetSingleMaterialAsync(int id);
        // Create new Material
        Task CreateNewMaterialAsync(MaterialDto materialDto, string imagesFolder);
        // Edit selected Material
        Task EditMaterialAsync(MaterialDto materialDto, string imagesFolder);
        // Delete selected Material
        Task DeleteMaterialAsync(int id, string imagesFolder);
        // Custom validation
        Task<Dictionary<string, string>> ValidateMaterialAsync(MaterialDto materialDto);
        // Return all Material records
        Task<List<MaterialDto>> GetAllMaterialsAsync();
    }
}
