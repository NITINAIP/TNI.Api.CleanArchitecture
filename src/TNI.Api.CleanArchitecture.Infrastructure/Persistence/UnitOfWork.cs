using TNI.Api.CleanArchitecture.Domain.Repositories;
using TNI.Api.CleanArchitecture.Infrastructure.Persistence;

namespace TNI.Api.CleanArchitecture.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
