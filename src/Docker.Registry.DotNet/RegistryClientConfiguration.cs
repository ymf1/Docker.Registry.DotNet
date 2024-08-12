//  Copyright 2017-2024 Rich Quackenbush, Jaben Cargman
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

using Docker.Registry.DotNet.Domain.Configuration;

namespace Docker.Registry.DotNet;

public class RegistryClientConfiguration
{
    private TimeSpan _defaultTimeout = TimeSpan.FromSeconds(100);

    public RegistryClientConfiguration(string baseAddress, TimeSpan defaultTimeout = default)
    {
        var isValidUri  = Uri.TryCreate(baseAddress, UriKind.Absolute, out var parsedBaseAddress);

        if (isValidUri ) this.SetBaseAddress(parsedBaseAddress);
        else throw new ArgumentException("BaseAddress is not a valid Uri", nameof(baseAddress));

        this.SetDefaultTimeout(defaultTimeout);
    }

    public RegistryClientConfiguration(
        Uri baseAddress,
        HttpMessageHandler? httpMessageHandler = null,
        TimeSpan defaultTimeout = default)
    {
        this.SetBaseAddress(baseAddress);
        this.SetDefaultTimeout(defaultTimeout);
        this.SetHttpMessageHandler(httpMessageHandler);
    }

    public RegistryClientConfiguration()
    {
    }

    public Uri? BaseAddress { get; private set; }

    public HttpMessageHandler? HttpMessageHandler { get; private set; }

    /// <summary>
    ///     Defaults to AnonymousOAuthAuthenticationProvider
    /// </summary>
    public AuthenticationProvider AuthenticationProvider { get; private set; } =
        new AnonymousOAuthAuthenticationProvider();

    public TimeSpan DefaultTimeout
    {
        get => this._defaultTimeout;
        private set
        {
            if (value != this._defaultTimeout && value != default)
            {
                this._defaultTimeout = value;
            }
        }
    }

    public RegistryClientConfiguration SetBaseAddress(Uri baseAddress)
    {
        if (baseAddress == null)
            throw new ArgumentException("BaseAddress cannot be null.", nameof(this.BaseAddress));

        if (baseAddress.Scheme is not ("http" or "https"))
            throw new ArgumentOutOfRangeException(
                nameof(this.BaseAddress),
                "BaseAddress must use either http or https schema.");

        this.BaseAddress = baseAddress;

        return this;
    }

    public RegistryClientConfiguration SetDefaultTimeout(TimeSpan defaultTimeout)
    {
        this.DefaultTimeout = defaultTimeout;

        return this;
    }

    public RegistryClientConfiguration SetHttpMessageHandler(HttpMessageHandler? messageHandler)
    {
        this.HttpMessageHandler = messageHandler;

        return this;
    }

    /// <summary>
    ///     Defaults to AnonymousOAuthAuthenticationProvider
    /// </summary>
    public RegistryClientConfiguration SetAuthenticationProvider(
        AuthenticationProvider? authenticationProvider)
    {
        this.AuthenticationProvider = authenticationProvider
                                      ?? new AnonymousOAuthAuthenticationProvider();

        return this;
    }

    public IRegistryClient CreateClient()
    {
        if (this.BaseAddress == null)
            throw new ArgumentException("BaseAddress cannot be null.", nameof(this.BaseAddress));

        return new RegistryClient(
            new FrozenRegistryClientConfigurationImpl(
                this.BaseAddress,
                this.HttpMessageHandler,
                this.AuthenticationProvider,
                this.DefaultTimeout));
    }

    [Obsolete("Use Configuration.SetAuthenticationProvider() instead.")]
    public IRegistryClient CreateClient(AuthenticationProvider authenticationProvider)
    {
        this.SetAuthenticationProvider(authenticationProvider);

        return this.CreateClient();
    }
}