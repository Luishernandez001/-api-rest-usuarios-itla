using ApiUsuarios.Data;
using ApiUsuarios.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiUsuarios.Controllers
{
    [Authorize]
    [Route("api/productos")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // ── CRUD ────────────────────────────────────────────────────────────────

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .ToListAsync();
        }

        // GET: api/productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado." });

            return producto;
        }

        // POST: api/productos
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == producto.IdCategoria);
            if (!categoriaExiste)
                return BadRequest(new { mensaje = "La categoría especificada no existe." });

            var proveedorExiste = await _context.Proveedores.AnyAsync(p => p.Id == producto.IdProveedor);
            if (!proveedorExiste)
                return BadRequest(new { mensaje = "El proveedor especificado no existe." });

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }

        // PUT: api/productos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.Id)
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del producto." });

            var existe = await _context.Productos.AnyAsync(p => p.Id == id);
            if (!existe)
                return NotFound(new { mensaje = "Producto no encontrado." });

            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == producto.IdCategoria);
            if (!categoriaExiste)
                return BadRequest(new { mensaje = "La categoría especificada no existe." });

            var proveedorExiste = await _context.Proveedores.AnyAsync(p => p.Id == producto.IdProveedor);
            if (!proveedorExiste)
                return BadRequest(new { mensaje = "El proveedor especificado no existe." });

            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado." });

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ── AGREGACIÓN ──────────────────────────────────────────────────────────

        // GET: api/productos/estadisticas
        // Devuelve: precio más alto, precio más bajo, suma total y precio promedio
        [HttpGet("estadisticas")]
        public async Task<IActionResult> GetEstadisticas()
        {
            if (!await _context.Productos.AnyAsync())
                return NotFound(new { mensaje = "No hay productos registrados." });

            var productoMasCaro = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .OrderByDescending(p => p.Precio)
                .FirstAsync();

            var productoMasBarato = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .OrderBy(p => p.Precio)
                .FirstAsync();

            var sumaTotal = await _context.Productos.SumAsync(p => p.Precio);
            var promedio = await _context.Productos.AverageAsync(p => p.Precio);

            return Ok(new
            {
                productoMasCaro,
                productoMasBarato,
                sumaTotal,
                precioPromedio = Math.Round(promedio, 2)
            });
        }

        // ── CONSULTAS ADICIONALES ───────────────────────────────────────────────

        // GET: api/productos/categoria/3
        [HttpGet("categoria/{idCategoria}")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetPorCategoria(int idCategoria)
        {
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == idCategoria);
            if (!categoriaExiste)
                return NotFound(new { mensaje = "Categoría no encontrada." });

            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p => p.IdCategoria == idCategoria)
                .ToListAsync();

            return Ok(productos);
        }

        // GET: api/productos/proveedor/2
        [HttpGet("proveedor/{idProveedor}")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetPorProveedor(int idProveedor)
        {
            var proveedorExiste = await _context.Proveedores.AnyAsync(p => p.Id == idProveedor);
            if (!proveedorExiste)
                return NotFound(new { mensaje = "Proveedor no encontrado." });

            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p => p.IdProveedor == idProveedor)
                .ToListAsync();

            return Ok(productos);
        }

        // GET: api/productos/total
        [HttpGet("total")]
        public async Task<IActionResult> GetTotal()
        {
            var total = await _context.Productos.CountAsync();
            return Ok(new { totalProductos = total });
        }
    }
}
