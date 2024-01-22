using BizApi.Data.Database;
using BizApi.Data.Entities;
using BizApi.Extensions;
using BizApi.Utility;
using Microsoft.EntityFrameworkCore;
using SharedProject.Dtos;

namespace BizApi.Repositories.Orders
{
    // Implementation class for IOrderRepository
    public class OrderRepository:IOrderRepository
    {
        private readonly AppDbContext context;

        public OrderRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Order
        public async Task CreateNewOrderAsync(OrderDto orderDto)
        {
            // Create new Order object
            Order order = new();

            // Set it's properties to the ones contained in orderDto
            order.Code = orderDto.Code;
            order.OrderDate = orderDto.OrderDate;
            order.Customer = (await context.Customers.FindAsync(orderDto.CustomerId))!;

            // Add order to database
            await context.Orders.AddAsync(order);

            // Iterate through orderDto's OrderDetailDtos collection
            // and add OrderDetail records to database
            foreach (var orderDetailDto in orderDto.OrderDetailDtos)
            {
                OrderDetail orderDetail = new();

                orderDetail.Order = order;
                orderDetail.Product = (await context.Products.FindAsync(orderDetailDto.ProductId))!;
                orderDetail.QtyProduct = orderDetailDto.Quantity;

                await context.OrderDetails.AddAsync(orderDetail);

                // Reduce Product's quantity
                orderDetail.Product.Qty -= orderDetailDto.Quantity;
            }
        }

        // Delete selected Order
        public async Task DeleteOrderAsync(int id)
        {
            // Find Order by id
            Order order = (await context.Orders
                                 .Include(e => e.OrderDetails)
                                 .FirstOrDefaultAsync(e => e.Id == id))!;

            // Remove Order from database
            context.Orders.Remove(order);

            // Remove all OrderDetail records from database
            // which are related to order
            foreach (var orderDetail in order.OrderDetails)
            {
                context.OrderDetails.Remove(orderDetail);
            }
        }

        // Edit selected Order
        public async Task EditOrderAsync(OrderDto orderDto)
        {
            // Find Order by id
            Order order = (await context.Orders
                                 .Include(e => e.OrderDetails)
                                 .ThenInclude(e => e.Product)
                                 .FirstOrDefaultAsync(e => e.Id == orderDto.Id))!;

            // Set it's properties to the ones contained in orderDto
            order.Code = orderDto.Code;
            order.OrderDate = orderDto.OrderDate;
            order.Customer = (await context.Customers.FindAsync(orderDto.CustomerId))!;

            // Remove all OrderDetail records from database
            // which are related to order
            foreach (var orderDetail in order.OrderDetails)
            {
                context.OrderDetails.Remove(orderDetail);

                // Restore Product's quantity
                orderDetail.Product.Qty += orderDetail.QtyProduct;
            }

            // Iterate through orderDto's OrderDetailDtos collection
            // and add OrderDetail records to database
            foreach (var orderDetailDto in orderDto.OrderDetailDtos)
            {
                OrderDetail orderDetail = new();

                orderDetail.Order = order;
                orderDetail.Product = (await context.Products.FindAsync(orderDetailDto.ProductId))!;
                orderDetail.QtyProduct = orderDetailDto.Quantity;

                await context.OrderDetails.AddAsync(orderDetail);

                // Reduce Product's quantity
                orderDetail.Product.Qty -= orderDetailDto.Quantity;
            }
        }

        // Return paginated collection of OrderDto objects
        public async Task<Pagination<OrderDto>> GetOrdersCollectionAsync(string? searchText, string? orderDate, int customerId, int productId, int pageIndex, int pageSize)
        {
            // Get all Order objects
            var allOrders = context.Orders
                           .Include(e => e.Customer)
                           .Include(e => e.OrderDetails)
                           .ThenInclude(e => e.Product)
                           .AsNoTracking()
                           .AsQueryable();

            // If searchText is not null or empty string,
            // filter allOrders by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allOrders = allOrders.Where(e => e.Code.ToLower().Contains(searchText.ToLower()));
            }

            // If orderDate is not null or empty string,
            // filter allOrders by orderDate
            if (!string.IsNullOrEmpty(orderDate))
            {
                DateTime ordDate = DateTime.Parse(orderDate);
                allOrders = allOrders.Where(e => e.OrderDate == ordDate);
            }

            // If customerId is greater than 0 (zero),
            // it means that user wants to filter results
            // by customer (Id) and therefore we filter 
            // allOrders by customerId
            if (customerId > 0)
            {
                allOrders = allOrders.Where(e => e.Customer == context.Customers.Find(customerId));
            }

            // If productId is greater than 0 (zero),
            // it means that user wants to filter results
            // by product (Id) and therefore we filter 
            // allOrders by productId
            if (productId > 0)
            {
                allOrders = allOrders.Where(e => e.OrderDetails.Select(e => e.Product).Contains(context.Products.Find(productId)));
            }

            // Variable that will contain OrderDto objects
            List<OrderDto> orderDtos = new();

            // Iterate through allOrders and populate orderDtos
            // using Order's extension method ConvertToDto
            foreach (var order in allOrders)
            {
                orderDtos.Add(order.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult method,
            // return Pagination<OrderDto> object
            var paginatedResult = PaginationUtility<OrderDto>.GetPaginatedResult(in orderDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single OrderDto object
        public async Task<OrderDto> GetSingleOrderAsync(int id)
        {
            // Find Order by id
            Order order = (await context.Orders
                           .Include(e => e.Customer)
                           .Include(e => e.OrderDetails)
                           .ThenInclude(e => e.Product)
                           .FirstOrDefaultAsync(e => e.Id == id))!;

            // Return OrderDto using Order's extension method
            // ConvertToDto
            return order.ConvertToDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateOrderAsync(OrderDto orderDto)
        {
            // Define variable that will contain possible errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Order records
            var allOrders = context.Orders
                            .Include(e => e.OrderDetails)
                            .ThenInclude(e => e.Product)
                            .AsNoTracking()
                            .AsQueryable();

            // If orderDto's Id value is greater than 0, it means that Order
            // is used in Update operation
            if (orderDto.Id > 0)
            {
                // Find Order by Id
                Order order = (await context.Orders
                                     .Include(e => e.Customer)
                                     .Include(e => e.OrderDetails)
                                     .ThenInclude(e => e.Product)
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(e => e.Id == orderDto.Id))!;

                // If order's Code is modified, check for Code uniqueness
                // among all Order records
                if (order.Code != orderDto.Code)
                {
                    // If provided orderDto's Code is already contained in
                    // any of the Order records then add error to errors Dictionary
                    if (allOrders.Select(e => e.Code.ToLower()).Contains(orderDto.Code.ToLower()))
                    {
                        errors.Add("Code", "There is already Order with this Code in database. Please provide different one!");
                    }
                }
            }
            // Otherwise it means that Order is used in Create operation
            else
            {
                // If provided orderDto's Code is already contained in
                // any of the Order records then add error to errors Dictionary
                if (allOrders.Select(e => e.Code.ToLower()).Contains(orderDto.Code.ToLower()))
                {
                    errors.Add("Code", "There is already Order with this Code in database. Please provide different one!");
                }
            }

            // If orderDto's customerId is equal to 0 (zero),
            // it means that user has not selected a Customer
            // and therefore add error to errors Dictionary
            if (orderDto.CustomerId == 0)
            {
                errors.Add("CustomerId", "Please select Customer from list!");
            }

            // If OrderDate is larger than today's date
            // then add error to errors Dictionary
            if (orderDto.OrderDate > DateTime.Now)
            {
                errors.Add("OrderDate", "Order date must not be larger than today's date!");
            }

            foreach (var orderDetailDto in orderDto.OrderDetailDtos)
            {
                Product product = context.Products.Find(orderDetailDto.ProductId)!;

                if (orderDetailDto.Quantity > product.Qty)
                {
                    errors.Add("OrderDetailDtos", "Error! Check if there is enough Product inventory.");

                    return errors;
                }
            }

            return errors;
        }

        // Return all Order dates
        public async Task<List<string>> GetOrderDates()
        {
            List<string> orderDates = await context.Orders.Select(e => e.OrderDate.ToShortDateString()).Distinct().ToListAsync();

            return orderDates;
        }
    }
}
