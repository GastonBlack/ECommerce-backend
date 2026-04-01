using ECommerceAPI.Data;
using ECommerceAPI.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ECommerceAPI.Services.ImageUpload;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.RateLimiting;

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
// CORS
// =====================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://ecommerce-git-main-gastonreds-projects.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = JwtRegisteredClaimNames.Name,
            ClockSkew = TimeSpan.Zero,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Si no viene en Authorization header, lo busca en cookies.
                var token = context.Request.Cookies["token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

// =====================================
// CSRF
// =====================================
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";

    // Cookie INTERNA de .NET
    options.Cookie.Name = ".AspNetCore.Antiforgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.SuppressXFrameOptionsHeader = false;
});

// =====================================
// RATE LIMITING
// =====================================
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Máximo 6 requests por minuto en auth.
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 6,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        )
    );

    // Máximo 5 actualizaciones de perfil cada 30 segundos.
    options.AddPolicy("updateProfile", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromSeconds(30),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        )
    );

    // Máximo 10 requests para ver el perfil por minuto.
    options.AddPolicy("seeProfile", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        )
    );

    // Máximo 60 requests cada 30 segundos en catalog.
    options.AddPolicy("catalog", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromSeconds(30),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        )
    );

    // Máximo 5 requests por minuto en compras.
    options.AddPolicy("payment", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        )
    );
});

// =====================================
// AUTHORIZATION
// =====================================
builder.Services.AddAuthorization();

// =====================================
// MERCADO PAGO
// =====================================
builder.Services.Configure<MercadoPagoSettings>(
    builder.Configuration.GetSection("MercadoPago")
);

// =====================================
// CLOUDINARY
// =====================================
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary")
);

builder.Services.AddSingleton<ICloudinaryService, CloudinaryService>();

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
builder.Services.AddScoped<ECommerceAPI.Services.Orders.IAdminOrderService, ECommerceAPI.Services.Orders.AdminOrderService>();
builder.Services.AddScoped<ECommerceAPI.Services.Payments.IPaymentService, ECommerceAPI.Services.Payments.PaymentService>();

// =====================================
// CONTROLLERS + SWAGGER
// =====================================
builder.Services.AddControllersWithViews(options =>
{
    // Esto aplica la validación a TODOS los POST, PUT, DELETE, etc.
    // Pero ignora automáticamente los GET.
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

// =====================================
// APP MIDDLEWARE
// =====================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();

// =====================================
// AÑADIR PRODUCTOS POR DEFALT
// =====================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Debug
    // await context.Database.EnsureDeletedAsync();

    await context.Database.MigrateAsync();
    await DbDefaultProducts.SeedAsync(context);
}
app.Run();