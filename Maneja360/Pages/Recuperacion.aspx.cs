using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Security;
using BLL;
using Maneja360.Infrastructure;

namespace Maneja360.Pages
{
    public partial class Recuperacion : System.Web.UI.Page
    {
        private IDictionary<string, IList<string>> _dvErrors;
        private readonly DVBL _dvbl = new DVBL();
        private readonly RespaldoBL _respaldoBL = new RespaldoBL();
        private readonly SignInManager _signInManager = new SignInManager();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Administrador"))
                {
                    _dvErrors = (IDictionary<string, IList<string>>)Session["DVErrors"];
                    recPanel.Visible = true;
                }
                else
                {
                    recPanel.Visible = false;
                    errorLabel.Text = "Usuario sin permisos de administrador.";
                    errorLabel.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                FormsAuthentication.RedirectToLoginPage();
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
            var path = Server.MapPath($@"~/Recover/{backupFile.FileName}"); 
            try
            {
                backupFile.SaveAs(path);
                _respaldoBL.RealizarRestore(path);
                errorMsg.Text = "Backup restaurado con éxito: " + backupFile.FileName;
                errorMsg.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception)
            {
                errorMsg.Text = "Error al realizar la restauración de la base de datos";
                errorMsg.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        protected void backupButton_OnClick(object sender, EventArgs e)
        {
            var path = Server.MapPath("~/Recover/");
            try
            {
                var filename = _respaldoBL.RealizarBackup(path);
                
                if (!File.Exists(filename)) return;
                
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                    
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filename));
                Response.ContentType = "application/octet-stream";
                    
                Response.WriteFile(filename, readIntoMemory: true);

                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                Response.End();
            }
            catch (Exception)
            {
                errorMsg.Text = "Error al realizar el backup de la base de datos";
                errorMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}