using CP1.Architecture;
using CP1.Architecture.Providers;
using CP1.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CP1.Mvc.Controllers;

public class AuthController : Controller
{
    private readonly IRestProvider _rest;
    private readonly string _minimalApiBase;

    public AuthController(IRestProvider rest, IConfiguration config)
    {
        _rest = rest;
        _minimalApiBase = config["Endpoints:MinimalApiBase"] ?? "https://localhost:7121";
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("UserId") is not null)
            return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string Email, string Password)
    {
        var payload = JsonProvider.Serialize(new UserLoginDTO { Email = Email, Password = Password });

        try
        {
            var json = await _rest.PostAsync($"{_minimalApiBase}/api/auth/login", payload);
            var user = JsonProvider.DeserializeSimple<UserDTO>(json);

            if (user is null)
            {
                ViewBag.Error = "Credenciales inválidas.";
                return View();
            }
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetInt32("RoleId", user.RoleId);
            HttpContext.Session.SetString("RoleName", user.RoleName);

            return RedirectToAction("Index", "Home");
        }
        catch
        {
            ViewBag.Error = "Contraseña o Correo incorrecto";
            return View();
        }
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
}