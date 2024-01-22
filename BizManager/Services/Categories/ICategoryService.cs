using SharedProject.Dtos;

namespace BizManager.Services.Categories
{
    // Interface that defines methods for communicating with Server
    public interface ICategoryService
    {
        // Return paginated filtered list of CategoryDto objects
        Task<Pagination<CategoryDto>> GetCategoriesAsync(string? searchText, int pageIndex, int pageSize);
        // Return single CategoryDto object
        Task<CategoryDto> GetSingleCategoryAsync(int id);
        // Create new Category
        Task<object> CreateNewCategoryAsync(CategoryDto categoryDto);
        // Edit selected Category
        Task<object> EditCategoryAsync(CategoryDto categoryDto);
        // Delete selected Category
        Task<int> DeleteCategoryAsync(int id);
        // Return all CategoryDto objects
        Task<List<CategoryDto>> GetAllCategoriesAsync();
    }
}
