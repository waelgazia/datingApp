using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using DatingApp.API.Data;
using DatingApp.API.helper;
using DatingApp.API.Globals;
using DatingApp.API.SignalR;
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
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<LogUserActivity>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<PresenceTracker>();

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

        // WebSockets cannot send custom headers (including Authorization). So JWT
        // token cannot be automatically sent to the Hub. SignalR solves this by
        // allowing the token to be passed using access_token query string
        // parameter. You must explicitly configure the client to send it that way.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                string? accessToken = context.Request.Query["access_token"];
                PathString requestPath = context.Request.Path;
                if (!string.IsNullOrWhiteSpace(accessToken) && requestPath.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.REQUIRE_ADMIN_ROLE, policy => policy.RequireRole(Roles.ADMIN))
    .AddPolicy(Policies.MODERATION_ROLE, policy => policy.RequireRole(Roles.MODERATOR))
    .AddPolicy(Policies.ADMIN_OR_MODERATION_ROLE, policy => policy.RequireRole(Roles.ADMIN, Roles.MODERATOR));

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

app.UseDefaultFiles();      // changes the request path '/' (default) to '/index.html' or the default html page
app.UseStaticFiles();       // serves static files under wwwroot folder.

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/messages");

app.MapFallbackToController("Index", "Fallback");

// database seeding
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    try
    {
        AppDbContext context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        await context.Connections.ExecuteDeleteAsync(); /* delete all user connections when restarting the server */

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