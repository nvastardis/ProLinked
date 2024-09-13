using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using ProLinked.Application.Managers;
using ProLinked.Application.Services.Blobs;
using ProLinked.Application.Services.Chats;
using ProLinked.Application.Services.Connections;
using ProLinked.Application.Services.Jobs;
using ProLinked.Application.Services.Notifications;
using ProLinked.Application.Services.Posts;
using ProLinked.Application.Services.Resumes;
using ProLinked.Domain.Shared.Blobs;
using ProLinked.Infrastructure.Data;
using ProLinked.Infrastructure.Repositories.Chats;
using ProLinked.Infrastructure.Repositories.Connections;
using ProLinked.Infrastructure.Repositories.Jobs;
using ProLinked.Infrastructure.Repositories.Notifications;
using ProLinked.Infrastructure.Repositories.Resumes;
using PostRepository = ProLinked.Infrastructure.Repositories.Posts.PostRepository;

namespace ProLinked.Infrastructure;

public static class DependencyInjection
{
    public static void AddRepositories(IServiceCollection serviceCollection)
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
    }

    public static void AddAzureBlobStoring(IServiceCollection serviceCollection, string azureConnectionString)
    {
        serviceCollection.AddSingleton(_ => new BlobContainerClient(
            azureConnectionString,
            ContainerConsts.ContainerName));

        serviceCollection.AddScoped<IBlobService, AzureBlobService>();
    }

    public static void AddDbConnection(IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddDbContext<ProLinkedDbContext>(options =>
            options.UseSqlServer(connectionString));
    }
}
