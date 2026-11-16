namespace DebridLinkSharp.Models;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];

    // The first page is 0.
    public int Page { get; init; }

    public int Pages { get; init; }

    // The number of the next or previous page, or -1 if there is none.
    public int Next { get; init; } = -1;

    public int Previous { get; init; } = -1;

    public bool HasNext => Next >= 0;

    public bool HasPrevious => Previous >= 0;
}