using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DataContext _context;

        public AccountService(UserManager<AppUser> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public Task<AppUser> FindByEmailAsync(string email) =>
            _userManager.FindByEmailAsync(email);

        public Task<AppUser> FindByIdAsync(string userId) =>
            _userManager.FindByIdAsync(userId);

        public Task<bool> CheckPasswordAsync(AppUser user, string password) =>
            _userManager.CheckPasswordAsync(user, password);

        public Task<bool> UserNameExistsAsync(string username) =>
            _userManager.Users.AnyAsync(u => u.UserName == username);

        public Task<bool> EmailExistsAsync(string email) =>
            _userManager.Users.AnyAsync(u => u.Email == email);

        public async Task<Result<AppUser>> CreateAsync(AppUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return Result<AppUser>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
            return Result<AppUser>.Success(user);
        }

        public async Task<Result<AppUser>> CreateWithRoleAsync(AppUser user, string password, string role)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return Result<AppUser>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
            user.GlobalRole = role;
            await _context.SaveChangesAsync();
            return Result<AppUser>.Success(user);
        }

        public async Task<Result<Unit>> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return Result<Unit>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
