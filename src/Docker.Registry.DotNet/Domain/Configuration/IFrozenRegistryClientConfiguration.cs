// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Configuration;

public interface IFrozenRegistryClientConfiguration
{
    Uri? BaseAddress { get; }

    HttpMessageHandler? HttpMessageHandler { get; }
    AuthenticationProvider AuthenticationProvider { get; }

    TimeSpan DefaultTimeout { get; }
}