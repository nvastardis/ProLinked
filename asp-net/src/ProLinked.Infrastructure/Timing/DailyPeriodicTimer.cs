namespace ProLinked.Infrastructure.Timing;

public class DailyPeriodicTimer: IPeriodicTimer
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromDays(1));

    public async ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
    {
        return await _timer.WaitForNextTickAsync(cancellationToken);
    }

    public void Dispose()
    {
        _timer.Dispose();
    }

}