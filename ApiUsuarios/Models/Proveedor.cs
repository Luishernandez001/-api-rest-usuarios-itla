using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiUsuarios.Models
{
    public class Proveedor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El contacto del proveedor es obligatorio.")]
        [MaxLength(150, ErrorMessage = "El contacto no puede superar los 150 caracteres.")]
        public string Contacto { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
