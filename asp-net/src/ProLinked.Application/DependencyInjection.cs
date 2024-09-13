using Microsoft.Extensions.DependencyInjection;
using ProLinked.Application.Managers;
using ProLinked.Application.Services.Blobs;
using ProLinked.Application.Services.Chats;
using ProLinked.Application.Services.Connections;
using ProLinked.Application.Services.Notifications;
using ProLinked.Application.Services.Posts;
using ProLinked.Application.Services.Resumes;
using ProLinked.Domain.Services.Jobs;


namespace ProLinked.Application;

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
