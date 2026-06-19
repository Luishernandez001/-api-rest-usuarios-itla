using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiUsuarios.Data;
using ApiUsuarios.Models;
using ApiUsuarios.Services;

namespace ApiUsuarios.Controllers
{
    [Authorize]
    [Route("api/usuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public UsuariosController(AppDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            return usuario;
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            var existeCorreo = await _context.Usuarios
                .AnyAsync(u => u.Correo == usuario.Correo);

            if (existeCorreo)
                return BadRequest(new { mensaje = "El correo electrónico ya está en uso." });

            var existeNombreUsuario = await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario);

            if (existeNombreUsuario)
                return BadRequest(new { mensaje = "El nombre de usuario ya está en uso." });

            usuario.Password = TokenService.HashPassword(usuario.Password);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            try
            {
                await _logService.RegistrarLogAsync(usuario);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al escribir el log del usuario: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        // GET: api/usuarios/logs
        [HttpGet("logs")]
        public async Task<ActionResult> GetLogs()
        {
            try
            {
                var logs = await _logService.ObtenerLogsAsync();

                if (logs.Count == 0)
                    return Ok(new { mensaje = "No hay registros en el historial de logs.", datos = logs });

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al leer el archivo de logs.", detalle = ex.Message });
            }
        }

        // PUT: api/usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del usuario." });

            var usuarioExistente = await _context.Usuarios.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuarioExistente == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            var existeOtroCorreo = await _context.Usuarios
                .AnyAsync(u => u.Correo == usuario.Correo && u.Id != id);

            if (existeOtroCorreo)
                return BadRequest(new { mensaje = "El correo electrónico ya está en uso por otro usuario." });

            var existeOtroNombreUsuario = await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario && u.Id != id);

            if (existeOtroNombreUsuario)
                return BadRequest(new { mensaje = "El nombre de usuario ya está en uso por otro usuario." });

            usuario.Password = string.IsNullOrWhiteSpace(usuario.Password)
                ? usuarioExistente.Password
                : TokenService.HashPassword(usuario.Password);

            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}