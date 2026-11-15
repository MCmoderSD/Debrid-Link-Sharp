namespace DebridLinkSharp.Internal;

internal static class QueryString
{
    public static string? Pair(string name, int? value) => value is { } number ? $"{name}={number}" : null;

    public static string? Pair(string name, string? value) => value == null ? null : $"{name}={Uri.EscapeDataString(value)}";

    public static string? Pair(string name, IEnumerable<string>? values) => values == null ? null : $"{name}={string.Join(',', values.Select(Uri.EscapeDataString))}";

    public static string Append(string path, params string?[] pairs)
    {
        var used = pairs.Where(pair => pair != null).ToList();
        return used.Count == 0 ? path : $"{path}?{string.Join('&', used)}";
    }
}