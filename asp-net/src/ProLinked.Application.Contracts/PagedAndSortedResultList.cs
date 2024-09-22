namespace ProLinked.Application.DTOs;

[Serializable]
public record PagedAndSortedResultList<T>(int Count, IReadOnlyList<T> Items)
{
    public int Count { get; } = Count;
    public IReadOnlyList<T> Items { get; } = Items;
}