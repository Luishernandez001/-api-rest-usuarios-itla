namespace ApiUsuarios.Models
{
    public class UsuarioLog
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public DateTime FechaDeNacimiento { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
