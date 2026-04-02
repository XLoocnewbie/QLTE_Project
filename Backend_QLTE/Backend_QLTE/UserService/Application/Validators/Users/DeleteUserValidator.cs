using Backend_QLTE.UserService.Application.DTOs.Admin.User;
using Backend_QLTE.UserService.Application.Exceptions.NotFounds;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Validators;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Validators.Users
{
    public class DeleteUserValidator : IEntityValidator<DeleteUserRequestDTO, User>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ValidateAsync(DeleteUserRequestDTO request, CancellationToken cancellationToken = default)
        {
           
        }

        public async Task<User> ValidateAndGetAsync(DeleteUserRequestDTO request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByUserIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new UserNotFoundByIdException(request.UserId);

            return user;
        }
    }
}
