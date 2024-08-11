//  Copyright 2017-2022 Rich Quackenbush, Jaben Cargman
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

namespace Docker.Registry.DotNet.Application.QueryStrings;

internal static class QueryStringExtensions
{
    /// <summary>
    ///     Adds the value to the query string if it's not null.
    /// </summary>
    /// <param name="readOnlyQueryString"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    internal static void AddIfNotNull<T>(this QueryString readOnlyQueryString, string key, T? value)
        where T : struct
    {
        if (value != null) readOnlyQueryString.Add(key, $"{value.Value}");
    }

    /// <summary>
    /// </summary>
    /// <param name="readOnlyQueryString"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    internal static void AddIfNotEmpty(this QueryString readOnlyQueryString, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value)) readOnlyQueryString.Add(key, value);
    }
}