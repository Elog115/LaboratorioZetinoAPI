using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Repositories;
using SisLabZetino.Infrastructure.Data;
using SisLabZetino.Infrastructure.Data.Repositories;
using SisLabZetino.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Base de datos MySQL
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    )
);

// Inyección de dependencias
builder.Services.AddScoped<IResultadoRepository, ResultadoRepository>();
builder.Services.AddScoped<ResultadoService>();
builder.Services.AddScoped<ITipoExamenRepository, TipoExamenRepository>();
builder.Services.AddScoped<TipoExamenService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ICitaRepository, CitaRepository>();
builder.Services.AddScoped<CitaService>();
builder.Services.AddScoped<IExamenRepository, ExamenRepository>();
builder.Services.AddScoped<ExamenService>();
builder.Services.AddScoped<IMuestraRepository, MuestraRepository>();
builder.Services.AddScoped<MuestraService>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<RolService>();


// Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
