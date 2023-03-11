using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DevTalles.Models
{
    public class OrdenDetalle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrdenId { get; set; }

        [ForeignKey("OrdenId")]
        public Orden Orden { get; set; }

        [Required]
        public int CursoID { get; set; }

        [ForeignKey("CursoID")]
        public Curso Curso { get; set; }

    }
}
