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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace TareaProgra1.Controllers
    
{
 
    public class ArticuloEntitiesController : Controller
    {
        private readonly BDContext _context;

        public ArticuloEntitiesController(BDContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Cuenta");

        }

        // GET: ArticuloEntities
        public async Task<IActionResult> Index()
        {
            var articulosOrdenados = await _context.Articulo.FromSqlRaw("EXEC GetAllArticles").ToListAsync();
            var claseArticulos = await _context.ClaseArticulo.FromSqlRaw("EXEC GetAllClassArticles").ToListAsync();

            var vistaLista = new List<Tablas>();

            foreach (var articulo in articulosOrdenados)
            {
                var vistaModel = new Tablas
                {
                    Id = articulo.Id,
                    Codigo = articulo.Codigo,
                    Nombre = articulo.Nombre,
                    IdClaseArticulo = articulo.IdClaseArticulo,
                    Precio = articulo.Precio,
                    EsActivo = articulo.EsActivo,
                    NombreClaseArticulo = claseArticulos.FirstOrDefault(ca => ca.Id == articulo.IdClaseArticulo)?.Nombre
                };
                vistaLista.Add(vistaModel);
            }

            ViewBag.claseArticulo = claseArticulos;
            return _context.Articulo != null ?
                        View(vistaLista) :
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
        /*
        public async Task<IActionResult> Edit(string? codigo)
        {
            if (codigo == null || _context.Articulo == null)
            {
                return NotFound();
            }
            var articuloEntity = await _context.Articulo.FirstOrDefaultAsync(m => m.Codigo == codigo);
            //var articuloEntity = await _context.Articulo.FindAsync(codigo);
            if (articuloEntity == null)
            {
                return NotFound();
            }
            return View(articuloEntity);
        }
        */

        public IActionResult Edit()
        {
            return View();
        }

        // POST: ArticuloEntities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Codigo,Nombre,NombreClaseArticulo,Precio")] ArticuloEntity articuloEntity)
        {
            if (string.IsNullOrEmpty(articuloEntity.Codigo) || _context.Articulo == null)
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
        /*
        public async Task<IActionResult> Delete(string? codigo)
        {
            if (codigo == null || _context.Articulo == null)
            {
                return NotFound();
            }

            var articuloEntity = await _context.Articulo.FirstOrDefaultAsync(m => m.Codigo == codigo);
            //var articuloEntity = await _context.Articulo.FirstOrDefaultAsync(m => m.Id == id);

            if (articuloEntity == null)
            {
                return NotFound();
            }

            return View(articuloEntity);
        }
        */

        public IActionResult Delete()
        {
            return View();
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
            var Tablas = await _context.Articulo.FindAsync(id);
            if (Tablas != null)
            {
                //_context.Articulo.Remove(articuloEntity);
                Tablas.EsActivo = false;
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticuloEntityExists(int id)
        {
          return (_context.Articulo?.Any(e => e.Id == id)).GetValueOrDefault();
        }



        //FILTRADOS FILTRADOS FILTRADOS
        public async Task<IActionResult> filtrarPorNombre(string strPorBuscar)
        {
                var strParametro = new SqlParameter("@str", strPorBuscar);

                var articulosOrdenados = await _context.Articulo.FromSqlRaw("EXEC GetStrArticles @str", strParametro).ToListAsync();
                var claseArticulos = await _context.ClaseArticulo.FromSqlRaw("EXEC GetAllClassArticles").ToListAsync();

                var vistaLista = new List<Tablas>();

                foreach (var articulo in articulosOrdenados)
                {
                    var vistaModel = new Tablas
                    {
                        Id = articulo.Id,
                        Codigo = articulo.Codigo,
                        Nombre = articulo.Nombre,
                        IdClaseArticulo = articulo.IdClaseArticulo,
                        Precio = articulo.Precio,
                        EsActivo = articulo.EsActivo,
                        NombreClaseArticulo = claseArticulos.FirstOrDefault(ca => ca.Id == articulo.IdClaseArticulo)?.Nombre
                    };
                    vistaLista.Add(vistaModel);
                }

            return PartialView("TablaParcial", vistaLista);
        }

        public async Task<IActionResult> filtrarPorCantidad(string cantidadPorBuscar)
        {
            var strParametro = new SqlParameter("@str", cantidadPorBuscar);

            var articulosOrdenados = await _context.Articulo.FromSqlRaw("EXEC GetQuantityArticles @str", strParametro).ToListAsync();
            var claseArticulos = await _context.ClaseArticulo.FromSqlRaw("EXEC GetAllClassArticles").ToListAsync();

            var vistaLista = new List<Tablas>();

            foreach (var articulo in articulosOrdenados)
            {
                var vistaModel = new Tablas
                {
                    Id = articulo.Id,
                    Codigo = articulo.Codigo,
                    Nombre = articulo.Nombre,
                    IdClaseArticulo = articulo.IdClaseArticulo,
                    Precio = articulo.Precio,
                    EsActivo = articulo.EsActivo,
                    NombreClaseArticulo = claseArticulos.FirstOrDefault(ca => ca.Id == articulo.IdClaseArticulo)?.Nombre
                };
                vistaLista.Add(vistaModel);
            }

            return PartialView("TablaParcial", vistaLista);
        }

        public async Task<IActionResult> filtrarPorClaseArticulo(string claseArticuloPorBuscar)
        {
            var strParametro = new SqlParameter("@str", claseArticuloPorBuscar);

            var articulosOrdenados = await _context.Articulo.FromSqlRaw("EXEC GetClassArticles @str", strParametro).ToListAsync();
            var claseArticulos = await _context.ClaseArticulo.FromSqlRaw("EXEC GetAllClassArticles").ToListAsync();

            var vistaLista = new List<Tablas>();

            foreach (var articulo in articulosOrdenados)
            {
                var vistaModel = new Tablas
                {
                    Id = articulo.Id,
                    Codigo = articulo.Codigo,
                    Nombre = articulo.Nombre,
                    IdClaseArticulo = articulo.IdClaseArticulo,
                    Precio = articulo.Precio,
                    EsActivo = articulo.EsActivo,
                    NombreClaseArticulo = claseArticulos.FirstOrDefault(ca => ca.Id == articulo.IdClaseArticulo)?.Nombre
                };
                vistaLista.Add(vistaModel);
            }

            return PartialView("TablaParcial", vistaLista);
        }

        public async Task<IActionResult> filtrarPorCodigoArticulo(string strPorBuscar)
        {
            var strParametro = new SqlParameter("@str", strPorBuscar);

            var articulosOrdenados = await _context.Articulo.FromSqlRaw("EXEC GetArticleByCode @str", strParametro).ToListAsync();
            var claseArticulos = await _context.ClaseArticulo.FromSqlRaw("EXEC GetAllClassArticles").ToListAsync();

            var vistaLista = new List<Tablas>();

            foreach (var articulo in articulosOrdenados)
            {
                var vistaModel = new Tablas
                {
                    Id = articulo.Id,
                    Codigo = articulo.Codigo,
                    Nombre = articulo.Nombre,
                    IdClaseArticulo = articulo.IdClaseArticulo,
                    Precio = articulo.Precio,
                    EsActivo = articulo.EsActivo,
                    NombreClaseArticulo = claseArticulos.FirstOrDefault(ca => ca.Id == articulo.IdClaseArticulo)?.Nombre
                };
                vistaLista.Add(vistaModel);
            }

            return PartialView("TablaEditarEliminar", vistaLista);
        }

        public IActionResult FiltradoPorNombre()
        {
            return View();
        }



    }
}
