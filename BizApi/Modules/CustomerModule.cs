using BizApi.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using SharedProject.Dtos;

namespace BizApi.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Customer entity
    public static class CustomerModule
    {
        public static void MapCustomerEndpoint(this WebApplication app, string imagesFolder)
        {
            // GET method for returning paginated list of CustomerDto objects
            app.MapGet("api/customers", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] int pageIndex, [FromQuery] int pageSize) =>
            {
                // Invoke CustomerRepository's method for returning
                // collection of CustomerDto object
                var paginatedResult = await unitOfWork.CustomerRepository.GetCustomersCollectionAsync(searchText ?? string.Empty, pageIndex, pageSize);

                return await Task.FromResult(Results.Ok(paginatedResult));
            });

            // GET method for returning single CustomerDto object
            app.MapGet("api/customers/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke CustomerRepository's method for returning
                // single CustomerDto object
                CustomerDto? customerDto = await unitOfWork.CustomerRepository.GetSingleCustomerAsync(id);

                // If categoryDto is not null, then return categoryDto,
                // otherwise return new CategoryDto object
                return customerDto ?? new CustomerDto();
            });

            // POST method for creating new Customer
            app.MapPost("api/customers/create", async ([FromServices] IUnitOfWork unitOfWork, HttpRequest request) =>
            {
                // Create new CustomerDto object
                CustomerDto customerDto = new();

                // Read the form from the request and
                // set properties of customerDto
                var formResult = await request.ReadFormAsync();

                customerDto.Code = formResult["Code"]!;
                customerDto.Name = formResult["Name"]!;
                customerDto.Contact = formResult["Contact"]!;
                customerDto.Address = formResult["Address"]!;
                customerDto.City = formResult["City"]!;
                customerDto.Postal = formResult["Postal"]!;
                customerDto.Phone = formResult["Phone"]!;
                customerDto.Email = formResult["Email"]!;
                customerDto.Web = formResult["Web"];

                // If form contains image file, then set customerDto's
                // Image property to the file contained in formResult
                if (formResult.Files.Count > 0)
                {
                    customerDto.Image = formResult.Files[0];
                }

                // Validate customerDto using CustomerRepository's ValidateCustomerAsync method
                var errorCheck = await unitOfWork.CustomerRepository.ValidateCustomerAsync(customerDto);

                // If there are errors, return status code 400 (Bad Request) along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke CustomerRepository's method for creating new Customer
                    await unitOfWork.CustomerRepository.CreateNewCustomerAsync(customerDto, imagesFolder);
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

            // PATCH method for editing selected Customer
            app.MapPatch("api/customers/patch", async ([FromServices] IUnitOfWork unitOfWork, HttpRequest request) =>
            {
                // Create new CustomerDto object
                CustomerDto customerDto = new();

                // Read the form from the formDataContent and
                // set properties of customerDto
                var formResult = await request.ReadFormAsync();

                customerDto.Id = int.Parse(formResult["Id"]!);
                customerDto.Code = formResult["Code"]!;
                customerDto.Name = formResult["Name"]!;
                customerDto.Contact = formResult["Contact"]!;
                customerDto.Address = formResult["Address"]!;
                customerDto.City = formResult["City"]!;
                customerDto.Postal = formResult["Postal"]!;
                customerDto.Phone = formResult["Phone"]!;
                customerDto.Email = formResult["Email"]!;
                customerDto.Web = formResult["Web"];

                // If form contains image file, then set customerDto's
                // Image property to the file contained in formResult
                if (formResult.Files.Count > 0)
                {
                    customerDto.Image = formResult.Files[0];
                }

                // Validate customerDto using CustomerRepository's ValidateCustomerAsync method
                var errorCheck = await unitOfWork.CustomerRepository.ValidateCustomerAsync(customerDto);

                // If there are errors, return status code 400 (Bad Request) along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke CustomerRepository's method for editing selected Customer
                    await unitOfWork.CustomerRepository.EditCustomerAsync(customerDto, imagesFolder);
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

            // DELETE method for deleting selected Customer
            app.MapDelete("api/customers/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke CustomerRepository's method for deleting selected Customer
                    await unitOfWork.CustomerRepository.DeleteCustomerAsync(id, imagesFolder);
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

            // GET method for returning all Customer records
            app.MapGet("api/customers/getall", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke CustomerRepository's method for returning
                // all CustomerDto object
                var allCustomers = await unitOfWork.CustomerRepository.GetAllCustomersAsync();

                return await Task.FromResult(allCustomers);
            });
        }
    }
}
