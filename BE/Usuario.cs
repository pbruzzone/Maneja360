using System.Collections.Generic;
using BE.Composite;

namespace BE
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Mail { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public bool Bloqueado { get; set; }
        public bool Activo { get; set; }
        public Idioma Idioma { get; set; }
        public bool ResetPassword { get; set; }
        public List<Perfil> Perfiles { get; set; } = new List<Perfil>();
    }
}
