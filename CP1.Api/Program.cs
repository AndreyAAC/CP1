using CP1.Core.Services;
using CP1.Data.Models;
using CP1.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Cp1Context>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionDB")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CP1 API", Version = "v1" });
});

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddCors(corsOptions => corsOptions.AddPolicy("CP1Client", policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("https://localhost:7246")));

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseHttpsRedirection();
app.UseCors("CP1Client");
app.MapControllers();

app.Run();
