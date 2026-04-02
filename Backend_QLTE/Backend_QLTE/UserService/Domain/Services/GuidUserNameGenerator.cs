using Backend_QLTE.UserService.Domain.Services.Interfaces;

namespace Backend_QLTE.UserService.Domain.Services
{
    public class GuidUserNameGenerator : IGuidUserNameGenerator
    {
        public string GenerateUserName() => Guid.NewGuid().ToString("N");
    }
}
