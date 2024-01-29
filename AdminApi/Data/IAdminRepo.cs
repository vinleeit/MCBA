using McbaData.Dtos;

namespace AdminApi.Data
{
    // Repository Interface (to be injected as a service)
    public interface IAdminRepo
    {
        public List<CustomerDto> GetCustomers();
        public CustomerDto? GetCustomer(int id);
        public bool EditCustomer(CustomerEditDto customer, int id);
        public bool EditCustomerLock(int id, bool isLock);
    }
}
