using CP1.Models.DTOs;

namespace CP1.Core.Services;

public interface ITaskService
{
    Task<List<TaskDTO>> ListAsync(string? q = null, CancellationToken cancellationToken = default);
    Task<TaskDTO?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<TaskDTO> CreateAsync(TaskDTO dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, TaskDTO dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}