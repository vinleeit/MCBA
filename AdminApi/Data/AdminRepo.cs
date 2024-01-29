using McbaData;
using McbaData.Dtos;
using McbaData.Models;

namespace AdminApi.Data
{
    // Concrete implementation of the repository  IAdminRepo
    public class AdminRepo(McbaContext context) : IAdminRepo
    {
        private readonly McbaContext _dbContext = context;

        public bool EditCustomer(CustomerEditDto customerEdit, int id)
        {
            // Find customer
            Customer? customer = _dbContext.Customers.Find(id);
            if (customer == null)
            {
                return false;
            }

            // Update data
            customer.TFN = customerEdit.TFN;
            customer.City = customerEdit.City;
            customer.State = customerEdit.State;
            customer.Postcode = customerEdit.Postcode;
            customer.Mobile = customerEdit.Mobile;
            customer.Address = customerEdit.Address;
            customer.Name = customerEdit.Name;

            _ = _dbContext.SaveChanges();
            return true;
        }

        public bool EditCustomerLock(int id, bool isLock)
        {
            // Find the login credentials
            Login login = (from l in _dbContext.Logins where l.CustomerID == id select l).First();
            if (login == null)
            {
                return false;
            }

            // Change the lock
            login.Locked = isLock;
            _ = _dbContext.SaveChanges();

            return true;
        }

        // Get customer from database and convert it to customer dto
        public CustomerDto? GetCustomer(int id)
        {
            Customer? customer = _dbContext.Customers.Find(id);
            // Create a Customer Dto object instance if the customer is found
            return customer == null
                ? null
                : new()
                {
                    Name = customer.Name,
                    CustomerId = customer.CustomerID,
                    Postcode = customer.Postcode,
                    Address = customer.Address,
                    Mobile = customer.Mobile,
                    State = customer.State,
                    City = customer.City,
                    TFN = customer.TFN,
                    IsLocked = _dbContext
                        .Logins.Where(l => l.CustomerID == id)
                        .Select(l => l.Locked)
                        .First()
                };
        }

        public List<CustomerDto> GetCustomers()
        {
            // FInd all customer, and get the lock status
            var queryResult =
                from c in _dbContext.Customers
                join l in _dbContext.Logins on c.CustomerID equals l.CustomerID
                select new { customer = c, locked = l.Locked };
            List<CustomerDto> data = [];
            foreach (var c in queryResult)
            {
                data.Add(
                    new()
                    {
                        Name = c.customer.Name,
                        TFN = c.customer.TFN,
                        City = c.customer.City,
                        State = c.customer.State,
                        Mobile = c.customer.Mobile,
                        Address = c.customer.Address,
                        Postcode = c.customer.Postcode,
                        CustomerId = c.customer.CustomerID,
                        IsLocked = c.locked
                    }
                );
            }
            return data;
        }
    }
}
