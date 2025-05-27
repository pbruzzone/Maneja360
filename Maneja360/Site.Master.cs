using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Maneja360.Infrastructure;

namespace Maneja360
{
    public partial class SiteMaster : MasterPage
    {
        private readonly SignInManager _signinManager = new SignInManager();

        protected void Page_Load(object sender, EventArgs e) { }

        protected void LoggingOut(object sender, LoginCancelEventArgs e)
        {
            _signinManager.SignOut();
        }
    }
}