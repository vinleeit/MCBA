namespace Mcba.Services.Interfaces;

public interface IAuthService
{
    public enum AuthError
    {
        Locked,
        InvalidCredential,
    }
    public Task<(AuthError? Error, int? Customer)> Login(string loginId, string password);
}
