using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiUsuarios.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [MinLength(3, ErrorMessage = "El nombre de usuario debe tener al menos 3 caracteres.")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede superar los 50 caracteres.")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        [MaxLength(150, ErrorMessage = "El correo electrónico no puede superar los 150 caracteres.")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime FechaDeNacimiento { get; set; }

        [JsonIgnore]
        public string Password { get; set; } = string.Empty;
    }
}
