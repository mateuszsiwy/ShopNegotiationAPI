using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ShopNegotiationAPI.Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using ShopNegotiationAPI.Application.Interfaces.Repositories;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Application.Services;
using ShopNegotiationAPI.Infrastructure.BackgroundServices;
using ShopNegotiationAPI.Infrastructure.Data;
using ShopNegotiationAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using ShopNegotiationAPI.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop Negotiation API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseInMemoryDatabase("ShopNegotiationDb"));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<INegotiationRepository, NegotiationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<INegotiationService, NegotiationService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHostedService<ExpiredNegotiationsService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ProductValidator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmployeesOnly", policy => policy.RequireRole("Employee"));
});
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    var employeeUser = new User
    {
        Username = "employee",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
        Role = "Employee"
    };
    if (await userManager.GetByUsername(employeeUser.Username) == null)
    {
        await authService.RegisterUser(employeeUser);
    }
    var initializer = new InitializeDatabase(
        scope.ServiceProvider.GetRequiredService<AppDbContext>(),
        scope.ServiceProvider.GetRequiredService<ILogger<InitializeDatabase>>());
    await initializer.InitializeAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();