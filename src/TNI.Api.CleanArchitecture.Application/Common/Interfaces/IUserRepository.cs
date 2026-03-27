using TNI.Api.CleanArchitecture.Domain.Common;
using TNI.Api.CleanArchitecture.Domain.Entities;
using TNI.Api.CleanArchitecture.Domain.Repositories;

namespace TNI.Api.CleanArchitecture.Application.Common.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
