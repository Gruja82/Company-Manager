using BizApi.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using SharedProject.Dtos;

namespace BizApi.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Supplier entity
    public static class SupplierModule
    {
        public static void MapSupplierEndpoint(this WebApplication app, string imagesFolder)
        {
            // GET method for returning paginated list of SupplierDto objects
            app.MapGet("api/suppliers", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] int pageIndex, [FromQuery] int pageSize) =>
            {
                // Invoke SupplierRepository's method for returning
                // collection of SupplierDto object
                var paginatedResult = await unitOfWork.SupplierRepository.GetSupplierCollectionAsync(searchText ?? string.Empty, pageIndex, pageSize);

                return await Task.FromResult(Results.Ok(paginatedResult));
            });

            // GET method for returning single SupplierDto object
            app.MapGet("api/suppliers/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke SupplierRepository's method for returning
                // single SupplierDto object
                SupplierDto? supplierDto = await unitOfWork.SupplierRepository.GetSingleSupplierAsync(id);

                // If supplierDto is not null, then return supplierDto,
                // otherwise return new SupplierDto object
                return supplierDto ?? new SupplierDto();
            });

            // POST method for creating new Supplier
            app.MapPost("api/suppliers/create", async ([FromServices] IUnitOfWork unitOfWork, HttpRequest request) =>
            {
                // Create new SupplierDto object
                SupplierDto supplierDto = new();

                // Read the form from the request and
                // set properties of supplierDto
                var formResult = await request.ReadFormAsync();

                supplierDto.Code = formResult["Code"]!;
                supplierDto.Name = formResult["Name"]!;
                supplierDto.Contact = formResult["Contact"]!;
                supplierDto.Address = formResult["Address"]!;
                supplierDto.City = formResult["City"]!;
                supplierDto.Postal = formResult["Postal"]!;
                supplierDto.Phone = formResult["Phone"]!;
                supplierDto.Email = formResult["Email"]!;
                supplierDto.Web = formResult["Web"];

                // If form contains image file, then set supplierDto's
                // Image property to the file contained in formResult
                if (formResult.Files.Count > 0)
                {
                    supplierDto.Image = formResult.Files[0];
                }

                // Validate supplierDto using SupplierRepository's ValidateSupplierAsync method
                var errorCheck = await unitOfWork.SupplierRepository.ValidateSupplierAsync(supplierDto);

                // If there are errors, return status code 400 (Bad Request) along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke SupplierRepository's method for creating new Supplier
                    await unitOfWork.SupplierRepository.CreateNewSupplierAsync(supplierDto, imagesFolder);
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

            // PATCH method for editing selected Supplier
            app.MapPatch("api/suppliers/patch", async ([FromServices] IUnitOfWork unitOfWork, HttpRequest request) =>
            {
                // Create new SupplierDto object
                SupplierDto supplierDto = new();

                // Read the form from the request and
                // set properties of supplierDto
                var formResult = await request.ReadFormAsync();

                supplierDto.Id = int.Parse(formResult["Id"]!);
                supplierDto.Code = formResult["Code"]!;
                supplierDto.Name = formResult["Name"]!;
                supplierDto.Contact = formResult["Contact"]!;
                supplierDto.Address = formResult["Address"]!;
                supplierDto.City = formResult["City"]!;
                supplierDto.Postal = formResult["Postal"]!;
                supplierDto.Phone = formResult["Phone"]!;
                supplierDto.Email = formResult["Email"]!;
                supplierDto.Web = formResult["Web"];

                // If form contains image file, then set supplierDto's
                // Image property to the file contained in formResult
                if (formResult.Files.Count > 0)
                {
                    supplierDto.Image = formResult.Files[0];
                }

                // Validate supplierDto using SupplierRepository's ValidateSupplierAsync method
                var errorCheck = await unitOfWork.SupplierRepository.ValidateSupplierAsync(supplierDto);

                // If there are errors, return status code 400 (Bad Request) along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke SupplierRepository's method for editing selected Supplier
                    await unitOfWork.SupplierRepository.EditSupplierAsync(supplierDto, imagesFolder);
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

            // DELETE method for deleting selected Supplier
            app.MapDelete("api/suppliers/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke SupplierRepository's method for deleting selected Supplier
                    await unitOfWork.SupplierRepository.DeleteSupplierAsync(id, imagesFolder);
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

            // GET method for returning all Supplier records
            app.MapGet("api/suppliers/getall", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke SupplierRepository's method for returning
                // all SupplierDto object
                var allSuppliers = await unitOfWork.SupplierRepository.GetAllSuppliersAsync();

                return await Task.FromResult(allSuppliers);
            });
        }
    }
}
