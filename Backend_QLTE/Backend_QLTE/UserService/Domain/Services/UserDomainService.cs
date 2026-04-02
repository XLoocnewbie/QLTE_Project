using Backend_QLTE.UserService.Domain.Entities;
using Backend_QLTE.UserService.Domain.Exceptions.Duplicate;
using Backend_QLTE.UserService.Domain.Exceptions.Invalid;
using Backend_QLTE.UserService.Domain.Services.Interfaces;

namespace Backend_QLTE.UserService.Domain.Services
{
    // Chỉ chứa nghiệp vụ thuần, không repo, không log, không transaction
    public class UserDomainService : IUserDomainService
    { 

        // Đăng ký tài khoản Local
        public void EnsureCanRegister(User user)
        {
            // Kiểm tra user có null không
            InValidUser(user);

            // Kiểm tra email
            ValidateEmail(user.Email);
        }

        // Đăng ký tài khoản từ bên ngoài
        public void EnsureCanProviderRegister(User user)
        {
            // Kiểm tra user có null không
            InValidUser(user);

            // Kiểm tra email
            ValidateEmail(user.Email);

            if (string.IsNullOrWhiteSpace(user.AuthId))
                throw new InvalidUserAuthIdException(user.AuthId);

            if (string.IsNullOrWhiteSpace(user.TypeLogin))
                throw new InvalidUserTypeLoginException(user.TypeLogin);
        }

        // Đăng nhập tài khoản từ bên ngoài
        public void EnsureCanProviderLogin(User user)
        {
            // Kiểm tra user có null không
            InValidUser(user);

            // Kiểm tra email
            ValidateEmail(user.Email);

            if (string.IsNullOrWhiteSpace(user.AuthId))
                throw new InvalidUserAuthIdException(user.AuthId);

            if (string.IsNullOrWhiteSpace(user.TypeLogin))
                throw new InvalidUserTypeLoginException(user.TypeLogin);
        }

        // Update Info người dùng
        public void EnsureCanUpdateInfoUser(User user)
        {
            // Kiểm tra user có null không
            InValidUser(user);

            if (string.IsNullOrWhiteSpace(user.UserName))
                throw new InvalidUserNameException(user.UserName);

            if (user.UserName.Length < 3 || user.UserName.Length > 50)
                throw new InvalidUserNameException(user.UserName);

            if (string.IsNullOrWhiteSpace(user.PhoneNumber) || user.PhoneNumber.Length < 10 || user.PhoneNumber.Length > 11)
                throw new InvalidUserPhoneNumberException(user.PhoneNumber);
        }

        // Login Local
        public void EnsureCanLoginLocal(User user)
        {
            // Kiểm tra user có null không
            InValidUser(user);

            // Kiểm tra email
            ValidateEmail(user.Email);
        }


        // Tìm User Dựa vào email
        public void EnsureCanUserEmail(User user)
        {
            // Kiểm tra user có null không
            InValidUser(user);
        }

        // Tìm User Dựa vào userId
        public void EnsureCanUserId(User user)
        {
            InValidUser(user);

            if (string.IsNullOrWhiteSpace(user.Id))
                throw new InvalidUserIdException(user.Id);
        }

        // Tìm User Dựa vào userId
        public void EnsureCanResetPassword(User user)
        {
            InValidUser(user);

            if (string.IsNullOrWhiteSpace(user.Id))
                throw new InvalidUserIdException(user.Id);

        }

        // Đổi email
        public void EnsureCanChangeEmail(User user , string newEmail)
        {
            // Kiểm tra user có null không
            InValidUser(user);

            // Kiểm tra email
            ValidateEmail(newEmail);

            if (!user.CanchangeEmail())
                throw new InvalidChangeEmailException(user.Email);

            if (user.IsDuplicateEmail(newEmail))
                throw new DuplicateEmailException(newEmail);

            user.Email = newEmail;
            user.ThoiGianDoiEmail = DateTime.UtcNow;
            user.ThoiGianCapNhat = DateTime.UtcNow;
        }

        // Lấy danh sách user phân trang
        public void EnsureCanGetListUser(int page, int limit)
        {
            ValidatePagination(page, limit);
        }

        public void EnsureCanNameND(string tenND, int page, int limit)
        {
            if(string.IsNullOrWhiteSpace(tenND))
                throw new InvalidTenNDException(tenND);

            ValidatePagination(page,limit);
        }

        // private methods //
        // Validate email
        private void ValidateEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidUserEmailException(email);

            if (!email.Contains("@"))
                throw new InvalidUserEmailException(email);
        }

        // Validate phân trang
        private void ValidatePagination(int page, int limit)
        {
            if (page <= 0)
                throw new InvalidUserPageException(page);

            if (limit <= 0 || limit > 100)
                throw new InvalidUserPageLimitException(limit);
        }

        // Kiểm tra user có null không
        private void InValidUser(User user)
        {
            if (user == null)
                throw new InvalidUserException(user?.Email);
        }


    }
}
