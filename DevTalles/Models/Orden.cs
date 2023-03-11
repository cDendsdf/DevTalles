using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DevTalles.Models
{
    public class Orden
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioAplicacionId { get; set; }

        [ForeignKey("UsuarioAplicacionId")]
        public UsuarioAplicacion UsuarioAplicacion { get; set; }

        public DateTime FechaOrden { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public string NombreCompleto { get; set; }

        [Required]
        public string Email { get; set; }

    }
}
