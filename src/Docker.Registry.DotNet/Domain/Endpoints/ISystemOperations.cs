

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Endpoints;


public interface ISystemOperations
{
    
    Task Ping(CancellationToken token = default);
}