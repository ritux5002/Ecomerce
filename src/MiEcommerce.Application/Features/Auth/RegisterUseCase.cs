using MediatR;
using MiEcommerce.Application.Contracts.Infrastructure;
using MiEcommerce.Domain.Entities;
using MiEcommerce.Domain.Exceptions;
using MiEcommerce.Domain.Interfaces;

namespace MiEcommerce.Application.Features.Auth;

public record RegisterCommand(string Name, string Email, string Password) : IRequest<Unit>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (existing is not null)
            throw new DomainRuleException("Ya existe un usuario con ese correo.");

        var passwordHash = _passwordHasher.Hash(command.Password);
        var user = User.Create(command.Name, command.Email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
