// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






using System.Diagnostics.CodeAnalysis;

namespace Docker.Registry.DotNet.Domain.ImageReferences;

public record ImageDigest
{
    public ImageDigest(string value)
    {
        var digest = value?.Trim()?.ToLower() ?? string.Empty;

        if (!IsValidDigest(digest))
        {
            throw new ArgumentException($"Invalid Digest: {digest}", nameof(value));
        }

        this.Value = digest;
    }

    public string Value { get; init; }

    public ImageReference ToReference() => new(this);

    public void Deconstruct(out string value)
    {
        value = this.Value;
    }

    public static ImageDigest Create(string digest)
    {
        return new ImageDigest(digest);
    }

    public static bool TryCreate(string reference, [NotNullWhen(true)] out ImageDigest? digest)
    {
        reference = reference?.Trim() ?? string.Empty;

        if (IsValidDigest(reference))
        {
            digest = new ImageDigest(reference);
            return true;
        }

        digest = null;

        return false;
    }

    /// <summary>
    /// Based on this site: https://ktomk.github.io/pipelines/doc/DOCKER-NAME-TAG.html
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static bool IsValidDigest(string value)
    {
        var twoParts = value?.Split(':') ?? [];

        if (twoParts.Length == 2)
        {
            var hash = twoParts[1].Trim();

            if (hash.Length == 64 && hash.All(c => c.IsHash()))
            {
                return true;
            }
        }

        return false;
    }

    public override string ToString() => this.Value;
}