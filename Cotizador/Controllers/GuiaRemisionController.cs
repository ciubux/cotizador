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
    public class GuiaRemisionController : Controller
    {
        // GET: GuiaRemision
        public ActionResult Index()
        {
            return View();
        }

        public String IniciarAtencion()
        {
            Guid idPedido = Guid.Parse(Request["idPedido"].ToString());
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];
            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.pedido = pedido;
            guiaRemision.fechaMovimiento = DateTime.Now;
            guiaRemision.ciudadOrigen = pedido.ciudad;
            guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.Venta;
            guiaRemision.transportista = new Transportista();


            TransportistaBL transportistaBL = new TransportistaBL();
            List<Transportista> transportistaList = transportistaBL.getTransportistas(pedido.ciudad.idCiudad);

            string jsonTransportistaList = JsonConvert.SerializeObject(transportistaList);
            string jsonGuiaRemision = JsonConvert.SerializeObject(guiaRemision);
            String json = "{\"guiaRemision\":" + jsonGuiaRemision + ", \"transportistaList\":" + jsonTransportistaList + "}";
            return json;
            
        }

    }
}