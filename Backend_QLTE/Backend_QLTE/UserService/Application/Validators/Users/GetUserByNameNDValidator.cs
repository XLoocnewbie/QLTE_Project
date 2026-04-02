using Backend_QLTE.UserService.Application.DTOs.Admin.User;
using Backend_QLTE.UserService.Application.Exceptions.Invalid;
using Backend_QLTE.UserService.Application.Exceptions.NotFounds;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Validators;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Validators.Users
{
    public class GetUserByNameNDValidator : IEntityValidator<FindUserByTenNDRequestTO, (List<User> user, int total, int last)>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByNameNDValidator(IUserRepository userRepository)    
        {
            _userRepository = userRepository;
        }
        public async Task ValidateAsync(FindUserByTenNDRequestTO request, CancellationToken cancellationToken = default)
        {
            
        }
        public async Task<(List<User> user,int total, int last)> ValidateAndGetAsync(FindUserByTenNDRequestTO request, CancellationToken cancellationToken = default)
        {
           
            var userList = await _userRepository.GetByNameNDAsync(request.TenND,request.Page,request.Limit, cancellationToken);

            if (userList.total == 0)
            {
                throw new UserListNotFoundException();
            }

            if (userList.last < request.Page)
            {
                throw new InvalidPageIndexOutOfRangeException(request.Page, userList.total);
            }

            return userList;
        }
    }
}
