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

        public IEnumerable<ArticuloEntity> getOrdenAlfabetico()
        {
            return Set<ArticuloEntity>().OrderBy(a => a.Nombre).ToList();
        }

        public DbSet<ArticuloEntity> Articulo { get; set; }
        
    }
}
