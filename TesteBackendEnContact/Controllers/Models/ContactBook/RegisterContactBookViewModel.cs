using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TesteBackendEnContact.Controllers.Models.ContactBook
{
    public class RegisterContactBookViewModel
    {
        [Required(ErrorMessage = "Name required.")]
        [StringLength(50, ErrorMessage = "The name must have a maximum of 100 characters.")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}