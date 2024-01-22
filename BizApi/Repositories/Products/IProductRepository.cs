using SharedProject.Dtos;

namespace BizApi.Repositories.Products
{
    // Interface that declares operations that can be applied
    // on ProductDto objects
    public interface IProductRepository
    {
        // Return collection of ProductDto objects
        Task<Pagination<ProductDto>> GetProductsCollectionAsync(string? searchText, int categoryId, int minimum, int pageIndex, int pageSize);
        // Return single ProductDto object
        Task<ProductDto> GetSingleProductAsync(int id);
        // Create new Product
        Task CreateNewProductAsync(ProductDto productDto, string imagesFolder);
        // Edit selected Product
        Task EditProductAsync(ProductDto productDto, string imagesFolder);
        // Delete selected Product
        Task DeleteProductAsync(int id, string imagesFolder);
        // Custom validation
        Task<Dictionary<string, string>> ValidateProductAsync(ProductDto productDto);
        // Return all ProductDto objects
        Task<List<ProductDto>> GetAllProductsAsync();
    }
}
