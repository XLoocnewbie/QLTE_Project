using Backend_QLTE.UserService.Application.DTOs.Client.User;
using Backend_QLTE.UserService.Application.Exceptions.Duplicates;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Validators;

namespace Backend_QLTE.UserService.Application.Validators.Users
{
    public class RegisterUserValidator : IValidator<UserRegisterDTO>

    {
        private readonly IUserRepository _userRepository;

        public RegisterUserValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        // Kiểm tra trùng email, sđt khi đăng ký
        public async Task ValidateAsync(UserRegisterDTO dto, CancellationToken cancellationToken = default)
        {
            var existingByEmail = await _userRepository.FindByEmailAsync(dto.Email,cancellationToken);
            if (existingByEmail != null)
                throw new DuplicateEmailException(dto.Email);

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                var existingByPhone = await _userRepository.FindByPhoneNumberAsync(dto.PhoneNumber, cancellationToken);
                if (existingByPhone != null)
                    throw new DuplicatePhoneNumberException(dto.PhoneNumber);
            }
        }
    }
}
