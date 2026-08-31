using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiEcommerce.Application.Features.Customers.Commands.RegisterCustomer;
using MiEcommerce.Application.Features.Customers.Commands.UpdateCustomer;
using MiEcommerce.Application.Features.Customers.Queries.GetCustomerById;
using MiEcommerce.Application.Features.Orders.Queries.GetByCustomer;

namespace MiEcommerce.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCustomerByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCustomerCommand(id, request.Name, request.Email);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpGet("{id}/orders")]
    public async Task<IActionResult> GetOrders(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrdersByCustomerQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}

public record UpdateCustomerRequest(string Name, string Email);
