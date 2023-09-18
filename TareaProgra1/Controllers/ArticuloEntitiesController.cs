using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TareaProgra1.Data;
using TareaProgra1.Models;
using Microsoft.Data.SqlClient;

namespace TareaProgra1.Controllers
{
    public class ArticuloEntitiesController : Controller
    {
        private readonly BDContext _context;

        public ArticuloEntitiesController(BDContext context)
        {
            _context = context;
        }

        // GET: ArticuloEntities
        public async Task<IActionResult> Index()
        {
            var articulosOrdenados = _context.Articulo.FromSqlRaw("EXEC GetAllArticles");
            return _context.Articulo != null ?
                        View(articulosOrdenados) :
                        Problem("Entity set 'BDContext.Articulo'  is null.");
        }

        // GET: ArticuloEntities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Articulo == null)
            {
                return NotFound();
            }

            var articuloEntity = await _context.Articulo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (articuloEntity == null)
            {
                return NotFound();
            }

            return View(articuloEntity);
        }

        // GET: ArticuloEntities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ArticuloEntities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nombre,IdClaseArticulo,Precio,EsActivo")] ArticuloEntity articuloEntity)
        {
            if (ModelState.IsValid)
            {
                SqlConnection conn = (SqlConnection)_context.Database.GetDbConnection();
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "InsertarArticulo";
               // cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = articuloEntity.Id;
                cmd.Parameters.Add("@Codigo", System.Data.SqlDbType.VarChar, 32).Value = articuloEntity.Codigo;
                cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.VarChar, 128).Value = articuloEntity.Nombre;
                cmd.Parameters.Add("@idClaseArticulo", System.Data.SqlDbType.Int).Value = articuloEntity.IdClaseArticulo;
                //cmd.Parameters.Add("@nombreClaseArticulo", System.Data.SqlDbType.VarChar, 128).Value = articuloEntity.NombreClaseArticulo;
                cmd.Parameters.Add("@Precio", System.Data.SqlDbType.Decimal).Value = articuloEntity.Precio;
                cmd.Parameters.Add("@EsActivo", System.Data.SqlDbType.Bit).Value = articuloEntity.EsActivo;
                cmd.ExecuteNonQuery();

                conn.Close(); 
               // _context.Add(articuloEntity);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(articuloEntity);
        }

        // GET: ArticuloEntities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Articulo == null)
            {
                return NotFound();
            }

            var articuloEntity = await _context.Articulo.FindAsync(id);
            if (articuloEntity == null)
            {
                return NotFound();
            }
            return View(articuloEntity);
        }

        // POST: ArticuloEntities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Precio")] ArticuloEntity articuloEntity)
        {
            if (id != articuloEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(articuloEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticuloEntityExists(articuloEntity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(articuloEntity);
        }

        // GET: ArticuloEntities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Articulo == null)
            {
                return NotFound();
            }

            var articuloEntity = await _context.Articulo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (articuloEntity == null)
            {
                return NotFound();
            }

            return View(articuloEntity);
        }

        // POST: ArticuloEntities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Articulo == null)
            {
                return Problem("Entity set 'BDContext.Articulo'  is null.");
            }
            var articuloEntity = await _context.Articulo.FindAsync(id);
            if (articuloEntity != null)
            {
                _context.Articulo.Remove(articuloEntity);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticuloEntityExists(int id)
        {
          return (_context.Articulo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
