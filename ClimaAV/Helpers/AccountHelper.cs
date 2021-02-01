using ClimaAV.Database;
using System.Web;

namespace ClimaAV.Helpers
{
    public class AccountHelper
    {
        public static void SetCurrentUser(Usuario user)
        {
            if (user == null)
                HttpContext.Current.Session.RemoveAll();
            else
                HttpContext.Current.Session["ApplicationUser"] = user;
        }
        public static Usuario GetCurrentUser()
        {
            return HttpContext.Current.Session["ApplicationUser"] as Usuario;
        }

    }
}