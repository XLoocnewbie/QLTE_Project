using Backend_QLTE.UserService.Application.DTOs.Admin.User;
using Backend_QLTE.UserService.Application.Exceptions.Invalid;
using Backend_QLTE.UserService.Application.Exceptions.NotFounds;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Validators;

namespace Backend_QLTE.UserService.Application.Validators.Users
{
    public class GetListUserValidator : IValidator<ListUserRequestDTO>
    {
        private readonly IUserRepository _userRepository;

        public GetListUserValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ValidateAsync(ListUserRequestDTO dto, CancellationToken cancellationToken = default)
        {
            var userList = await _userRepository.GetAllUsersAsync(dto.page,dto.limit,cancellationToken);

            if(userList.total == 0)
            {
                throw new UserListNotFoundException();
            }

            if (userList.last < dto.page)
            {
                throw new InvalidPageIndexOutOfRangeException(dto.page, userList.total);
            }
        }
    }
}
