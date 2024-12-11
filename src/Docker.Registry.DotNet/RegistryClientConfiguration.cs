//  Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






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

   
}