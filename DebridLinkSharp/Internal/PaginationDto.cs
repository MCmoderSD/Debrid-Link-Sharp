namespace DebridLinkSharp.Internal;

internal sealed class PaginationDto
{
    public int Page { get; init; }

    public int Pages { get; init; } = 1;

    public int Next { get; init; } = -1;

    public int Previous { get; init; } = -1;
}