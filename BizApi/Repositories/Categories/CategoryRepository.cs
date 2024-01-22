using BizApi.Data.Database;
using BizApi.Data.Entities;
using BizApi.Extensions;
using BizApi.Utility;
using Microsoft.EntityFrameworkCore;
using SharedProject.Dtos;

namespace BizApi.Repositories.Categories
{
    // Implmentation class for ICategoryRepository
    public class CategoryRepository:ICategoryRepository
    {
        private readonly AppDbContext context;

        public CategoryRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Category
        public async Task CreateNewCategoryAsync(CategoryDto categoryDto)
        {
            // Create new Category object
            Category category = new();

            // Set it's properties to the ones contained in categoryDto
            category.Code = categoryDto.Code;
            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;

            // Add category to database
            await context.Categories.AddAsync(category);
        }

        // Delete selected Category
        public async Task DeleteCategoryAsync(int id)
        {
            // Find Category record from database by Primary Key (id)
            Category category = (await context.Categories.FindAsync(id))!;

            // Remove category from database
            context.Categories.Remove(category);
        }

        // Edit selected Category
        public async Task EditCategoryAsync(CategoryDto categoryDto)
        {
            // Find Category record from database by Primary Key
            Category category = (await context.Categories.FindAsync(categoryDto.Id))!;

            // Set category's properties to the ones contained in categoryDto
            category.Code = categoryDto.Code;
            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;
        }

        // Return paginated collection of CategoryDto objects
        public async Task<Pagination<CategoryDto>> GetCategoriesCollectionAsync(string? searchText, int pageIndex, int pageSize)
        {
            // Return all Category objects
            var allCategories = context.Categories.AsNoTracking().AsQueryable();

            // If searchText is not null or empty string,
            // then filter allCategories by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allCategories = allCategories.Where(e => e.Code.ToLower().Contains(searchText.ToLower())
                                                || e.Name.ToLower().Contains(searchText.ToLower()))
                                                .AsQueryable();
            }

            // Variable that will contain CategoryDto objects
            List<CategoryDto> categoryDtos = new();

            // Iterate through allCategories and populate categoryDtos
            // using Category's extension method ConvertToDto
            foreach (var category in allCategories)
            {
                categoryDtos.Add(category.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method, 
            // return Pagination<CategoryDto> object
            var paginatedResult = PaginationUtility<CategoryDto>.GetPaginatedResult(in categoryDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single CategoryDto object
        public async Task<CategoryDto> GetSingleCategoryAsync(int id)
        {
            // Find Category record from database by Primary Key (id)
            Category category = (await context.Categories.FindAsync(id))!;

            // Return CategoryDto object using Category's
            // extension method ConvertToDto
            return category.ConvertToDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateCategoryAsync(CategoryDto categoryDto)
        {
            // Variable that will contain possible validatin errors
            Dictionary<string, string> errors = new();

            // Variable that will contain all Category records
            var allCategories = context.Categories.AsNoTracking().AsQueryable();

            // If categoryDto's Id value is larger than 0
            // it means that Category is used in Edit operation
            if (categoryDto.Id > 0)
            {
                // Find Category record from database by Primary Key
                Category category = (await context.Categories.FindAsync(categoryDto.Id))!;

                // If categoryDto's Code value is not equal to category's Code value,
                // it means that user has modified Code value. Therefore we check
                // for Code uniqueness among all Category records
                if (category.Code != categoryDto.Code)
                {
                    // If categoryDto's Code value is already contained
                    // in any of the Category records in database,
                    // then add validation error to errors Dictionary
                    if (allCategories.Select(e => e.Code.ToLower()).Contains(categoryDto.Code.ToLower()))
                    {
                        errors.Add("Code", "There is already Category with this Code value in database. Please provide different one!");
                    }
                }

                // If categoryDto's Name value is not equal to category's Name value,
                // it means that user has modified Name value. Therefore we check
                // for Name uniqueness among all Category records
                if (category.Name != categoryDto.Name)
                {
                    // If categoryDto's Name value is already contained
                    // in any of the Category records in database,
                    // then add validation error to errors Dictionary
                    if (allCategories.Select(e => e.Name.ToLower()).Contains(categoryDto.Name.ToLower()))
                    {
                        errors.Add("Name", "There is already Category with this Name value in database. Please provide different one!");
                    }
                }
            }
            // Otherwise it means that Category is used in Create operation
            else
            {
                // If categoryDto's Code value is already contained
                // in any of the Category records in database,
                // then add validation error to errors Dictionary
                if (allCategories.Select(e => e.Code.ToLower()).Contains(categoryDto.Code.ToLower()))
                {
                    errors.Add("Code", "There is already Category with this Code value in database. Please provide different one!");
                }

                // If categoryDto's Name value is already contained
                // in any of the Category records in database,
                // then add validation error to errors Dictionary
                if (allCategories.Select(e => e.Name.ToLower()).Contains(categoryDto.Name.ToLower()))
                {
                    errors.Add("Name", "There is already Category with this Name value in database. Please provide different one!");
                }
            }

            // Return errors Dictionary
            return errors;
        }

        // Return all Category records
        public async Task<List<CategoryDto>> GetAllCategories()
        {
            // Return all Category records
            var allCategories = context.Categories.AsNoTracking().AsQueryable();

            // Variable that will contain list of CategoryDto objects
            List<CategoryDto> categoryDtos = new();

            // Iterate through allCategories and populate categoryDtos
            // using Category's extension method ConvertToDto
            foreach (var category in allCategories)
            {
                categoryDtos.Add(category.ConvertToDto());
            }

            // Return categoryDtos
            return await Task.FromResult(categoryDtos);
        }
    }
}
