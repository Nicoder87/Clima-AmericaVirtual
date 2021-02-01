using ClimaAV.Database;
using ClimaAV.Helpers;
using ClimaAV.Models;
using System.Linq;
using System.Web.Mvc;

namespace ClimaAV.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult IniciarSesion(UsuarioModel model)
        {
            if (ModelState.IsValid)
            {
                var result = LoginUser(model.UsuarioNombre, model.Password);

                if (result == LoginResult.NoExiste)
                {
                    TempData["msj"] = "El usuario no existe";
                }
                else if (result == LoginResult.PasswordMal)
                {
                    TempData["msj"] = "La password que ingresaste es incorrecta";
                }
                else
                {
                    TempData["msj"] = "Bienvenido " + model.UsuarioNombre;
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            Session.Remove("ApplicationUser");

            Session.Abandon();
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }
        public LoginResult LoginUser(string username, string password)
        {
            string passwordHash = Encryption.Encrypt(password);

            Usuario user_name = GetUserName(username);

            if (user_name == null)
            {
                return LoginResult.NoExiste;
            }

            Usuario user = GetUser(username, passwordHash);
            if (user != null)
            {
                AccountHelper.SetCurrentUser(user);
                return LoginResult.Ok;
            }
            else
            {
                AccountHelper.SetCurrentUser(null);
                return LoginResult.PasswordMal;
            }
        }
        private Usuario GetUser(string username, string hash)
        {
            Usuario entity;
            using (ClimaAVEntities connection = new ClimaAVEntities())
            {
                entity = connection.Usuario.FirstOrDefault(x => x.UsuarioNombre == username && x.Password == hash);
            }
            return entity;
        }

        private Usuario GetUserName(string username)
        {
            Usuario entity;
            using (ClimaAVEntities connection = new ClimaAVEntities())
            {
                entity = connection.Usuario.FirstOrDefault(x => x.UsuarioNombre == username);
            }
            return entity;
        }

        public enum LoginResult : int
        {
            Ok = 1,
            NoExiste = 2,
            PasswordMal = 3
        }
    }
}
