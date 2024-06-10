using Microsoft.EntityFrameworkCore;
using pandx.Wheel.Domain.UnitOfWork;

namespace pandx.Wheel.Persistence.EntityFrameworkCore;

public class EfCoreUnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext
{
    private readonly TDbContext _context;

    public EfCoreUnitOfWork(TDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
}