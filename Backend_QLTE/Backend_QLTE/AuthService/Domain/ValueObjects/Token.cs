using Backend_QLTE.AuthService.Domain.Exceptions.Invalid;

namespace Backend_QLTE.AuthService.Domain.ValueObjects
{
    public record Token
    {
        public string Value { get; init; }
        public DateTime Expiry { get; init; }

        public Token(string value, DateTime expiry)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidTokenException();

            Value = value;
            Expiry = expiry;
        }

        /// Factory method để tạo token mới.
        public static Token Create(string value, DateTime expiry)
        {
            return new Token(value, expiry);
        }

        /// Kiểm tra token còn hạn không.
        public bool IsExpired() => DateTime.UtcNow >= Expiry;

        /// Override ToString để tiện log/debug.
        public override string ToString() => $"[Token: {Value}, Expiry: {Expiry}]";
    }
}
