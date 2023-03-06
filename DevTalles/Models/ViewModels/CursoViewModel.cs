using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevTalles.Models.ViewModels
{
    public class CursoViewModel
    {
       public Curso Curso { get; set; }   
        public IEnumerable<SelectListItem> DropDowmCategoria { get; set; }
        public IEnumerable<SelectListItem> DropDowmSubCategoria { get; set; }
        
    }
}
