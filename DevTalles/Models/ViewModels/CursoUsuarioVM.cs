namespace DevTalles.Models.ViewModels
{
	public class CursoUsuarioVM
	{

		public IEnumerable<Curso> cursos { get; set; }
		public IEnumerable<OrdenDetalle> ListaCurso { get; set; }
		public IEnumerable<Categoria> Categorias { get; set; }
	}
}
