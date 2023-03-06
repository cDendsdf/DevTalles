namespace DevTalles.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Curso> Cursos { get; set; }
        public IEnumerable<Categoria> Categorias { get; set; }

        public bool CursosDisponibles { get; set; }
    }
}
