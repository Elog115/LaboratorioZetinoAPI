using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProyectoZetino.WebMVC.Services; // Asegúrate que el namespace sea correcto
using System;

var builder = WebApplication.CreateBuilder(args);

// Servicios de MVC y sesiones
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// --- ESTA ES LA NUEVA LÓGICA ---
// Lee la URL base desde appsettings.json
var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];

// Verifica si la URL se encontró en el archivo
if (string.IsNullOrEmpty(baseUrl))
{
    // Lanza un error si no encuentra el appsettings.json
    throw new InvalidOperationException("No se encontró 'ApiSettings:BaseUrl' en el archivo appsettings.json del proyecto MVC.");
}

// Registrar ApiClient como cliente HTTP tipado
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

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

// Ruta por defecto (apuntando a Login)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();