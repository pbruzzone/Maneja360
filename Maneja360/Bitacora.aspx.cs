using System;
using System.Web.UI.WebControls;
using BE;
using BLL;

namespace Maneja360
{
	public partial class Bitacora : System.Web.UI.Page
	{
        private readonly BitacoraEventoBL _bitacoraBL = new BitacoraEventoBL(); 

		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                CargarFechas();
                CargarModulos();
                CargarEventos();
                CargarCriticidad();
                CargarBitacora();
            }
        }

        private void CargarFechas()
        {
            FechaDesde.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            FechaHasta.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void CargarBitacora()
        {
            var nombreUsuario = NombreUsuario.Text.Trim();

            DateTime? fechaIni = DateTime.TryParse(FechaDesde.Text, out var fi) ? fi : (DateTime?)null;
            DateTime? fechaFin = DateTime.TryParse(FechaHasta.Text, out var ff) ? ff : (DateTime?)null;

            var modulo = Modulo.ObtenerPorCodigo(Convert.ToInt32(ComboModulo.SelectedValue));
            var evento = Evento.ObtenerPorCodigo(Convert.ToInt32(ComboEvento.SelectedValue));

            var criticidad = (Criticidad)Enum.Parse(typeof(Criticidad), ComboCriticidad.SelectedValue);

            GridBitacora.DataSource = _bitacoraBL.Listar(nombreUsuario, fechaIni, fechaFin, modulo, evento, criticidad);
            GridBitacora.DataBind();
        }

        private void CargarModulos()
        {
            ComboModulo.DataSource = Modulo.ObtenerTodos();
            ComboModulo.DataBind();
        }

        private void CargarEventos()
        {
            ComboEvento.DataSource = Modulo.ObtenerPorCodigo(Convert.ToInt32(ComboModulo.SelectedValue)).Eventos;
            ComboEvento.DataBind();
        }

        private void CargarCriticidad()
        {
            ComboCriticidad.DataSource = Enum.GetNames(typeof(Criticidad));
            ComboCriticidad.DataBind();
        }

        protected void Filtrar_OnClick(object sender, EventArgs e)
        {
            CargarBitacora();
        }


        protected void Limpiar_OnClick(object sender, EventArgs e)
        {
            NombreUsuario.Text = "";
            ComboModulo.SelectedValue = "0";
            ComboEvento.SelectedValue = "0";
            ComboCriticidad.SelectedValue = "Ninguna";
            CargarFechas();

            CargarBitacora();
        }


        protected void ComboModulo_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            CargarEventos();
        }

        protected void BitacoraGrid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;
            }
        }
    }
}