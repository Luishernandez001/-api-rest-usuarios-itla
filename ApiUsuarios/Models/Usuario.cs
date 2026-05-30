using System.ComponentModel.DataAnnotations;

namespace ApiUsuarios.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public DateTime FechaDeNacimiento { get; set; }
    }
}
