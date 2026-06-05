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

        public UsuariosController(AppDbContext context)
        {
            _context = context;
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

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
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