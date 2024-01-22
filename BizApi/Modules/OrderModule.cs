using BizApi.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedProject.Dtos;

namespace BizApi.Modules
{
    // This static class defines extension methods
    // which define all API endoints for Order entity
    public static class OrderModule
    {
        public static void MapOrderEndpoint(this WebApplication app)
        {
            // GET method for returning paginated list of OrderDto objects
            app.MapGet("api/orders", async ([FromServices] IUnitOfWork unitOfWork, [FromQuery] string? searchText, [FromQuery] string? orderDate,
                [FromQuery] int customerId, [FromQuery] int productId, [FromQuery] int pageIndex, [FromQuery] int pageSize) =>
            {
                // Invoke OrderRepository's method for returning
                // collection of OrderDto object
                var paginatedResult = await unitOfWork.OrderRepository.GetOrdersCollectionAsync(searchText, orderDate, customerId, productId, pageIndex, pageSize);

                return await Task.FromResult(paginatedResult);
            });

            // GET method for returning single OrderDto object
            app.MapGet("api/orders/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                // Invoke OrderRepository's method for returning
                // single OrderDto object
                OrderDto? orderDto = await unitOfWork.OrderRepository.GetSingleOrderAsync(id);

                // If orderDto is not null, then return orderDto,
                // otherwise return new OrderDto object
                return orderDto ?? new OrderDto();
            });

            // POST method for creating new Order
            app.MapPost("api/orders/create", async ([FromServices] IUnitOfWork unitOfWork, OrderDto orderDto) =>
            {
                // Validate orderDto using OrderRepository's method
                // ValidateOrderAsync
                var errorCheck = await unitOfWork.OrderRepository.ValidateOrderAsync(orderDto);

                // If there are any errors, then return BadRequest status code along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke OrderRepository's method for creating new Order
                    await unitOfWork.OrderRepository.CreateNewOrderAsync(orderDto);
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

            // PATCH method for editing selected Order
            app.MapPatch("api/orders/patch", async ([FromServices] IUnitOfWork unitOfWork, OrderDto orderDto) =>
            {
                // Validate orderDto using OrderRepository's method
                // ValidateOrderAsync
                var errorCheck = await unitOfWork.OrderRepository.ValidateOrderAsync(orderDto);

                // If there are any errors, then return BadRequest status code along with errorCheck
                if (errorCheck.Any())
                {
                    return Results.BadRequest(errorCheck);
                }

                try
                {
                    // Invoke OrderRepository's method for editing selected Order
                    await unitOfWork.OrderRepository.EditOrderAsync(orderDto);
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

            // DELETE method for deleting selected Order
            app.MapDelete("api/orders/delete/{id}", async ([FromServices] IUnitOfWork unitOfWork, [FromRoute] int id) =>
            {
                try
                {
                    // Invoke OrderRepository's method for deleting selected Order
                    await unitOfWork.OrderRepository.DeleteOrderAsync(id);
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

            // GET method for returning list of Order Dates
            app.MapGet("api/orders/getorderdates", async ([FromServices] IUnitOfWork unitOfWork) =>
            {
                // Invoke OrderRepository's method for returning
                // list of Order dates
                List<string>? orderDates = await unitOfWork.OrderRepository.GetOrderDates();

                // If orderDates is not null then return orderDates
                // otherwise return new List<string> object
                return await Task.FromResult(orderDates) ?? await Task.FromResult(new List<string>());
            });
        }
    }
}
