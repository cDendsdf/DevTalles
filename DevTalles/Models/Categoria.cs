using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DevTalles.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        
        [Required(ErrorMessage ="El campo Categoria es obligatorio")]
        [Remote(action: "ValidarNombre", controller: "Categoria")]
        public string NombreCategoria { get; set; }

        [Required(ErrorMessage = "El campo Orden es obligatorio")]
        [Range(1, int.MaxValue,ErrorMessage ="El valor debe de ser entre {1} y {2}")]
        public int MostrarOrden { get; set; }


        public IEnumerable<SubCategoria> SubCategoria { get; set; }

    }
}
