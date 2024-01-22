using SharedProject.Dtos;

namespace BizManager.Services.Products
{
    // Interface that defines methods for communicating with Server
    public interface IProductService
    {
        // Return paginated filtered list of ProductDto objects
        Task<Pagination<ProductDto>> GetProductsAsync(string? searchText, int categoryId, int minimum, int pageIndex, int pageSize);
        // Return single ProductDto object
        Task<ProductDto> GetSingleProductAsync(int id);
        // Create new Product
        Task<object> CreateNewProductAsync(MultipartFormDataContent formDataContent);
        // Edit selected Product
        Task<object> EditProductAsync(MultipartFormDataContent formDataContent);
        // Delete selected Product
        Task<int> DeleteProductAsync(int id);

        Task<List<ProductDto>> GetAllProductsAsync();
    }
}
