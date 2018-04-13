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

        private Cotizacion CotizacionSession
        {
            get
            {

                Cotizacion cotizacion = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.misCotizaciones: cotizacion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION_BUSQUEDA]; break;
                    case Constantes.paginas.Cotizacion: cotizacion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION]; break;
                }
                return cotizacion;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.misCotizaciones: this.Session[Constantes.VAR_SESSION_COTIZACION_BUSQUEDA] = value; break;
                    case Constantes.paginas.Cotizacion: this.Session[Constantes.VAR_SESSION_COTIZACION] = value; break;
                }
            }
        }


        // GET: GuiaRemision
        public ActionResult Index()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.BUSQUEDA_GUIA_REMISION;
            return View();
        }

        public Boolean ConsultarSiExisteGuiaRemision()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
            if (guiaRemision == null)
            {
                return false;
            }
            else
                return true;
        }

        public String ChangeIdCiudad()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
            Guid idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadOrigen = ciudadBL.getCiudad(idCiudad);
            guiaRemision.transportista = new Transportista();
            TransportistaBL transportistaBL = new TransportistaBL();
            ciudadOrigen.transportistaList = transportistaBL.getTransportistas(idCiudad);
            guiaRemision.ciudadOrigen = ciudadOrigen;
            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
            return JsonConvert.SerializeObject(guiaRemision.ciudadOrigen);
        }

        
        private void instanciarGuiaRemision()
        {
            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.fechaMovimiento = DateTime.Now;
            guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.Venta;
            guiaRemision.transportista = new Transportista();
            guiaRemision.ciudadOrigen = new Ciudad();
            guiaRemision.ciudadOrigen.idCiudad = Guid.Empty;
            guiaRemision.pedido = new Pedido();
            guiaRemision.pedido.pedidoDetalleList = new List<PedidoDetalle>();
            guiaRemision.pedido.ciudad = new Ciudad();
            guiaRemision.pedido.ubigeoEntrega = new Ubigeo();
            guiaRemision.ciudadOrigen.transportistaList = new List<Transportista>();

            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
            
       }


        public void iniciarAtencionDesdePedido()
        {
            try
            {
                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];

                if (this.Session[Constantes.VAR_SESSION_GUIA] == null)
                {
                    instanciarGuiaRemision();
                }
                GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
                guiaRemision.pedido = pedido;
                guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.Venta;
                guiaRemision.transportista = new Transportista();
                guiaRemision.ciudadOrigen = pedido.ciudad;

                TransportistaBL transportistaBL = new TransportistaBL();
                guiaRemision.ciudadOrigen.transportistaList = transportistaBL.getTransportistas(pedido.ciudad.idCiudad);

            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }
            
        }
        

        public ActionResult Guiar()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.MANTENIMIENTO_GUIA_REMISION;
            try
            {
                if (this.Session[Constantes.VAR_SESSION_GUIA] == null)
                {
                    instanciarGuiaRemision();
                }
                GuiaRemision guiaRemision = (GuiaRemision) this.Session[Constantes.VAR_SESSION_GUIA];
                
                ViewBag.fechaMovimientotmp = guiaRemision.fechaMovimiento.ToString(Constantes.formatoFecha);
                ViewBag.guiaRemision = guiaRemision;
            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }

            return View();
        }

        public String ChangeTransportista()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
            Guid idTransportista = Guid.Parse(this.Request.Params["idTransportista"]);
            guiaRemision.transportista  = guiaRemision.ciudadOrigen.transportistaList.Where(t => t.idTransportista == idTransportista).FirstOrDefault();
            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
            String jsonTransportista = JsonConvert.SerializeObject(guiaRemision.transportista);
            return jsonTransportista;
        }

        public ActionResult CancelarCreacionGuiaRemision()
        {
            this.Session[Constantes.VAR_SESSION_GUIA] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("Index");
        }
    }
}