using StatusApp.Domain;
using StatusApp.Domain.Model.DTOs;
using System;
using System.Threading.Tasks;

namespace StatusApp.Services
{
    public interface IUserService
    {
        event EventHandler<EventArgs> OnAutomaticLogout;
        Task<LoginResponseType> LoginUserAsync(string username, string password);
        void LogoutUser();
        Task<TokenCreationResponse> CreateUserCreationTokenAsync();
    }
}
