using Mcba.Services.Interfaces;
using McbaData;
using Microsoft.EntityFrameworkCore;
using SimpleHashing.Net;

namespace Mcba.Services;

public class AuthService(McbaContext context) : IAuthService
{
    private readonly McbaContext _dbContext = context;

    public async Task<int?> Login(string loginId, string password)
    {
        string hash = await (
            from t in _dbContext.Logins
            where t.LoginID == loginId
            select t.PasswordHash
        ).FirstAsync();
        return hash == null
            ? null
            : new SimpleHash().Verify(password, hash!)
                ? await (
                    from c in _dbContext.Customers
                    where c.Login.LoginID == loginId
                    select c.CustomerID
                ).FirstOrDefaultAsync()
                : null;
    }
}
