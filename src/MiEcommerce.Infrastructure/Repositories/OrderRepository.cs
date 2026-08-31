using Microsoft.EntityFrameworkCore;
using MiEcommerce.Domain.Entities;
using MiEcommerce.Domain.Interfaces;
using MiEcommerce.Infrastructure.Persistence;

namespace MiEcommerce.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(x => x.Items)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Where(x => x.CustomerId == customerId)
            .Include(x => x.Items)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public async Task AddOrderItemAsync(OrderItem item, CancellationToken cancellationToken = default)
    {
        // Se agrega explícitamente al DbSet (en vez de confiar en la detección automática
        // de EF sobre la colección Order.Items) porque OrderItem usa una clave Guid
        // generada del lado del cliente: cuando EF descubre esta entidad por primera vez
        // a través de la navegación de una orden ya rastreada, no puede distinguir "es
        // nueva" de "ya existe con este Id" y termina marcándola como Modified en lugar
        // de Added, lo que genera un UPDATE sobre una fila que todavía no existe.
        await _context.OrderItems.AddAsync(item, cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (_context.Entry(order).State == EntityState.Detached)
        {
            _context.Orders.Update(order);
        }

        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await GetByIdAsync(id, cancellationToken);
        if (order is not null)
        {
            _context.Orders.Remove(order);
        }
    }
}
