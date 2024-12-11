// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






namespace Docker.Registry.DotNet.Infrastructure.Helpers;

public static class HttpContentHelper
{
    public static Task<string> ReadAsStringAsyncWithCancellation(
        this HttpContent content,
        CancellationToken token)
    {
#if NET5_0_OR_GREATER
        return content.ReadAsStringAsync(token);
#else
        return content.ReadAsStringAsync();
#endif
    }

    public static Task<Stream> ReadAsStreamAsyncWithCancellation(
        this HttpContent content,
        CancellationToken token)
    {
#if NET5_0_OR_GREATER
        return content.ReadAsStreamAsync(token);
#else
        return content.ReadAsStreamAsync();
#endif
    }
}