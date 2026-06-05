using System.ComponentModel.DataAnnotations;

namespace ApiUsuarios.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede superar los 50 caracteres.")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede superar los 100 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}
