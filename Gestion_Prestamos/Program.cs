using Gestion_Prestamos.Data;
using Microsoft.EntityFrameworkCore; // Asegúrate de tener esta importación
using MySql.EntityFrameworkCore.Extensions; // Asegúrate de tener esta importación si no está presente

var builder = WebApplication.CreateBuilder(args);

// Registrar IHttpClientFactory
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddControllers();

// Configurar la conexión a la base de datos MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 39)))); // Ajusta la versión del servidor MySQL según sea necesario

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configurar Swagger para documentar la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usa la política CORS
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
