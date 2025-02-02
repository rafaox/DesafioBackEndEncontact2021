using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TesteBackendEnContact.Controllers.Models.Auth
{
    public class AuthenticateRequestViewModel
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado está em um formato inválido.")]
        [StringLength(300, ErrorMessage = "O e-mail deve ter no máximo 300 caracteres.")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}