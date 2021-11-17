using System.Web.Mvc;
using System.Web.Routing;

namespace Protocolo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Protocolo", action = "Home", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "EditarPerfil",
                url: "Usuario/Editar/{logon}", 
                defaults: new { controller = "Usuario", action = "EditarPerfil", logon = UrlParameter.Optional });
        }
    }
}
