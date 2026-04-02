using Backend_QLTE.UserService.Application.DTOs.Client.Login;
using Backend_QLTE.UserService.Application.Exceptions.NotFounds;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Validators;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Validators.Users
{
    public class LocalLoginUserValidator : IEntityValidator<LoginRequestDTO,User>
    {
        private readonly IUserRepository _userRepository;

        public LocalLoginUserValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task ValidateAsync(LoginRequestDTO updateInfo, CancellationToken cancellationToken = default)
        {

        }
        public async Task<User> ValidateAndGetAsync(LoginRequestDTO loginRequest, CancellationToken cancellationToken = default)
        {
            // Check tài khoản tồn tại chưa (email hoặc username)
            var checkAccount = await _userRepository.FindByEmailAsync(loginRequest.Account, cancellationToken)
                    ?? await _userRepository.FindByUsernameAsync(loginRequest.Account, cancellationToken);

            if (checkAccount == null)
            {
                throw new UserNotFoundException(loginRequest.Account);
            }

            return checkAccount;
        }
    }
}
