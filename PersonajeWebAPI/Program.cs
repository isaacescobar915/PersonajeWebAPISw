using Microsoft.EntityFrameworkCore;
using PersonajeWebAPI.Controllers;
using PersonajeWebAPI.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();

// Configurar Entity Framework
builder.Services.AddDbContext<PersonajeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar repositorios
builder.Services.AddScoped<PersonajeADORepository>();
builder.Services.AddScoped<PersonajeEFRepository>();

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Personaje API - Comparación ADO.NET vs Entity Framework",
        Version = "v1",
        Description = "API para comparar implementaciones de acceso a datos usando ADO.NET y Entity Framework"
    });

    // Incluir comentarios XML para documentación
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Personaje API v1");
        c.RoutePrefix = "swagger"; // Swagger estará en /swagger
    });
}

// Crear base de datos automáticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PersonajeContext>();
    await context.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("🚀 API iniciada. Swagger disponible en: /swagger");
Console.WriteLine("📊 Compara ADO.NET vs Entity Framework:");
Console.WriteLine("   - ADO.NET: /api/PersonajesADO");
Console.WriteLine("   - Entity Framework: /api/PersonajesEF");

app.Run();