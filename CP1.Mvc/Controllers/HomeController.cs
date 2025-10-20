using CP1.Architecture;
using CP1.Architecture.Providers;
using CP1.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CP1.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly IRestProvider _restProvider;
    private readonly string _readMinimalApi;  // Minimal API (Read y ReadyById)
    private readonly string _cudApi;          // API (Create, Update, Delete)

    public HomeController(IRestProvider restProvider, IConfiguration configuration)
    {
        _restProvider = restProvider;
        _readMinimalApi = configuration["Endpoints:MinimalApiBase"] ?? "https://localhost:7121";
        _cudApi = configuration["Endpoints:ApiBase"] ?? "https://localhost:7276";
    }

    public async Task<IActionResult> Index()
    {
        var json = await _restProvider.GetAsync($"{_readMinimalApi}/api/tasks", null);
        var items = JsonProvider.DeserializeSimple<List<TaskDTO>>(json) ?? [];

        var top5 = items.Take(5).ToList();
        ViewBag.HasMore = items.Count > 5;

        return View(top5);
    }

    public async Task<IActionResult> Task()
    {
        var json = await _restProvider.GetAsync($"{_readMinimalApi}/api/tasks", null);
        var items = JsonProvider.DeserializeSimple<List<TaskDTO>>(json) ?? [];
        return View(items);
    }

    [HttpGet]
    public IActionResult CreateTask() => View(new TaskDTO());

    [HttpPost]
    public async Task<IActionResult> CreateTask(TaskDTO dto)
    {
        var content = JsonProvider.Serialize(dto);
        await _restProvider.PostAsync($"{_cudApi}/api/tasks", content);
        return RedirectToAction(nameof(Task));
    }

    [HttpGet]
    public async Task<IActionResult> EditTask(int id)
    {
        var json = await _restProvider.GetAsync($"{_readMinimalApi}/api/tasks/{id}", null);
        var item = JsonProvider.DeserializeSimple<TaskDTO>(json);
        return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> EditTask(TaskDTO dto)
    {
        var content = JsonProvider.Serialize(dto);
        await _restProvider.PutAsync($"{_cudApi}/api/tasks/{dto.TaskId}", "", content);

        return RedirectToAction(nameof(Task));
    }


    [HttpGet]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var json = await _restProvider.GetAsync($"{_readMinimalApi}/api/tasks/{id}", null);
        var item = JsonProvider.DeserializeSimple<TaskDTO>(json);
        return View(item);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDelete(int id)
    {
        try
        {
            await _restProvider.DeleteAsync($"{_cudApi}/api/tasks/{id}", "");
            TempData["AlertSuccess"] = "Tarea eliminada correctamente.";
        }
        catch (ApplicationException ex)
        {
            TempData["AlertWarning"] = "No se pudo eliminar la tarea (verifica que exista).";
        }

        return RedirectToAction(nameof(Task));
    }
}