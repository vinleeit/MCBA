namespace Mcba.Services.Interfaces;

public interface IAuthService
{
    public Task<int?> Login(string loginId, string password);
}
