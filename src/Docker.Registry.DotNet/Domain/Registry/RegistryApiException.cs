

// 



// 

// 









// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Registry;

public class RegistryApiException : Exception
{
    internal RegistryApiException(RegistryApiResponse response)
        : base($"Docker API responded with status code={response.StatusCode}")
    {
        StatusCode = response.StatusCode;
        Headers = response.Headers;
    }

    public HttpStatusCode StatusCode { get; }

    public HttpResponseHeaders Headers { get; }
}

public class RegistryApiException<TBody> : RegistryApiException
{
    internal RegistryApiException(RegistryApiResponse<TBody> response)
        : base(response)
    {
        Body = response.Body;
    }

    public TBody? Body { get; }
}