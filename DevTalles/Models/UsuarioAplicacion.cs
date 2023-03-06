using Microsoft.AspNetCore.Identity;

namespace DevTalles.Models
{
    public class UsuarioAplicacion:IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}
