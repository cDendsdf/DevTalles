
using System.Text.Json;

namespace DevTalles.Utilities
{
    public static class SessionExtencion
    {
        //es un método de extensión para la interfaz ISession que permite a los usuarios almacenar
        //y recuperar objetos serializados en la sesión de una aplicación web.

        public static T Get<T>(this ISession session , string Key)
        {
            var value = session.GetString(Key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }



        public static void Set<T>(this ISession session, string Key, T value)
        {
            session.SetString(Key, JsonSerializer.Serialize(value));
        }
    }
}
