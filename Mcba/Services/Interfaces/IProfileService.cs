using Mcba.Models;

namespace Mcba.Services;

// TODO: Create GetCustomerProfile and get the profile from the login (need rebase)
public interface IProfileService
{
    public enum ProfileError
    {
        CustomerNotFound,
        NoDataChange,
    }

    public Task<ProfileError?> UpdateCustomerProfile(Customer newCustomer);
    public Task<ProfileError?> UpdateCustomerPassword(int customerID, string newPassword);
}