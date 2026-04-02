using Azure.Core;
using Backend_QLTE.UserService.Application.DTOs.Client.Login;
using Backend_QLTE.UserService.Application.Exceptions.Duplicates;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Validators;

namespace Backend_QLTE.UserService.Application.Validators.Users
{
    public class ProviderRegisterUserValidator : IValidator<ExternalLoginUserInfoDTO>
    {
        private readonly IUserRepository _userRepository;

        public ProviderRegisterUserValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ValidateAsync(ExternalLoginUserInfoDTO externalLoginUserInfo, CancellationToken cancellationToken = default)
        {
            // Kiểm tra trùng email
            var emailExist = await _userRepository.FindByEmailAsync(externalLoginUserInfo.Email,cancellationToken);
            if (emailExist != null)
                throw new DuplicateEmailException(externalLoginUserInfo.Email);
        }
    }
}
