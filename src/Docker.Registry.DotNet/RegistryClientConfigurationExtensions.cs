// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






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
            new BasicAuthenticationProvider(username, password));

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