namespace Backend_QLTE.AuthService.Domain.Models
{
    public class Otp
    {
        public string UserId { get; }
        public string Type { get; }
        public string Code { get; }
        public DateTime Expiry { get; }

        private const int ExpiryMinutes = 5;

        private Otp(string userId, string type, string code)
        {
            UserId = userId;
            Type = type;
            Code = code;
            Expiry = DateTime.UtcNow.AddMinutes(ExpiryMinutes);
        }

        public static Otp Create(string userId, string type)
        {
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();
            return new Otp(userId, type, code);
        }

        public static Otp Restore(string userId, string type, string code)
        {
            return new Otp(userId, type, code);
        }

        public bool Verify(string code)
        {
            return Code == code && Expiry > DateTime.UtcNow;
        }
    }

}
