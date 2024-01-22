using BizApi.Data.Database;
using BizApi.Data.Entities;
using BizApi.Extensions;
using BizApi.Utility;
using Microsoft.EntityFrameworkCore;
using SharedProject.Dtos;

namespace BizApi.Repositories.Customers
{
    // Implementation class for ICustomerRepository
    public class CustomerRepository:ICustomerRepository
    {
        private readonly AppDbContext context;

        public CustomerRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Create new Customer
        public async Task CreateNewCustomerAsync(CustomerDto customerDto, string imagesFolder)
        {
            // Create new Customer object
            Customer customer = new();

            // Set it's properties to the ones contained in customerDto
            customer.Code = customerDto.Code;
            customer.Name = customerDto.Name;
            customer.Contact = customerDto.Contact;
            customer.Address = customerDto.Address;
            customer.City = customerDto.City;
            customer.Postal = customerDto.Postal;
            customer.Phone = customerDto.Phone;
            customer.Email = customerDto.Email;
            customer.Web = customerDto.Web;

            customer.ImageUrl = customerDto.StoreImage(imagesFolder);

            // Add customer to database
            await context.Customers.AddAsync(customer);
        }

        // Delete selected Customer
        public async Task DeleteCustomerAsync(int id, string imagesFolder)
        {
            // Find Customer by id
            Customer customer = (await context.Customers.FindAsync(id))!;

            // Remove customer from database
            context.Customers.Remove(customer);

            // If selected Customer has an image, then delete it
            if (!string.IsNullOrEmpty(imagesFolder) && !string.IsNullOrEmpty(customer.ImageUrl))
            {
                if (System.IO.File.Exists(Path.Combine(imagesFolder, customer.ImageUrl!)))
                {
                    System.IO.File.Delete(Path.Combine(imagesFolder, customer.ImageUrl!));
                }
            }
        }

        // Edit selected Customer
        public async Task EditCustomerAsync(CustomerDto customerDto, string imagesFolder)
        {
            // Find Customer by id
            Customer customer = (await context.Customers.FindAsync(customerDto.Id))!;

            // Set it's properties to the ones contained in customerDto
            customer.Code = customerDto.Code;
            customer.Name = customerDto.Name;
            customer.Contact = customerDto.Contact;
            customer.Address = customerDto.Address;
            customer.City = customerDto.City;
            customer.Postal = customerDto.Postal;
            customer.Phone = customerDto.Phone;
            customer.Email = customerDto.Email;
            customer.Web = customerDto.Web;

            // If customerDto's Image is not null, it means that customer's image
            // is changed, so we need to delete old image first
            if (customerDto.Image != null)
            {
                if (System.IO.File.Exists(Path.Combine(imagesFolder, customer.ImageUrl)))
                {
                    System.IO.File.Delete(Path.Combine(imagesFolder, customer.ImageUrl));
                }

                // Store new image
                customer.ImageUrl = customerDto.StoreImage(imagesFolder);
            }
        }

        // Return paginated collection of CustomerDto objects
        public async Task<Pagination<CustomerDto>> GetCustomersCollectionAsync(string searchText, int pageIndex, int pageSize)
        {
            // Get all Customer objects
            var allCustomers = context.Customers
                                .AsNoTracking()
                                .AsQueryable();

            // If searchText is not null or empty string,
            // filter allCustomers by searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                allCustomers = allCustomers.Where(e => e.Name.ToLower().Contains(searchText.ToLower())
                                                || e.Contact.ToLower().Contains(searchText.ToLower())
                                                || e.Email.ToLower().Contains(searchText.ToLower()));
            }

            // Variable that will contain CustomerDto objects
            List<CustomerDto> customerDtos = new();

            // Iterate through allCustomers and populate
            // customerDtos using Customer's extension method
            // ConvertToDto
            foreach (var customer in allCustomers)
            {
                customerDtos.Add(customer.ConvertToDto());
            }

            // Using PaginationUtility's GetPaginatedResult() method, 
            // return Pagionation<CustomerDto> object
            var paginatedResult = PaginationUtility<CustomerDto>.GetPaginatedResult(in customerDtos, pageIndex, pageSize);

            return await Task.FromResult(paginatedResult);
        }

        // Return single CustomerDto object
        public async Task<CustomerDto> GetSingleCustomerAsync(int id)
        {
            // Find Customer by id
            Customer customer = (await context.Customers.FindAsync(id))!;

            // Return CustomerDto using Customer's extension method
            // ConvertToDto
            return customer.ConvertToDto();
        }

        // Custom validation
        public async Task<Dictionary<string, string>> ValidateCustomerAsync(CustomerDto customerDto)
        {
            // Define variable that will contain possible errors
            Dictionary<string, string> errors = new();

            // Variable that contains all Customer records
            var allCustomers = context.Customers
                                .AsNoTracking()
                                .AsQueryable();

            // If customerDto's Id value is greater than 0, it means that Customer
            // is used in Update operation
            if (customerDto.Id > 0)
            {
                // Find Customer by id
                Customer customer = (await context.Customers.FindAsync(customerDto.Id))!;

                // If customer's Code is modified, check for Code uniqueness
                // among all Customer records
                if (customer.Code != customerDto.Code)
                {
                    // If provided customerDto's Code is already contained in
                    // any of the Customer records then add error to errors Dictionary
                    if (allCustomers.Select(e => e.Code.ToLower()).Contains(customerDto.Code.ToLower()))
                    {
                        errors.Add("Code", "There is already Customer with this Code in database. Please provide different one!");
                    }
                }

                // If customer's Name is modified, check for Name uniqueness
                // among all Customer records
                if (customer.Name != customerDto.Name)
                {
                    // If provided customerDto's Name is already contained in
                    // any of the Customer records then add error to errors Dictionary
                    if (allCustomers.Select(e => e.Name.ToLower()).Contains(customerDto.Name.ToLower()))
                    {
                        errors.Add("Name", "There is already Customer with this Name in database. Please provide different one!");
                    }
                }

                // If customer's Email is modified, check for uniqueness
                // among all Customer records
                if (customer.Email != customerDto.Email)
                {
                    // If provided customerDto's Email is already contained in
                    // any of the Customer records then add error to errors Dictionary
                    if (allCustomers.Select(e => e.Email.ToLower()).Contains(customerDto.Email.ToLower()))
                    {
                        errors.Add("Email", "This E-mail is already used by another company. Please provide different one!");
                    }
                }
            }
            // Otherwise it means that Customer is used in Create operation
            else
            {
                // If provided customerDto's Code is already contained in
                // any of the Customer records then add error to errors Dictionary
                if (allCustomers.Select(e => e.Code.ToLower()).Contains(customerDto.Code.ToLower()))
                {
                    errors.Add("Code", "There is already Customer with this Code in database. Please provide different one!");
                }

                // If provided customerDto's Name is already contained in
                // any of the Customer records then add error to errors Dictionary
                if (allCustomers.Select(e => e.Name.ToLower()).Contains(customerDto.Name.ToLower()))
                {
                    errors.Add("Name", "There is already Customer with this Name in database. Please provide different one!");
                }

                // If provided customerDto's Email is already contained in
                // any of the Customer records then add error to errors Dictionary
                if (allCustomers.Select(e => e.Email.ToLower()).Contains(customerDto.Email.ToLower()))
                {
                    errors.Add("Email", "This E-mail is already used by another company. Please provide different one!");
                }
            }

            // Return errors Dictionary
            return errors;
        }

        // Return all Customer records
        public async Task<List<CustomerDto>> GetAllCustomersAsync()
        {
            var allCustomers = context.Customers
                                .AsNoTracking()
                                .AsQueryable();

            List<CustomerDto> customerDtos = new();

            foreach (var customer in allCustomers)
            {
                customerDtos.Add(customer.ConvertToDto());
            }

            return await Task.FromResult(customerDtos);
        }
    }
}
