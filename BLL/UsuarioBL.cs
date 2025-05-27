using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;

namespace BLL
{
    public class UsuarioBL 
    {
        private readonly UsuarioDAL _dal = new UsuarioDAL();
        private readonly PasswordHasher _passwordHasher = new PasswordHasher();

        public void Eliminar(Usuario entity)
        {
            entity.Activo = false;
            _dal.Eliminar(entity);
        }

        public void Guardar(Usuario entity)
        {
            if (entity.UsuarioId == 0)
            {
                bool existeDni = _dal.Listar().Any(u => u.DNI == entity.DNI);
                if (existeDni)
                {
                    throw new Exception("Ya se encuentra un usuario registrado con ese DNI");
                }
                string newPassword = entity.DNI + entity.Apellido;
                entity.Password = _passwordHasher.GetHash(newPassword);
                entity.ResetPassword = true;
            }
            _dal.Guardar(entity);
        }

        public IEnumerable<Usuario> Listar()
        {
            return _dal.Listar();
        }

        public Usuario Obtener(int id)
        {
            return _dal.Obtener(id);
        }

        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            var usuario = _dal.ObtenerPorNombreDeUsuario(nombreUsuario);
            return usuario;
        }

        public bool ActualizarPassword(Usuario usr, string password)
        {
            if (string.IsNullOrEmpty(password)) return false;

            usr.Password = _passwordHasher.GetHash(password);
            usr.ResetPassword = false;

            _dal.ActualizarPassword(usr);

            return true;
        }

        public void Bloquear(Usuario usr)
        {
            usr.Bloqueado = true;
            Guardar(usr);
        }

        public void Desbloquear(Usuario usr)
        {
            usr.Bloqueado = false;
            Guardar(usr);
        }

        public void ActualizarIdioma(Usuario usuario)
        {
            _dal.ActualizarIdioma(usuario);
        }
    }
}