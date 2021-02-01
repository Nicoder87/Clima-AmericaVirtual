using System.Collections.Generic;

namespace ClimaAV.Models
{
    public class HomeModel
    {
        public BuscadorModel Buscador { get; set; }
        public ClimaActualModel ClimaActual { get; set; }
        public List<ClimaSemanaModel> ClimaSemana { get; set; }
    }
}
