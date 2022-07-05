using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Cotizador.Models;
using Model;
using Newtonsoft.Json;

namespace Cotizador.Controllers
{
    public class AlmacenController : Controller
    {

        AlmacenBL almacenBl = new AlmacenBL();
        // GET: Ciudad
        public ActionResult Index()
        {
            return View();
        }

        // GET: Ciudad/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        
        public String GetAlmacenesSede(Guid idCiudad)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            List<Almacen> lista = almacenBl.getAlmacenesSedes(idCiudad);

            return JsonConvert.SerializeObject(lista);
        }

    }
}
