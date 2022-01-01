using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TesteBackendEnContact.Controllers.Models.Company
{
    public class RegisterCompanyViewModel
    {
        [Required(ErrorMessage = "Contact book id is required.")]
        [JsonPropertyName("contact_book_id")]
        public int ContactBookId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "The name must have a maximum of 100 characters.")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}