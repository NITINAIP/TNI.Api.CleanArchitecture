using TNI.Api.CleanArchitecture.Domain.Common;

namespace TNI.Api.CleanArchitecture.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public bool IsUsed { get; private set; }

    public bool IsValid => !IsRevoked && !IsUsed && ExpiresAt > DateTimeOffset.UtcNow;

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, int daysValid = 7)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(daysValid)
        };
    }

    public void Revoke()
    {
        IsRevoked = true;
        IsUsed = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
