using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pizza.Backend.Application;
using Pizza.Backend.Infrastructure.Data;
using Pizza.Backend.Infrastructure.Repositories;
using Pizza.Backend.Ports;
using Pizza.Backend.Infrastructure;

// Load .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.

// Add DbContexts
var postgresConnectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

var mysqlConnectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseMySql(mysqlConnectionString, new MySqlServerVersion(new Version(8, 0, 21))));

var mariadbConnectionString = Environment.GetEnvironmentVariable("MARIADB_CONNECTION_STRING");
builder.Services.AddDbContext<ProductsDbContext>(options =>
    options.UseMySql(mariadbConnectionString, new MySqlServerVersion(new Version(10, 11))));

builder.Services.AddMemoryCache();

builder.Services.AddControllers();

// Add custom services for Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISucursalRepository, SucursalRepository>();
builder.Services.AddScoped<ISucursalService, SucursalService>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICarritoRepository, CarritoRepository>();
builder.Services.AddScoped<ICarritoService, CarritoService>();
builder.Services.AddScoped<ITarjetaRepository, TarjetaRepository>();
builder.Services.AddScoped<ITarjetaService, TarjetaService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new InvalidOperationException("JWT Secret key is not configured.");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string CORS_POLICY = "AllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS_POLICY, builder =>
        // For production, replace with your frontend's actual domain
        // e.g., builder.WithOrigins("https://your-pizzeria.com")
        builder.WithOrigins("http://localhost:3000", "https://localhost:3001") 
               .AllowAnyMethod()
               .AllowAnyHeader());
});

var app = builder.Build();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    var mainDbContext = services.GetRequiredService<MainDbContext>();
    mainDbContext.Database.Migrate();

    var inventoryDbContext = services.GetRequiredService<InventoryDbContext>();
    inventoryDbContext.Database.Migrate();

    var productsDbContext = services.GetRequiredService<ProductsDbContext>();
    productsDbContext.Database.Migrate();

    SeedData.Initialize(services);
}

// 2. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CORS_POLICY);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
