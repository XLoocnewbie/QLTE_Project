using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Domain.Entities;
using Backend_QLTE.UserService.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Backend_QLTE.UserService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // Lấy tất cả User
        public async Task<(List<User> user, int total, int last)> GetAllUsersAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            var query =  _userManager.Users.AsQueryable();

            var total = await query.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);

            var users = await query
                .OrderBy(u => u.Id) // thứ tự ổn định
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (users, total, last);
        }

        // Đăng Ký User
        public async Task<IdentityResult> RegisterUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user,password);
            return result;
        }

        // Đăng Ký User không mật khẩu
        public async Task<IdentityResult> RegisterUserNotPasswordAsync(User user)
        {
            var result = await _userManager.CreateAsync(user);
            return result;
        }


        // Update User
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            var result =  await _userManager.UpdateAsync(user);
            return result;
        }

        // Lấy danh sách vai trò của người dùng
        public async Task<IList<string>> GetRolesUserAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }


        // Gán vai trò cho người dùng
        public async Task<IdentityResult> AssignRoleToUserAsync(User user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            return result;
        }

        // Tìm User Dựa vào UserId 
        public async Task<User?> FindByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var result = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId,cancellationToken);
            return result;
        }

        // Tìm User Dựa vào Email 
        public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = _userManager.NormalizeEmail(email);
            var result = await _userManager.Users 
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
            return result;
        }

        // Lấy User Dựa vào UserName 
        public async Task<User?> FindByUsernameAsync(string userName, CancellationToken cancellationToken = default)
        {
            var normalizedUserName = _userManager.NormalizeName(userName);
            var result = await _userManager.Users
                .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName,cancellationToken);
            return result;
        }

        // Lấy User Dựa vào SDT 
        public async Task<User?> FindByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            var result = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber,cancellationToken);
            return result;
        }

        // Check Xem authId đã tồn tại chưa
        public async Task<User?> FindByAuthIdAndTypeLoginAsync(string authId , string typeLogin, CancellationToken cancellationToken = default)
        {
            var result = await _userManager.Users
                .FirstOrDefaultAsync(u => u.AuthId == authId && u.TypeLogin == typeLogin,cancellationToken);
            return result;
        }

        // Tìm User Dựa vào TenND
        public async Task<(List<User> user, int total, int last)> GetByNameNDAsync(string nameND, int page, int limit, CancellationToken cancellationToken = default)
        {
            var query = _userManager.Users
                .Where(u => u.NameND.Contains(nameND))
                .AsQueryable();

            var total = await query.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);
            var users = await query
                .OrderBy(u => u.Id) // thứ tự ổn định
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (users,total,last);
        }

        // Check password của User
        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var result = await _userManager.CheckPasswordAsync(user, password);
            return result;
        }

        // Tạo token đặt lại mật khẩu
        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        // Đặt lại mật khẩu với token
        public async Task<IdentityResult> ResetPasswordWithTokenAsync(User user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        // Tạo mã token để thay đổi email
        public async Task<string> GenerateChangeEmailTokenAsync(User user , string newEmail)
        {
            return await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        }

        // Thay đổi email với token
        public async Task<IdentityResult> ChangeEmailWithTokenAsync(User user, string newEmail, string token)
        {
            return await _userManager.ChangeEmailAsync(user, newEmail, token);
        }
        // Xóa User 
        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            var result = await _userManager.DeleteAsync(user);
            return result;
        }
    }
}
