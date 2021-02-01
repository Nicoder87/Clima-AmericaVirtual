using System.ComponentModel.DataAnnotations;

namespace ClimaAV.Models
{

    public class UsuarioModel
    {
        public long Id { get; set; }
        
        [Required(ErrorMessage = "Debes ingresar el Usuario")]
        [Display(Name = "Usuario")]
        public string UsuarioNombre { get; set; }
        
        [Required(ErrorMessage = "Debes ingresar la Contraseña")]

        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
    }
}