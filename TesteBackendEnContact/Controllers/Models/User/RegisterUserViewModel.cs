using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TesteBackendEnContact.Controllers.Models.User
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "E-mail is required.")]
        [EmailAddress(ErrorMessage = "Invalid e-mail.")]
        [StringLength(100, ErrorMessage = "The e-mail must have a maximum of 100 characters.")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}