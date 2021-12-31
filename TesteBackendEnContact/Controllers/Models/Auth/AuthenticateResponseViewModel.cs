using System.Text.Json.Serialization;

namespace TesteBackendEnContact.Controllers.Models.Auth
{
    public class AuthenticateResponseViewModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("jwt_token")]
        public string JwtToken { get; set; }
    }
}