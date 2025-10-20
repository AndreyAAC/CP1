using CP1.Architecture;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRestProvider, RestProvider>();

builder.Configuration["Endpoints:MinimalApiBase"] = "https://localhost:7121";
builder.Configuration["Endpoints:ApiBase"] = "https://localhost:7276";

var app = builder.Build();

if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();