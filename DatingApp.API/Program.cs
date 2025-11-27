using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using DatingApp.API.Data;
using DatingApp.API.helper;
using DatingApp.API.Globals;
using DatingApp.API.Services;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using DatingApp.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty);
});
builder.Services.AddCors();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IMembersRepository, MembersRepository>();
builder.Services.AddScoped<ILikesRepository, LikesRepository>();
builder.Services.AddScoped<IMessagesRepository, MessagesRepository>();
builder.Services.AddScoped<LogUserActivity>();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddIdentityCore<AppUser>(
        options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        }
    )
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        string tokenSecretKey = builder.Configuration["Authentication:TokenSecretKey"]
            ?? throw new NullReferenceException("Can not get token secret key!");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecretKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.REQUIRE_ADMIN_ROLE, policy => policy.RequireRole(Roles.ADMIN))
    .AddPolicy(Policies.MODERATE_PHOTO_ROLE, policy => policy.RequireRole(Roles.MODERATOR));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(corsPolicy => corsPolicy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()   // allows send and receive cookies (i.e., refresh token cookie)
    .WithOrigins("http://localhost:4200", "https://localhost:4200")
    .WithExposedHeaders([HeaderNames.Pagination])
);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// database seeding
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    try
    {
        AppDbContext context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
        await Seed.SeedUserAsync(userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError("An error occurred during migration: {0}", ex.Message);
    }
}

app.Run();