using CP1.Core.Services;
using CP1.Data.Models;
using CP1.Data.Repositories;
using CP1.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Cp1Context>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionDB")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CP1 Minimal API", Version = "v1" });
});


builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors(corsOptions => corsOptions.AddPolicy("CP1Client", policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("https://localhost:7246"))); 

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseCors("CP1Client");

var tasks = app.MapGroup("/api/tasks").WithTags("Tasks");

// GET /api/tasks?q=
tasks.MapGet("", async (ITaskService taskService, string? q) =>
    Results.Ok(await taskService.ListAsync(q)));

// GET /api/tasks/{id}
tasks.MapGet("/{id:int}", async (ITaskService taskService, int id) =>
{
    var dto = await taskService.GetAsync(id);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
});

var auth = app.MapGroup("/api/auth").WithTags("Auth");

// POST /api/auth/login
auth.MapPost("/login", async (IAuthService authService, UserLoginDTO req) =>
{
    if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
        return Results.BadRequest("Email and Password are required.");

    var user = await authService.LoginAsync(req.Email, req.Password);
    return user is null ? Results.Unauthorized() : Results.Ok(user);
});

var admin = app.MapGroup("/api/admin").WithTags("Admin");

// GET /api/admin/users-with-role
admin.MapGet("/users-with-role", async (Cp1Context db) =>
{
    var query = from u in db.Users.AsNoTracking()
                join ur in db.UserRoles.AsNoTracking() on u.UserId equals ur.UserId
                join r in db.Roles.AsNoTracking() on ur.RoleId equals r.RoleId
                select new UserRoleDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    FullName = u.FullName,
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                };

    return Results.Ok(await query.OrderBy(x => x.Username).ToListAsync());
});

// GET /api/admin/roles
admin.MapGet("/roles", async (Cp1Context db) =>
{
    var roles = await db.Roles.AsNoTracking()
        .Select(r => new RoleDTO { RoleId = r.RoleId, RoleName = r.RoleName, Description = r.Description })
        .OrderBy(r => r.RoleName)
        .ToListAsync();
    return Results.Ok(roles);
});

app.Run();