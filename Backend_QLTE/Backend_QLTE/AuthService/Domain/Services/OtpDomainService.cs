using Backend_QLTE.AuthService.Domain.Exceptions.Failed;
using Backend_QLTE.AuthService.Domain.Exceptions.Invalid;
using Backend_QLTE.AuthService.Domain.Interfaces.Services;
using Backend_QLTE.AuthService.Domain.Models;

namespace Backend_QLTE.AuthService.Domain.Services
{
    public class OtpDomainService : IOtpDomainService
    {
        public bool VerifyOtp(Otp otp,string code)
        {
            if (otp == null) 
                throw new InvalidOtpException(code);
            return otp.Verify(code);

        }

        public Otp CreateOtp(string userId, string type)
        {
            var otp = Otp.Create(userId, type);
            if(otp == null)
                throw new CreateOtpFailedException();

            return otp;
        }
    }
}
