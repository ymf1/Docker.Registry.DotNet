

// 



// 

// 






namespace Docker.Registry.DotNet.Infrastructure.Helpers;

public static class StringExtensions
{
    public static string ToDelimitedString(
        this IEnumerable<string> strings,
        string delimiter = "")
    {
        return string.Join(delimiter, strings.IfNullEmpty().ToArray());
    }

    public static string? TakeAfter(this string? str, int afterIndex)
    {
        if (str == null) return null;

        int strLength = str.Length;

        return afterIndex >= strLength ? str : str.Substring(afterIndex, strLength - afterIndex);
    }
}