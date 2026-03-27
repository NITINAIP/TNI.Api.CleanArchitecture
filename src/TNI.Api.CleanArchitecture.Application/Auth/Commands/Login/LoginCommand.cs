using MediatR;
using TNI.Api.CleanArchitecture.Application.Auth.DTOs;

namespace TNI.Api.CleanArchitecture.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<TokenPairDto>;
