using McbaData.Dtos;
using McbaData.Models;

namespace AdminApi.Data;

public interface IAdminRepo
{
    public List<CustomerDto> GetCustomers();
    public CustomerDto? GetCustomer(int id);
    public bool EditCustomer(CustomerEditDto customer, int id);
    public bool EditCustomerLock(int id, bool isLock);
}
