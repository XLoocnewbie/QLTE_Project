using Backend_QLTE.AuthService.Domain.Exceptions.Invalid;

namespace Backend_QLTE.AuthService.Domain.ValueObjects
{
    public class RefreshTokenString
    {
        public string Value { get; }

        public RefreshTokenString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidRefreshTokenException();

            Value = value;
        }

        public override string ToString() => Value;
    }
}
