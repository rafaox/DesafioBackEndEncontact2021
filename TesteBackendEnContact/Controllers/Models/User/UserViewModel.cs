using System.Text.Json.Serialization;

namespace TesteBackendEnContact.Controllers.Models.User
{
    public class UserViewModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}