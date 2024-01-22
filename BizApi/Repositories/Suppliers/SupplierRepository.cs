using BizApi.Data.Database;
using BizApi.Data.Entities;
using BizApi.Extensions;
using BizApi.Utility;
using Microsoft.EntityFrameworkCore;
using SharedProject.Dtos;

namespace BizApi.Repositories.Suppliers
{
    // Implementation class for ISupplierRepository
    public class SupplierRepository:ISupplierRepository
    {
        private readonly AppDbContext context;

        public SupplierRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Supplier
        public async Task CreateNewSupplierAsync(SupplierDto supplierDto, string imagesFolder)
        {
            // Create new Supplier object
            Supplier supplier = new();

            // Set it's properties to the ones contained in supplierDto
            supplier.Code = supplierDto.Code;
            supplier.Name = supplierDto.Name;
            supplier.Contact = supplierDto.Contact;
            supplier.Address = supplierDto.Address;
            supplier.City = supplierDto.City;
            supplier.Postal = supplierDto.Postal;
            supplier.Phone = supplierDto.Phone;
            supplier.Email = supplierDto.Email;
            supplier.Web = supplierDto.Web;

            supplier.ImageUrl = supplierDto.StoreImage(imagesFolder);

            // Add supplier to database
            await context.Suppliers.AddAsync(supplier);
        }

        // Delete selected Supplier
        public async Task DeleteSupplierAsync(int id, string imagesFolder)
        {
            // Find Supplier by id
            Supplier supplier = (await context.Suppliers.FindAsync(id))!;

            // Remove supplier from database
            context.Suppliers.Remove(supplier);

            // If selected Supplier has an image, then delete it
            if (!string.IsNullOrEmpty(imagesFolder) && !string.IsNullOrEmpty(supplier.ImageUrl))
            {
                if (System.IO.File.Exists(Path.Combine(imagesFolder, supplier.ImageUrl)))
                {
                    System.IO.File.Delete(Path.Combine(imagesFolder, supplier.ImageUrl));
                }
            }
        }

        // Edit selected Supplier
        public async Task EditSupplierAsync(SupplierDto supplierDto, string imagesFolder)
        {
            // Find Supplier by id
            Supplier supplier = (await context.Suppliers.FindAsync(supplierDto.Id))!;

            // Set it's properties to the ones contained in supplierDto
            supplier.Code = supplierDto.Code;
            supplier.Name = supplierDto.Name;
            supplier.Contact = supplierDto.Contact;
            supplier.Address = supplierDto.Address;
            supplier.City = supplierDto.City;
            supplier.Postal = supplierDto.Postal;
            supplier.Phone = supplierDto.Phone;
            supplier.Email = supplierDto.Email;
            supplier.Web = supplierDto.Web;

            // If supplierDto's Image is not null, it means that supplier's image
            // is changed, so we need to delete old image first
            if (supplierDto.Image != null)
            {
                if (System.IO.File.Exists(Path.Combine(imagesFolder, supplier.ImageUrl)))
                {
                    System.IO.File.Delete(Path.Combine(imagesFolder, supplier.ImageUrl));
                }

                // Store new image
                supplier.ImageUrl = supplierDto.StoreImage(imagesFolder);
            }
        }

        // Return single SupplierDto object
        public async Task<SupplierDto> GetSingleSupplierAsync(int id)
        {
            // Find Supplier by id
            Supplier supplier = (await context.Suppliers.FindAsync(id))!;

            // Return SupplierDto using Supplier's extension method
            // ConvertToDto
            return supplier.ConvertToDto();
        }

        // Return paginated collection of SupplierDto objects
        public async Task<Pagination<SupplierDto>> GetSupplierCollectionAsync(string searchText, int pageIndex, int pageSize)
        {
            // Get all Supplier objects
            var allSuppliers = context.Suppliers
                               .AsNoTracking()
                               .AsQueryable();

            // If searchText is not null or empty string,
            // filter allSuppliers by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allSuppliers = allSuppliers.Where(e => e.Code.ToLower().Contains(searchText.ToLower())
                                                || e.Contact.ToLower().Contains(searchText.ToLower())
                                                || e.Email.ToLower().Contains(searchText.ToLower()));
            }

            // Variable that will contain SupplierDto objects
            List<SupplierDto> supplierDtos = new();

            // Iterate through allSuppliers and populate
            // supplierDtos using Supplier's extension method
            // ConvertToDto
            foreach (var supplier in allSuppliers)
            {
                supplierDtos.Add(supplier.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult() method, 
            // return Pagionation<SupplierDto> object
            var paginatedResult = PaginationUtility<SupplierDto>.GetPaginatedResult(in supplierDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateSupplierAsync(SupplierDto supplierDto)
        {
            // Define variable that will contain possible errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Supplier records
            var allSuppliers = context.Suppliers
                               .AsNoTracking()
                               .AsQueryable();

            // If supplierDto's Id value is greater than 0, it means that Supplier
            // is used in Update operation
            if (supplierDto.Id > 0)
            {
                // Find Supplier by id
                Supplier supplier = (await context.Suppliers.FindAsync(supplierDto.Id))!;

                // If supplier's Code is modified, check for Code uniqueness
                // among all Supplier records
                if (supplier.Code != supplierDto.Code)
                {
                    // If provided supplierDto's Code is already contained in
                    // any of the Supplier records then add error to errors Dictionary
                    if (allSuppliers.Select(e => e.Code.ToLower()).Contains(supplierDto.Code.ToLower()))
                    {
                        errors.Add("Code", "There is already Supplier with this Code in database. Please provide different one!");
                    }
                }

                // If supplier's Name is modified, check for Name uniqueness
                // among all Supplier records
                if (supplier.Name != supplierDto.Name)
                {
                    // If provided supplierDto's Name is already contained in
                    // any of the Supplier records then add error to errors Dictionary
                    if (allSuppliers.Select(e => e.Name.ToLower()).Contains(supplierDto.Name.ToLower()))
                    {
                        errors.Add("Name", "There is already Supplier with this Name in database. Please provide different one!");
                    }
                }

                // If supplier's Email is modified, check for Email uniqueness
                // among all Supplier records
                if (supplier.Email != supplierDto.Email)
                {
                    // If provided supplierDto's Email is already contained in
                    // any of the Supplier records then add error to errors Dictionary
                    if (allSuppliers.Select(e => e.Email.ToLower()).Contains(supplierDto.Email.ToLower()))
                    {
                        errors.Add("Email", "There is already Supplier with this Email in database. Please provide different one!");
                    }
                }
            }
            // Otherwise it means that Supplier is used in Create operation
            else
            {
                // If provided supplierDto's Code is already contained in
                // any of the Supplier records then add error to errors Dictionary
                if (allSuppliers.Select(e => e.Code.ToLower()).Contains(supplierDto.Code.ToLower()))
                {
                    errors.Add("Code", "There is already Supplier with this Code in database. Please provide different one!");
                }

                // If provided supplierDto's Name is already contained in
                // any of the Supplier records then add error to errors Dictionary
                if (allSuppliers.Select(e => e.Name.ToLower()).Contains(supplierDto.Name.ToLower()))
                {
                    errors.Add("Name", "There is already Supplier with this Name in database. Please provide different one!");
                }

                // If provided supplierDto's Email is already contained in
                // any of the Supplier records then add error to errors Dictionary
                if (allSuppliers.Select(e => e.Email.ToLower()).Contains(supplierDto.Email.ToLower()))
                {
                    errors.Add("Email", "There is already Supplier with this Email in database. Please provide different one!");
                }
            }

            // Return errors Dictionary
            return errors;
        }

        // Return all Supplier records
        public async Task<List<SupplierDto>> GetAllSuppliersAsync()
        {
            var allSuppliers = context.Suppliers
                                .AsNoTracking()
                                .AsQueryable();

            List<SupplierDto> supplierDtos = new();

            foreach (var supplier in allSuppliers)
            {
                supplierDtos.Add(supplier.ConvertToDto());
            }

            return await Task.FromResult(supplierDtos);
        }
    }
}
