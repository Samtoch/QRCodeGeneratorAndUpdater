using QRCodeApp.Models;

namespace QRCodeApp.Services
{
    public interface IUserService
    {
        Task<User> QueryUserByLogin(Login user);
        Task<int> CreateUserAction(string username, string actionId);
        Task<int> UpdateUserAction(string actionId);
    }
}
