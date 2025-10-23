using CP1.Data.Models;
using CP1.Data.Repositories;
using CP1.Models.DTOs;

namespace CP1.Core.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
    { _taskRepository = taskRepository; _unitOfWork = unitOfWork; }

    private static TaskDTO CrearDTO(TaskItem task) => new()
    {
        Id = task.Id,
        Name = task.Name,
        Description = task.Description,
        Status = task.Status,
        DueDate = task.DueDate,
        CreatedAt = task.CreatedAt,
        Approved = task.Approved
    };

    private static void ApplyDto(TaskItem target, TaskDTO source, bool isUpdate)
    {
        target.Name = source.Name;
        target.Description = source.Description;
        target.Status = source.Status;
        target.DueDate = source.DueDate;
        target.Approved = source.Approved;
        if (!isUpdate && source.CreatedAt.HasValue)
            target.CreatedAt = source.CreatedAt;
    }

    public async Task<List<TaskDTO>> ListAsync(string? query = null, CancellationToken cancellationToken = default)
    {
        var list = await _taskRepository.ReadAsync(
            string.IsNullOrWhiteSpace(query) ? null : task => task.Name.Contains(query!), cancellationToken);
        return list.Select(CrearDTO).ToList();
    }

    public async Task<TaskDTO?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _taskRepository.FindAsync(id, cancellationToken);
        return entity is null ? null : CrearDTO(entity);
    }

    public async Task<TaskDTO> CreateAsync(TaskDTO dto, CancellationToken cancellationToken = default)
    {
        var entity = new TaskItem();
        ApplyDto(entity, dto, isUpdate: false);
        await _taskRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CrearDTO(entity);
    }

    public async Task<bool> UpdateAsync(int id, TaskDTO dto, CancellationToken cancellationToken = default)
    {
        var entity = await _taskRepository.FindAsync(id, cancellationToken);
        if (entity is null) return false;
        ApplyDto(entity, dto, isUpdate: true);
        _taskRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _taskRepository.FindAsync(id, cancellationToken);
        if (entity is null) return false;
        _taskRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}