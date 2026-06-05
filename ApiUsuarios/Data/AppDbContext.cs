using ApiUsuarios.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiUsuarios.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NombreUsuario)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
