using MediatR;
using TNI.Api.CleanArchitecture.Application.Auth.DTOs;
using TNI.Api.CleanArchitecture.Application.Common.Interfaces;
using TNI.Api.CleanArchitecture.Application.Exceptions;
using TNI.Api.CleanArchitecture.Domain.Repositories;

namespace TNI.Api.CleanArchitecture.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenPairDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<TokenPairDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials.");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new TokenPairDto(accessToken, refreshToken);
    }
}
