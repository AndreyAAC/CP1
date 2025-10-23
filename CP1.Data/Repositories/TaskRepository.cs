using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CP1.Data.Models;

namespace CP1.Data.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly Cp1Context _db;
    public TaskRepository(Cp1Context db) => _db = db;

    public Task<TaskItem?> FindAsync(int id, CancellationToken cancellationToken = default)
        => _db.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<List<TaskItem>> ReadAsync(Expression<Func<TaskItem, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Tasks.AsNoTracking().AsQueryable();
        if (filter != null) query = query.Where(filter);
        return await query.ToListAsync(cancellationToken);
    }

    public Task AddAsync(TaskItem entity, CancellationToken cancellationToken = default)
        => _db.Tasks.AddAsync(entity, cancellationToken).AsTask();

    public void Update(TaskItem entity) => _db.Tasks.Update(entity);
    public void Remove(TaskItem entity) => _db.Tasks.Remove(entity);

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => _db.Tasks.AnyAsync(x => x.Id == id, ct);
}