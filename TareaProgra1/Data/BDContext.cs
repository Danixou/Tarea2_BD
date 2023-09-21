using System;
using Microsoft.EntityFrameworkCore;
using TareaProgra1.Models;


namespace TareaProgra1.Data
{
    public class BDContext:DbContext
    {
        public string Conexion { get; }

        public BDContext(string valor)
        {
            Conexion = valor;
        }

        public BDContext(DbContextOptions<BDContext> options):base(options)
        {
                
        }

        public DbSet<ArticuloEntity> Articulo { get; set; }

        public DbSet<ClaseArticuloEntity> ClaseArticulo { get; set; }
        
    }
}
