﻿// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman
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