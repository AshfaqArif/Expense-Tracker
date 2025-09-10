
using System.Text;
using DonationApp.Api.Data;
using DonationApp.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace DonationApp.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        // EF Core SQLite
        var connectionString = builder.Configuration.GetConnectionString("Default");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // Domain services
        builder.Services.AddScoped<IOtpService, OtpService>();
        builder.Services.AddScoped<ITokenService, TokenService>();

        // Authentication - JWT (used for admin; donor login via OTP exchanges for JWT)
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var jwtKey = jwtSection.GetValue<string>("Key") ?? "";
        var jwtIssuer = jwtSection.GetValue<string>("Issuer") ?? "";
        var jwtAudience = jwtSection.GetValue<string>("Audience") ?? "";
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
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

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        // Placeholder root endpoint
        app.MapGet("/", () => Results.Ok(new { message = "Donation App API" }));

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await DataSeeder.SeedAsync(db);
        }

        await app.RunAsync();
    }
}
