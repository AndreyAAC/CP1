using CP1.Data.Models;
using CP1.Data.Repositories;
using CP1.Models.DTOs;

namespace CP1.Core.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _tastRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
    { _tastRepository = taskRepository; _unitOfWork = unitOfWork; }

    private static TaskDTO ToDto(TaskItem task) => new()
    {
        TaskId = task.TaskId,
        Name = task.Name,
        Description = task.Description,
        CreatedDate = task.CreatedDate,
        CreatedBy = task.CreatedBy,
        Status = task.Status
    };

    private static void ApplyDto(TaskItem target, TaskDTO source, bool isUpdate)
    {
        target.Name = source.Name;
        target.Description = source.Description;
        target.CreatedBy = source.CreatedBy;
        target.Status = source.Status;
        if (!isUpdate) target.CreatedDate = DateTime.UtcNow;
    }

    public async Task<List<TaskDTO>> ListAsync(string? query = null, CancellationToken cancellationToken = default)
    {
        var list = await _tastRepository.ReadAsync(
            string.IsNullOrWhiteSpace(query) ? null : t => t.Name.Contains(query!), cancellationToken);
        return list.Select(ToDto).ToList();
    }

    public async Task<TaskDTO?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _tastRepository.FindAsync(id, cancellationToken);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<TaskDTO> CreateAsync(TaskDTO dto, CancellationToken cancellationToken = default)
    {
        var entity = new TaskItem();
        ApplyDto(entity, dto, isUpdate: false);
        await _tastRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, TaskDTO dto, CancellationToken cancellationToken = default)
    {
        var entity = await _tastRepository.FindAsync(id, cancellationToken);
        if (entity is null) return false;
        ApplyDto(entity, dto, isUpdate: true);
        _tastRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _tastRepository.FindAsync(id, cancellationToken);
        if (entity is null) return false;
        _tastRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}