using DevTalles.Data;
using DevTalles.Models;
using DevTalles.Models.ViewModels;
using DevTalles.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevTalles.Controllers
{
    [Authorize]
    public class CarroController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        [BindProperty]//Para utilizar esta variable en todo el controller y no perder sus datos
        public UsuarioProductoVM usuarioProductoVM { get; set; }    

        public CarroController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        //Este controller se encarga de obtener los cursos que se encuentran en el carrito de compras almacenado en la sesión del usuario, para devolverlos a la vista Index.

        public IActionResult Index()
        {

            List<CarroCompra> listaCompras = new();

            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.VariableSession) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.VariableSession).Count() > 0)
            {
                listaCompras = HttpContext.Session.Get<List<CarroCompra>>(WC.VariableSession);
            }

            List<int> idsCarroCompra = listaCompras.Select(c => c.CursoId).ToList();

            IEnumerable<Curso> curso = dbContext.Cursos.Where(c => idsCarroCompra.Contains(c.Id));


            return View(curso);
        }


        //Este codigo busca el usuario conectado a la aplicacion y los productos seleccionados
        //en la sesion para luego llenar el viewModel UsuarioProductoVM con los datos del usuario
        //y los cursos seleccionados para mostrarlos en la vista.
        public IActionResult Resumen()
        {
            //traer usuario conectado ala aplicacion

            var clainUsers = (ClaimsIdentity)User.Identity;
            var clain = clainUsers.FindFirst(ClaimTypes.NameIdentifier);



            List<CarroCompra> listaCompras = new();

            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.VariableSession) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.VariableSession).Count() > 0)
            {
                listaCompras = HttpContext.Session.Get<List<CarroCompra>>(WC.VariableSession);
            }

            List<int> idsCarroCompra = listaCompras.Select(c => c.CursoId).ToList();

           

            usuarioProductoVM = new UsuarioProductoVM()
            {
                UsuarioAplicacion = dbContext.UsuariosAplicacion.FirstOrDefault(us => us.Id == clain.Value),
                CursosLista = dbContext.Cursos.Where(c => idsCarroCompra.Contains(c.Id)).ToList(),
                
            };

            return View(usuarioProductoVM);
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Resumen")]
        public IActionResult ResumenPost(UsuarioProductoVM usuarioProductoVM)
        {
            

            var ClaimUser = (ClaimsIdentity)User.Identity;
            var Claim = ClaimUser.FindFirst(ClaimTypes.NameIdentifier);

            //Gurdamos la Orden Con la informacion del usuario
            Orden ordenCurso = new Orden()

            {
                UsuarioAplicacionId = Claim.Value,
                NombreCompleto = usuarioProductoVM.UsuarioAplicacion.NombreCompleto,
                Email = usuarioProductoVM.UsuarioAplicacion.Email,
                FechaOrden = DateTime.Now,
                Telefono = usuarioProductoVM.UsuarioAplicacion.PhoneNumber
            };

            dbContext.Ordenes.Add(ordenCurso);
             dbContext.SaveChanges();

            //Grabamos la orden con la data relacionada entre orden y producto

            foreach (var curso in usuarioProductoVM.CursosLista)
            {
                OrdenDetalle ordenDetalle = new OrdenDetalle()
                {
                    OrdenId = ordenCurso.Id,
                    CursoID = curso.Id,
                };
                dbContext.OrdenDetalles.Add(ordenDetalle);
            }
           
            dbContext.SaveChanges();

            HttpContext.Session.Clear();

            return RedirectToAction("Index","Home");
            
            
        }

      



        [HttpDelete]
        public IActionResult Eliminar(int id)
        {
            List<CarroCompra> listCarro = new();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.VariableSession) != null && HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.VariableSession).Count()>0)
            {
                listCarro = HttpContext.Session.Get<List<CarroCompra>>(WC.VariableSession);
            }

            listCarro.Remove(listCarro.FirstOrDefault(c => c.CursoId == id));
            HttpContext.Session.Set(WC.VariableSession,listCarro);

            return RedirectToAction("Index");
        }
    }
}
