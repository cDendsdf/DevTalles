namespace DevTalles.Models.ViewModels
{
    public class UsuarioProductoVM
    {
        public UsuarioProductoVM()
        {
            CursosLista = new List<Curso>();
        }

        public UsuarioAplicacion UsuarioAplicacion { get; set; }
        public IList<Curso> CursosLista { get; set; }
    }
}
