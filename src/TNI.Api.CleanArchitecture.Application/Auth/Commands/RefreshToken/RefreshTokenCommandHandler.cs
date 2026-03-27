using MediatR;
using TNI.Api.CleanArchitecture.Application.Auth.DTOs;
using TNI.Api.CleanArchitecture.Application.Common.Interfaces;
using TNI.Api.CleanArchitecture.Application.Exceptions;
using TNI.Api.CleanArchitecture.Domain.Repositories;

namespace TNI.Api.CleanArchitecture.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenPairDto>
{
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(ITokenService tokenService, IUnitOfWork unitOfWork)
    {
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TokenPairDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _tokenService.ValidateRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (result is null)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        var (user, oldRefreshToken) = result.Value;
        oldRefreshToken.Revoke();

        var accessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new TokenPairDto(accessToken, newRefreshToken);
    }
}
