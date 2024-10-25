using Microsoft.EntityFrameworkCore;
using Gestion_Prestamos.Models;
using System.Threading.Tasks;

namespace Gestion_Prestamos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Login> gep_login { get; set; }
        public DbSet<Rol> gep_rol { get; set; }
        public DbSet<Cliente> gep_clientes { get; set; }
        public DbSet<Usuario> gep_usuario { get; set; }

        public DbSet<Tasa> gep_tasas { get; set; }
        public DbSet<Prestamo> gep_prestamo { get; set; }
        public DbSet<CancelacionAnticipadaPrestamo> gep_cancelacion_anticipada_prestamo { get; set; }
        public DbSet<PagosPrestamos> gep_pagos_prestamos { get; set; }
        public DbSet<Bitacora> gep_bitacora_de_prestamos { get; set; }
        public DbSet<Transaccion> gep_transaccion { get; set; }
        public DbSet<Garantia> gep_garantia { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar índices únicos para usr_email y usr_login
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.usr_email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.usr_login)
                .IsUnique();
        }
    }
}
