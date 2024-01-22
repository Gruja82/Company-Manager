using BizApi.Data.Database;
using BizApi.Data.Entities;
using BizApi.Repositories.Categories;
using BizApi.Repositories.Customers;
using BizApi.Repositories.Materials;
using BizApi.Repositories.Orders;
using BizApi.Repositories.Productions;
using BizApi.Repositories.Products;
using BizApi.Repositories.Purchases;
using BizApi.Repositories.Suppliers;
using BizApi.Repositories.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using SharedProject.Dtos;
using System.Globalization;
using System.Text;

namespace BizApi.Extensions
{
    // This static class contains extension methods for facilitating Program.cs
    // file and to avoid writing similar code over and over
    public static class ExtensionMethods
    {
        // This extension method fills database tables with some initial data
        public static void SeedSomeData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasData(
                        new Customer { Id = 1, Code = "35981655", Name = "Alfreds Futterkiste", Contact = "Maria Anders", Address = "Obere Str. 57", City = "Berlin", Postal = "12209", Phone = "030-0074321", Email = "alb@yahoo.com" },
                        new Customer { Id = 2, Code = "21891648", Name = "Ana Trujillo Emparedados y helados", Contact = "Ana Trujillo", Address = "Avda. de la Constitución 2222", City = "México D.F.", Postal = "05021", Phone = "030-0074321", Email = "anna@yahoo.com" },
                        new Customer { Id = 3, Code = "54818898", Name = "Antonio Moreno Taquería", Contact = "Antonio Moreno", Address = "Mataderos  2312", City = "México D.F.", Postal = "05023", Phone = "(5) 555-3932", Email = "antonio@yahoo.com" },
                        new Customer { Id = 4, Code = "9988557", Name = "Around the Horn", Contact = "Thomas Hardy", Address = "120 Hanover Sq.", City = "London", Postal = "WA1 1DP", Phone = "(171) 555-7788", Email = "around@yahoo.com" },
                        new Customer { Id = 5, Code = "6545748", Name = "Berglunds snabbköp", Contact = "Christina Berglund", Address = "Berguvsvägen  8", City = "Luleå", Postal = "S-958 22", Phone = "0921-12 34 65", Email = "berg@yahoo.com" },
                        new Customer { Id = 6, Code = "3551578", Name = "Blauer See Delikatessen", Contact = "Hanna Moos", Address = "Forsterstr. 57", City = "Mannheim", Postal = "68306", Phone = "0621-08460", Email = "blaurer@yahoo.com" },
                        new Customer { Id = 7, Code = "4588745", Name = "Blondesddsl père et fils", Contact = "Frédérique Citeaux", Address = "24, place Kléber", City = "Strasbourg", Postal = "67000", Phone = "88.60.15.31", Email = "blondes@yahoo.com" },
                        new Customer { Id = 8, Code = "7885698", Name = "Bólido Comidas preparadas", Contact = "Martín Sommer", Address = "C/ Araquil, 67", City = "Madrid", Postal = "28023", Phone = "(91) 555 22 82", Email = "bolido@yahoo.com" },
                        new Customer { Id = 9, Code = "14585789", Name = "Bon app'", Contact = "Laurence Lebihan", Address = "12, rue des Bouchers", City = "Marseille", Postal = "13008", Phone = "91.24.45.40", Email = "bonapp@yahoo.com" },
                        new Customer { Id = 10, Code = "85487125", Name = "Bottom-Dollar Markets", Contact = "Elizabeth Lincoln", Address = "23 Tsawassen Blvd.", City = "Tsawassen", Postal = "T2F 8M4", Phone = "(604) 555-4729", Email = "bottomdollar@yahoo.com" },
                        new Customer { Id = 11, Code = "2125469", Name = "B's Beverages", Contact = "Victoria Ashworth", Address = "Fauntleroy Circus", City = "London", Postal = "EC2 5NT", Phone = "(171) 555-1212", Email = "beverages@yahoo.com" }
                        );

            modelBuilder.Entity<Supplier>()
                .HasData(
                        new Supplier { Id = 1, Code = "3545168", Name = "Cactus Comidas para llevar", Contact = "Patricio Simpson", Address = "Cerrito 333", City = "Buenos Aires", Postal = "1010", Phone = "(1) 135-5555", Email = "cactus@yahoo.com" },
                        new Supplier { Id = 2, Code = "648945515", Name = "Centro comercial Moctezuma", Contact = "Francisco Chang", Address = "Sierras de Granada 9993", City = "México D.F.", Postal = "05022", Phone = "(5) 555-3392", Email = "centro@yahoo.com" },
                        new Supplier { Id = 3, Code = "318654", Name = "Chop-suey Chinese", Contact = "Yang Wang", Address = "Hauptstr. 29", City = "Bern", Postal = "3012", Phone = "0452-076545", Email = "chop@yahoo.com" },
                        new Supplier { Id = 4, Code = "569698547", Name = "Comércio Mineiro", Contact = "Pedro Afonso", Address = "Av. dos Lusíadas, 23", City = "Sao Paulo", Postal = "05432-043", Phone = "(11) 555-7647", Email = "comercio@yahoo.com" },
                        new Supplier { Id = 5, Code = "6542247", Name = "Consolidated Holdings", Contact = "Elizabeth Brown", Address = "Berkeley Gardens 12  Brewery", City = "London", Postal = "WX1 6LT", Phone = "(171) 555-2282", Email = "consolidated@yahoo.com" },
                        new Supplier { Id = 6, Code = "41578", Name = "Drachenblut Delikatessen", Contact = "Sven Ottlieb", Address = "Walserweg 21", City = "Aachen", Postal = "52066", Phone = "0241-039123", Email = "drachen@yahoo.com" },
                        new Supplier { Id = 7, Code = "332654", Name = "Du monde entier", Contact = "Janine Labrune", Address = "67, rue des Cinquante Otages", City = "Nantes", Postal = "44000", Phone = "40.67.88.88", Email = "dumonde@yahoo.com" },
                        new Supplier { Id = 8, Code = "645879", Name = "Eastern Connection", Contact = "Ann Devon", Address = "35 King George", City = "London", Postal = "WX3 6FW", Phone = "(171) 555-0297", Email = "eastern@yahoo.com" },
                        new Supplier { Id = 9, Code = "125487", Name = "Ernst Handel", Contact = "Roland Mendel", Address = "Kirchgasse 6", City = "Graz", Postal = "8010", Phone = "7675-3425", Email = "ernst@yahoo.com" },
                        new Supplier { Id = 10, Code = "4526987", Name = "Familia Arquibaldo", Contact = "Aria Cruz", Address = "Rua Orós, 92", City = "Sao Paulo", Postal = "05442-030", Phone = "(11) 555-9857", Email = "familia@yahoo.com" },
                        new Supplier { Id = 11, Code = "1254873", Name = "FISSA Fabrica Inter. Salchichas S.A.", Contact = "Diego Roel", Address = "C/ Moralzarzal, 86", City = "Madrid", Postal = "28034", Phone = "(91) 555 94 44", Email = "fissa@yahoo.com" }
                );

            modelBuilder.Entity<Category>()
                .HasData(
                        new Category { Id = 1, Code = "65155", Name = "Beverages", Description = "Soft drinks, coffees, teas, beers, and ales" },
                        new Category { Id = 2, Code = "511516", Name = "Condiments", Description = "Sweet and savory sauces, relishes, spreads, and seasonings" },
                        new Category { Id = 3, Code = "998816", Name = "Confections", Description = "Desserts, candies, and sweet breads" },
                        new Category { Id = 4, Code = "8855447", Name = "Dairy Products", Description = "Cheeses" },
                        new Category { Id = 5, Code = "6646485", Name = "Grains/Cereals", Description = "Breads, crackers, pasta, and cereal" },
                        new Category { Id = 6, Code = "8744581", Name = "Meat/Poultry", Description = "Prepared meats" },
                        new Category { Id = 7, Code = "219848198", Name = "Produce", Description = "Dried fruit and bean curd" },
                        new Category { Id = 8, Code = "2154548", Name = "Seafood", Description = "Seaweed and fish" }
                );
        }

        // This extension method applies any pending migrations that are not run.
        // If database is not created, ite creates it.
        public static void MigrateDatabase(this WebApplication app)
        {
            using (var serviceScope = app.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                var context = services.GetRequiredService<AppDbContext>();

                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
            }
        }

        // This extension method converts string value to double
        public static double ConvertStringToDouble(this string strValue)
        {
            NumberFormatInfo provider = new();
            provider.NumberDecimalSeparator = ".";
            return Convert.ToDouble(strValue, provider);
        }

        // This extension metod extends WebApplicationBuilder by adding functionallity
        // for registering AppDbContext and all services to DI container
        public static void AddServicesToContainer(this WebApplicationBuilder builder, string connString)
        {
            //builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connString));
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connString));

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductionRepository, ProductionRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        // This extension method extends BaseDto object.
        // It's purpose is to store image file in specified folder
        // and to return image file name
        public static string StoreImage(this BaseDto baseDto, string imagesFolder)
        {
            // Variable that represents image file name
            string? imageFileName = string.Empty;

            // If BaseDto's Image is not null, then generate unique file name
            // and store image file in specified folder
            if (baseDto.Image != null)
            {
                StringBuilder sb = new();

                sb.Append(Guid.NewGuid().ToString().Substring(0, 16));
                string[] fileNames = baseDto.Image.FileName.Split('.');
                sb.Append(".");
                sb.Append(fileNames[1]);
                imageFileName = sb.ToString();

                string filePath = Path.Combine(imagesFolder, imageFileName);

                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                using var fileStream = new FileStream(filePath, FileMode.Create);
                baseDto.Image.CopyTo(fileStream);
            }

            // Return image file name
            return imageFileName;
        }

        // This extension method converts Category object to CategoryDto object
        public static CategoryDto ConvertToDto(this Category category)
        {
            // Create new CategoryDto object
            CategoryDto categoryDto = new();

            // Set it's properties to the ones contained in category
            categoryDto.Id = category.Id;
            categoryDto.Code = category.Code;
            categoryDto.Name = category.Name;
            categoryDto.Description = category.Description;

            // Return categoryDto
            return categoryDto;
        }

        // This overloaded extension method converts Customer object to CustomerDto object
        public static CustomerDto ConvertToDto(this Customer customer)
        {
            // Create new CustomerDto object
            CustomerDto customerDto = new();

            // Set it's properties to the ones contained in customer
            customerDto.Id = customer.Id;
            customerDto.Code = customer.Code;
            customerDto.Name = customer.Name;
            customerDto.Contact = customer.Contact;
            customerDto.Address = customer.Address;
            customerDto.City = customer.City;
            customerDto.Postal = customer.Postal;
            customerDto.Phone = customer.Phone;
            customerDto.Email = customer.Email;
            customerDto.Web = customer.Web;
            customerDto.ImageUrl = customer.ImageUrl;

            // Return customerDto
            return customerDto;
        }

        // This overloaded extension method converts Material object to MaterialDto object
        public static MaterialDto ConvertToDto(this Material material)
        {
            // Create new MaterialDto object
            MaterialDto materialDto = new();

            // Set it's properties to the ones contained in material
            materialDto.Id = material.Id;
            materialDto.Code = material.Code;
            materialDto.Name = material.Name;
            materialDto.CategoryId = material.Category.Id;
            materialDto.CategoryName = material.Category.Name;
            materialDto.Quantity = material.Qty;
            materialDto.Unit = material.Unit;
            materialDto.Price = material.Price;
            materialDto.ImageUrl = material.ImageUrl;

            // Return materialDto
            return materialDto;
        }

        // This overloaded extension method convert ProductDetail object to ProductDetailDto object
        public static ProductDetailDto ConvertToDto(this ProductDetail productDetail)
        {
            // Create new ProductDetailDto object
            ProductDetailDto productDetailDto = new();

            // Set it's properties to the ones contained in productDetail
            productDetailDto.Id = productDetail.Id;
            productDetailDto.ProductId = productDetail.Product.Id;
            productDetailDto.MaterialId = productDetail.Material.Id;
            productDetailDto.MaterialQty = productDetail.QtyMaterial;
            productDetailDto.MaterialPrice = productDetail.Material.Price;
            productDetailDto.ProductName = productDetail.Product.Name!;
            productDetailDto.MaterialName = productDetail.Material.Name;

            // Return productDetailDto
            return productDetailDto;
        }

        // This overloaded extension method convert Product object to ProductDto object
        public static ProductDto ConvertToDto(this Product product)
        {
            // Create new ProductDto object
            ProductDto productDto = new();

            // Set it's properties to the ones contained in product
            productDto.Id = product.Id;
            productDto.Code = product.Code!;
            productDto.Name = product.Name!;
            productDto.CategoryId = product.Category.Id;
            productDto.CategoryName = product.Category.Name;
            productDto.Quantity = product.Qty;
            productDto.Unit = product.Unit;
            productDto.Price = product.Price;
            productDto.ImageUrl = product.ImageUrl;
            foreach (var productDetail in product.ProductDetails)
            {
                productDto.ProductDetailDtos.Add(productDetail.ConvertToDto());
            }

            // Return productDto
            return productDto;
        }

        // This overloaded extension method convert Production object to ProductionDto object
        public static ProductionDto ConvertToDto(this Production production)
        {
            // Create new ProductionDto object
            ProductionDto productionDto = new();

            // Set it's properties to the ones contained in production
            productionDto.Id = production.Id;
            productionDto.Code = production.Code;
            productionDto.ProductionDate = production.ProductionDate;
            productionDto.ProductId = production.ProductId;
            productionDto.ProductName = production.Product.Name;
            productionDto.Quantity = production.QtyProduct;

            // Return productionDto
            return productionDto;
        }

        // This overloaded extension method convert OrderDetail object to OrderDetailDto object
        public static OrderDetailDto ConvertToDto(this OrderDetail orderDetail)
        {
            // Create new OrderDetailDto object
            OrderDetailDto orderDetailDto = new();

            // Set it's properties to the ones contained in orderDetail
            orderDetailDto.Id = orderDetail.Id;
            orderDetailDto.OrderId = orderDetail.Order.Id;
            orderDetailDto.ProductId = orderDetail.Product.Id;
            orderDetailDto.ProductName = orderDetail.Product.Name;
            orderDetailDto.ProductPrice = orderDetail.Product.Price;
            orderDetailDto.Quantity = orderDetail.QtyProduct;

            return orderDetailDto;
        }

        // This overloaded extension method convert Order object to OrderDto object
        public static OrderDto ConvertToDto(this Order order)
        {
            // Create new OrderDto object
            OrderDto orderDto = new();

            // Set it's properties to the ones contained in order
            orderDto.Id = order.Id;
            orderDto.Code = order.Code;
            orderDto.OrderDate = order.OrderDate;
            orderDto.CustomerId = order.Customer.Id;
            orderDto.CustomerName = order.Customer.Name;
            foreach (var orderDetail in order.OrderDetails)
            {
                orderDto.OrderDetailDtos.Add(orderDetail.ConvertToDto());
            }

            return orderDto;
        }

        // This overloaded extension method convert Supplier object to SupplierDto object
        public static SupplierDto ConvertToDto(this Supplier supplier)
        {
            // Create new SupplierDto object
            SupplierDto supplierDto = new();

            // Set it's properties to the ones contained in supplier
            supplierDto.Id = supplier.Id;
            supplierDto.Code = supplier.Code;
            supplierDto.Name = supplier.Name;
            supplierDto.Contact = supplier.Contact;
            supplierDto.Address = supplier.Address;
            supplierDto.City = supplier.City;
            supplierDto.Postal = supplier.Postal;
            supplierDto.Phone = supplier.Phone;
            supplierDto.Email = supplier.Email;
            supplierDto.Web = supplier.Web;
            supplierDto.ImageUrl = supplier.ImageUrl;

            return supplierDto;
        }

        // This overloaded extension method convert PurchaseDetail object to PurchaseDetailDto object
        public static PurchaseDetailDto ConvertToDto(this PurchaseDetail purchaseDetail)
        {
            // Create new PurchaseDetailDto object
            PurchaseDetailDto purchaseDetailDto = new();

            // Set it's properties to the ones contained in purchaseDetail
            purchaseDetailDto.Id = purchaseDetail.Id;
            purchaseDetailDto.PurchaseId = purchaseDetail.Purchase.Id;
            purchaseDetailDto.MaterialId = purchaseDetail.Material.Id;
            purchaseDetailDto.MaterialPrice = purchaseDetail.Material.Price;
            purchaseDetailDto.Quantity = purchaseDetail.QtyMaterial;

            return purchaseDetailDto;
        }

        // This overloaded extension method convert Purchase object to PurchaseDto object
        public static PurchaseDto ConvertToDto(this Purchase purchase)
        {
            // Create new PurchaseDto object
            PurchaseDto purchaseDto = new();

            // Set it's properties to the ones contained in purchase
            purchaseDto.Id = purchase.Id;
            purchaseDto.Code = purchase.Code;
            purchaseDto.PurchaseDate = purchase.PurchaseDate;
            purchaseDto.SupplierId = purchase.Supplier.Id;
            purchaseDto.SupplierName = purchase.Supplier.Name;
            foreach (var purchaseDetail in purchase.PurchaseDetails)
            {
                purchaseDto.PurchaseDetailDtos.Add(purchaseDetail.ConvertToDto());
            }

            // Return purchaseDto
            return purchaseDto;
        }
    }
}
