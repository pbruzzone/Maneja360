using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE.Composite;
using BLL;

namespace Maneja360
{
    public partial class _Default : Page
    {
        private readonly UsuarioBL _usuarioBL = new UsuarioBL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarPerfiles();
            }
        }
        
        protected void CargarPerfiles()
        {
            var usr = _usuarioBL.ObtenerPorNombreUsuario(Context.User.Identity.Name);

            var root = new TreeNode(usr.NombreUsuario) { SelectAction = TreeNodeSelectAction.None };
            ProfileTreeView.Nodes.Add(root);

            foreach (var perfil in usr.Perfiles)
            {
                MostrarEnTreeView(root, perfil);
            }
        }
        
        private void MostrarEnTreeView(TreeNode tn, Perfil p)
        {
            var n = new TreeNode(p.Nombre, p.PerfilId.ToString()) { SelectAction = TreeNodeSelectAction.None };
         
            tn.ChildNodes.Add(n);

            if (p.Hijos == null) return;
            
            foreach (var item in p.Hijos)
            {
                MostrarEnTreeView(n, item);
            }
        }

    }
}