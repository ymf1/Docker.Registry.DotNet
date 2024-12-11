

// 



// 

// 






namespace Docker.Registry.DotNet.Application.OAuth;

[DataContract]
internal class OAuthToken
{
    [DataMember(Name = "token")]
    public string? Token { get; set; }

    [DataMember(Name = "access_token")]
    public string? AccessToken { get; set; }

    [DataMember(Name = "expires_in")]
    public int ExpiresIn { get; set; }

    [DataMember(Name = "issued_at")]
    public DateTime IssuedAt { get; set; }
}