using ECommerceAPI.Data;
using ECommerceAPI.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// DATABASE
// =====================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// =====================================
// JWT SETTINGS
// =====================================
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.Key);

// =====================================
// AUTHENTICATION
// =====================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

// =====================================
// AUTHORIZATION
// =====================================
builder.Services.AddAuthorization();

// =====================================
// SERVICES (DI)
// =====================================
builder.Services.AddScoped<ECommerceAPI.Services.Jwt.IJwtService, ECommerceAPI.Services.Jwt.JwtService>();
builder.Services.AddScoped<ECommerceAPI.Services.Auth.IAuthService, ECommerceAPI.Services.Auth.AuthService>();
builder.Services.AddScoped<ECommerceAPI.Services.Users.IUserService, ECommerceAPI.Services.Users.UserService>();
builder.Services.AddScoped<ECommerceAPI.Services.Products.IProductService, ECommerceAPI.Services.Products.ProductService>();
builder.Services.AddScoped<ECommerceAPI.Services.Categories.ICategoryService, ECommerceAPI.Services.Categories.CategoryService>();
builder.Services.AddScoped<ECommerceAPI.Services.Cart.ICartService, ECommerceAPI.Services.Cart.CartService>();
builder.Services.AddScoped<ECommerceAPI.Services.Orders.IOrderService, ECommerceAPI.Services.Orders.OrderService>();

// =====================================
// CONTROLLERS + SWAGGER
// =====================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================================
// APP MIDDLEWARE
// =====================================
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
