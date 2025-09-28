using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Repositories;
using SisLabZetino.Infrastructure.Data;
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
builder.Services.AddScoped<IUsuarioSistemaRepository, UsuarioSistemaRepository>();
builder.Services.AddScoped<UsuarioSistemaService>();



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
