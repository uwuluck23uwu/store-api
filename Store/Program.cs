global using ClassLibrary.Models.Data;
global using ClassLibrary.Models.Response;
global using Store.Data;
global using Store.Services.IServices;

using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to allow larger request body (50MB)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 52428800; // 50 MB
});

// Settings
var key = builder.Configuration.GetValue<string>("Settings:SecretProgram");
builder.Configuration["ApiKey"] = key; // Make ApiKey available for services
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Controllers + JSON
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    opt.JsonSerializerOptions.WriteIndented = true;
    opt.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Configure form options to allow larger file uploads (50MB)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800; // 50 MB
    options.ValueLengthLimit = 52428800;
    options.MemoryBufferThreshold = Int32.MaxValue;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", cors =>
    {
        cors.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// JWT Authentication
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key ?? "")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
        ValidAudience = jwtSettings.GetValue<string>("Audience"),
        ClockSkew = TimeSpan.Zero,
    };
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Store API",
        Description = "Community Marketplace API for buying and selling local products (กล้วยตาก, กล้วยทอด, ผลิตภัณฑ์แปรรูป)",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Store API Team",
            Email = "support@storeapi.com"
        }
    });

    // JWT Bearer Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \n\nEnter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Register Services
builder.Services.AddScoped<IImageService, Store.Services.ImageService>();
builder.Services.AddScoped<IAuthenService, Store.Services.AuthenService>();
builder.Services.AddScoped<ICategoryService, Store.Services.CategoryService>();
builder.Services.AddScoped<ISellerService, Store.Services.SellerService>();
builder.Services.AddScoped<IProductService, Store.Services.ProductService>();
builder.Services.AddScoped<ICartService, Store.Services.CartService>();
builder.Services.AddScoped<IOrderService, Store.Services.OrderService>();
builder.Services.AddScoped<IReviewService, Store.Services.ReviewService>();
builder.Services.AddScoped<ILocationService, Store.Services.LocationService>();
builder.Services.AddScoped<IUserService, Store.Services.UserService>();
builder.Services.AddScoped<IAppBannerService, Store.Services.AppBannerService>();
builder.Services.AddScoped<ICultureService, Store.Services.CultureService>();

var app = builder.Build();

// Configure the HTTP request pipeline

// Enable Swagger in all environments (Development and Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store API v1");
    c.RoutePrefix = string.Empty; // Make Swagger UI the root page (https://localhost:xxxx/)
    c.DocumentTitle = "Store API - Swagger UI";
    c.DefaultModelsExpandDepth(2);
    c.DefaultModelExpandDepth(2);
});

app.UseStaticFiles(); // Enable static files (wwwroot/images)

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
