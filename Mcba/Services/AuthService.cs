using Mcba.Services.Interfaces;
using McbaData;
using Microsoft.EntityFrameworkCore;
using SimpleHashing.Net;
using static Mcba.Services.Interfaces.IAuthService;

namespace Mcba.Services;

public class AuthService(McbaContext context) : IAuthService
{

    private readonly McbaContext _dbContext = context;

    public async Task<(AuthError? Error, int? Customer)> Login(string loginId, string password)
    {
        var result = await (
            from t in _dbContext.Logins
            where t.LoginID == loginId
            select new
            {
                Hash = t.PasswordHash,
                Locked = t.Locked,
            }
        ).FirstAsync();
        return (result.Locked)
            ? (AuthError.Locked, null)
            : (result.Hash == null)
                ? (AuthError.InvalidCredential, null)
                : new SimpleHash().Verify(password, result.Hash!)
                    ? (null, await (from c in _dbContext.Customers where c.Login.LoginID == loginId select c.CustomerID).FirstOrDefaultAsync())
                    : (AuthError.InvalidCredential, null);
    }
}
