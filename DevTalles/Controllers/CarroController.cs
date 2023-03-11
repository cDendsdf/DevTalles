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

            List<Curso> curso = dbContext.Cursos.Where(c => idsCarroCompra.Contains(c.Id)).ToList();


            return View(curso);
        }


        //Este codigo busca el usuario conectado a la aplicacion y los productos seleccionados
        //en la sesion para luego llenar el viewModel UsuarioProductoVM con los datos del usuario
        //y los cursos seleccionados para mostrarlos en la vista.
        public IActionResult Resumen()
        {

            UsuarioAplicacion usuario;
            if (User.IsInRole(WC.AdminRole))
            {

                //si el usuario es admin y desea actualizar una orden existente
                if (HttpContext.Session.Get<int>(WC.SessionOrdenId) != 0)
                {
                    //obtenemos la orden a actualizar
                    Orden orden = dbContext.Ordenes.Find(HttpContext.Session.Get<int>(WC.SessionOrdenId));


                    //obtenemos el usuario que pertenece ala orden
                    usuario = new UsuarioAplicacion()
                    {
                        Email = orden.Email,
                        NombreCompleto = orden.NombreCompleto,
                        PhoneNumber = orden.Telefono
                    };
                }
                else
                { //si no le damos la opcion al admin que ingrese al usuario
                    usuario = new UsuarioAplicacion();
                }
            }
            else
            {
                //traer usuario conectado ala aplicacion

                var clainUsers = (ClaimsIdentity)User.Identity;
                var clain = clainUsers.FindFirst(ClaimTypes.NameIdentifier);

                usuario = dbContext.UsuariosAplicacion.FirstOrDefault(us => us.Id == clain.Value);
            }


           



            List<CarroCompra> listaCompras = new();

            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.VariableSession) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.VariableSession).Count() > 0)
            {
                listaCompras = HttpContext.Session.Get<List<CarroCompra>>(WC.VariableSession);
            }

            List<int> idsCarroCompra = listaCompras.Select(c => c.CursoId).ToList();



            usuarioProductoVM = new UsuarioProductoVM()
            {
                UsuarioAplicacion = usuario,
                CursosLista = dbContext.Cursos.Where(c => idsCarroCompra.Contains(c.Id)).ToList()

            };

            return View(usuarioProductoVM);
        }


       //Una vez confirmada la orden se guarda la orden y el detalle de la orden en
       //la base de datos 



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Resumen")]
        public IActionResult ResumenPost(UsuarioProductoVM usuarioProductoVM)
        {
          //Obtenemos el usuario autenticado
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
                    UsuarioId = Claim.Value
                    
                    
                };
                dbContext.OrdenDetalles.Add(ordenDetalle);
            }

            dbContext.SaveChanges();

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");


        }




        //Esta se encarga de eliminar los productos que estan en la vista carro de compras

        public IActionResult Eliminar(int id)
        {
            List<CarroCompra> listCarro = new();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.VariableSession) != null && HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.VariableSession).Count() > 0)
            {
                listCarro = HttpContext.Session.Get<List<CarroCompra>>(WC.VariableSession);
            }

            listCarro.Remove(listCarro.FirstOrDefault(c => c.CursoId == id));
            HttpContext.Session.Set(WC.VariableSession, listCarro);

            return RedirectToAction("Index");
        }
    }
}
