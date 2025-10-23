// ProyectoZetino.WebMVC/Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProyectoZetino.WebMVC.Configuration;
using ProyectoZetino.WebMVC.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de ApiSettings (si lo usas)
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Servicios de MVC y sesiones
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Registrar ApiClient como cliente HTTP tipado usando IApiClient
// Usar� ApiSettings:BaseUrl si est� configurado, sino usa un valor por defecto.
var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:44393/";
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

// Agrega otros servicios que necesites aqu�...

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
