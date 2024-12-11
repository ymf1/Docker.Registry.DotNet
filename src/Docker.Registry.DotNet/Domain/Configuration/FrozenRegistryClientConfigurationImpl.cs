// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Configuration;

public record FrozenRegistryClientConfigurationImpl(
    Uri BaseAddress,
    HttpMessageHandler? HttpMessageHandler,
    AuthenticationProvider AuthenticationProvider,
    TimeSpan DefaultTimeout)
    : IFrozenRegistryClientConfiguration;