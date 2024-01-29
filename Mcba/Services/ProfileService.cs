using Mcba.Services.Interfaces;
using McbaData;
using McbaData.Models;
using Microsoft.EntityFrameworkCore;
using SimpleHashing.Net;

namespace Mcba.Services;

public class ProfileService(McbaContext dbContext) : IProfileService
{
    private readonly McbaContext _dbContext = dbContext;

    /// <summary>
    /// Update customer profile if data is changed.
    /// </summary>
    /// <param name="newCustomer">new data to be updated</param>
    /// <returns>any error associated to the method</returns>
    public async Task<IProfileService.ProfileError?> UpdateCustomerProfile(Customer newCustomer)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(
            b => b.CustomerID == newCustomer.CustomerID
        );
        ;
        if (customer != null)
        {
            customer.Name = newCustomer.Name;
            customer.TFN = newCustomer.TFN;
            customer.Address = newCustomer.Address;
            customer.City = newCustomer.City;
            customer.State = newCustomer.State;
            customer.Postcode = newCustomer.Postcode;
            customer.Mobile = newCustomer.Mobile;
            if (_dbContext.Entry(customer).State != EntityState.Unchanged)
            {
                await _dbContext.SaveChangesAsync();
                return null;
            }
            return IProfileService.ProfileError.NoDataChange;
        }
        return IProfileService.ProfileError.CustomerNotFound;
    }

    /// <summary>
    /// Update customer password.
    /// </summary>
    /// <param name="customerID">the ID of target customer</param>
    /// <param name="newPassword">the new raw password string</param>
    /// <returns></returns>
    public async Task<IProfileService.ProfileError?> UpdateCustomerPassword(
        int customerID,
        string newPassword
    )
    {
        var login = await _dbContext.Logins.FirstOrDefaultAsync(b => b.CustomerID == customerID);
        if (login != null)
        {
            // Default compute according to specification
            login.PasswordHash = new SimpleHash().Compute(newPassword);
            await _dbContext.SaveChangesAsync();
            return null;
        }
        return IProfileService.ProfileError.CustomerNotFound;
    }
}
