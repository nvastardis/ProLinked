namespace ProLinked.Infrastructure.Timing
{
    public interface IPeriodicTimer : IDisposable
    {
        ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default);
    }
}