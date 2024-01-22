using BizApi.Data.Database;
using BizApi.Data.Entities;
using BizApi.Extensions;
using BizApi.Utility;
using Microsoft.EntityFrameworkCore;
using SharedProject.Dtos;

namespace BizApi.Repositories.Materials
{
    // Implementation class for IMaterialRepository
    public class MaterialRepository:IMaterialRepository
    {
        private readonly AppDbContext context;

        public MaterialRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Material
        public async Task CreateNewMaterialAsync(MaterialDto materialDto, string imagesFolder)
        {
            // Create new Material object
            Material material = new();

            // Set it's properties to the ones contained in materialDto
            material.Code = materialDto.Code;
            material.Name = materialDto.Name;
            material.Category = (await context.Categories.FindAsync(materialDto.CategoryId))!;
            material.Qty = 0;
            material.Price = materialDto.Price;

            material.ImageUrl = materialDto.StoreImage(imagesFolder);

            // Add material to database
            await context.Materials.AddAsync(material);
        }

        // Delete selected Material
        public async Task DeleteMaterialAsync(int id, string imagesFolder)
        {
            // Find Material by id
            Material material = (await context.Materials.FindAsync(id))!;

            // Remove material from database
            context.Materials.Remove(material);

            // If selected Material has an Image, then delete it
            if(!string.IsNullOrEmpty(imagesFolder) && !string.IsNullOrEmpty(material.ImageUrl))
            {
                if (System.IO.File.Exists(Path.Combine(imagesFolder, material.ImageUrl!)))
                {
                    System.IO.File.Delete(Path.Combine(imagesFolder, material.ImageUrl!));
                }
            }
        }

        // Edit selected Material
        public async Task EditMaterialAsync(MaterialDto materialDto, string imagesFolder)
        {
            // Find Material by id
            Material material = (await context.Materials.FindAsync(materialDto.Id))!;

            // Set it's properties to the ones contained in materialDto
            material.Code = materialDto.Code;
            material.Name = materialDto.Name;
            material.Category = (await context.Categories.FindAsync(materialDto.CategoryId))!;
            material.Price = materialDto.Price;

            // If matyerialDto's Image is not null, it means that material's image
            // is changed, so we need to delete old image first
            if (materialDto.Image != null)
            {
                if (System.IO.File.Exists(Path.Combine(imagesFolder, material.ImageUrl)))
                {
                    System.IO.File.Delete(Path.Combine(imagesFolder, material.ImageUrl));
                }

                // Store new image
                material.ImageUrl = materialDto.StoreImage(imagesFolder);
            }
        }

        // Return paginated collection of MaterialDto objects
        public async Task<Pagination<MaterialDto>> GetMaterialsCollectionAsync(string? searchText, int categoryId, int pageIndex, int pageSize)
        {
            // Get all Material objects
            var allMaterials = context.Materials
                                .Include(e => e.Category)
                                .AsNoTracking()
                                .AsQueryable();

            // If searchText is not null or empty string,
            // filter allMaterials by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allMaterials = allMaterials.Where(e => e.Code.ToLower().Contains(searchText.ToLower())
                                                || e.Name.ToLower().Contains(searchText.ToLower()));
            }

            // If categoryId is greater than 0 (zero),
            // it means that user wants to filter results
            // by category (Id) and therefore we filter 
            // allMaterials by categoryId
            if(categoryId > 0)
            {
                allMaterials = allMaterials.Where(e => e.Category == context.Categories.Find(categoryId));
            }

            // Variable that will contain MaterialDto objects
            List<MaterialDto> materialDtos = new();

            // Iterate through allMaterials and populate materialDtos
            // using Material's extension method ConvertToDto
            foreach (var material in allMaterials)
            {
                materialDtos.Add(material.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method,
            // return Pagination<MaterialDto> object
            var paginatedResult = PaginationUtility<MaterialDto>.GetPaginatedResult(in materialDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single MatrialDto object
        public async Task<MaterialDto> GetSingleMaterialAsync(int id)
        {
            // Find Material by id
            Material material = (await context.Materials
                                .Include(e => e.Category)
                                .FirstOrDefaultAsync(e => e.Id == id))!;

            // Return MaterialDto using Material's extension method
            // ConvertToDto
            return material.ConvertToDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateMaterialAsync(MaterialDto materialDto)
        {
            // Define variable that will contain possible errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Material records
            var allMaterials = context.Materials
                               .Include(e => e.Category)
                               .AsNoTracking()
                               .AsQueryable();

            // If materialDto's Id value is greater than 0, it means that Material
            // is used in Update operation
            if (materialDto.Id > 0)
            {
                // Find Material by id
                Material material = (await context.Materials.FindAsync(materialDto.Id))!;

                // If material's Code is modified, check for Code uniqueness
                // among all Material records
                if (material.Code != materialDto.Code)
                {
                    // If provided materialDto's Code is already contained in
                    // any of the Material records then add error to errors Dictionary
                    if (allMaterials.Select(e => e.Code.ToLower()).Contains(materialDto.Code.ToLower()))
                    {
                        errors.Add("Code", "There is already Material with this Code in database. Please provide different one!");
                    }
                }

                // If material's Name is modified, check for Name uniqueness
                // among all Material records
                if (material.Name != materialDto.Name)
                {
                    // If provided materialDto's Name is already contained in
                    // any of the Material records then add error to errors Dictionary
                    if (allMaterials.Select(e => e.Name.ToLower()).Contains(materialDto.Name.ToLower()))
                    {
                        errors.Add("Name", "There is already Material with this Name in database. Please provide different one!");
                    }
                }
            }
            // Otherwise it means that Material is used in Create operation
            else
            {
                // If provided materialDto's Code is already contained in
                // any of the Material records then add error to errors Dictionary
                if (allMaterials.Select(e => e.Code.ToLower()).Contains(materialDto.Code.ToLower()))
                {
                    errors.Add("Code", "There is already Material with this Code in database. Please provide different one!");
                }

                // If provided materialDto's Name is already contained in
                // any of the Material records then add error to errors Dictionary
                if (allMaterials.Select(e => e.Name.ToLower()).Contains(materialDto.Name.ToLower()))
                {
                    errors.Add("Name", "There is already Material with this Name in database. Please provide different one!");
                }
            }

            // If materialDto's CategoryId is equal to 0 (zero),
            // it means that user has not selected a Category
            // and therefore add error to errors Dictionary
            if (materialDto.CategoryId == 0)
            {
                errors.Add("CategoryId", "Please select Category from list!");
            }

            // Return errors Dictionary
            return errors;
        }

        // Return all Material records
        public async Task<List<MaterialDto>> GetAllMaterialsAsync()
        {
            // Variable that contains all Material records
            var allMaterials = context.Materials
                               .Include(e => e.Category)
                               .AsNoTracking()
                               .AsQueryable();

            // Variable that will contain MaterialDto objects
            List<MaterialDto> materialDtos = new();

            // Iterate through allMaterials and populate
            // materialDtos using Material's extension method
            // ConvertToDto
            foreach (var material in allMaterials)
            {
                materialDtos.Add(material.ConvertToDto());
            }

            // Return materialDtos
            return await Task.FromResult(materialDtos);
        }
    }
}
