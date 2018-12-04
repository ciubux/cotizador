using BusinessLayer;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class DireccionEntregaController : Controller
    {
        // GET: DireccionEntrega
        public ActionResult Index()
        {
            return View();
        }

        public String GetDireccionesEntrega()
        {

            String ubigeo = Request["ubigeo"].ToString();
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];

            DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
            List<DireccionEntrega> direccionEntrega = direccionEntregaBL.getDireccionesEntrega(pedido.cliente.idCliente, ubigeo);
            return JsonConvert.SerializeObject(direccionEntrega);
        }



    }
}