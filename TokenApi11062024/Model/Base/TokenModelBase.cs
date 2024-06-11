using System.Text.Json.Serialization;

namespace TokenApi11062024.Model.Base
{
    public abstract class TokenModelBase
    {
        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }
        [JsonPropertyName("refreshToken")]
        public string? RefreshToken { get; set; }
    }
}
