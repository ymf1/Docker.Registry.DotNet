// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






namespace Docker.Registry.DotNet.Infrastructure.Helpers;

internal static class CharHelpers
{
    /// <summary>
    /// Only for lower-case strings
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    internal static bool IsHash(this char c) => c is >= '0' and <= '9' or >= 'a' and <= 'f' or '=' or '_' or '-';
}