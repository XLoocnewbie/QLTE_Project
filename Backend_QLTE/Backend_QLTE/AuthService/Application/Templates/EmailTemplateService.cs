using Backend_QLTE.AuthService.Application.Interfaces.Templates;

namespace Backend_QLTE.AuthService.Application.Templates
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public string GetOtpSubject(string type) => $"Your OTP Code {type}";

        public string GetOtpBody(string otpCode) =>
            $"Mã OTP của bạn là: {otpCode}. Có hiệu lực trong 5 phút.";


    }
}
