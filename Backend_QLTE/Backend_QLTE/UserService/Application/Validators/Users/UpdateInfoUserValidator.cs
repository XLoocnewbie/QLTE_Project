using Backend_QLTE.UserService.Application.DTOs.Client.User;
using Backend_QLTE.UserService.Application.Exceptions.Duplicates;
using Backend_QLTE.UserService.Application.Exceptions.NotFounds;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Validators;
using Backend_QLTE.UserService.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Backend_QLTE.UserService.Application.Validators.Users
{
    public class UpdateInfoUserValidator : IContextValidator<InfoUserUpdateRequestDTO,string>
    {
        private readonly IUserRepository _userRepository;

        public UpdateInfoUserValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        // 🔹 Bắt buộc implement từ IValidator<T>
        public async Task ValidateAsync(InfoUserUpdateRequestDTO updateInfo, CancellationToken cancellationToken = default)
        {
            
        }

        public async Task ValidateAsync(InfoUserUpdateRequestDTO updateInfo,string userId, CancellationToken cancellationToken = default)
        {
            // Check user có tồn tại không 
            var user = await _userRepository.FindByUserIdAsync(userId,cancellationToken);
            if (user == null)
                throw new UserNotFoundByIdException(userId);

            // Check userName tồn tại chưa
            var userName = await _userRepository.FindByUsernameAsync(updateInfo.UserName,cancellationToken);
            if (userName != null && userName.Id != userId)
                throw new DuplicateUsernameException(updateInfo.UserName);

            // Check số điện thoại đã tồn tại chưa
            var phoneNumber = await _userRepository.FindByPhoneNumberAsync(updateInfo.PhoneNumber, cancellationToken);
            if (phoneNumber != null && phoneNumber.Id != userId)
                throw new DuplicatePhoneNumberException(updateInfo.PhoneNumber);

            await ValidateAsync(updateInfo, cancellationToken);
        }
    }
}
