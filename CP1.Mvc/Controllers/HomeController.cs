using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CP1.Architecture;
using CP1.Architecture.Providers;
using CP1.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CP1.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRestProvider _restProvider;

        private readonly string _readMinimalApi;

        private readonly string _cudApi;

        public HomeController(IRestProvider restProvider, IConfiguration configuration)
        {
            _restProvider = restProvider;
            _readMinimalApi = configuration["Endpoints:MinimalApiBase"] ?? "https://localhost:7121";
            _cudApi = configuration["Endpoints:ApiBase"] ?? "https://localhost:7276";
        }

        private int? CurrentRoleId => HttpContext.Session.GetInt32("RoleId");
        private bool IsAdmin => CurrentRoleId == 1;
        private bool IsSpecialist => CurrentRoleId == 2;


        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");

            var json = await _restProvider.GetAsync($"{_readMinimalApi}/api/tasks", null);
            var items = JsonProvider.DeserializeSimple<List<TaskDTO>>(json) ?? [];

            var visible = items
                .Where(t => t.Approved.HasValue)          
                .OrderByDescending(t => t.Approved == true) 
                .ThenBy(t => t.DueDate)
                .ToList();

            var top5 = visible.Take(5).ToList();
            ViewBag.HasMore = visible.Count > 5;

            return View(top5);
        }

        public async Task<IActionResult> Task()
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");

            var json = await _restProvider.GetAsync($"{_readMinimalApi}/api/tasks", null);
            var items = JsonProvider.DeserializeSimple<List<TaskDTO>>(json) ?? [];

            var ordered = items
                .OrderByDescending(t => t.Approved == true)   
                .ThenBy(t => t.Approved.HasValue)             
                .ThenBy(t => t.DueDate)
                .ToList();

            return View(ordered);
        }

        [HttpGet]
        public IActionResult CreateTask()
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");

            ViewBag.IsSpecialist = IsSpecialist;
            return View(new TaskDTO
            {
                Status = "Pending",
                DueDate = DateTime.Now.AddDays(1)
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskDTO dto)
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");

            if (IsSpecialist)
                dto.Approved = null;

            var content = JsonProvider.Serialize(dto);
            await _restProvider.PostAsync($"{_cudApi}/api/tasks", content);
            return RedirectToAction(nameof(Task));
        }

        [HttpGet]
        public async Task<IActionResult> EditTask(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");

            var json = await _restProvider.GetAsync($"{_readMinimalApi}/api/tasks/{id}", null);
            var item = JsonProvider.DeserializeSimple<TaskDTO>(json);

            ViewBag.IsSpecialist = IsSpecialist;
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> EditTask(TaskDTO dto)
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");

            if (IsSpecialist)
                dto.Approved = null;

            var content = JsonProvider.Serialize(dto);

            await _restProvider.PutAsync($"{_cudApi}/api/tasks/{dto.Id}", "", content);
            return RedirectToAction(nameof(Task));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");

            var json = await _restProvider.GetAsync($"{_readMinimalApi}/api/tasks/{id}", null);
            var item = JsonProvider.DeserializeSimple<TaskDTO>(json);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");

            try
            {
                await _restProvider.DeleteAsync($"{_cudApi}/api/tasks/{id}", "");
                TempData["AlertSuccess"] = "Tarea eliminada correctamente.";
            }
            catch (ApplicationException)
            {
                TempData["AlertWarning"] = "No se pudo eliminar la tarea (verifica que exista).";
            }

            return RedirectToAction(nameof(Task));
        }


        [HttpGet]
        public async Task<IActionResult> ApprovedTask()
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");
            if (!IsAdmin) return Forbid();

            var json = await _restProvider.GetAsync($"{_readMinimalApi}/api/tasks", null);
            var items = JsonProvider.DeserializeSimple<List<TaskDTO>>(json) ?? [];

            var ordered = items
                .OrderBy(t => t.Approved.HasValue ? (t.Approved.Value ? 1 : 2) : 0)
                .ThenBy(t => t.DueDate)
                .ToList();

            return View(ordered);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeTaskStatus(int id, string state)
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");
            if (!IsAdmin) return Forbid();

            var json = await _restProvider.GetAsync($"{_readMinimalApi}/api/tasks/{id}", null);
            var task = JsonProvider.DeserializeSimple<TaskDTO>(json);
            if (task is null) return NotFound();

            if (string.Equals(state, "deny", StringComparison.OrdinalIgnoreCase))
            {
                if (task.CreatedAt.HasValue && (DateTime.UtcNow - task.CreatedAt.Value).TotalHours > 24)
                {
                    TempData["AlertWarning"] = "No se puede cambiar a Denegado: la tarea tiene más de 24 horas de creada.";
                    return RedirectToAction(nameof(ApprovedTask));
                }
            }

            if (string.Equals(state, "approve", StringComparison.OrdinalIgnoreCase))
                task.Approved = true;
            else if (string.Equals(state, "deny", StringComparison.OrdinalIgnoreCase))
                task.Approved = false;
            else
                return BadRequest("Estado inválido.");

            var content = JsonProvider.Serialize(task);
            await _restProvider.PutAsync($"{_cudApi}/api/tasks/{task.Id}", "", content);

            TempData["AlertSuccess"] = "Estado actualizado correctamente.";
            return RedirectToAction(nameof(ApprovedTask));
        }


        [HttpGet]
        public async Task<IActionResult> AssignRole()
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");
            if (!IsAdmin) return Forbid();

            var usersJson = await _restProvider.GetAsync($"{_readMinimalApi}/api/admin/users-with-role", null);
            var rolesJson = await _restProvider.GetAsync($"{_readMinimalApi}/api/admin/roles", null);

            var users = JsonProvider.DeserializeSimple<List<UserRoleDTO>>(usersJson) ?? [];
            var roles = JsonProvider.DeserializeSimple<List<RoleDTO>>(rolesJson) ?? [];

            ViewBag.Roles = roles;
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUserRole(int userId, int roleId)
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");
            if (!IsAdmin) return Forbid();

            await _restProvider.PutAsync($"{_cudApi}/api/users/{userId}/role/{roleId}", "", "{}");
            TempData["AlertSuccess"] = "Rol asignado correctamente.";
            return RedirectToAction(nameof(AssignRole));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(RoleDTO dto)
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");
            if (!IsAdmin) return Forbid();

            var payload = JsonProvider.Serialize(dto);
            await _restProvider.PostAsync($"{_cudApi}/api/roles", payload);

            TempData["AlertSuccess"] = "Rol creado correctamente.";
            return RedirectToAction(nameof(AssignRole));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(RoleDTO dto)
        {
            if (HttpContext.Session.GetInt32("UserId") is null)
                return RedirectToAction("Login", "Auth");
            if (!IsAdmin) return Forbid();

            var payload = JsonProvider.Serialize(dto);
            await _restProvider.PutAsync($"{_cudApi}/api/roles/{dto.RoleId}", "", payload);

            TempData["AlertSuccess"] = "Rol actualizado correctamente.";
            return RedirectToAction(nameof(AssignRole));
        }
    }
}