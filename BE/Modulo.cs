using System.Collections.Generic;

namespace BE
{
    public class Modulo
    {
        public static readonly Modulo Ninguno;
        public static readonly Modulo Admin;
        public static readonly Modulo Maestros;
        public static readonly Modulo Usuario;
        public static readonly Modulo Ventas;
        public static readonly Modulo Compras;

        private static readonly Dictionary<int, Modulo> Modulos;

        public int Codigo { get; }
        public string Nombre { get; }
        public IEnumerable<Evento> Eventos { get; }

        static Modulo()
        {
            Ninguno = new Modulo(0, "Ninguno", new[] { Evento.Ninguno });

            Admin = new Modulo(1, "Admin",
            new[]
            {
                Evento.Ninguno,
            });

            Usuario = new Modulo(2, "Usuario",
            new[]
            {
                Evento.Ninguno,
                Evento.Login,
                Evento.Logout,
                Evento.CrearUsuario,
                Evento.CambiarClave,
                Evento.BloquearUsuario
            });

            Ventas = new Modulo(3, "Ventas",
            new[]
            {
                Evento.Ninguno,
                Evento.RegistrarCliente,
                Evento.VenderProducto,
            });

            Compras = new Modulo(4, "Compras",
            new[]
            {
                Evento.Ninguno,
                Evento.ComprarProducto
            });

            Maestros = new Modulo(5, "Maestros",
            new[]
            {
                Evento.Ninguno
            });

            Modulos = new Dictionary<int, Modulo>
            {
                [Ninguno.Codigo] = Ninguno,
                [Admin.Codigo] = Admin,
                [Usuario.Codigo] = Usuario,
                [Ventas.Codigo] = Ventas,
                [Compras.Codigo] = Compras,
                [Maestros.Codigo] = Maestros
            };
        }

        private Modulo(int codigo, string nombre, IEnumerable<Evento> eventos)
        {
            Codigo = codigo;
            Nombre = nombre;
            Eventos = eventos;
        }

        public static Modulo ObtenerPorCodigo(int codigo)
        {
            return Modulos.TryGetValue(codigo, out var modulo) ? modulo : Ninguno;
        }

        public static IEnumerable<Modulo> ObtenerTodos()
        {
            return Modulos.Values;
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
