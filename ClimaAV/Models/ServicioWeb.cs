using System.Collections.Generic;

namespace ClimaAV.Models
{
    public class ClimaActualJson
    {
        public string wx_desc { get; set; }
        public string wx_icon { get; set; }
        public string temp_c { get; set; }
        public string temp_f { get; set; }
        public string humid_pct { get; set; }
        public string windspd_kmh { get; set; }
        public string cloudtotal_pct { get; set; }
    }

    public class ClimaSemanaJson
    {
        public List<Days> Days { get; set; }
    }

    public class Days
    {
        public string date { get; set; }
        public string wx_icon { get; set; }
        public string temp_c { get; set; }
        public string temp_f { get; set; }
        public List<Timeframes> Timeframes { get; set; }
    }

    public class Timeframes
    {
        public string date { get; set; }
        public string wx_icon { get; set; }
        public string temp_c { get; set; }
        public string temp_f { get; set; }
    }
}
