using System.Collections.Generic;

namespace BE.Composite
{
    public class Patente : Perfil
    {
        public override void AgregarHijo(Perfil permiso) { }

        public override IList<Perfil> Hijos => new List<Perfil>();

        public override TipoPerfil Tipo => TipoPerfil.Patente;

        public override void RemoverHijo(Perfil permiso) { }
    }
}