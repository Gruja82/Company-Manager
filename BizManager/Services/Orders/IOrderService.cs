using SharedProject.Dtos;

namespace BizManager.Services.Orders
{
    // Interface that defines methods for communicating with Server
    public interface IOrderService
    {
        // Return paginated filtered list of OrderDto objects
        Task<Pagination<OrderDto>> GetOrdersAsync(string? searchText, string? orderDate,int customerId, int productId, int pageIndex, int pageSize);
        // Return single OrderDto object
        Task<OrderDto> GetSingleOrderAsync(int id);
        // Create new Order
        Task<object> CreateNewOrderAsync(OrderDto orderDto);
        // Edit selected Order
        Task<object> EditOrderAsync(OrderDto orderDto);
        // Delete selected Order
        Task<int> DeleteOrderAsync(int id);
        // Return list of Order dates
        Task<List<string>> GetOrderDatesAsync();
    }
}
