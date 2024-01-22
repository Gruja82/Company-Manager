using BizApi.Data.Database;
using BizApi.Data.Entities;
using BizApi.Extensions;
using BizApi.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SharedProject.Dtos;

namespace BizApi.Repositories.Productions
{
    // Implementation class for IPRoductionRepository
    public class ProductionRepository:IProductionRepository
    {
        private readonly AppDbContext context;

        public ProductionRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Production
        public async Task CreateNewProductionAsync(ProductionDto productionDto)
        {
            // Create new Production object
            Production production = new();

            // Set it's properties to the ones contained in productionDto
            production.Code = productionDto.Code;
            production.ProductionDate = productionDto.ProductionDate;
            production.Product = (await context.Products.FindAsync(productionDto.ProductId))!;
            production.QtyProduct = productionDto.Quantity;

            // Add production to database
            await context.Productions.AddAsync(production);

            // Increase Product's quantity
            production.Product.Qty += productionDto.Quantity;

            // Reduce quantity of each Material that is used in production
            foreach (var productDetail in production.Product.ProductDetails)
            {
                productDetail.Material.Qty -= productionDto.Quantity * productDetail.QtyMaterial;
            }

        }

        // Delete selected Production
        public async Task DeleteProductionAsync(int id)
        {
            // Find Production by id
            Production production = (await context.Productions.FindAsync(id))!;

            // Remove production from database
            context.Productions.Remove(production);
        }

        // Edit selected Production
        public async Task EditProductionAsync(ProductionDto productionDto)
        {
            // Find Production by id
            Production production = (await context.Productions
                                    .Include(e => e.Product)
                                    .ThenInclude(e => e.ProductDetails)
                                    .ThenInclude(e => e.Material)
                                    .FirstOrDefaultAsync(e => e.Id == productionDto.Id))!;

            // Restore all Material quantity that is used
            // in product production
            foreach (var productDetail in production.Product.ProductDetails)
            {
                productDetail.Material.Qty += production.QtyProduct * productDetail.QtyMaterial;
            }

            // Decrease Product's quantity
            production.Product.Qty -= production.QtyProduct;

            // Set it's properties to the ones contained in productionDto
            production.Code = productionDto.Code;
            production.ProductionDate = productionDto.ProductionDate;
            production.Product = (await context.Products.FindAsync(productionDto.ProductId))!;
            production.QtyProduct = productionDto.Quantity;

            // Increase Product's quantity
            production.Product.Qty += productionDto.Quantity;

            // Reduce quantity of each Material that is used in production
            foreach (var productDetail in production.Product.ProductDetails)
            {
                productDetail.Material.Qty -= productionDto.Quantity * productDetail.QtyMaterial;
            }
        }

        // Return paginated collection of ProductionDto objects
        public async Task<Pagination<ProductionDto>> GetProductionsCollectionAsync(string? searchText, string? productionDate, int productId, int pageIndex, int pageSize)
        {
            // Get all Production objects
            var allProductions = context.Productions
                                .Include(e => e.Product)
                                .ThenInclude(e => e.Category)
                                .AsNoTracking()
                                .AsQueryable();

            // If searchText is not null or empty string,
            // filter allProductions by searchText
            if(!string.IsNullOrEmpty(searchText))
            {
                allProductions = allProductions.Where(e => e.Code.ToLower().Contains(searchText.ToLower()));
            }

            // If productionDate is not null or empty string,
            // filter allProductions by productionDate
            if (!string.IsNullOrEmpty(productionDate))
            {
                DateTime prodDate = DateTime.Parse(productionDate);
                allProductions = allProductions.Where(e => e.ProductionDate == prodDate);
            }

            // If productId is greater than 0 (zero),
            // it means that user wants to filter results
            // by product (Id) and therefore we filter 
            // allProductions by productId
            if (productId > 0)
            {
                allProductions = allProductions.Where(e => e.Product == context.Products.Find(productId));
            }

            // Variable that will contain ProductionDto objects
            List<ProductionDto> productionDtos = new();

            // Iterate through allProductions and populate productionDtos
            // using Production's extension method ConvertToDto
            foreach (var production in allProductions)
            {
                productionDtos.Add(production.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method,
            // return Pagination<ProductionDto> object
            var paginatedResult = PaginationUtility<ProductionDto>.GetPaginatedResult(in productionDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single ProductionDto object
        public async Task<ProductionDto> GetSingleProductionAsync(int id)
        {
            // Find Production by id
            Production production = (await context.Productions
                                     .Include(e => e.Product)
                                     .FirstOrDefaultAsync(e => e.Id == id))!;

            // Return ProductionDto using Production's
            // extension method ConvertToDto
            return production.ConvertToDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateProductionAsync(ProductionDto productionDto)
        {
            // Define variable that will contain possible errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Production records
            var allProductions = context.Productions
                                .Include(e => e.Product)
                                .ThenInclude(e => e.ProductDetails)
                                .ThenInclude(e => e.Material)
                                .AsNoTracking()
                                .AsQueryable();

            // If productionDto's Id value is greater than 0, it means that Production
            // is used in Update operation
            if (productionDto.Id > 0)
            {
                // Find Production by Id
                Production production = (await context.Productions
                                         .Include(e => e.Product)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(e => e.Id == productionDto.Id))!;

                // If production's Code is modified, check for Code uniqueness
                // among all Production records
                if (production.Code != productionDto.Code)
                {
                    // If provided productionDto's Code is already contained in
                    // any of the Production records then add error to errors Dictionary
                    if (allProductions.Select(e => e.Code.ToLower()).Contains(productionDto.Code.ToLower()))
                    {
                        errors.Add("Code", "There is already Production with this Code in database. Please provide different one!");
                    }
                }
            }
            // Otherwise it means that Production us used in Create operation
            else
            {
                // If provided productionDto's Code is already contained in
                // any of the Production records then add error to errors Dictionary
                if (allProductions.Select(e => e.Code.ToLower()).Contains(productionDto.Code.ToLower()))
                {
                    errors.Add("Code", "There is already Production with this Code in database. Please provide different one!");
                }
            }

            // If Production's quantity is not greater than 0 (zero)
            // add error to errors Dictionary
            if (productionDto.Quantity <= 0)
            {
                errors.Add("Quantity", "Product quantity must be greater than 0 (zero)!");
            }

            // If there is not enough material quantity
            // for the product in production, add error
            // to errors Dictionary
            Product product = (await context.Products
                                    .Include(e => e.ProductDetails)
                                    .ThenInclude(e => e.Material)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(e => e.Id == productionDto.ProductId))!;

            foreach (var productDetail in product.ProductDetails)
            {
                if (productDetail.Material.Qty < productionDto.Quantity * productDetail.QtyMaterial)
                {
                    errors.Add("ProductId", "There is not enough material in stock for production of this Product!");
                }
            }

            // If ProductionDate is larger than today's date
            // then add error to errors Dictionary
            if(productionDto.ProductionDate > DateTime.Now)
            {
                errors.Add("ProductionDate", "Production date must not be larger than today's date!");
            }

            // Return errors Dictionary
            return errors;
        }

        // Return all Production dates
        public async Task<List<string>> GetAllDatesAsync()
        {
            // Get all production dates
            List<string> allDates = await context.Productions.Select(e => e.ProductionDate.ToShortDateString()).Distinct().ToListAsync();

            // Return allDates
            return allDates;
        }
    }
}
