using MediatR;
using MiEcommerce.Application.DTOs;
using MiEcommerce.Domain.Entities;
using MiEcommerce.Domain.Exceptions;
using MiEcommerce.Domain.Interfaces;

namespace MiEcommerce.Application.Features.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerByIdDto>;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerByIdDto>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerByIdDto> Handle(GetCustomerByIdQuery query, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(query.Id, cancellationToken);
        if (customer is null)
            throw new Domain.Exceptions.NotFoundException(nameof(Customer), query.Id);

        return new CustomerByIdDto(customer.Id, customer.Name, customer.Email.Value, customer.CreatedAt);
    }
}

