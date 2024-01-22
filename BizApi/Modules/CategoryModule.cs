using BizApi.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using SharedProject.Dtos;

namespace BizApi.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Category entity
    public static class CategoryModule
    {
        public static void MapCategoryEndpoint(this WebApplication app)
        {
            // GET method for returning paginated list of CategoryDto objects
            app.MapGet("api/categories", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] int pageIndex, [FromQuery] int pageSize) =>
            {
                // Invoke CategoryRepository's method for returning
                // collection of CategoryDto object
                var paginatedResult = await unitOfWork.CategoryRepository.GetCategoriesCollectionAsync(searchText ?? string.Empty, pageIndex, pageSize);

                return await Task.FromResult(Results.Ok(paginatedResult));
            });

            // GET method for returning single CategoryDto object
            app.MapGet("api/categories/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke CategoryRepository's method for returning
                // single CategoryDto object
                CategoryDto? categoryDto = await unitOfWork.CategoryRepository.GetSingleCategoryAsync(id);

                // If categoryDto is not null, then return categoryDto,
                // otherwise return new CategoryDto object
                return categoryDto ?? new CategoryDto();
            });

            // POST method for creating new Category
            app.MapPost("api/categories/create", async ([FromServices] IUnitOfWork unitOfWork, CategoryDto categoryDto) =>
            {
                // Validate categoryDto using CategoryRepository's method
                // ValidateCategoryAsync
                var errorCheck = await unitOfWork.CategoryRepository.ValidateCategoryAsync(categoryDto);

                // If there are any errors, then return BadRequest status code along with errorCheck
                if(errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke CategoryRepository's method for creating new Category
                    await unitOfWork.CategoryRepository.CreateNewCategoryAsync(categoryDto);
                }
                catch (Exception)
                {
                    // If there is any exception,
                    // rollback changes made on entity
                    unitOfWork.RollBackChanges();
                    return Results.StatusCode(500);
                }

                // Save changes to database
                await unitOfWork.ConfirmChangesAsync();
                // Return status code Created (201)
                return Results.StatusCode(201);
            });

            // PATCH method for editing selected Category
            app.MapPatch("api/categories/patch", async ([FromServices] IUnitOfWork unitOfWork, CategoryDto categoryDto) =>
            {
                // Validate categoryDto using CategoryRepository's method
                // ValidateCategoryAsync
                var errorCheck = await unitOfWork.CategoryRepository.ValidateCategoryAsync(categoryDto);

                // If there are any errors, then return BadRequest status code along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke CategoryRepository's method for editing selected Category
                    await unitOfWork.CategoryRepository.EditCategoryAsync(categoryDto);
                }
                catch (Exception)
                {
                    // If there is any exception,
                    // rollback changes made on entity
                    unitOfWork.RollBackChanges();
                    return Results.StatusCode(500);
                }

                // Save changes to database
                await unitOfWork.ConfirmChangesAsync();
                // Return status code NoContent (204)
                return Results.StatusCode(204);
            });

            // DELETE method for deleting selected Category
            app.MapDelete("api/categories/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke CategoryRepository's method for deleting selected Category
                    await unitOfWork.CategoryRepository.DeleteCategoryAsync(id);
                }
                catch (Exception)
                {
                    // If there is any exception,
                    // rollback changes made on entity
                    unitOfWork.RollBackChanges();
                    return Results.StatusCode(500);
                }

                // Save changes to database
                await unitOfWork.ConfirmChangesAsync();
                // Return status code NoContent (204)
                return Results.Ok();
            });

            // GET method for returning list of CategoryDto objects
            app.MapGet("api/categories/getall", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke MaterialRepository's method for returning
                // list of CategoryDto objects
                List<CategoryDto> categoryDtos = await unitOfWork.CategoryRepository.GetAllCategories();

                // Return OK result along with categoryDtos
                return Results.Ok(categoryDtos);
            });
        }

    }
}
