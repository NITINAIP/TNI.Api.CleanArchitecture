using TNI.Api.CleanArchitecture.Domain.Common;
using TNI.Api.CleanArchitecture.Domain.Exceptions;

namespace TNI.Api.CleanArchitecture.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");

        email = email.Trim().ToLowerInvariant();

        if (!System.Text.RegularExpressions.Regex.IsMatch(email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            System.Text.RegularExpressions.RegexOptions.None,
            TimeSpan.FromMilliseconds(100)))
        {
            throw new DomainException($"'{email}' is not a valid email address.");
        }

        return new Email(email);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
