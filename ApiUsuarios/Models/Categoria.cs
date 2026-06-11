using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiUsuarios.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
