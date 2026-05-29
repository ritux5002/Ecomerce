using MiEcommerce.Domain.Entities;

namespace MiEcommerce.Application.Contracts.Infrastructure;

public interface ITokenService
{
    string GenerateToken(User user);
}
