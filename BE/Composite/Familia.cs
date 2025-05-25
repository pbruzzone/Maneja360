using System.Collections.Generic;
using System.Linq;

namespace BE.Composite
{
    public class Familia : Perfil
    {
        private readonly List<Perfil> _hijos = new List<Perfil>();

        public override void AgregarHijo(Perfil permiso)
        {
            _hijos.Add(permiso);
        }

        public override IList<Perfil> Hijos => _hijos.ToList();

        public override TipoPerfil Tipo => TipoPerfil.Familia;

        public override void RemoverHijo(Perfil permiso)
        {
            _hijos.Remove(permiso);
        }
    }
}