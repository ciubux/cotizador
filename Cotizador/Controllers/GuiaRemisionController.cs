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
            Ciudad ciudadEncontrada = ciudadBL.getCiudad(idCiudad);
            guiaRemision.transportista = new Transportista();

            List<Transportista> transportistaList = this.crearListaTransportistas(ciudadEncontrada.idCiudad);

            this.Session[Constantes.VAR_SESSION_TRANSPORTISTAS] = transportistaList;

            guiaRemision.ciudadOrigen = ciudadEncontrada;
            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";
        }



        public String ChangeIdCiudadBusqueda()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA];
            Guid idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);

            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
            pedido.cliente = new Cliente();
            //Para realizar el cambio de ciudad ningun producto debe estar agregado
            pedido.ciudad = ciudadNueva;
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";


        }

        private void instanciarGuiaRemision()
        {
            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.fechaMovimiento = DateTime.Now;
            guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.Venta;
            guiaRemision.transportista = new Transportista();
            guiaRemision.ciudadOrigen = new Ciudad();
            guiaRemision.ciudadOrigen.idCiudad = Guid.Empty;
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
                guiaRemision.documentoDetalle = pedido.documentoDetalle;

                List<Transportista> transportistaList  = this.crearListaTransportistas(guiaRemision.ciudadOrigen.idCiudad);

                this.Session[Constantes.VAR_SESSION_TRANSPORTISTAS] = transportistaList;


            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }
            
        }


        private List<Transportista> crearListaTransportistas(Guid idCiudad)
        {
            List<Transportista> transportistaList = new List<Transportista>();
            Transportista transportista = new Transportista();
            transportista.idTransportista = Guid.Empty;
            transportista.descripcion = "Nuevo Transportista";

            if (idCiudad != Guid.Empty)
            {
                TransportistaBL transportistaBL = new TransportistaBL();
                List<Transportista> transportistaListTmp = transportistaBL.getTransportistas(idCiudad);

                transportistaList.Add(transportista);
                foreach (Transportista transportistaTmp in transportistaListTmp)
                {
                    transportistaList.Add(transportistaTmp);
                }
            }
            return transportistaList;

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
                ViewBag.transportistaList = (List<Transportista>)this.Session[Constantes.VAR_SESSION_TRANSPORTISTAS];
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
            //Se recupera la lista de transportistas
            List<Transportista> transportistaList = (List<Transportista>)this.Session[Constantes.VAR_SESSION_TRANSPORTISTAS];

            //se busca en la lista de transportistas
            foreach (Transportista transportistaTmp in transportistaList)
            {
                if (transportistaTmp.idTransportista == idTransportista)
                {
                    guiaRemision.transportista = transportistaTmp;
                    break;
                }
            }


            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;

            String jsonTransportista = JsonConvert.SerializeObject(guiaRemision.transportista);

            return jsonTransportista;
        }
    }
}