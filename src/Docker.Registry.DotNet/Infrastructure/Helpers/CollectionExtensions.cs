// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






namespace Docker.Registry.DotNet.Infrastructure.Helpers;

internal static class CollectionExtensions
{
#if !NET5_0_OR_GREATER
    internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
    {
        return [..items];
    }
#endif
}