using Mcba.Models;

namespace Mcba.Services.Interfaces;

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