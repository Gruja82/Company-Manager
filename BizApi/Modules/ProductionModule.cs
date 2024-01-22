using BizApi.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using SharedProject.Dtos;

namespace BizApi.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Production entity
    public static class ProductionModule
    {
        public static void MapProductionEndpoint(this WebApplication app)
        {
            // GET method for returning paginated list of ProductionDto objects
            app.MapGet("api/productions", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] string? productionDate,
                                                [FromQuery] int productId, [FromQuery] int pageIndex, [FromQuery] int pageSize) =>
            {
                // Invoke ProductionRepository's method for returning
                // collection of ProductionDto object
                var paginatedResult = await unitOfWork.ProductionRepository.GetProductionsCollectionAsync(searchText, productionDate, productId, pageIndex, pageSize);

                return await Task.FromResult(paginatedResult);
            });

            // GET method for returning single ProductionDto object
            app.MapGet("api/productions/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke ProductionRepository's method for returning
                // single ProductionDto object
                ProductionDto? productionDto = await unitOfWork.ProductionRepository.GetSingleProductionAsync(id);

                // If productionDto is not null, then return productionDto,
                // otherwise return new ProductionDto object
                return productionDto ?? new ProductionDto();
            });

            // POST method for creating new Production
            app.MapPost("api/productions/create", async ([FromServices] IUnitOfWork unitOfWork, ProductionDto productionDto) =>
            {
                // Validate productionDto using ProductionRepository's method
                // ValidateProductionAsync
                var errorCheck = await unitOfWork.ProductionRepository.ValidateProductionAsync(productionDto);

                // If there are any errors, then return BadRequest status code along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke ProductionRepository's method for creating new Production
                    await unitOfWork.ProductionRepository.CreateNewProductionAsync(productionDto);
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

            // PATCH method for editing selected Production
            app.MapPatch("api/productions/patch", async ([FromServices] IUnitOfWork unitOfWork, ProductionDto productionDto) =>
            {
                // Validate productionDto using ProductionRepository's method
                // ValidateProductionAsync
                var errorCheck = await unitOfWork.ProductionRepository.ValidateProductionAsync(productionDto);

                // If there are any errors, then return BadRequest status code along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke ProductionRepository's method for editing selected Production
                    await unitOfWork.ProductionRepository.EditProductionAsync(productionDto);
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

            // DELETE method for deleting selected Production
            app.MapDelete("api/productions/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke ProductionRepository's method for deleting selected Production
                    await unitOfWork.ProductionRepository.DeleteProductionAsync(id);
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

            // GET method for returning all production dates
            app.MapGet("api/productions/alldates", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke ProductionRepository's method for returning
                // all production dates
                var productionDates = await unitOfWork.ProductionRepository.GetAllDatesAsync();

                return await Task.FromResult(productionDates);
            });
        }
    }
}
