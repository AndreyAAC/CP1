using CP1.Core.Services;
using CP1.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CP1.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private static readonly HashSet<string> AllowedStatuses =
        new(StringComparer.OrdinalIgnoreCase) { "Pending", "InProgress", "Completed" };

    private readonly ITaskService _taskService;
    public TasksController(ITaskService taskService) => _taskService = taskService;

    [HttpPost]
    public async Task<ActionResult<TaskDTO>> Create([FromBody] TaskDTO dto)
    {
        if (dto is null) return BadRequest("Campo requerido");
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Nombre requerido");
        if (dto.DueDate == default) return BadRequest("Fecha es requerida");
        if (string.IsNullOrWhiteSpace(dto.Status)) dto.Status = "Pending";
        if (!AllowedStatuses.Contains(dto.Status)) return BadRequest("Estado invalido");

        var created = await _taskService.CreateAsync(dto);
        return CreatedAtRoute("GetTaskById", new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskDTO dto)
    {
        if (dto is null) return BadRequest("Campo requerido");
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Nombre requerido");
        if (dto.DueDate == default) return BadRequest("Fecha es requerida");
        if (string.IsNullOrWhiteSpace(dto.Status)) dto.Status = "Pending";
        if (!AllowedStatuses.Contains(dto.Status)) return BadRequest("Estado invalido.");

        var ok = await _taskService.UpdateAsync(id, dto);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _taskService.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("{id:int}", Name = "GetTaskById")]
    public async Task<ActionResult<TaskDTO>> GetById(int id)
    {
        var dto = await _taskService.GetAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }
}