namespace DevTalles.Models.ViewModels
{
    public class OrdenVM
    {
        public Orden Orden { get; set; }

        public IList<OrdenDetalle> OrdenDetalle { get; set; }
    }
}
