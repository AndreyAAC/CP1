using System.Linq.Expressions;
using CP1.Data.Models;

namespace CP1.Data.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> FindAsync(int id, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> ReadAsync(Expression<Func<TaskItem, bool>>? filter = null, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem entity, CancellationToken cancellationToken = default);
    void Update(TaskItem entity);
    void Remove(TaskItem entity);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}