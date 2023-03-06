using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTalles.Models
{
    public class SubCategoria
    {
        [Key]
        public int SubCategoriaId { get; set; }

        [Required(ErrorMessage = "El Nombre de la Sub Categoria es obligatorio.")]
        [Display(Name ="SubCategoria")]
        [Remote(action:"ValidarNombre",controller:"SubCategoria")]
        public string Nombre { get; set; }


        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria? Categoria { get; set; }
    }
}
