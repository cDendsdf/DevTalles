using DevTalles.Data;
using DevTalles.Models;
using DevTalles.Models.ViewModels;
using DevTalles.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Threading.Tasks.Dataflow;

namespace DevTalles.Controllers
{
    [Authorize(Roles =WC.AdminRole)]
    public class OrdenesController : Controller
    {
        private readonly ApplicationDbContext db;

        [BindProperty]
        public OrdenVM OrdenViewModel { get; set; }

        public OrdenesController(ApplicationDbContext _db)
        {
            db = _db;
        }


        public IActionResult Index()
        {
            return View();
        }  
        
        public IActionResult Detalle(int id)
        {

            OrdenViewModel = new OrdenVM()
            {
                Orden = db.Ordenes.Find(id),
                OrdenDetalle = db.OrdenDetalles.Include(o => o.Curso).Where(o => o.OrdenId == id).ToList()
            };



            return View(OrdenViewModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Detalle")]
        public IActionResult DetalleOrden()
        {

            


            List<CarroCompra> carroCompras = new();
            OrdenViewModel.OrdenDetalle = db.OrdenDetalles.Where(c => c.OrdenId ==OrdenViewModel.Orden.Id).ToList();

            foreach (var detalle in OrdenViewModel.OrdenDetalle)
            {
                CarroCompra compra = new()
                {
                    CursoId = detalle.CursoID
                };

                carroCompras.Add(compra);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.VariableSession, carroCompras);
            HttpContext.Session.Set(WC.SessionOrdenId, OrdenViewModel.Orden.Id);
            return RedirectToAction("Index", "Carro");

        }


        [HttpPost]
        public IActionResult Eliminar()
        {
            Orden orden = db.Ordenes.FirstOrDefault(o => o.Id == OrdenViewModel.Orden.Id);
            List<OrdenDetalle> ordenDetalle = db.OrdenDetalles.Where(od => od.OrdenId == orden.Id).ToList();


            db.OrdenDetalles.RemoveRange(ordenDetalle);
            db.Ordenes.Remove(orden);
            db.SaveChanges();

            return View("Index");
        }





        #region
        [HttpGet]

        public IActionResult GetAll()
        {
            return Json(new {data = db.Ordenes.ToList()});
        }
        #endregion





    }
}
