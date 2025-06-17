using System.Web;
using System.Web.Security;
using BE;
using BLL;

namespace Maneja360.Infrastructure
{
    public class SignInManager
    {
        private readonly PasswordHasher _passwordHasher = new PasswordHasher();
        private readonly UsuarioBL _usuarioBL = new UsuarioBL();
        private readonly DVBL _dvBL = new DVBL();
        private readonly BitacoraEventoBL _bitacoraBL = new BitacoraEventoBL();

        private int _retriesCount = 0;

        private static HttpContext Context => HttpContext.Current;

        public Usuario Usuario
        {
            get => (Usuario)Context?.Session["Usuario"];
            private set
            {
                if (Context != null)
                {
                    Context.Session["Usuario"] = value;
                }
            }
        }

        public SignInStatus PasswordSignIn(string nombreUsuario, string password, bool rememberMe, bool shouldLockout)
        {
            var usr = _usuarioBL.ObtenerPorNombreUsuario(nombreUsuario);
            if (usr == null) return SignInStatus.Failure;
            if (usr.Bloqueado) return SignInStatus.LockedOut;

            var (isValidDV, errors) = _dvBL.VerificarDV();

            if (ValidPassword(usr, password))
            {
                FormsAuthentication.SetAuthCookie(nombreUsuario, false);
                Usuario = usr;
                _retriesCount = 0;
                
                if (!isValidDV)
                {
                    Context.Session["DVErrors"] = errors;
                    return SignInStatus.DVError;
                }

                _bitacoraBL.Guardar(Evento.Login, Modulo.Usuario, Criticidad.Alta, Usuario.NombreUsuario);
                return SignInStatus.Success;
            }

            _retriesCount++;

            if (_retriesCount != 3) return SignInStatus.Failure;
            
            _usuarioBL.Bloquear(usr);
            _bitacoraBL.Guardar(Evento.BloquearUsuario, Modulo.Usuario, Criticidad.Alta, Usuario.NombreUsuario);
            _retriesCount = 0;
            return SignInStatus.LockedOut;
        }

        public void SignOut(bool saveEvent = true)
        {
            FormsAuthentication.SignOut();
            if (saveEvent)
            {
                var nombreUsuario = Usuario?.NombreUsuario ?? Context?.User?.Identity?.Name;
                _bitacoraBL.Guardar(Evento.Logout, Modulo.Usuario, Criticidad.Alta, nombreUsuario);
            }
            Usuario = null;
        }

        public bool IsLogged()
        {
            return Usuario != null;
        }

        public bool ValidPassword(Usuario usuario, string password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            if (usuario == null || string.IsNullOrEmpty(password)) return false;

            var expectedHash = _passwordHasher.GetHash(password);
            return expectedHash.Equals(usuario.Password);
        }
    }
}