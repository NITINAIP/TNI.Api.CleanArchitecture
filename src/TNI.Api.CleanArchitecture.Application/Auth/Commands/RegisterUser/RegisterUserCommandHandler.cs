using TNI.Api.CleanArchitecture.Application.Auth.DTOs;
using TNI.Api.CleanArchitecture.Application.Common.Interfaces;
using TNI.Api.CleanArchitecture.Domain.Entities;
using TNI.Api.CleanArchitecture.Domain.Repositories;
using MediatR;

namespace TNI.Api.CleanArchitecture.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisteredUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisteredUserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
            throw new Application.Exceptions.ConflictException("User", request.Email);

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email.ToLowerInvariant(), passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new RegisteredUserDto(user.Id, user.Email);
    }
}
