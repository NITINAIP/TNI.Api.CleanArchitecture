using Microsoft.EntityFrameworkCore;
using TNI.Api.CleanArchitecture.Application.Common.Interfaces;
using TNI.Api.CleanArchitecture.Domain.Entities;
using TNI.Api.CleanArchitecture.Infrastructure.Persistence;

namespace TNI.Api.CleanArchitecture.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
}
