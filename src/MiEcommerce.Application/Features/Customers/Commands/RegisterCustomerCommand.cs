using FluentValidation;
using MediatR;
using MiEcommerce.Application.DTOs;
using MiEcommerce.Domain.Entities;
using MiEcommerce.Domain.Interfaces;

namespace MiEcommerce.Application.Features.Customers.Commands.RegisterCustomer;

public record RegisterCustomerCommand(string Name, string Email) : IRequest<RegisterCustomerResponse>;

public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
{
    public RegisterCustomerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, RegisterCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterCustomerResponse> Handle(RegisterCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = Customer.Create(command.Name, command.Email);
        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterCustomerResponse(customer.Id);
    }
}

