using DevTalles.Data;
using DevTalles.Models;
using DevTalles.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DevTalles.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class SubCategoriaController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SubCategoriaController(ApplicationDbContext db)
        {
            _db = db;
        }


        //Mostramos y agrupamos las distintas categorias con las subcategorias correspondientes
        public async Task<IActionResult> Index()
        {

            var categoriasRelacionadas = await _db.SubCategorias.Include(tc => tc.Categoria).ToListAsync();

            var model = categoriasRelacionadas.GroupBy(c => c.Categoria).Select(grupo => new CategoriasSubCategoriaViewModel { Categoria = grupo.Key.NombreCategoria, SubCategorias = grupo.AsEnumerable() }).ToList();

            return View(model);
        }


        //Get Mostramos el formulario para crear subCategoria
        public IActionResult Crear()
        {
            SubCategoriaCreacionViewModel model = new SubCategoriaCreacionViewModel();

            model.DropdownCategoria = DropdownCategoria();
            return View(model);
        }

        //Esta Accion Agrega el Registro en la base de datos


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(SubCategoriaCreacionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.DropdownCategoria = DropdownCategoria();
                return View(model);

            }






            _db.SubCategorias.Add(model.subCategoria);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");



        }
        //Esta Accion muestra el Registro en la vista
        public IActionResult Editar(int? id)
        {
            var model = new SubCategoriaCreacionViewModel()
            {
                subCategoria = new SubCategoria(),
                DropdownCategoria = DropdownCategoria()
            };



            if (id is null || id == 0)
            {
                return NotFound();
            }


            model.subCategoria = _db.SubCategorias.Find(id);





            if (model.subCategoria is null)
            {
                return NotFound();
            }

            return View(model);
        }


        //Esta Accion Actualiza el Registro en la base de datos
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Editar(SubCategoria subCategoria)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            _db.SubCategorias.Update(subCategoria);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }




        [HttpDelete]
        //Esta Accion Elimina el Registro en la base de datos


        public async Task<ActionResult> Eliminar(int id)
        {

            var model = _db.SubCategorias.Find(id);
            if (model is null) { return NotFound(); }

            _db.SubCategorias.Remove(model);
            await _db.SaveChangesAsync();

            return Json(new { success = true });




        }








        //Validamos que no se repita el nombre

        [HttpGet]
        public async Task<IActionResult> ValidarNombre(string nombre)
        {

            var existe = await _db.SubCategorias.AnyAsync(name => name.Nombre == nombre);
            if (existe)
            {
                return Json($"El nombre {nombre} Ya Existe");
            }
            return Json(true);
        }

        public IEnumerable<SelectListItem> DropdownCategoria()
        {
            return _db.Categorias.Select(c => new SelectListItem(c.NombreCategoria, c.CategoriaId.ToString()));
        }
    }
}
