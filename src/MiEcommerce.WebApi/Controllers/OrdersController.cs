using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiEcommerce.Application.Features.Orders.Commands.AddItemToOrder;
using MiEcommerce.Application.Features.Orders.Commands.CancelOrder;
using MiEcommerce.Application.Features.Orders.Commands.ConfirmOrder;
using MiEcommerce.Application.Features.Orders.Commands.CreateOrder;
using MiEcommerce.Application.Features.Orders.Commands.DeliverOrder;
using MiEcommerce.Application.Features.Orders.Commands.ShipOrder;
using MiEcommerce.Application.Features.Orders.Queries.GetAll;
using MiEcommerce.Application.Features.Orders.Queries.GetById;

namespace MiEcommerce.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllOrdersQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{orderId}/items")]
    public async Task<IActionResult> AddItem(
        Guid orderId,
        [FromBody] AddItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddItemToOrderCommand(orderId, request.ProductId, request.Quantity);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{orderId}/confirm")]
    public async Task<IActionResult> Confirm(Guid orderId, CancellationToken cancellationToken)
    {
        var command = new ConfirmOrderCommand(orderId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{orderId}/cancel")]
    public async Task<IActionResult> Cancel(Guid orderId, CancellationToken cancellationToken)
    {
        var command = new CancelOrderCommand(orderId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{orderId}/ship")]
    public async Task<IActionResult> Ship(Guid orderId, CancellationToken cancellationToken)
    {
        var command = new ShipOrderCommand(orderId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{orderId}/deliver")]
    public async Task<IActionResult> Deliver(Guid orderId, CancellationToken cancellationToken)
    {
        var command = new DeliverOrderCommand(orderId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}

public record AddItemRequest(Guid ProductId, int Quantity);
