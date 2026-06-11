using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiUsuarios.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres.")]
        [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El proveedor es obligatorio.")]
        public int IdProveedor { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public int IdCategoria { get; set; }

        public Proveedor? Proveedor { get; set; }
        public Categoria? Categoria { get; set; }
    }
}
