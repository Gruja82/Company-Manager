using BizManager.Services.Categories;
using BizManager.Services.Customers;
using BizManager.Services.Materials;
using BizManager.Services.Orders;
using BizManager.Services.Productions;
using BizManager.Services.Products;
using BizManager.Services.Purchases;
using BizManager.Services.Suppliers;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BizManager.Extensions
{
    // This static class contains extension methods for facilitating Program.cs file
    public static class ExtensionMethods
    {
        public static void AddServicesToContainer(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IMaterialService, MaterialService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductionService, ProductionService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IPurchaseService, PurchaseService>();
        }
    }
}
