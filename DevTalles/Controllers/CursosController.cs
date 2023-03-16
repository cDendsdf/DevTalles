using DevTalles.Data;
using DevTalles.Models;
using DevTalles.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DevTalles.Controllers
{
    [Authorize(Roles=WC.AdminRole)]
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment hostEnvironment;

        public CursosController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            this.db = db;
            this.hostEnvironment = hostEnvironment;
        }

        //Esta Accion muestra la vista index con una tabla donde se muestran todos los CursosLista
        public IActionResult Index()
        {

            IEnumerable<Curso> Cursos = db.Cursos.Include(c => c.Categoria).Include(cs => cs.SubCategoria);
            return View(Cursos);
        }





        //UPSERT Es una Accion que sirve para actualizar campos o agregar campos validando si se pasa el ID

        public async Task<IActionResult> Upsert(int? id)
        {
            var model = new CursoViewModel()
            {
                Curso = new Curso(),
                DropDowmCategoria = DropdownCategoria(),
                DropDowmSubCategoria = DropdownSubCategoria()
            };



            if (id == null)
            {
                return View(model);

            }
            else
            {
                model.Curso = await db.Cursos.FindAsync(id);
                
                
             
                if (model.Curso is null)
                {
                    return NotFound();
                }
                return View(model);
            }

        }


        //Esta accion de tipo Post Realiza la correspondiente Actualizacion o Agregacion de datos en la base de datos


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CursoViewModel model)
        {


            //Validamos que el model sea valido
            if (!ModelState.IsValid)
            {


                model.DropDowmCategoria = DropdownCategoria();
                model.DropDowmSubCategoria = DropdownSubCategoria();

                return View(model);

            }
            else
            {//Comenzamos a ver si se ha cargado algun archivo en la vista

                var file = HttpContext.Request.Form.Files;

                //obtenemos la ruto principal donde se guardara la imagen en este caso la wwwRoot
                var RootPath = hostEnvironment.WebRootPath;

                //si el id de curso es igual a cero significa que crearemos un nuevo curso 
                if (model.Curso.Id == 0)
                {



                    //validamos que se guarde una imagen identificando que el usuario desea agregar una imagen

                    if (file.Count > 0)
                    {
                        string Upload = RootPath + WC.RutaImagen;
                        string filename = Guid.NewGuid().ToString();
                        var extencion = Path.GetExtension(file[0].FileName);


                        using (FileStream fileStream = new FileStream(Path.Combine(Upload, filename + extencion), FileMode.Create))
                        {
                            file[0].CopyTo(fileStream);
                        }

                        model.Curso.ImagenUrl = filename + extencion;
                        db.Cursos.Add(model.Curso);

                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        //en nuestro sistema requerimos que se seleccione una imagen por lo cual si no se hace regresamos ala misma vista pasandole el model

                        model.DropDowmCategoria = DropdownCategoria();
                        model.DropDowmSubCategoria = DropdownSubCategoria();
                        return View(model);

                    }






                }
                else
                {
                    //actualizamo un curso ya existente


                    var cursoDb = await db.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == model.Curso.Id);


                    //Actualizamos el curso si agrega una nueva imagen se borra la anterior y se guarda la nueva
                    if (file.Count > 0)
                    {
                        string Upload = RootPath + WC.RutaImagen;
                        string filename = Guid.NewGuid().ToString();
                        string extencion = Path.GetExtension(file[0].FileName);

                        var urlimagenAnterior = Path.Combine(Upload, cursoDb.ImagenUrl);



                        if (System.IO.File.Exists(urlimagenAnterior))
                        {
                            System.IO.File.Delete(urlimagenAnterior);
                        }


                        using (FileStream fileStream = new FileStream(Path.Combine(Upload, filename + extencion), FileMode.Create))
                        {
                            file[0].CopyTo(fileStream);
                        }

                        model.Curso.ImagenUrl = filename + extencion;
                        db.Cursos.Update(model.Curso);
                        await db.SaveChangesAsync();

                    }
                    else
                    {
                        //en nuestro sistema requerimos que se seleccione una imagen por lo cual si no se hace regresamos ala misma vista pasandole el model

                        
         
                        
                        model.Curso.ImagenUrl = cursoDb.ImagenUrl;
                        db.Cursos.Update(model.Curso);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }

                }
                return View(model);
            


            }


            

        }



        //Esta accion es para Eliminar los datos de la base de datos

        [HttpDelete]
      

        public async Task<IActionResult> Delete(int id)
        {
            var CursoDB = await db.Cursos.FindAsync(id);

            if (CursoDB == null) 
            {
                return Json(new { success = false, message = "Error al borrar tu Curso" });
            }
            else
            {
                var wwwroot = hostEnvironment.WebRootPath;
                var upload = wwwroot + CursoDB.ImagenUrl;


                if (System.IO.File.Exists(upload))
                {
                    System.IO.File.Delete(upload);
                }
                db.Cursos.Remove(CursoDB);
                await db.SaveChangesAsync();

                return Json(new { success = true, message = "Curso eliminado Correctamente" });
            }
        }


        [HttpGet]
        public async Task<JsonResult> ObtenerSubCategoria(int id)
        {
            var subcategorias = db.SubCategorias.Where(sc => sc.CategoriaId == id);

            IEnumerable<SelectListItem> categorias = subcategorias.Select(sb => new SelectListItem(sb.Nombre, sb.SubCategoriaId.ToString()));
            return Json(categorias);
        }




        //metodos para llenar el SelectList

        public IEnumerable<SelectListItem> DropdownCategoria()
        {
            return db.Categorias.Select(c => new SelectListItem(c.NombreCategoria, c.CategoriaId.ToString()));
        }
        public IEnumerable<SelectListItem> DropdownSubCategoria()
        {
            return db.SubCategorias.Select(c => new SelectListItem(c.Nombre, c.SubCategoriaId.ToString()));
        }
    }
}
