using System.Collections.Generic;

namespace Protocolo.Helpers
{
    public sealed class Perfil
    {
        public static readonly string Administrador = "Administrador";

        public static readonly string Gestor = "Gestor";

        public static readonly string Comum = "Comum";

        public static readonly string Consulta = "Consulta";

        public static List<string> AsList()
        {
            return new List<string>
            {
                Administrador,
                Gestor,
                Comum,
                Consulta
            };
        }
    }
}