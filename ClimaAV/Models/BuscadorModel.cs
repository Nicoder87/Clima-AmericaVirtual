using System.ComponentModel.DataAnnotations;

namespace ClimaAV.Models
{
    public class BuscadorModel
    {
        [Required(ErrorMessage = "Debes ingresar el {0}")]
        [Display(Name = "País")]
        public string IdPais { get; set; }

        [Required(ErrorMessage = "Debes ingresar la {0}")]
        [Display(Name = "Ciudad")]
        public string IdCiudad { get; set; }
    }
}
