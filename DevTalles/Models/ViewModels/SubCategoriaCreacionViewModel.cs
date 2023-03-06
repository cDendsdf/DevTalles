using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevTalles.Models.ViewModels
{
    public class SubCategoriaCreacionViewModel
    {
        public SubCategoria subCategoria { get; set; }
        public IEnumerable<SelectListItem> DropdownCategoria { get; set; }
    }
}
