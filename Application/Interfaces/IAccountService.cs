using Application.Core;
using Domain;
using MediatR;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        Task<AppUser> FindByEmailAsync(string email);
        Task<AppUser> FindByIdAsync(string userId);
        Task<bool> CheckPasswordAsync(AppUser user, string password);
        Task<bool> UserNameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<Result<AppUser>> CreateAsync(AppUser user, string password);
        Task<Result<AppUser>> CreateWithRoleAsync(AppUser user, string password, string role);
        Task<Result<Unit>> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
    }
}
