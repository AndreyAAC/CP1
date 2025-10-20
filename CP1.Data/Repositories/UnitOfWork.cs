using CP1.Data.Models;

namespace CP1.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly Cp1Context _db;
    public UnitOfWork(Cp1Context db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);
}