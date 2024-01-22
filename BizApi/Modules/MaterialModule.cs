using BizApi.Extensions;
using BizApi.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using SharedProject.Dtos;

namespace BizApi.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Material entity
    public static class MaterialModule
    {
        public static void MapMaterialEndpoint(this WebApplication app, string imagesFolder)
        {
            // GET method for returning paginated list of MaterialDto objects
            app.MapGet("api/materials", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] int categoryId,
                                                [FromQuery] int pageIndex, [FromQuery] int pageSize) =>
            {
                // Invoke MaterialRepository's method for returning
                // collection of MaterialDto object
                var paginatedResult = await unitOfWork.MaterialRepository.GetMaterialsCollectionAsync(searchText ?? string.Empty, categoryId, pageIndex, pageSize);

                return await Task.FromResult(Results.Ok(paginatedResult));
            });

            // GET method for returning single MaterialDto object
            app.MapGet("api/materials/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke MaterialRepository's method for returning
                // single MaterialDto object
                MaterialDto? materialDto = await unitOfWork.MaterialRepository.GetSingleMaterialAsync(id);

                // If materialDto is not null, then return materialDto,
                // otherwise return new MaterialDto object
                return materialDto ?? new MaterialDto();
            });

            // POST method for creating new Material
            app.MapPost("api/materials/create", async ([FromServices] IUnitOfWork unitOfWork, HttpRequest request) =>
            {
                // Create new MaterialDto object
                MaterialDto materialDto = new();

                // Read the form from the request and
                // set properties of materialDto 
                var formResult = await request.ReadFormAsync();

                materialDto.Code = formResult["Code"]!;
                materialDto.Name = formResult["Name"]!;
                materialDto.CategoryId = int.Parse(formResult["CategoryId"]!);
                materialDto.Quantity = 0;
                materialDto.Unit = formResult["Unit"]!;
                materialDto.Price = formResult["Price"]!.ToString().ConvertStringToDouble();

                // If form contains image file, then set materialDto's
                // Image property to the file contained in formResult
                if (formResult.Files.Count > 0)
                {
                    materialDto.Image = formResult.Files[0];
                }

                // Validate materialDto using MaterialRepository's ValidateMaterialAsync method
                var errorCheck = await unitOfWork.MaterialRepository.ValidateMaterialAsync(materialDto);

                // If there are errors, return status code 400 (Bad Request) along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke MaterialRepository's method for creating new Material
                    await unitOfWork.MaterialRepository.CreateNewMaterialAsync(materialDto, imagesFolder);
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

            // PATCH method for editing selected Material
            app.MapPatch("api/materials/patch", async ([FromServices] IUnitOfWork unitOfWork, HttpRequest request) =>
            {
                // Create new MaterialDto object
                MaterialDto materialDto = new();

                // Read the form from the formDataContent and
                // set properties of materialDto 
                var formResult = await request.ReadFormAsync();

                materialDto.Id = int.Parse(formResult["Id"]!.ToString());
                materialDto.Code = formResult["Code"]!;
                materialDto.Name = formResult["Name"]!;
                materialDto.CategoryId = int.Parse(formResult["CategoryId"]!);
                materialDto.Quantity = 0;
                materialDto.Unit = formResult["Unit"]!;
                materialDto.Price = formResult["Price"]!.ToString().ConvertStringToDouble();

                // If form contains image file, then set materialDto's
                // Image property to the file contained in formResult
                if (formResult.Files.Count > 0)
                {
                    materialDto.Image = formResult.Files[0];
                }

                // Validate materialDto using MaterialRepository's ValidateMaterialAsync method
                var errorCheck = await unitOfWork.MaterialRepository.ValidateMaterialAsync(materialDto);

                // If there are errors, return status code 400 (Bad Request) along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke MaterialRepository's method for editing selected Material
                    await unitOfWork.MaterialRepository.EditMaterialAsync(materialDto, imagesFolder);
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

            // DELETE method for deleting selected Material
            app.MapDelete("api/materials/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke MaterialRepository's method for deleting selected Material
                    await unitOfWork.MaterialRepository.DeleteMaterialAsync(id, imagesFolder);
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

            // GET method for returning list of all Material records
            app.MapGet("api/materials/getall", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke MaterialRepository's method for 
                // returning list of all Material records
                List<MaterialDto> materialDtos = await unitOfWork.MaterialRepository.GetAllMaterialsAsync();

                return await Task.FromResult(Results.Ok(materialDtos));
            });
        }
    }
}
