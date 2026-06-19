using ApiUsuarios.Models;
using System.Text.Json;

namespace ApiUsuarios.Services
{
    public class LogService
    {
        private readonly string _logFilePath;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public LogService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            var logFileName = configuration["Log:FileName"] ?? "usuarios_log.json";
            _logFilePath = Path.Combine(environment.ContentRootPath, logFileName);
        }

        public async Task RegistrarLogAsync(Usuario usuario)
        {
            var entry = new UsuarioLog
            {
                Id = usuario.Id,
                NombreUsuario = usuario.NombreUsuario,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                FechaDeNacimiento = usuario.FechaDeNacimiento,
                FechaRegistro = DateTime.Now
            };

            List<UsuarioLog> logs = await LeerLogsDesdeArchivoAsync();
            logs.Add(entry);

            var json = JsonSerializer.Serialize(logs, _jsonOptions);
            await File.WriteAllTextAsync(_logFilePath, json);
        }

        public async Task<List<UsuarioLog>> ObtenerLogsAsync()
        {
            return await LeerLogsDesdeArchivoAsync();
        }

        private async Task<List<UsuarioLog>> LeerLogsDesdeArchivoAsync()
        {
            if (!File.Exists(_logFilePath))
                return new List<UsuarioLog>();

            try
            {
                var json = await File.ReadAllTextAsync(_logFilePath);

                if (string.IsNullOrWhiteSpace(json))
                    return new List<UsuarioLog>();

                return JsonSerializer.Deserialize<List<UsuarioLog>>(json, _jsonOptions)
                       ?? new List<UsuarioLog>();
            }
            catch
            {
                return new List<UsuarioLog>();
            }
        }
    }
}
