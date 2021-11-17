using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Protocolo
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelValidatorProviders.Providers.Clear();
            ModelValidatorProviders.Providers.Add(new DataAnnotationsModelValidatorProvider());
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
            {
                return;
            }

            FormsAuthenticationTicket authTicket;

            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }

            string[] roles = authTicket.UserData.Split(';');

            if (Context.User != null)
            {
                Context.User = new GenericPrincipal(Context.User.Identity, roles);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (Request.ContentLength > 2097152)
            {
                Server.ClearError();
                Response.Clear();
                Response.Write("Arquivo enviado maior do que 2 MB");
            }
        }
    }
}
