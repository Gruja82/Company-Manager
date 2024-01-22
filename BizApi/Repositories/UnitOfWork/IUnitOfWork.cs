using BizApi.Repositories.Categories;
using BizApi.Repositories.Customers;
using BizApi.Repositories.Materials;
using BizApi.Repositories.Orders;
using BizApi.Repositories.Productions;
using BizApi.Repositories.Products;
using BizApi.Repositories.Purchases;
using BizApi.Repositories.Suppliers;
using Microsoft.EntityFrameworkCore;

namespace BizApi.Repositories.UnitOfWork
{
    // Interface that wraps up specific entity repositories
    // and declares method for saving changes to database
    public interface IUnitOfWork:IDisposable
    {
        public ICategoryRepository CategoryRepository { get; }
        public ICustomerRepository CustomerRepository { get; }
        public IMaterialRepository MaterialRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IProductionRepository ProductionRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public ISupplierRepository SupplierRepository { get; }
        public IPurchaseRepository PurchaseRepository { get; }
        // Save changes made on entities
        Task ConfirmChangesAsync();
        // Rollback changes made on entities
        void RollBackChanges();
    }
}
