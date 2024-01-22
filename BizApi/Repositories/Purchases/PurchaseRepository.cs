using BizApi.Data.Database;
using BizApi.Data.Entities;
using BizApi.Extensions;
using BizApi.Utility;
using Microsoft.EntityFrameworkCore;
using SharedProject.Dtos;

namespace BizApi.Repositories.Purchases
{
    // Implementation class for IPurchaseRepository
    public class PurchaseRepository:IPurchaseRepository
    {
        private readonly AppDbContext context;

        public PurchaseRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Purchase
        public async Task CreateNewPurchase(PurchaseDto purchaseDto)
        {
            // Create new Purchase object
            Purchase purchase = new();

            // Set it's properties to the ones contained in purchaseDto
            purchase.Code = purchaseDto.Code;
            purchase.PurchaseDate = purchaseDto.PurchaseDate;
            purchase.Supplier = (await context.Suppliers.FindAsync(purchaseDto.SupplierId))!;

            // Add purchase to database
            await context.Purchases.AddAsync(purchase);

            // Iterate through purchaseDto's PurchaseDetailDtos collection
            // and add PurchaseDetail records to database
            foreach (var purchaseDetailDto in purchaseDto.PurchaseDetailDtos)
            {
                PurchaseDetail purchaseDetail = new();

                purchaseDetail.Purchase = purchase;
                purchaseDetail.Material = (await context.Materials.FindAsync(purchaseDetailDto.MaterialId))!;
                purchaseDetail.QtyMaterial = purchaseDetailDto.Quantity;

                await context.PurchaseDetails.AddAsync(purchaseDetail);

                // Increase Material's quantity
                purchaseDetail.Material.Qty += purchaseDetailDto.Quantity;
            }
        }

        // Delete selected Purchase
        public async Task DeletePurchaseAsync(int id)
        {
            // Find Purchase by id
            Purchase purchase = (await context.Purchases
                                        .Include(e => e.PurchaseDetails)
                                        .FirstOrDefaultAsync(e => e.Id == id))!;

            // Remove purchase from database
            context.Purchases.Remove(purchase);

            // Remove all PurchaseDetail records from database
            // which are related to purchase
            foreach (var purchaseDetail in purchase.PurchaseDetails)
            {
                context.PurchaseDetails.Remove(purchaseDetail);
            }
        }

        // Edit selected Purchase
        public async Task EditPurchaseAsync(PurchaseDto purchaseDto)
        {
            // Find Purchase by Id
            Purchase purchase = (await context.Purchases
                                       .Include(e => e.PurchaseDetails)
                                       .ThenInclude(e => e.Material)
                                       .FirstOrDefaultAsync(e => e.Id == purchaseDto.Id))!;

            // Set it's properties to the ones contained in purchaseDto
            purchase.Code = purchaseDto.Code;
            purchase.PurchaseDate = purchaseDto.PurchaseDate;
            purchase.Supplier = (await context.Suppliers.FindAsync(purchaseDto.SupplierId))!;

            // Remove all PurchaseDetail records from database
            // which are related to purchase
            foreach (var purchaseDetail in purchase.PurchaseDetails)
            {
                context.PurchaseDetails.Remove(purchaseDetail);

                // Restore Material's quantity
                purchaseDetail.Material.Qty -= purchaseDetail.QtyMaterial;
            }

            // Iterate through purchaseDto's PurchaseDetailDtos collection
            // and add PurchaseDetail records to database
            foreach (var purchaseDetailDto in purchaseDto.PurchaseDetailDtos)
            {
                PurchaseDetail purchaseDetail = new();

                purchaseDetail.Purchase = purchase;
                purchaseDetail.Material = (await context.Materials.FindAsync(purchaseDetailDto.MaterialId))!;
                purchaseDetail.QtyMaterial = purchaseDetailDto.Quantity;

                await context.PurchaseDetails.AddAsync(purchaseDetail);

                // Increase Material's quantity
                purchaseDetail.Material.Qty += purchaseDetailDto.Quantity;
            }
        }

        // Return all Purchase dates
        public async Task<List<string>> GetPurchaseDates()
        {
            List<string> purchaseDates = await context.Purchases.Select(e => e.PurchaseDate.ToShortDateString()).ToListAsync();

            return purchaseDates;
        }

        // Return paginated collection of PurchaseDto objects
        public async Task<Pagination<PurchaseDto>> GetPurchasesCollectionAsync(string? searchText, string? purchaseDate, int supplierId, int materialId, int pageIndex, int pageSize)
        {
            // Get all Purchase objects
            var allPurchases = context.Purchases
                               .Include(e => e.Supplier)
                               .Include(e => e.PurchaseDetails)
                               .ThenInclude(e => e.Material)
                               .AsNoTracking()
                               .AsQueryable();

            // If searchText is not null or empty string,
            // filter allPurchases by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allPurchases = allPurchases.Where(e => e.Code.ToLower().Contains(searchText.ToLower()));
            }

            // If purchaseDate is not null or empty string,
            // filter allPurchases by purchaseDate
            if (!string.IsNullOrEmpty(purchaseDate))
            {
                DateTime purDate = DateTime.Parse(purchaseDate);
                allPurchases = allPurchases.Where(e => e.PurchaseDate == purDate);
            }

            // If supplierId is greater than 0 (zero),
            // it means that user wants to filter results
            // by supplier (Id) and therefore we filter 
            // allPurchases by supplierId
            if (supplierId > 0)
            {
                allPurchases = allPurchases.Where(e => e.Supplier == context.Suppliers.Find(supplierId));
            }

            // If materialId is greater than 0 (zero),
            // it means that user wants to filter results
            // by material (Id) and therefore we filter 
            // allPurchases by materialId
            if (materialId > 0)
            {
                allPurchases = allPurchases.Where(e => e.PurchaseDetails.Select(e => e.Material).Contains(context.Materials.Find(materialId)));
            }

            // Variable that will contain PurchaseDto objects
            List<PurchaseDto> purchaseDtos = new();

            // Iterate through allPurchases and populate purchaseDtos
            // using Purchase's extension method ConvertToDto
            foreach (var purchase in allPurchases)
            {
                purchaseDtos.Add(purchase.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method,
            // return Pagination<PurchaseDto> object
            var paginatedResult = PaginationUtility<PurchaseDto>.GetPaginatedResult(in purchaseDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single PurchaseDto object
        public async Task<PurchaseDto> GetSinglePurchaseAsync(int id)
        {
            // Find Purchase by id
            Purchase purchase = (await context.Purchases
                                       .Include(e => e.Supplier)
                                       .Include(e => e.PurchaseDetails)
                                       .ThenInclude(e => e.Material)
                                       .FirstOrDefaultAsync(e => e.Id == id))!;

            // Return PurchaseDto using Purchase's extension method
            // ConvertToDto
            return purchase.ConvertToDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidatePurchaseAsync(PurchaseDto purchaseDto)
        {
            // Define variable that will contain possible errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Purchase records
            var allPurchases = context.Purchases
                               .Include(e => e.PurchaseDetails)
                               .ThenInclude(e => e.Material)
                               .AsNoTracking()
                               .AsQueryable();

            // If purchaseDto's Id value is greater than 0, it means that Purchase
            // is used in Update operation
            if (purchaseDto.Id > 0)
            {
                // Find Purchase by Id
                Purchase purchase = (await context.Purchases
                                           .Include(e => e.Supplier)
                                           .Include(e => e.PurchaseDetails)
                                           .ThenInclude(e => e.Material)
                                           .FirstOrDefaultAsync(e => e.Id == purchaseDto.Id))!;

                // If purchase's Code is modified, check for Code uniqueness
                // among all Purchase records
                if (purchase.Code != purchaseDto.Code)
                {
                    // If provided purchaseDto's Code is already contained in
                    // any of the Purchase records then add error to errors Dictionary
                    if (allPurchases.Select(e => e.Code.ToLower()).Contains(purchaseDto.Code.ToLower()))
                    {
                        errors.Add("Code", "There is already Purchase with this Code in database. Please provide different one!");
                    }
                }
            }
            // Otherwise it means that Purchase is used in Create operation
            else
            {
                // If provided purchaseDto's Code is already contained in
                // any of the Purchase records then add error to errors Dictionary
                if (allPurchases.Select(e => e.Code.ToLower()).Contains(purchaseDto.Code.ToLower()))
                {
                    errors.Add("Code", "There is already Purchase with this Code in database. Please provide different one!");
                }
            }

            // If purchaseDto's supplierId is equal to 0 (zero),
            // it means that user has not selected a Supplier
            // and therefore add error to errors Dictionary
            if (purchaseDto.SupplierId == 0)
            {
                errors.Add("SupplierId", "Please select Supplier from list!");
            }

            // If PurchaseDate is larger than today's date
            // then add error to errors Dictionary
            if (purchaseDto.PurchaseDate > DateTime.Now)
            {
                errors.Add("PurchaseDate", "Purchase date must not be larger than today's date!");
            }

            return errors;
        }
    }
}
