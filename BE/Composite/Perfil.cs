using System;
using System.Collections.Generic;

namespace BE.Composite
{
    public abstract class Perfil
    {
        public int PerfilId { get; set; }

        public string Nombre { get; set; }

        public TipoPermiso Permiso { get; set; }

        public abstract void AgregarHijo(Perfil permiso);

        public abstract void RemoverHijo(Perfil permiso);

        public abstract IList<Perfil> Hijos { get; }

        public abstract TipoPerfil Tipo { get; }

        public static Perfil Crear(int id, string nombre, string permiso)
        {

            if (string.IsNullOrEmpty(permiso))
            {
                return new Familia() { PerfilId = id, Nombre = nombre };
            }

            TipoPermiso per = (TipoPermiso)Enum.Parse(typeof(TipoPermiso), permiso);

            return new Patente() { PerfilId = id, Nombre = nombre, Permiso = per };
        }


        public bool Contiene(TipoPermiso permiso)
        {
            return Contiene(this, permiso);
        }

        private bool Contiene(Perfil comp, TipoPermiso permiso)
        {
            if (comp.Permiso == permiso) return true;

            if (comp.Tipo == TipoPerfil.Familia)
            {
                foreach (var child in comp.Hijos)
                {
                    if (Contiene(child, permiso)) return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return $"{Nombre} ({Tipo})";
        }
    }
}