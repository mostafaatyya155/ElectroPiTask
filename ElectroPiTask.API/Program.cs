
using API.Extensions;
using ElectroPiTask.API.Extensions;
using ElectroPiTask.Application.Common.Interfaces;
using ElectroPiTask.Application.Services;
using ElectroPiTask.Infrastructure.Persistence;
using ElectroPiTask.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Text;

namespace ElectroPiTask.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {

                var builder = WebApplication.CreateBuilder(args);

                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(5000); // HTTP
                    options.ListenAnyIP(5001, listenOptions =>
                    {
                        listenOptions.UseHttps(); // HTTPS
                    });
                });

                // Configure Serilog
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", "ElectroPiTask")
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File(
                        path: "Logs/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                        retainedFileCountLimit: 90)  // Keep 90 days of logs
                    .CreateLogger();

                Log.Information("Starting Application");

                builder.Host.UseSerilog();

                // Add services to the container.

                builder.Services.AddControllers();

                // FluentValidation
                builder.Services.AddFluentValidation();

                builder.Services.AddSwaggerDocumentation();

                // Database Configuration
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


                // JWT Authentication
                var jwtSecret = builder.Configuration["JWT:Secret"];
                var key = Encoding.ASCII.GetBytes(jwtSecret);

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        ClockSkew = TimeSpan.Zero
                    };
                });
                builder.Services.AddAuthorization();

                // CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
                });

                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi();

                // Dependency Injection
                builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
                builder.Services.AddScoped<AuthService>();
                builder.Services.AddScoped<TokenService>();
                builder.Services.AddScoped<ProjectService>();
                builder.Services.AddScoped<ProjectTaskService>();


                var app = builder.Build();

                // Seed database
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    await ApplicationDbContextSeed.SeedAsync(scope.ServiceProvider);
                }

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                }

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseCors("AllowAll");

                app.UseAuthentication();
                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {

                Log.Fatal(ex, "Application failed to start");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}
