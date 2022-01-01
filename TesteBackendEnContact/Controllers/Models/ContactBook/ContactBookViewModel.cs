using System.Text.Json.Serialization;

namespace TesteBackendEnContact.Controllers.Models.ContactBook
{
    public class ContactBookViewModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}