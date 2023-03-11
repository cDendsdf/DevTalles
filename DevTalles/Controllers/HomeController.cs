using DevTalles.Data;
using DevTalles.Models;
using DevTalles.Models.ViewModels;
using DevTalles.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace DevTalles.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            this.db = db;
        }


        public IActionResult Index()
        {
            var ordenDetalle = db.OrdenDetalles.ToList();

            var claimUser = (ClaimsIdentity)User.Identity;
            var claim = claimUser.FindFirst((ClaimTypes.NameIdentifier));
            if (ordenDetalle.Count() != 0)
            {
              


                if (claim != null)
                {
                     ordenDetalle = db.OrdenDetalles.Where(c => c.UsuarioId == claim.Value).ToList();
                    HomeVM model = new()
                    {
                        Categorias = db.Categorias.ToList(),
                        CursosDisponibles = db.Cursos.Any(),
                        Cursos = db.Cursos.Include(c => c.Categoria).Include(c => c.SubCategoria)
                    };

                    foreach (var item in ordenDetalle)
                    {
                        model.Cursos = model.Cursos.Where(c => c.Id != item.CursoID);
                    }

                    return View(model);
                }
                else
                {
                    HomeVM model = new()
                    {
                        Cursos = db.Cursos.Include(c => c.Categoria).Include(c => c.SubCategoria),
                        Categorias = db.Categorias.ToList(),
                        CursosDisponibles = db.Cursos.Any()
                    };

                    return View(model);
                }
             

               
            }
            else
            {
                HomeVM model = new()
                {
                    Cursos = db.Cursos.Include(c => c.Categoria).Include(c => c.SubCategoria),
                    Categorias = db.Categorias.ToList(),
                    CursosDisponibles = db.Cursos.Any()
                };

                return View(model);
            }
            }


        public IActionResult UsuarioCurso()
        {


            var claimUser = (ClaimsIdentity)User.Identity;
            var claim = claimUser.FindFirst((ClaimTypes.NameIdentifier));


            CursoUsuarioVM model = new()
            {
                ListaCurso = db.OrdenDetalles.Include(c => c.Curso).Where(Us => Us.UsuarioId == claim.Value),
                Categorias = db.Categorias,

            };
            return View(model);
        }




        // se encarga de mostrar la informacion del Curso que se quiere agregar al carro de  compras
        //vali da primero si no existe este Curso en el carro si ya existe nos da la opcion para remover del carro  
        public IActionResult Detalle(int id)
        {
            List<CarroCompra> listaCompra = new List<CarroCompra>();

            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(@WC.VariableSession) != null
                && HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.VariableSession).Count() > 0)
            {
                listaCompra = HttpContext.Session.Get<List<CarroCompra>>(@WC.VariableSession);
            }


            DetalleProductoVM model = new()
            {
                curso = db.Cursos.Include(c => c.Categoria).Include(sb => sb.SubCategoria).Where(c => c.Id == id).FirstOrDefault(),
                ExisteEnCarro = false

            };

            foreach (var item in listaCompra)
            {
                if (item.CursoId == id)
                {
                    model.ExisteEnCarro = true;
                }
            }
            return View(model);
        }

        public IActionResult DetalleCursoUsuario(int id)
        {
            List<CarroCompra> listaCompra = new List<CarroCompra>();

            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(@WC.VariableSession) != null
                && HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.VariableSession).Count() > 0)
            {
                listaCompra = HttpContext.Session.Get<List<CarroCompra>>(@WC.VariableSession);
            }


            DetalleProductoVM model = new()
            {
                curso = db.Cursos.Include(c => c.Categoria).Include(sb => sb.SubCategoria).Where(c => c.Id == id).FirstOrDefault(),
                ExisteEnCarro = false

            };

            foreach (var item in listaCompra)
            {
                if (item.CursoId == id)
                {
                    model.ExisteEnCarro = true;
                }
            }
            return View(model);
        }



        //aqui se pone la logica de agregar una sessio que guarde los producto en el carro de compras


        [Authorize]

        [HttpPost, ActionName("Detalle")]
        public IActionResult DetallePost(int id)
        { //se crea una lista de objetos CarroCompra para almacenar los cursos. Luego, se verifica si hay elementos en la sesión,
          //y si hay, se asignan a la lista. Por último, se agrega el nuevo curso a la lista y se guarda en la sesión. 

            List<CarroCompra> listaCompra = new List<CarroCompra>();

            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(@WC.VariableSession) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(@WC.VariableSession).Count() > 0)
            {
                listaCompra = HttpContext.Session.Get<List<CarroCompra>>(@WC.VariableSession);
            }
            listaCompra.Add(new CarroCompra { CursoId = id });
            HttpContext.Session.Set(WC.VariableSession, listaCompra);

            return RedirectToAction("Index");
        }


        public IActionResult RemoverDeCarro(int id)
        { //e crea una lista de objetos CarroCompra para almacenar los cursos. Luego, se verifica si hay elementos en la sesión,
          //y si hay, se asignan a la lista. Por último, se Elimina el  curso de la lista y se Actualiza en la sesión. 

            List<CarroCompra> listaCompra = new List<CarroCompra>();

            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(@WC.VariableSession) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(@WC.VariableSession).Count() > 0)
            {
                listaCompra = HttpContext.Session.Get<List<CarroCompra>>(@WC.VariableSession);
            }

            var CursoRemover = listaCompra.SingleOrDefault(c => c.CursoId == id);

            listaCompra.Remove(CursoRemover);

            HttpContext.Session.Set(WC.VariableSession, listaCompra);

            return RedirectToAction("Index");
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}