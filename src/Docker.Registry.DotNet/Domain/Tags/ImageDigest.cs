// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman
//  and Docker.Registry.DotNet Contributors
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Diagnostics.CodeAnalysis;

namespace Docker.Registry.DotNet.Domain.Tags;

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