using BizApi.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using SharedProject.Dtos;

namespace BizApi.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Purchase entity
    public static class PurchaseModule
    {
        public static void MapPurchaseEndpoint(this WebApplication app)
        {
            // GET method for returning paginated list of PurchaseDto objects
            app.MapGet("api/purchases", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] string? purchaseDate,
                                              [FromQuery] int supplierId, [FromQuery] int materialId, [FromQuery] int pageIndex, [FromQuery] int pageSize) =>
            {
                // Invoke PurchaseRepository's method for returning
                // collection of PurchaseDto object
                var paginatedResult = await unitOfWork.PurchaseRepository.GetPurchasesCollectionAsync(searchText, purchaseDate, supplierId, materialId, pageIndex, pageSize);

                return await Task.FromResult(paginatedResult);
            });

            // GET method for returning single PurchaseDto object
            app.MapGet("api/purchases/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke PurchaseRepository's method for returning
                // singlePurchaseDto object
                PurchaseDto? purchaseDto = await unitOfWork.PurchaseRepository.GetSinglePurchaseAsync(id);

                // If purchaseDto is not null, then return purchaseDto,
                // otherwise return new PurchaseDto object
                return purchaseDto ?? new PurchaseDto();
            });

            // POST method for creating new Purchase
            app.MapPost("api/purchases/create", async ([FromServices] IUnitOfWork unitOfWork, PurchaseDto purchaseDto) =>
            {
                // Validate purchaseDto using PurchaseRepository's method
                // ValidatePurchaseAsync
                var errorCheck = await unitOfWork.PurchaseRepository.ValidatePurchaseAsync(purchaseDto);

                // If there are any errors, then return BadRequest status code along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke PurchaseRepository's method for creating new Purchase
                    await unitOfWork.PurchaseRepository.CreateNewPurchase(purchaseDto);
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

            // PATCH method for editing selected Purchase
            app.MapPatch("api/purchases/patch", async ([FromServices] IUnitOfWork unitOfWork, PurchaseDto purchaseDto) =>
            {
                // Validate purchaseDto using PurchaseRepository's method
                // ValidatePurchaseAsync
                var errorCheck = await unitOfWork.PurchaseRepository.ValidatePurchaseAsync(purchaseDto);

                // If there are any errors, then return BadRequest status code along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke PurchaseRepository's method for editing selected Purchase
                    await unitOfWork.PurchaseRepository.EditPurchaseAsync(purchaseDto);
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
                // Return status code No Content (204)
                return Results.StatusCode(204);
            });

            // DELETE method for deleting selected Purchase
            app.MapDelete("api/purchases/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke PurchaseRepository's method for deleting selected Purchase
                    await unitOfWork.PurchaseRepository.DeletePurchaseAsync(id);
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
                // Return status code No Content (204)
                return Results.StatusCode(204);
            });

            // GET method for returning list of Purchase Dates
            app.MapGet("api/purchases/getpurchasedates", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke PurchaseRepository's method for returning
                // list of Purchase dates
                List<string>? purchaseDates = await unitOfWork.PurchaseRepository.GetPurchaseDates();

                // If purchaseDates is not null then return purchaseDates
                // otherwise return new List<string> object
                return await Task.FromResult(purchaseDates) ?? await Task.FromResult(new List<string>());
            });
        }
    }
}
