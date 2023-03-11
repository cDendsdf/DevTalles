using DevTalles.Models;

namespace DevTalles.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            

            //Validando si existe almenos un registro en una tabla catálogo.
            if (context.Categorias.Any())
            {
                return; //Base de datos ya ha sido iniciada.
            }

            if (!context.Categorias.Any())
            {
                var categoriaas = new Categoria[] {
                    new Categoria{NombreCategoria="Desarrollo Web",MostrarOrden=1},
                    new Categoria{NombreCategoria="Desarrollo Mobile",MostrarOrden=1},
                    new Categoria{NombreCategoria="Seguridad Informatica",MostrarOrden=1},
                    
                    
                };
                foreach (var cat  in categoriaas)
                {
                    context.Categorias.Add(cat);
                }
                context.SaveChanges();
            }
            else
            {
                return;
            }

            if (!context.SubCategorias.Any())
            {
                var subCate = new SubCategoria[] {
                    
                    new SubCategoria{Nombre ="ASP NET CORE" , CategoriaId=1},
                    new SubCategoria{Nombre ="Flutter" , CategoriaId=2},
                    new SubCategoria{Nombre ="Hacking etico" , CategoriaId=3},
                    
                };
                foreach (var sub  in subCate)
                {
                    context.SubCategorias.Add(sub);
                }
                context.SaveChanges();
            }

            

          
            context.SaveChanges();
        }

    }
}

