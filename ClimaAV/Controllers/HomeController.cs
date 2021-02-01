using ClimaAV.Database;
using ClimaAV.Helpers;
using ClimaAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ClimaAV.Controllers
{
    public class HomeController : Controller
    {
        // Clase obtenida del mapeo con la BD
        readonly ClimaAVEntities db = new ClimaAVEntities();

        [HttpGet]
        public ActionResult Index()
        {
            CargoCombos();

            HomeModel model = new HomeModel();

            model.Buscador = new BuscadorModel();
            model.ClimaActual = new ClimaActualModel();
            model.ClimaSemana = new List<ClimaSemanaModel>();

            // Verifico si hay algun mensaje
            if (TempData["msj"] != null && !string.IsNullOrEmpty(TempData["msj"].ToString()))
            {
                ViewBag.MessageOk = TempData["msj"].ToString();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(BuscadorModel model)
        {
            //Compruebo que el usuario se encuentre logueado
            var user = AccountHelper.GetCurrentUser();
            if (user == null)
            {
                TempData["msj"] = "Por favor ingrese en Inicio de Sesion";
                return RedirectToAction("Index", "Home");
            }

            CargoCombos();

            HomeModel home = new HomeModel();
            home.ClimaActual = new ClimaActualModel();
            home.Buscador = model;

            home.ClimaActual = ObtenerClimaActual(model);
            home.ClimaSemana = ObtenerClimaSemana(model);

            // Preparo objeto para la transacción

            var ciudad = db.Ciudad.Where(c => c.CP == model.IdCiudad).Select(c => c.Id).FirstOrDefault();

            Transaccion transaccion = new Transaccion {

                Id = Guid.NewGuid(),
                IdUsuario = user.Id,
                IdCiudad = ciudad,
                Fecha = DateTime.Now
            };

            try
            {
                // Genero el insert para grabar la transacción

                db.Transaccion.Add(transaccion);
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return View(home);
        }

        private ClimaActualModel ObtenerClimaActual(BuscadorModel buscador)
        {
            string urlCurrent = "http://api.weatherunlocked.com/api/current/" + buscador.IdPais + "." 
                + buscador.IdCiudad + "?lang=es&app_id=ebfc8909&app_key=34509753694a6b1bc5c0939de49d3ffc";

            using (WebClient client = new WebClient())
            {
                //Descargo el recurso json de la api del clima weatherunlocked
                string json = client.DownloadString(urlCurrent);
                ClimaActualJson ClimaActual = (new JavaScriptSerializer()).Deserialize<ClimaActualJson>(json);

                ClimaActualModel model = new ClimaActualModel();

                // Obtengo nombres de pais y ciudad seleccionados
                model.Pais = db.Pais.Where(x => x.Id == buscador.IdPais).Select(x => x.Nombre).FirstOrDefault();
                model.Ciudad = db.Ciudad.Where(x => x.CP == buscador.IdCiudad).Select(x => x.Nombre).FirstOrDefault();

                // Armo el modelo a partir del json
                model.Clima = ClimaActual.wx_desc;
                string icono = ClimaActual.wx_icon;
                model.Icono = "./Images/" + icono.Replace(".gif", ".png");
                model.GC = ClimaActual.temp_c;
                model.GF = ClimaActual.temp_f;
                model.Humedad = ClimaActual.humid_pct;
                model.Viento = ClimaActual.windspd_kmh;
                model.Precipitaciones = ClimaActual.cloudtotal_pct;

                return model;
            }
        }

        private List<ClimaSemanaModel> ObtenerClimaSemana(BuscadorModel buscador)
        {
            string urlForecast = "http://api.weatherunlocked.com/api/forecast/" + buscador.IdPais + "." +
                buscador.IdCiudad + "?lang=es&app_id=ebfc8909&app_key=34509753694a6b1bc5c0939de49d3ffc";

            using (WebClient client = new WebClient())
            {
                string jsonForecast = client.DownloadString(urlForecast);
                ClimaSemanaJson ClimaSemanaJson = (new JavaScriptSerializer())
                    .Deserialize<ClimaSemanaJson>(jsonForecast);
                
                List<ClimaSemanaModel> model = new List<ClimaSemanaModel>();
                var cont = 0;

                foreach (var item in ClimaSemanaJson.Days)
                {
                    cont += 1;
                    // Esto lo hago para obtener los 5 días posteriores al actual
                    if (cont > 2) 
                    {
                        ClimaSemanaModel mo = new ClimaSemanaModel();
                        mo.Dia = item.Timeframes[0].date;
                        string a = item.Timeframes[0].wx_icon;
                        mo.Icono = "./Images/" + a.Replace(".gif",".png") ;
                        mo.GC = item.Timeframes[0].temp_c;
                        mo.GF = item.Timeframes[0].temp_f;

                        // Agrego objeto a mi lista de días
                        model.Add(mo);
                    }
                }

                return model;
            }
        }

        public void CargoCombos()
        {
            ViewBag.Paises = db.Pais.OrderBy(p => p.Nombre).ToList();
            ViewBag.Ciudades = db.Ciudad.OrderBy(c => c.Nombre).ToList();
        }
        
        public ActionResult GetCiudades(string pais)
        {
            // Cargo el combo de ciudades a partir de lo seleccionado en país
            if (pais == "")
            {
                var ciudad = "<option>SIN DEFINIR</option>";
                return Content(string.Join("", ciudad));
            }

            var ciudades = db.Ciudad.Where(c => c.Pais == pais)
            .Select(c => "<option value='" + c.CP + "'>" + c.Nombre + "</option>'");

            return Content(string.Join("", ciudades));
        }
    }
}
