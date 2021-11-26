using StatusApp.Domain;
using StatusApp.Domain.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StatusApp.Services
{
    public interface IUserService
    {
        UserResponse CurrentUser { get; set; }
        event EventHandler<EventArgs> OnAutomaticLogout;
        Task<LoginResponseType> LoginUserAsync(string username, string password);
        void LogoutUser();
        Task<TokenCreationResponse> CreateUserCreationTokenAsync();
        Task<UserResponse> CreateUserAsync(string username, string password, string token);
        Task<List<UserResponse>> GetUsersAsync();
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UpdatePasswordAsync(string oldPassword, string newPassword);
    }
}
