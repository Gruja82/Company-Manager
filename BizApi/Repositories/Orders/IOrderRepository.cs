using SharedProject.Dtos;

namespace BizApi.Repositories.Orders
{
    // Interface that declares operations that can be applied
    // on OrderDto objects
    public interface IOrderRepository
    {
        // Return collection of OrderDto objects
        Task<Pagination<OrderDto>> GetOrdersCollectionAsync(string? searchText, string? orderDate, int customerId, int productId, int pageIndex, int pageSize);
        // Return single OrderDto object
        Task<OrderDto> GetSingleOrderAsync(int id);
        // Create new Order
        Task CreateNewOrderAsync(OrderDto orderDto);
        // Edit selected Order
        Task EditOrderAsync(OrderDto orderDto);
        // Delete selected Order
        Task DeleteOrderAsync(int id);
        // Custom validation
        Task<Dictionary<string, string>> ValidateOrderAsync(OrderDto orderDto);
        // Return all Order dates
        Task<List<string>> GetOrderDates();
    }
}
