using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using BLL;
using Maneja360.Infrastructure;

namespace Maneja360.Account
{
    public partial class Login : System.Web.UI.Page
    {
        private readonly SignInManager _signinManager = new SignInManager();

        protected void Page_Load(object sender, EventArgs e) { }

        protected void LogIn(object sender, EventArgs e)
        {
            if (!IsValid) return;

            var result = _signinManager
                .PasswordSignIn(NombreUsuario.Text, Password.Text, RememberMe.Checked, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                    break;
                case SignInStatus.LockedOut:
                    Response.Redirect("/Account/Lockout");
                    break;
                case SignInStatus.DVError:
                    var user = _signinManager.Usuario;
                    if (user != null && user.Perfiles.Any(p => p.Nombre == "Administrador"))
                    {
                        Response.Redirect("/Pages/Recuperacion.aspx");
                    }
                    FailureText.Text = "Error de verificación de datos. Por favor, contacte al administrador.";
                    ErrorMessage.Visible = true;
                    FormsAuthentication.SignOut();
                    break;
                case SignInStatus.Failure:
                default:
                    FailureText.Text = "Intento de inicio de sesión no válido";
                    ErrorMessage.Visible = true;
                    break;
            }
        }
        private static void RedirectToReturnUrl(string returnUrl, HttpResponse response)
        {
            if (!string.IsNullOrEmpty(returnUrl) && IsLocalUrl(returnUrl))
            {
                response.Redirect(returnUrl);
            }
            else
            {
                response.Redirect("~/");
            }
        }

        private static bool IsLocalUrl(string url)
        {
            return !string.IsNullOrEmpty(url) 
                   && ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) 
                       || (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }
    }
}