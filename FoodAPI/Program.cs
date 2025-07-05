using FirebaseAdmin;
using FoodAPI.DbContexts;
using FoodAPI.Interfaces;
using FoodAPI.Repositories;
using FoodAPI.Services;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Add SeriLog for better log information
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

builder.WebHost.ConfigureKestrel(options => {
    options.Listen(IPAddress.Any, 5087);
    options.Listen(IPAddress.Any, 7208);
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase_token.json"),
});





//Add DbContext
builder.Services.AddDbContext<FoodOrderContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Add repository di
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFoodCategoryRepository, FoodCategoryRepository>();
builder.Services.AddScoped<IFoodItemRepository, FoodItemRepository>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IShippingInfoRepository, ShippingInfoRepository>();
builder.Services.AddScoped<IMapRoutingService, MapRoutingService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UserAccessLevel", policy => policy.RequireRole("Admin", "User", "Owner"))
    .AddPolicy("OwnerAccessLevel", policy => policy.RequireRole("Admin", "Owner"))
    .AddPolicy("AdminAccessLevel", policy => policy.RequireRole("Admin"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();