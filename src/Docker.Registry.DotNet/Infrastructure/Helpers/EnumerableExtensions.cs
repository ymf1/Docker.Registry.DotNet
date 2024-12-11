

// 



// 

// 






namespace Docker.Registry.DotNet.Infrastructure.Helpers;

public static class EnumerableExtensions
{
    public static IEnumerable<T> IfNullEmpty<T>(this IEnumerable<T>? enumerable)
    {
            return enumerable ?? [];
        }
}