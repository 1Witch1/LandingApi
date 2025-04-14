using LandingApi.Models;

namespace LandingApi.Services
{
    public interface IAuthService
    {
        Task<string> Authenticate(string name, string password);
        Task<bool> Register(User user, string password);
    }
}
