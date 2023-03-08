namespace DevTalles.Models.ViewModels
{
    public class UsuarioProductoVM
    {
        public UsuarioProductoVM()
        {
            Cursos = new List<Curso>();
        }

        public UsuarioAplicacion UsuarioAplicacion { get; set; }
        public IEnumerable<Curso> Cursos { get; set; }
    }
}
