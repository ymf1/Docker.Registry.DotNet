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

public record ImageReference
{
    public ImageReference(string value)
    {
        var parsedTag = value?.Trim() ?? string.Empty;

        if (IsSha256Digest(parsedTag.ToLower()))
        {
            // save as all lower
            this.Value = parsedTag.ToLower();
        }
        else
        {
            var errors = ValidateTag(parsedTag).ToList();

            if (errors.Any())
            {
                throw new ArgumentException(
                    $"Invalid Image Reference: {errors.ToDelimitedString(", ")}",
                    nameof(value));
            }

            this.Value = parsedTag;
        }
    }

    public static ImageReference Latest { get; } = new("latest");

    public string Value { get; init; }

    public bool IsDigest => IsSha256Digest(Value);

    public bool IsTag => !this.IsDigest;

    public void Deconstruct(out string value)
    {
        value = this.Value;
    }

    public static ImageReference Create(string tag)
    {
        return new ImageReference(tag);
    }

    public static bool TryCreate(string reference, [NotNullWhen(true)] out ImageReference? imageTag)
    {
        reference = reference?.Trim() ?? string.Empty;

        var errors = ValidateTag(reference).ToList();

        imageTag = null;

        if (errors.Any())
        {
            return false;
        }

        imageTag = new ImageReference(reference);

        return true;
    }

    static bool IsSha256Digest(string value)
    {
        const string DigestPrefix = "sha256:";

        if (value?.StartsWith(DigestPrefix) ?? false)
        {
            // sha256 hash is always 64 characters
            string hash = value.TakeAfter(DigestPrefix.Length) ?? string.Empty;

            if (hash.Length == 64 && hash.All(c => c.IsHexLowerCase()))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<string> ValidateTag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            yield return "Value must not be null or empty";

            yield break;
        }

        if (value.Length > 128)
        {
            yield return $"Value is too large. Value is {value.Length
            } characters and maximum tag size is 128 characters.";

            yield break;
        }

        if (IsSha256Digest(value.ToLower()))
        {
            yield break;
        }

        var validChars = new[] { '-', '_', '.' };

        var invalidCharacters =
#if NET7_0_OR_GREATER
            value.Where(c => !char.IsAsciiLetterOrDigit(c) && !validChars.Contains(c)).ToArray();
#else
            value.Where(c => !char.IsLetterOrDigit(c) && !validChars.Contains(c)).ToArray();
#endif

        if (invalidCharacters.Any())
        {
            yield return @$"Value ""{value}"" is invalid characters: ""{
                invalidCharacters.Select(s => $"{s}").ToDelimitedString(",")
            }"". Image References can only contain lowercase and uppercase letters, digits, underscores, periods, and hyphens.";

            yield break;
        }

        if (value.StartsWith(".") || value.StartsWith("-"))
        {
            yield return @$"Value ""{value}"" is invalid. Image References can't start with a period or hyphen.";
        }
    }

    public override string ToString() => this.Value;
}