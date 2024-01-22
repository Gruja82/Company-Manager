using BizApi.Extensions;
using BizApi.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedProject.Dtos;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace BizApi.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Product entity
    public static class ProductModule
    {
        public static void MapProductEndpoint(this WebApplication app, string imagesFolder)
        {
            // GET method for returning paginated list of ProductDto objects
            app.MapGet("api/products", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] int categoryId,
                                              [FromQuery] int minimum, [FromQuery] int pageIndex, [FromQuery] int pageSize) =>
            {
                // Invoke ProductRepository's method for returning
                // collection of ProductDto object
                var paginatedResult = await unitOfWork.ProductRepository.GetProductsCollectionAsync(searchText ?? string.Empty, categoryId, minimum, pageIndex, pageSize);

                return await Task.FromResult(paginatedResult);
            });

            // GET method for returning single ProductDto object
            app.MapGet("api/products/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke ProductRepository's method for returning
                // single ProductDto object
                ProductDto? productDto = await unitOfWork.ProductRepository.GetSingleProductAsync(id);

                // If productDto is not null, then return productDto,
                // otherwise return new ProductDto object
                return productDto ?? new ProductDto();
            });

            // POST method for creating new Product
            app.MapPost("api/products/create", async ([FromServices] IUnitOfWork unitOfWork, HttpRequest request) =>
            {
                // Create new ProductDto object
                ProductDto productDto = new();

                // Read the form from the request and
                // set properties of productDto
                var formResult = await request.ReadFormAsync();

                productDto.Code = formResult["Code"]!;
                productDto.Name = formResult["Name"]!;
                productDto.CategoryId = int.Parse(formResult["CategoryId"]!);
                productDto.Price = formResult["Price"]!.ToString().ConvertStringToDouble();
                productDto.Unit = formResult["Unit"]!;
                productDto.ProductDetailDtos = JsonConvert.DeserializeObject<List<ProductDetailDto>>(formResult["ProductDetailDtos"]!)!;

                // If form contains image file, then set productDto's
                // Image property to the file contained in formResult
                if (formResult.Files.Count > 0)
                {
                    productDto.Image = formResult.Files[0];
                }

                // Validate productDto using ProductRepository's ValidateProductAsync method
                var errorCheck = await unitOfWork.ProductRepository.ValidateProductAsync(productDto);

                // If there are errors, return status code 400 (Bad Request) along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke ProductRepository's method for creating new Product
                    await unitOfWork.ProductRepository.CreateNewProductAsync(productDto, imagesFolder);
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

            // PATCH method for editing selected Product
            app.MapPatch("api/products/patch", async ([FromServices] IUnitOfWork unitOfWork, HttpRequest request) =>
            {
                // Create new ProductDto object
                ProductDto productDto = new();

                // Read the form from the formDataContent and
                // set properties of productDto
                var formResult = await request.ReadFormAsync();

                productDto.Id = int.Parse(formResult["Id"]!.ToString());
                productDto.Code = formResult["Code"]!;
                productDto.Name = formResult["Name"]!;
                productDto.CategoryId = int.Parse(formResult["CategoryId"]!);
                productDto.Price = formResult["Price"]!.ToString().ConvertStringToDouble();
                productDto.Unit = formResult["Unit"]!;
                productDto.ProductDetailDtos = JsonConvert.DeserializeObject<List<ProductDetailDto>>(formResult["ProductDetailDtos"]!)!;

                // If form contains image file, then set productDto's
                // Image property to the file contained in formResult
                if (formResult.Files.Count > 0)
                {
                    productDto.Image = formResult.Files[0];
                }

                // Validate productDto using ProductRepository's ValidateProductAsync method
                var errorCheck = await unitOfWork.ProductRepository.ValidateProductAsync(productDto);

                // If there are errors, return status code 400 (Bad Request) along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke ProductRepository's method for creating new Product
                    await unitOfWork.ProductRepository.EditProductAsync(productDto, imagesFolder);
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

            // DELETE method for deleting selected Material
            app.MapDelete("api/products/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke ProductRepository's method for deleting selected Material
                    await unitOfWork.ProductRepository.DeleteProductAsync(id, imagesFolder);
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

            app.MapGet("api/products/getall", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                List<ProductDto>? productDtos = await unitOfWork.ProductRepository.GetAllProductsAsync();

                return productDtos;
            });
        }
    }
}
