using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    //public class UsuarioOld
    //{
    //    MpUsuario mpUsuario = new MpUsuario();
    //    GestionEventos gestionEventos = new GestionEventos();

    //    // Hashing de la contraseña
    //    public static string HashSHA256(string input)
    //    {
    //        using (SHA256 sha256 = SHA256.Create())
    //        {
    //            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
    //            byte[] hashBytes = sha256.ComputeHash(inputBytes);

    //            StringBuilder builder = new StringBuilder();
    //            for (int i = 0; i < hashBytes.Length; i++)
    //            {
    //                builder.Append(hashBytes[i].ToString("x2"));
    //            }
    //            return builder.ToString();
    //        }
    //    }

    //    public void login(BE.Usuario usuarioALoguear)
    //    {

    //        usuarioALoguear.Password = HashSHA256(usuarioALoguear.Password);
    //        BE.Usuario usuarioCargado = mpUsuario.BuscarPorEmail(usuarioALoguear.Email);

           
    //        if (validarUsuario(usuarioALoguear, usuarioCargado))
    //        {
    //            //Login exitoso
    //            SessionManager.Login(usuarioCargado);
    //            gestionEventos.persistirEvento("Login", BE.Modulo.Users.ToString(), 3);
    //            usuarioCargado.Intentos = 0;
    //            mpUsuario.modificarUsuario(usuarioCargado);
    //        }
    //        else
    //        {
    //            usuarioCargado.Intentos++;
    //            if (usuarioCargado.Intentos > 3)
    //            {
    //                bloquearUsuario(usuarioCargado);
    //                throw new Exception("El usuario ha sido bloqueado por 3 intentos fallidos");
    //            }
    //            else
    //            {
    //                mpUsuario.modificarUsuario(usuarioCargado);
    //                throw new Exception("Credenciales invalidas");
    //            }
    //        }
    //    }

    //    private bool validarUsuario(BE.Usuario usuarioALoguear, BE.Usuario usuarioCargado)
    //    {
    //        if (usuarioCargado == null)
    //            throw new Exception("Usuario no encontrado");
    //        if (usuarioCargado.Bloqueado)
    //            throw new Exception("El usuario esta bloqueado");
    //        if (usuarioCargado.Password == usuarioALoguear.Password)
    //            return true;
    //        else
    //            return false;
    //    }

    //    private void bloquearUsuario(BE.Usuario usuarioCargado)
    //    {
    //        usuarioCargado.Bloqueado = true;
    //        mpUsuario.modificarUsuario(usuarioCargado);
    //    }

    //    public void logout()
    //    {
    //        gestionEventos.persistirEvento("Logout", BE.Modulo.Users.ToString(), 3);
    //        SessionManager.Logout();
    //    }
    //}
}
