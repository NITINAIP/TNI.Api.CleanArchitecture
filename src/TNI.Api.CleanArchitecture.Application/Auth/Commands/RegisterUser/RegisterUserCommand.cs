using MediatR;
using TNI.Api.CleanArchitecture.Application.Auth.DTOs;

namespace TNI.Api.CleanArchitecture.Application.Auth.Commands.RegisterUser;

public record RegisterUserCommand(string Email, string Password, string ConfirmPassword) : IRequest<RegisteredUserDto>;
