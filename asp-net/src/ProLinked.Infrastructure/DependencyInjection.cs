using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProLinked.Domain;
using ProLinked.Domain.Azure;
using ProLinked.Domain.Contracts.Chats;
using ProLinked.Domain.Contracts.Connections;
using ProLinked.Domain.Contracts.Jobs;
using ProLinked.Domain.Contracts.Notifications;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Repositories.Posts;
using ProLinked.Domain.Repositories.Resumes;
using ProLinked.Domain.Shared.Blobs;
using ProLinked.Infrastructure.Data;
using ProLinked.Infrastructure.Data.Azure;
using ProLinked.Infrastructure.Data.Repositories;
using ProLinked.Infrastructure.Data.Repositories.Chats;
using ProLinked.Infrastructure.Data.Repositories.Connections;
using ProLinked.Infrastructure.Data.Repositories.Jobs;
using ProLinked.Infrastructure.Data.Repositories.Notifications;
using ProLinked.Infrastructure.Data.Repositories.Resumes;
using PostRepository = ProLinked.Infrastructure.Data.Repositories.Posts.PostRepository;

namespace ProLinked.Infrastructure;

public static class DependencyInjection
{
    public static void AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IChatRepository, ChatRepository>();
        serviceCollection.AddTransient<IConnectionRepository, ConnectionRepository>();
        serviceCollection.AddTransient<IConnectionRequestRepository, ConnectionRequestRepository>();
        serviceCollection.AddTransient<IAdvertisementRepository, AdvertisementRepository>();
        serviceCollection.AddTransient<IApplicationRepository, ApplicationRepository>();
        serviceCollection.AddTransient<INotificationRepository, NotificationRepository>();
        serviceCollection.AddTransient<IPostRepository, PostRepository>();
        serviceCollection.AddTransient<IResumeRepository, ResumeRepository>();
        serviceCollection.AddTransient<ISkillRepository, SkillRepository>();
        serviceCollection.AddTransient<IEducationRepository, EducationRepository>();
        serviceCollection.AddTransient<IExperienceRepository, ExperienceRepository>();
        serviceCollection.AddTransient(typeof(IRepository<>),typeof(ProLinkedBaseRepository<>));
        serviceCollection.AddTransient(typeof(IRepository<,>),typeof(ProLinkedBaseRepository<,>));
    }

    public static void AddAzureBlobStoring(this IServiceCollection serviceCollection, string azureConnectionString)
    {
        serviceCollection.AddSingleton(_ => new BlobContainerClient(
            azureConnectionString,
            ContainerConsts.ContainerName));

        serviceCollection.AddScoped<IAzureBlobService, AzureBlobService>();
    }

    public static void AddDbConnection(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddDbContext<ProLinkedDbContext>(options =>
            options.UseSqlServer(connectionString));
    }

    public static void AddIdentity(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddIdentityCore<AppUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ProLinkedDbContext>()
            .AddApiEndpoints();
        serviceCollection.AddDataProtection();

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
}