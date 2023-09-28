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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;
using System.Diagnostics;
using System.Xml;

namespace TareaProgra1.Controllers
    
{
    //[Authorize]
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
            string xmlFilePath = "datos.xml";
            string connectionString = "Server=databasetarea1.ccdblu414uis.us-east-1.rds.amazonaws.com,1433;Database=Tarea2BDI;User ID=admin;Password=bases5181;TrustServerCertificate=true";
            //LEER CLASE ARTICULO
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);

                XmlNodeList claseArticulosXML = xmlDoc.SelectNodes("/root/ClasesdeArticulos/ClasedeArticulos");

                foreach (XmlNode clase in claseArticulosXML)
                {
                    string nombreClaseArticulos = clase.Attributes["Nombre"].Value;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand("InsertarClaseArticulo", connection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.Add("@Nombre", SqlDbType.VarChar, 128).Value = nombreClaseArticulos;
                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR al leer el earchivo XML: " + ex.Message);
            }

            //LEER ARTICULOS
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);

                XmlNodeList claseArticulosXML = xmlDoc.SelectNodes("/root/Articulos/Articulo");

                foreach (XmlNode clase in claseArticulosXML)
                {

                    string codigoArticulo = clase.Attributes["Codigo"].Value;
                    string nombreArticulo = clase.Attributes["Nombre"].Value;
                    string precioString = clase.Attributes["Precio"].Value;
                    decimal precioArticulo = 0;
                    try
                    {
                        precioArticulo = decimal.Parse(precioString);
                        Console.WriteLine("Conversión a: " + precioArticulo);
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("ERROR " + ex.Message);
                    }
                    string nombreClaseArticulo = clase.Attributes["ClasedeArticulos"].Value;
                    int idClaseArticulo;
                    bool activo = true;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand("GetIdClassArticleByName", connection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.Add("@Nombre", SqlDbType.VarChar, 128).Value = nombreClaseArticulo;

                            SqlParameter parametroResultado = new SqlParameter("@Resultado", SqlDbType.Int);
                            parametroResultado.Direction = ParameterDirection.Output;
                            command.Parameters.Add(parametroResultado);

                            command.ExecuteNonQuery();

                            idClaseArticulo = (int)parametroResultado.Value;
                        }
                        using (SqlCommand command2 = new SqlCommand("InsertarArticulo", connection))
                        {
                            command2.CommandType = System.Data.CommandType.StoredProcedure;
                            command2.Parameters.Add("@Nombre", SqlDbType.VarChar, 128).Value = nombreArticulo;
                            command2.Parameters.Add("@Codigo", SqlDbType.VarChar, 32).Value = codigoArticulo;
                            command2.Parameters.Add("@idClaseArticulo", SqlDbType.Int).Value = idClaseArticulo;
                            command2.Parameters.Add("@Precio", SqlDbType.Money).Value = precioArticulo;
                            command2.Parameters.Add("@EsActivo", SqlDbType.Bit).Value = activo;

                            command2.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR al leer el earchivo XML: " + ex.Message);
            }

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
        
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null || _context.Articulo == null)
            {
                return NotFound();
            }
            //var articuloEntity = await _context.Articulo.FirstOrDefaultAsync(m => m.Codigo == codigo);
            var articuloEntity = await _context.Articulo.FindAsync(Id);
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
        
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null || _context.Articulo == null)
            {
                return NotFound();
            }

            //var articuloEntity = await _context.Articulo.FirstOrDefaultAsync(m => m.Codigo == codigo);
            var articuloEntity = await _context.Articulo.FirstOrDefaultAsync(m => m.Id == Id);

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
            string connectionString = "Server=databasetarea1.ccdblu414uis.us-east-1.rds.amazonaws.com,1433;Database=Tarea2BDI;User ID=admin;Password=bases5181;TrustServerCertificate=true";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using(SqlCommand command = new SqlCommand("EliminarArticulo", connection))
                    {   

                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }

            }
            catch (Exception ex) 
            {
                Debug.WriteLine("ERROR: " + ex);
            }

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

            return PartialView("TablaParcial", vistaLista);
        }

        public IActionResult FiltradoPorNombre()
        {
            return View();
        }



    }
}
