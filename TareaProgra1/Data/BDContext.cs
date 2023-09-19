using System;
using Microsoft.EntityFrameworkCore;
using TareaProgra1.Models;


namespace TareaProgra1.Data
{
    public class BDContext:DbContext
    {
        public BDContext(DbContextOptions<BDContext> options):base(options)
        {
                
        }

        public DbSet<ArticuloEntity> Articulo { get; set; }

        public DbSet<ClaseArticuloEntity> ClaseArticulo { get; set; }
        
    }
}
