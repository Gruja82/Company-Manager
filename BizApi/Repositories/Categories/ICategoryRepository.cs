using SharedProject.Dtos;

namespace BizApi.Repositories.Categories
{
    // Interface that declares operations that can be applied
    // on CategoryDto objects
    public interface ICategoryRepository
    {
        // Return collection of CategoryDto objects
        Task<Pagination<CategoryDto>> GetCategoriesCollectionAsync(string? searchText, int pageIndex, int pageSize);
        // Return single CategoryDto object
        Task<CategoryDto> GetSingleCategoryAsync(int id);
        // Create new Category
        Task CreateNewCategoryAsync(CategoryDto categoryDto);
        // Edit selected Category
        Task EditCategoryAsync(CategoryDto categoryDto);
        // Delete selected Category
        Task DeleteCategoryAsync(int id);
        // Custom validation
        Task<Dictionary<string, string>> ValidateCategoryAsync(CategoryDto categoryDto);
        // Return all Category records
        Task<List<CategoryDto>> GetAllCategories();
    }
}
