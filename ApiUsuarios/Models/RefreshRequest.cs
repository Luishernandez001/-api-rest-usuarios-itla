using System.ComponentModel.DataAnnotations;

namespace ApiUsuarios.Models
{
    public class RefreshRequest
    {
        [Required(ErrorMessage = "El refresh token es obligatorio.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
