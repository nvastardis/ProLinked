using System.Text;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProLinked.Data;
using ProLinked.Domain.Identity;
using ProLinked.Infrastructure.BlobStorage;
using ProLinked.Infrastructure.Identity.Auth;
using ProLinked.Infrastructure.Identity.Manage;
using ProLinked.Infrastructure.Localization;

namespace ProLinked;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        ConfigureLocalization(builder.Services);
        ConfigureBlobStorage(builder.Services, builder.Configuration);
        ConfigureDatabase(builder.Services, builder.Configuration);
        ConfigureIdentity(builder.Services);
        ConfigureAuthentication(builder.Services, builder.Configuration);
        ConfigureAuthorization(builder.Services);
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        ConfigureSwagger(builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        app.UseAuthentication();
        app.UseAuthorization();

        app.Run();
    }

    private static void ConfigureDatabase(IServiceCollection serviceCollection, ConfigurationManager configurationManager)
    {
        var connectionString = configurationManager.GetConnectionString("Default");

        serviceCollection.AddDbContext<ProLinkedDbContext>(options =>
            options.UseSqlServer(connectionString));
    }

    private static void ConfigureIdentity(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddIdentityCore<AppUser>(
                options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ProLinkedDbContext>()
            .AddApiEndpoints();

        serviceCollection.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            options.SignIn.RequireConfirmedAccount = false;
        });

    }

    private static void ConfigureBlobStorage(IServiceCollection serviceCollection, ConfigurationManager configurationManager)
    {
        var azureStorageConfigurationSection = configurationManager.GetSection("AzureStorageAccountSettings");
        var azureStorageConnectionString = azureStorageConfigurationSection.GetConnectionString("ConnectionString");

        serviceCollection.AddSingleton(_ => new BlobContainerClient(
            azureStorageConnectionString,
            ContainerConsts.ContainerName));

        serviceCollection.AddScoped<IBlobService, AzureBlobService>();
    }

    private static void ConfigureLocalization(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IStringLocalizerFactory, ProLinkedLocalizerFactory>();
        serviceCollection.AddSingleton<IStringLocalizer, ProLinkedLocalizer>();
        serviceCollection.AddLocalization(options =>
            options.ResourcesPath = @"Shared\Localization\ProLinked\en.json");
    }

    private static void ConfigureAuthentication(IServiceCollection serviceCollection, ConfigurationManager configurationManager)
    {

        AuthSettings.PrivateKey = configurationManager["JwtKey"]!;
        AuthSettings.RefreshKey = configurationManager["JwtRefreshKey"]!;
        AuthSettings.Issuer = configurationManager["JwtSettings:Issuer"]!;
        AuthSettings.Audience = configurationManager["JwtSettings:Audience"]!;


        serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters =
                    new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configurationManager["JwtKey"]!)),
                        ValidIssuer = configurationManager["JwtSettings:Issuer"],
                        ValidAudience = configurationManager["JwtSettings:Audience"]
                    };
            });

        serviceCollection.AddTransient<IAuthService, AuthService>();
        serviceCollection.AddTransient<ManageService>();
        serviceCollection.AddTransient<JwtTokenManager>();
    }

    private static void ConfigureAuthorization(IServiceCollection serviceCollection)
    {
        serviceCollection.AddAuthorization();
    }

    private static void ConfigureSwagger(IServiceCollection serviceCollection)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen(c =>
        {
            c.MapType<DateOnly>(() => new OpenApiSchema {
                Type = "string",
                Format = "date" });

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ProLinked Web Api",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
    }
}
