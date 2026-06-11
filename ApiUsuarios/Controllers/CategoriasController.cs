using ApiUsuarios.Data;
using ApiUsuarios.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiUsuarios.Controllers
{
    [Authorize]
    [Route("api/categorias")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            return await _context.Categorias.ToListAsync();
        }

        // GET: api/categorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound(new { mensaje = "Categoría no encontrada." });

            return categoria;
        }

        // POST: api/categorias
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
        {
            var existe = await _context.Categorias
                .AnyAsync(c => c.Nombre == categoria.Nombre);

            if (existe)
                return BadRequest(new { mensaje = "Ya existe una categoría con ese nombre." });

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoria);
        }

        // PUT: api/categorias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
        {
            if (id != categoria.Id)
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID de la categoría." });

            var existe = await _context.Categorias.AnyAsync(c => c.Id == id);
            if (!existe)
                return NotFound(new { mensaje = "Categoría no encontrada." });

            var nombreDuplicado = await _context.Categorias
                .AnyAsync(c => c.Nombre == categoria.Nombre && c.Id != id);

            if (nombreDuplicado)
                return BadRequest(new { mensaje = "Ya existe otra categoría con ese nombre." });

            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound(new { mensaje = "Categoría no encontrada." });

            var tieneProductos = await _context.Productos.AnyAsync(p => p.IdCategoria == id);
            if (tieneProductos)
                return BadRequest(new { mensaje = "No se puede eliminar la categoría porque tiene productos asociados." });

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
