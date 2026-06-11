using ApiUsuarios.Data;
using ApiUsuarios.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiUsuarios.Controllers
{
    [Authorize]
    [Route("api/proveedores")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/proveedores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
        {
            return await _context.Proveedores.ToListAsync();
        }

        // GET: api/proveedores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
                return NotFound(new { mensaje = "Proveedor no encontrado." });

            return proveedor;
        }

        // POST: api/proveedores
        [HttpPost]
        public async Task<ActionResult<Proveedor>> PostProveedor(Proveedor proveedor)
        {
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.Id }, proveedor);
        }

        // PUT: api/proveedores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(int id, Proveedor proveedor)
        {
            if (id != proveedor.Id)
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del proveedor." });

            var existe = await _context.Proveedores.AnyAsync(p => p.Id == id);
            if (!existe)
                return NotFound(new { mensaje = "Proveedor no encontrado." });

            _context.Entry(proveedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/proveedores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
                return NotFound(new { mensaje = "Proveedor no encontrado." });

            var tieneProductos = await _context.Productos.AnyAsync(p => p.IdProveedor == id);
            if (tieneProductos)
                return BadRequest(new { mensaje = "No se puede eliminar el proveedor porque tiene productos asociados." });

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
