using ProLinked.Domain.Contracts.Blobs;
using ProLinked.Domain.Contracts.Chats;
using ProLinked.Domain.Contracts.Connections;
using ProLinked.Domain.Contracts.Jobs;
using ProLinked.Domain.Contracts.Notifications;
using ProLinked.Domain.Contracts.Posts;
using ProLinked.Domain.Contracts.Resumes;
using ProLinked.Domain.Services;

namespace ProLinked.Domain;

public static class DependencyInjection
{
    public static void AddDomainServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IBlobManager, BlobManager>();
        serviceCollection.AddTransient<IChatManager, ChatManager>();
        serviceCollection.AddTransient<IConnectionManager, ConnectionManager>();
        serviceCollection.AddTransient<IJobManager, JobManager>();
        serviceCollection.AddTransient<INotificationManager, NotificationManager>();
        serviceCollection.AddTransient<IPostManager, PostManager>();
        serviceCollection.AddTransient<IResumeManager, ResumeManager>();
    }
}