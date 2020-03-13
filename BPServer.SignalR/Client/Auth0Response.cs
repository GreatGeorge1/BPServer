using RestSharp.Deserializers;

namespace BPServer.SignalR
{
    public class Auth0Response
    {
        [DeserializeAs(Name = "access_token")]
        public string AccessToken { get; set; }
        [DeserializeAs(Name = "token_type")]
        public string TokenType { get; set; }
    }
}
