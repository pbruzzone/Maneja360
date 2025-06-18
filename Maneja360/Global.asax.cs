using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using DAL;

namespace Maneja360
{
    public class Global : HttpApplication
    {
        private readonly PerfilDAL _perfilDAL = new PerfilDAL();

        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DAO.EnsureDatabaseExists();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (Context.User != null && Context.User.Identity.IsAuthenticated)
            {
                string nombreUsuario = Context.User.Identity.Name;

                string[] rolesUsuario = _perfilDAL.ObtenerPerfilesUsuario(nombreUsuario)
                    .Select(p => p.Nombre)
                    .ToArray();

                Context.User = new GenericPrincipal(Context.User.Identity, rolesUsuario);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}