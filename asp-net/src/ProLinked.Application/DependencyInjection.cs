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
using ProLinked.Application.Contracts.Jobs;
using ProLinked.Application.Contracts.Posts;
using ProLinked.Application.Contracts.Resumes;
using ProLinked.Application.Localization;
using ProLinked.Application.Services.Blobs;
using ProLinked.Application.Services.Chats;
using ProLinked.Application.Services.Connections;
using ProLinked.Application.Services.Identity;
using ProLinked.Application.Services.Jobs;
using ProLinked.Application.Services.Posts;
using ProLinked.Application.Services.Resumes;
using ProLinked.Domain;

namespace ProLinked.Application;

public static class DependencyInjection
{
    public static void AddProLinkedLocalization(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton<IStringLocalizerFactory, ProLinkedLocalizerFactory>();
        serviceCollection.AddSingleton<IStringLocalizer>(
            new ProLinkedLocalizer(
                [configuration["LocalizationFile"]]));
        serviceCollection.AddLocalization(options =>
            options.ResourcesPath = @"Shared\Localization\ProLinked\en.json");
    }

    public static void AddProLinkedAuthentication(this IServiceCollection serviceCollection, IConfiguration configurationManager)
    {
        AuthSettings.PrivateKey = configurationManager["JwtSettings:SecretKey"]!;
        AuthSettings.RefreshKey = configurationManager["JwtSettings:RefreshKey"]!;
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
                        Encoding.UTF8.GetBytes(AuthSettings.PrivateKey))
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
        serviceCollection.AddScoped<IConnectionRequestService, ConnectionRequestService>();
        serviceCollection.AddScoped<IJobService, JobService>();
        serviceCollection.AddScoped<ICommentService, CommentService>();
        serviceCollection.AddScoped<IPostService, PostService>();
        serviceCollection.AddScoped<IResumeService, ResumeService>();
        serviceCollection.AddScoped<ISkillService, SkillService>();
    }

    public static void AddProLinkedDomainServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDomainServices();
    }
}