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