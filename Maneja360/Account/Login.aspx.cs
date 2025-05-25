using System;
using System.Web;
using Maneja360.Infrastructure;

namespace Maneja360.Account
{
    public partial class Login : System.Web.UI.Page
    {

        private readonly SignInManager signinManager = new SignInManager();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (!IsValid) return;

            var result = signinManager
                .PasswordSignIn(NombreUsuario.Text, Password.Text, RememberMe.Checked, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                    break;
                case SignInStatus.LockedOut:
                    Response.Redirect("/Account/Lockout");
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
            return !string.IsNullOrEmpty(url) && ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) || (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }
    }
}