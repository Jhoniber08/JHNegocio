// ============================================================
//  JH Negocio API — Servicio REST de inventario
//  Autor: Jhoniber De la Cruz
//  ASP.NET Core Web API (.NET 10)
// ============================================================
using JHNegocio.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "JH Negocio API",
        Version = "v1",
        Description = "Servicio REST de productos e inventario consumido por la app .NET MAUI JH Negocio."
    });
});

// Repositorio en memoria (Singleton para que los datos vivan mientras corre la API)
builder.Services.AddSingleton<IProductoRepository, ProductoRepository>();

// CORS abierto para pruebas desde cualquier cliente
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// 2. Pipeline HTTP
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JH Negocio API v1");
    c.DocumentTitle = "JH Negocio API";
});

app.UseCors();
app.MapControllers();

// La raíz redirige a Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
