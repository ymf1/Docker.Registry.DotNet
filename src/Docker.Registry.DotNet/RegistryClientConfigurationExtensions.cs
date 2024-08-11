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

namespace Docker.Registry.DotNet;

public static class RegistryClientConfigurationExtensions
{
    public static RegistryClientConfiguration UseBasicAuthentication(
        this RegistryClientConfiguration configuration,
        string username,
        string password)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        configuration.SetAuthenticationProvider(
            new PasswordOAuthAuthenticationProvider(username, password));

        return configuration;
    }

    public static RegistryClientConfiguration UsePasswordOAuthAuthentication(
        this RegistryClientConfiguration configuration,
        string username,
        string password)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        configuration.SetAuthenticationProvider(
            new PasswordOAuthAuthenticationProvider(username, password));

        return configuration;
    }

    /// <summary>
    /// Supports Docker Hub Authentication.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static RegistryClientConfiguration UseDockerHubAuthentication(
        this RegistryClientConfiguration configuration,
        string username,
        string password)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        configuration.SetAuthenticationProvider(
            new DockerHubJwtAuthenticationProvider(username, password));

        return configuration;
    }

    /// <summary>
    /// Default authentication provider.
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static RegistryClientConfiguration UseAnonymousOAuthAuthentication(
        this RegistryClientConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        configuration.SetAuthenticationProvider(new AnonymousOAuthAuthenticationProvider());

        return configuration;
    }
}