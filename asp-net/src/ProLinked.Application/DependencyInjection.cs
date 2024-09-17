using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using ProLinked.Application.Contracts.Blobs;
using ProLinked.Application.Contracts.Chats;
using ProLinked.Application.Contracts.Connections;
using ProLinked.Application.Contracts.Identity;
using ProLinked.Application.Localization;
using ProLinked.Application.Services.Blobs;
using ProLinked.Application.Services.Chats;
using ProLinked.Application.Services.Connections;
using ProLinked.Application.Services.Identity;

namespace ProLinked.Application;

public static class DependencyInjection
{
    public static void AddProLinkedLocalization(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IStringLocalizerFactory, ProLinkedLocalizerFactory>();
        serviceCollection.AddSingleton<IStringLocalizer, ProLinkedLocalizer>();
        serviceCollection.AddLocalization(options =>
            options.ResourcesPath = @"Shared\Localization\ProLinked\en.json");
    }

    public static void AddProLinkedAuthentication(this IServiceCollection serviceCollection, ConfigurationManager configurationManager)
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
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters =
                new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configurationManager["JwtSettings:Issuer"],
                    ValidAudience = configurationManager["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configurationManager["JwtKey"]!))
                };
        });

        serviceCollection.AddScoped<IAuthService, AuthService>();
        serviceCollection.AddScoped<IManageService, ManageService>();
        serviceCollection.AddScoped<IJwtTokenService, JwtTokenService>();
    }

    public static void AddAutoMapper(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(config =>
        {
            config.AddProfile<ProLinkedAutoMapperProfile>();
        });
    }

    public static void AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBlobService, BlobService>();
        serviceCollection.AddScoped<IChatService, ChatService>();
        serviceCollection.AddScoped<IConnectionService, ConnectionService>();
    }
}