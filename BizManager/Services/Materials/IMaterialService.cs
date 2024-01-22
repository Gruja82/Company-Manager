using SharedProject.Dtos;

namespace BizManager.Services.Materials
{
    // Interface that defines methods for communicating with Server
    public interface IMaterialService
    {
        // Return paginated filtered list of MaterialDto objects
        Task<Pagination<MaterialDto>> GetMaterialsAsync(string? searchText, int categoryId, int pageIndex, int pageSize);
        // Return single MaterialDto object
        Task<MaterialDto> GetSingleMaterialAsync(int id);
        // Create new Material
        Task<object> CreateNewMaterialAsync(MultipartFormDataContent formDataContent);
        // Edit selected Material
        Task<object> EditMaterialAsync(MultipartFormDataContent formDataContent);
        // Delete selected Material
        Task<int> DeleteMaterialAsync(int id);
        // Return all Material records
        Task<List<MaterialDto>> GetAllMaterialsAsync();
    }
}
