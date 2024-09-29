using ProLinked.Infrastructure.Timing;

namespace ProLinked.Infrastructure.Recommendation;

public class RecommendationService: IHostedService, IDisposable
{
    private readonly IPeriodicTimer _timer;
    private readonly Recommender _recommender;

    public RecommendationService(
        Recommender recommender,
        IPeriodicTimer timer)
    {
        _recommender = recommender;
        _timer = timer;
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested &&
               await _timer.WaitForNextTickAsync(cancellationToken))
        {
            await UpdateRecommendations(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer.Dispose();
    }

    private async Task UpdateRecommendations(CancellationToken cancellationToken)
    {
        await _recommender.SetJobRecommendationsAsync(cancellationToken);
        await _recommender.SetJobRecommendationsAsync(cancellationToken);
    }
}