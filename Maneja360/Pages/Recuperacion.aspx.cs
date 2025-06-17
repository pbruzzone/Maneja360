using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using Maneja360.Infrastructure;

namespace Maneja360.Pages
{
    public partial class Recuperacion : System.Web.UI.Page
    {
        private IDictionary<string, IList<string>> _dvErrors;
        private readonly DVBL _dvbl = new DVBL();
        private readonly SignInManager _signInManager = new SignInManager();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = _signInManager.Usuario;
            if (usuario == null || usuario.Perfiles.Any(p => p.Nombre == "Administrador"))
            {
                _dvErrors = (IDictionary<string, IList<string>>)Session["DVErrors"];
                recPanel.Visible = true;
            }
            else
            {
                errorLabel.Text = "Usuario sin permisos de administrador.";
            }
        }

        protected object GetErrors()
        {
            if (_dvErrors != null && _dvErrors.Count > 0)
            {
                var sb = new StringBuilder("<ul>");
                foreach (var kv in _dvErrors)
                {
                    var key = kv.Key;

                    sb.Append($"<li>{key}<ul>");

                    foreach (var entry in kv.Value)
                    {
                        sb.Append($"<li>{entry}</li>");
                    }

                    sb.Append("</ul>");
                }

                return sb.ToString();
            }

            return string.Empty;
        }

        protected void recalcularButton_OnClick(object sender, EventArgs e)
        {
           _dvbl.RecalcularDV();
           errorLabel.Text = "Dígitos verificadores recalculados vuelva a iniciar sesión";
           _signInManager.SignOut(saveEvent: false);
        }

        protected void restoreButton_OnClick(object sender, EventArgs e)
        {
            
        }
    }
}