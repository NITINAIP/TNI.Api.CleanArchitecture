using TNI.Api.CleanArchitecture.Domain.Entities;

namespace TNI.Api.CleanArchitecture.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken = default);
    Task<(User user, Domain.Entities.RefreshToken refreshToken)?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
}
