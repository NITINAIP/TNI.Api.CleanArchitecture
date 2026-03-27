using MediatR;
using TNI.Api.CleanArchitecture.Application.Auth.DTOs;

namespace TNI.Api.CleanArchitecture.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<TokenPairDto>;
