using BizApi.Data.Database;
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
    // Implementation class for IUnitOfWork
    public class UnitOfWork:IUnitOfWork
    {
        private readonly AppDbContext context;

        public UnitOfWork(AppDbContext context, ICategoryRepository categoryRepository, ICustomerRepository customerRepository,
            IMaterialRepository materialRepository, IProductRepository productRepository, IProductionRepository productionRepository,
            IOrderRepository orderRepository, ISupplierRepository supplierRepository, IPurchaseRepository purchaseRepository)
        {
            this.context = context;
            CategoryRepository = categoryRepository;
            CustomerRepository = customerRepository;
            MaterialRepository = materialRepository;
            ProductRepository = productRepository;
            ProductionRepository = productionRepository;
            OrderRepository = orderRepository;
            SupplierRepository = supplierRepository;
            PurchaseRepository = purchaseRepository;
        }

        public ICategoryRepository CategoryRepository { get; }
        public ICustomerRepository CustomerRepository { get; }
        public IMaterialRepository MaterialRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IProductionRepository ProductionRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public ISupplierRepository SupplierRepository { get; }
        public IPurchaseRepository PurchaseRepository { get; }

        // Save changes made on entities
        public async Task ConfirmChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        // Method for manual disposing DbContext object
        public void Dispose()
        {
            context.Dispose();
            GC.SuppressFinalize(this);
        }

        // Rollback changes made on entities
        public void RollBackChanges()
        {
            foreach (var entityEntry in context.ChangeTracker.Entries())
            {
                switch (entityEntry.State)
                {
                    case EntityState.Deleted:
                        entityEntry.Reload();
                        break;
                    case EntityState.Modified:
                        entityEntry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entityEntry.State = EntityState.Detached;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
