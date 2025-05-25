using System.Collections.Generic;

namespace BE
{
    public class Evento
    {
        public static readonly Evento Ninguno = new Evento(0, "Ninguno");
        public static readonly Evento Login = new Evento(1, "Login");
        public static readonly Evento Logout = new Evento(2, "Logout");
        public static readonly Evento CrearUsuario = new Evento(3, "Crear Usuario");
        public static readonly Evento CambiarClave = new Evento(4, "Cambiar Clave");
        public static readonly Evento BloquearUsuario = new Evento(5, "Bloquear Usuario");
        public static readonly Evento RegistrarCliente = new Evento(6, "Registrar Cliente");
        public static readonly Evento VenderProducto = new Evento(7, "Vender Producto");
        public static readonly Evento ComprarProducto = new Evento(8, "Comprar Producto");

        private static readonly Dictionary<int, Evento> Eventos = new Dictionary<int, Evento>
        {
            [Ninguno.Codigo] = Ninguno,
            [Login.Codigo] = Login,
            [Logout.Codigo] = Logout,
            [CrearUsuario.Codigo] = CrearUsuario,
            [CambiarClave.Codigo] = CambiarClave,
            [BloquearUsuario.Codigo] = BloquearUsuario,
            [RegistrarCliente.Codigo] = RegistrarCliente,
            [VenderProducto.Codigo] = VenderProducto,
            [ComprarProducto.Codigo] = ComprarProducto,
        };

        public int Codigo { get; }
        public string Descripcion { get; }

        private Evento(int codigo, string nombre)
        {
            Codigo = codigo;
            Descripcion = nombre;
        }

        public static Evento ObtenerPorCodigo(int codigo)
        {
            return Eventos.TryGetValue(codigo, out var evento) ? evento : Ninguno;
        }

        public static IEnumerable<Evento> ObtenerTodos()
        {
            return Eventos.Values;
        }

        public override string ToString()
        {
            return Descripcion;
        }
    }
}
