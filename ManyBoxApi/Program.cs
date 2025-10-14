using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ManyBoxApi.Data;
using ManyBoxApi.Services;
using System.Text;
using DotNetEnv;
using Microsoft.Extensions.FileProviders;
using System.IO;
using ManyBoxApi.Hubs;
using ManyBoxApi.Repositories;
using QuestPDF.Infrastructure;
using System.Globalization;

// Carga las variables del archivo .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Agrega esta línea para que la API escuche en todas las interfaces de red.
// Usa el puerto 5000. Puedes cambiarlo si lo necesitas.
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Lee datos sensibles desde variables de entorno (.env)
var mysqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST");
var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
var mysqlUser = Environment.GetEnvironmentVariable("MYSQL_USER");
var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

var connectionString = $"Server={mysqlHost};Database={mysqlDatabase};UserID={mysqlUser};Password={mysqlPassword};SslMode=none;";

// JWT
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? throw new InvalidOperationException("JWT_KEY not found in environment");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "manybox-api";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "manybox-app";

// Fedex (si necesitas en los servicios, puedes acceder así)
var fedexClientId = Environment.GetEnvironmentVariable("FEDEX_CLIENT_ID");
var fedexClientSecret = Environment.GetEnvironmentVariable("FEDEX_CLIENT_SECRET");

// Servicios básicos
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new ManyBoxApi.Helpers.JsonConverters.DecimalJsonConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});
builder.Services.AddHttpClient<FedexAddressValidationService>();
builder.Services.AddHttpClient<FedexShipService>();
builder.Services.AddScoped<FedexShipService>();
builder.Services.AddSignalR();

builder.Services.AddHttpClient<DhlService>();
builder.Services.AddScoped<DhlService>();

builder.Services.AddScoped<EnvioApiService>();

// Inyección de dependencias para la nueva arquitectura
builder.Services.AddScoped<IEnvioRepository, EnvioRepository>();
builder.Services.AddScoped<IEnvioService, EnvioService>();

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

builder.Services.AddScoped<IDireccionRepository, DireccionRepository>();
builder.Services.AddScoped<IDireccionService, DireccionService>();

// Configuración de DbContext con la cadena de conexión de .env
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Registra el AuthService
builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpClient<AuthService>();

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Configuración de CORS
const string DevelopmentCorsPolicy = "DevelopmentPolicy";
const string ProductionCorsPolicy = "ProductionPolicy";

builder.Services.AddCors(options =>
{
    // Política para desarrollo: permite cualquier origen, método y cabecera.
    options.AddPolicy(DevelopmentCorsPolicy, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    // Política para producción: más restrictiva.
    // DEBES cambiar "https://your-frontend-app.com" por la URL real de tu frontend.
    options.AddPolicy(ProductionCorsPolicy, policy =>
    {
        policy.WithOrigins("http://100.64.197.11:5000", "https://app.manybox.com") // Reemplaza con la URL de tu app
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// Configuración de la cultura para usar punto como separador decimal
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("en-US") };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Verificar conexión a la base de datos al iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await dbContext.Database.CanConnectAsync();
        logger.LogInformation("Conexión con la base de datos verificada exitosamente.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al intentar conectar con la base de datos.");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseCors(DevelopmentCorsPolicy); // Usa la política de desarrollo
    logger.LogInformation("Entorno de desarrollo detectado. Usando CORS de desarrollo y Swagger.");
}
else
{
    app.UseHttpsRedirection();
    app.UseCors(ProductionCorsPolicy); // Usa la política de producción
}

app.UseRequestLocalization();

// Agrega para servir archivos estáticos de la carpeta ArchivosFedex
var fedexFolder = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosFedex");
if (!Directory.Exists(fedexFolder))
{
    Directory.CreateDirectory(fedexFolder); // Crea la carpeta si no existe
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(fedexFolder),
    RequestPath = "/ArchivosFedex"
});

// NUEVO: servir archivos subidos (chat)
var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
Directory.CreateDirectory(uploadsRoot);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsRoot),
    RequestPath = "/Uploads"
});

// Configurar licencia QuestPDF global
QuestPDF.Settings.License = LicenseType.Community;

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ManyBoxApi.Hubs.ChatHub>("/chathub");
app.MapHub<ManyBoxApi.Hubs.NotificacionesHub>("/notificacioneshub");
app.MapHub<ManyBoxApi.Hubs.DashboardHub>("/dashboardhub");

logger.LogInformation("Iniciando la aplicación ManyBox API...");
app.Run();
