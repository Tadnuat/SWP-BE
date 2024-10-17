using KoiShipping.Repo.Entities;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using KoiShipping.API;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Allow CORS for all origins, headers, and methods
builder.Services.AddCors(options =>
    options.AddPolicy("MyPolicy", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Configure database context using connection string from appsettings.json
builder.Services.AddDbContext<KoiShippingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbContext")));

// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register TokenService
builder.Services.AddSingleton<TokenService>();

builder.Services.AddTransient<IEmailService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var email = configuration["EmailSettings:Email"];
    var password = configuration["EmailSettings:Password"];
    return new EmailService(email, password);
});

// Add JSON options to increase depth of serialization (if necessary)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.MaxDepth = 100;
    });

// Add Swagger for API documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KoiShipping API", Version = "v1.0" });

    // Configure Swagger for JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT token in the following format: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
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
            new string[] { }
        }
    });
});

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
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
})
// Add Google Authentication
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";
    googleOptions.Scope.Add("email"); // Thêm scope cho email
});

// Add Authorization policies based on roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DeliveringStaffPolicy", policy =>
        policy.RequireClaim("Role", "Delivering Staff"));
    options.AddPolicy("SaleStaffPolicy", policy =>
        policy.RequireClaim("Role", "Sale Staff"));
    options.AddPolicy("ManagerPolicy", policy =>
        policy.RequireClaim("Role", "Manager"));
    options.AddPolicy("CustomerPolicy", policy =>
        policy.RequireClaim("Role", "Customer"));
});

// ----------------- SignalR Configuration Start -----------------
// Add SignalR services
builder.Services.AddSignalR(); // Đăng ký SignalR với DI container

// ----------------- SignalR Configuration End -------------------

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.IsStaging())
{
    // Enable Swagger for API testing
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "KoiShipping API v1.0");
        c.InjectStylesheet("/static/css/swaggerui-dark.css"); // Optional: Custom styling for Swagger
    });
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable CORS
app.UseCors("MyPolicy");

// Enable Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// ----------------- SignalR Middleware Start -----------------
// Map SignalR hub
app.MapHub<OrderHub>("/orderHub"); // "/orderHub" là endpoint của SignalR hub cho thông báo đơn hàng mới
// ----------------- SignalR Middleware End -------------------

// Map the controllers
app.MapControllers();

app.Run();
