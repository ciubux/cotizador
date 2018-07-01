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
    public class SolicitanteController : Controller
    {
        // GET: DireccionEntrega
        public ActionResult Index()
        {
            return View();
        }

        public String GetSolicitante()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            SolicitanteBL solicitanteBL = new SolicitanteBL();
            List<Solicitante> direccionEntrega = solicitanteBL.getSolicitantes(pedido.cliente.idCliente);
            return JsonConvert.SerializeObject(direccionEntrega);
        }



    }
}