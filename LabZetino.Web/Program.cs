using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using SisLabZetino.Application.Services;
using SisLabZetino.Domain.Repositories;
using SisLabZetino.Infrastructure.Data;
using SisLabZetino.Infrastructure.Data.Repositories;
using SisLabZetino.Infrastructure.Repositories;

// Librerías para JWT
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===============================================
// 🔹 REGISTRO DE SERVICIOS Y REPOSITORIOS
// ===============================================
builder.Services.AddScoped<IResultadoRepository, ResultadoRepository>();
builder.Services.AddScoped<ResultadoService>();

builder.Services.AddScoped<ITipoExamenRepository, TipoExamenRepository>();
builder.Services.AddScoped<TipoExamenService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<ICitaRepository, CitaRepository>();
builder.Services.AddScoped<CitaService>();

builder.Services.AddScoped<IExamenRepository, ExamenRepository>();
builder.Services.AddScoped<ExamenService>();

builder.Services.AddScoped<IMuestraRepository, MuestraRepository>();
builder.Services.AddScoped<MuestraService>();

builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<RolService>();

builder.Services.AddScoped<IOrdenExamenRepository, OrdenExamenRepository>();
builder.Services.AddScoped<OrdenExamenService>();

builder.Services.AddScoped<INotificacionEmailRepository, NotificacionEmailRepository>();
builder.Services.AddScoped<NotificacionEmailService>();

builder.Services.AddScoped<ITipoMuestraRepository, TipoMuestraRepository>();
builder.Services.AddScoped<TipoMuestraService>();

// ===============================================
// 🔹 BASE DE DATOS (MySQL)
// ===============================================
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 36)),
    mySqlOptions => mySqlOptions.EnableRetryOnFailure()));

// ===============================================
// 🔹 CONFIGURAR SWAGGER CON JWT
// ===============================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingrese: Bearer {token}",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// ===============================================
// 🔹 CONFIGURAR JWT
// ===============================================
var key = builder.Configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key no configurada");
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false; // true en prod con HTTPS
        opt.SaveToken = true;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

            // 👇 --- ¡AQUÍ ESTÁ LA CORRECCIÓN! --- 👇
            // Le decimos que SÍ valide el emisor y la audiencia
            ValidateIssuer = true,
            ValidateAudience = true,
            // Le decimos QUÉ valores debe esperar (los que leyó del appsettings.json)
            ValidIssuer = issuer,
            ValidAudience = audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ===============================================
// 🔹 MIDDLEWARE
// ===============================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();