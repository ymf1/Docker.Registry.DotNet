// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






namespace Docker.Registry.DotNet.Application.Registry;

public class RegistryUriBuilder(Uri baseUri) : IRegistryUriBuilder
{
    public virtual Uri Build(string? path = null, string? queryString = null)
    {
        using var activity = Assembly.Source.StartActivity("RegistryUriBuilder.Build()");

        var pathIsUri = false;

        path = path?.Trim() ?? string.Empty;

        if (Uri.TryCreate(path, UriKind.Absolute, out var uri)) pathIsUri = true;
        else
        {
            // not absolute -- use the base uri
            uri = baseUri;
        }

        var builder = new UriBuilder(uri);

        if (!pathIsUri && !string.IsNullOrEmpty(path))
            builder.Path = path;

        if (!string.IsNullOrWhiteSpace(queryString))
        {
            if (string.IsNullOrWhiteSpace(builder.Query)) builder.Query = queryString!.Trim();
            else builder.Query += $"&{queryString!.Trim()}";
        }

        return builder.Uri;
    }
}