using McbaData;
using McbaData.Dtos;
using McbaData.Models;

namespace AdminApi.Data;

public class AdminRepo(McbaContext context) : IAdminRepo
{
    private readonly McbaContext _dbContext = context;

    public bool EditCustomer(CustomerEditDto customerEdit, int id)
    {
        // Find customer
        var customer = _dbContext.Customers.Find(id);
        if (customer == null)
        {
            return false;
        }

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
        Login login = (from l in _dbContext.Logins where l.CustomerID == id select l).First();
        if (login == null)
        {
            return false;
        }

        login.Locked = isLock;
        _ = _dbContext.SaveChanges();

        return true;
    }

    public CustomerDto? GetCustomer(int id)
    {
        Customer? customer = _dbContext.Customers.Find(id);
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
            };
    }

    public List<CustomerDto> GetCustomers()
    {
        List<CustomerDto> data = [];
        foreach (Customer c in _dbContext.Customers)
        {
            data.Add(
                new()
                {
                    Name = c.Name,
                    TFN = c.TFN,
                    City = c.City,
                    State = c.State,
                    Mobile = c.Mobile,
                    Address = c.Address,
                    Postcode = c.Postcode,
                    CustomerId = c.CustomerID,
                }
            );
        }
        return data;
    }
}
