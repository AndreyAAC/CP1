using CP1.Core.Services;
using CP1.Data.Models;
using CP1.Data.Repositories;
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

// DI
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITaskService, TaskService>();

// CORS (para MVC)
builder.Services.AddCors(corsOptions => corsOptions.AddPolicy("CP1Client", policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("https://localhost:7246"))); // pon el puerto real de CP1.Mvc

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseCors("CP1Client");

// Rutas
var tasks = app.MapGroup("/api/tasks").WithTags("Tasks");

// GET /api/tasks?q=
tasks.MapGet("", async (ITaskService taskService) =>
    Results.Ok(await taskService.ListAsync(null)));

// GET /api/tasks/{id}
tasks.MapGet("/{id:int}", async (ITaskService taskService, int id) =>
{
    var dto = await taskService.GetAsync(id);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
});

app.Run();