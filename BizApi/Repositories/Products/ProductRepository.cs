using BizApi.Data.Database;
using BizApi.Data.Entities;
using BizApi.Extensions;
using BizApi.Utility;
using Microsoft.EntityFrameworkCore;
using SharedProject.Dtos;

namespace BizApi.Repositories.Products
{
    // Implementation class for IProductRepository
    public class ProductRepository:IProductRepository
    {
        private readonly AppDbContext context;

        public ProductRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new product
        public async Task CreateNewProductAsync(ProductDto productDto, string imagesFolder)
        {
            // Create new Product object
            Product product = new();

            // Set it's properties to the ones contained in productDto
            product.Code = productDto.Code;
            product.Name = productDto.Name;
            product.Category = (await context.Categories.FindAsync(productDto.CategoryId))!;
            product.Qty = 0;
            product.Price = productDto.Price;

            product.ImageUrl = productDto.StoreImage(imagesFolder);

            // Add product to database
            await context.Products.AddAsync(product);

            // Iterate through productDto's ProductDetailDtos collection
            // and add ProductDetail records to database
            foreach (var productDetailDto in productDto.ProductDetailDtos)
            {
                ProductDetail productDetail = new();

                productDetail.Product = product;
                productDetail.Material = (await context.Materials.FindAsync(productDetailDto.MaterialId))!;
                productDetail.QtyMaterial = productDetailDto.MaterialQty;

                await context.ProductDetails.AddAsync(productDetail);
            }

            
        }

        // Delete selected Product
        public async Task DeleteProductAsync(int id, string imagesFolder)
        {
            // Find product by id
            Product product = (await context.Products
                               .Include(e => e.ProductDetails)
                               .FirstOrDefaultAsync(e => e.Id == id))!;

            // Remove product from database
            context.Products.Remove(product);

            // If selected Product has an Image, then delete it
            if (!string.IsNullOrEmpty(imagesFolder) && !string.IsNullOrEmpty(product.ImageUrl))
            {
                if (System.IO.File.Exists(Path.Combine(imagesFolder, product.ImageUrl!)))
                {
                    System.IO.File.Delete(Path.Combine(imagesFolder, product.ImageUrl!));
                }
            }

            // Remove all ProductDetail records from database
            // which are related to product
            foreach (var productDetail in product.ProductDetails)
            {
                context.ProductDetails.Remove(productDetail);
            }
        }

        // Edit selected Product
        public async Task EditProductAsync(ProductDto productDto, string imagesFolder)
        {
            // Find product by id
            Product product = (await context.Products
                              .Include(e => e.Category)
                              .Include(e => e.ProductDetails)
                              .ThenInclude(e => e.Material)
                              .FirstOrDefaultAsync(e => e.Id == productDto.Id))!;

            // Set it's properties to the ones contained in productDto
            product.Code = productDto.Code;
            product.Name = productDto.Name;
            product.Category = (await context.Categories.FindAsync(productDto.CategoryId))!;
            product.Price = productDto.Price;

            // If productDto's Image is not null, it means that product's image
            // is changed, so we need to delete old image first
            if (productDto.Image != null)
            {
                if (System.IO.File.Exists(Path.Combine(imagesFolder, product.ImageUrl)))
                {
                    System.IO.File.Delete(Path.Combine(imagesFolder, product.ImageUrl));
                }

                // Store new image
                product.ImageUrl = productDto.StoreImage(imagesFolder);
            }

            // First remove all ProductDetail records from database
            // which are related to product
            foreach (var productDetail in product.ProductDetails)
            {
                context.ProductDetails.Remove(productDetail);
            }

            // Then iterate through productDto's ProductDetailDtos
            // and add ProductDetail records to database
            foreach (var productDetailDto in productDto.ProductDetailDtos)
            {
                ProductDetail productDetail = new();

                productDetail.Product = product;
                productDetail.Material = (await context.Materials.FindAsync(productDetailDto.MaterialId))!;
                productDetail.QtyMaterial = productDetailDto.MaterialQty;

                await context.ProductDetails.AddAsync(productDetail);
            }
        }

        // Return paginated collection of ProductDto objects
        public async Task<Pagination<ProductDto>> GetProductsCollectionAsync(string? searchText, int categoryId, int minimum, int pageIndex, int pageSize)
        {
            // Get all Product objects
            var allProducts = context.Products
                              .Include(e => e.Category)
                              .Include(e => e.ProductDetails)
                              .ThenInclude(e => e.Material)
                              .AsNoTracking()
                              .AsQueryable();

            // If searchText is not null or empty string,
            // filter allProducts by searchText
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                allProducts = allProducts.Where(e => e.Code.ToLower().Contains(searchText.ToLower())
                                               || e.Name.ToLower().Contains(searchText.ToLower()));
            }

            // If categoryId is greater than 0 (zero),
            // it means that user wants to filter results
            // by category (Id) and therefore we filter 
            // allProducts by categoryId
            if (categoryId > 0)
            {
                allProducts = allProducts.Where(e => e.Category == context.Categories.Find(categoryId));
            }

            // If minimum is greater than -1
            // it means that user wants to see Product records
            // that have their quantities less than or equal
            // to minimum value
            if (minimum > -1)
            {
                allProducts = allProducts.Where(e => e.Qty <= minimum);
            }

            // Variable that will contain ProductDto objects
            List<ProductDto> productDtos = new();

            // Iterate through allProducts and populate productDtos
            // using Product's extension method ConvertToDto
            foreach (var product in allProducts)
            {
                productDtos.Add(product.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method,
            // return Pagination<ProductDto> object
            var paginatedResult = PaginationUtility<ProductDto>.GetPaginatedResult(in productDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single ProductDto object
        public async Task<ProductDto> GetSingleProductAsync(int id)
        {
            // Find Product by id
            Product product = (await context.Products
                                .Include(e => e.Category)
                                .Include(e => e.ProductDetails)
                                .ThenInclude(e => e.Material)
                                .FirstOrDefaultAsync(e => e.Id == id))!;

            // Return ProductDto using Product's extension method
            // ConvertToDto
            return product.ConvertToDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateProductAsync(ProductDto productDto)
        {
            // Define variable that will contain possible errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Product records
            var allProducts = context.Products
                              .Include(e => e.Category)
                              .Include(e => e.ProductDetails)
                              .AsNoTracking()
                              .AsQueryable();

            // If productDto's Id value is greater than 0, it means that Product
            // is used in Update operation
            if (productDto.Id > 0)
            {
                // Find Product by Id
                Product product = (await context.Products
                                   .Include(e => e.Category)
                                   .Include(e => e.ProductDetails)
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(e => e.Id == productDto.Id))!;

                // If product's Code is modified, check for Code uniqueness
                // among all Product records
                if (product.Code != productDto.Code)
                {
                    // If provided productDto's Code is already contained in
                    // any of the Product records then add error to errors Dictionary
                    if (allProducts.Select(e => e.Code.ToLower()).Contains(productDto.Code.ToLower()))
                    {
                        errors.Add("Code", "There is already Product with this Code in database. Please provide different one!");
                    }
                }

                // If product's Name is modified, check for Name uniqueness
                // among all Product records
                if (product.Name != productDto.Name)
                {
                    // If provided productDto's Name is already contained in
                    // any of the Product records then add error to errors Dictionary
                    if (allProducts.Select(e => e.Name.ToLower()).Contains(productDto.Name.ToLower()))
                    {
                        errors.Add("Name", "There is already Product with this Name in database. Please provide different one!");
                    }
                }
            }
            // Otherwise it means that Product is used in Create operation
            else
            {
                // If provided productDto's Code is already contained in
                // any of the Product records then add error to errors Dictionary
                if (allProducts.Select(e => e.Code.ToLower()).Contains(productDto.Code.ToLower()))
                {
                    errors.Add("Code", "There is already Product with this Code in database. Please provide different one!");
                }

                // If provided productDto's Name is already contained in
                // any of the Product records then add error to errors Dictionary
                if (allProducts.Select(e => e.Name.ToLower()).Contains(productDto.Name.ToLower()))
                {
                    errors.Add("Name", "There is already Product with this Name in database. Please provide different one!");
                }
            }

            // If productDto's CategoryId is equal to 0 (zero),
            // it means that user has not selected a Category
            // and therefore add error to errors Dictionary
            if (productDto.CategoryId == 0)
            {
                errors.Add("CategoryId", "Please select Category from list!");
            }

            // If productDto's ProductDetailDtos does not contain at least one
            // record then add error to errors Dictionary
            if (!productDto.ProductDetailDtos.Any())
            {
                errors.Add("ProductDetailDtos", "You must add at least one Material to Product specification list!");
            }

            // Return errors Dictionary
            return errors;
        }

        // Return all ProductDto objects
        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var allProducts = context.Products
                              .Include(e => e.Category)
                              .Include(e => e.ProductDetails)
                              .ThenInclude(e => e.Material)
                              .AsNoTracking()
                              .AsQueryable();

            List<ProductDto> productDtos = new();

            foreach (var product in allProducts)
            {
                productDtos.Add(product.ConvertToDto());
            }

            return await Task.FromResult(productDtos);
        }
    }
}
