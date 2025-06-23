using Microsoft.EntityFrameworkCore;
using PersonajeWebAPI.Models;

namespace PersonajeWebAPI.DataAccess
{
    public class PersonajeContext : DbContext
    {
        public DbSet<Personaje> Personajes { get; set; }

        public PersonajeContext(DbContextOptions<PersonajeContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Personaje>(entity =>
            {
                entity.ToTable("PersonajesEF"); // Tabla separada para EF
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Clase).HasMaxLength(50);
                entity.Property(e => e.Nivel).IsRequired();
                entity.Property(e => e.Vida).IsRequired();
                entity.Property(e => e.FechaCreacion).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}