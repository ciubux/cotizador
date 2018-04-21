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

        private GuiaRemision GuiaRemisionSession
        {
            get
            {

                GuiaRemision guiaRemision = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaGuiasRemision: guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoGuiaRemision: guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA]; break;
                }
                return guiaRemision;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaGuiasRemision: this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoGuiaRemision: this.Session[Constantes.VAR_SESSION_GUIA] = value; break;
                }
            }
        }

        private void instanciarGuiaRemisionBusqueda()
        {
            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.seguimientoMovimientoAlmacenSalida = new SeguimientoMovimientoAlmacenSalida();
            guiaRemision.seguimientoMovimientoAlmacenSalida.estado = SeguimientoMovimientoAlmacenSalida.estadosSeguimientoMovimientoAlmacenSalida.Enviado;
            guiaRemision.ciudadOrigen = new Ciudad();

            guiaRemision.pedido = new Pedido();
            guiaRemision.pedido.cliente = new Cliente();

            guiaRemision.fechaMovimientoDesde = DateTime.Now.AddDays(-10);
            guiaRemision.fechaMovimientoHasta = DateTime.Now.AddDays(10);

            

            this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] = guiaRemision;
        }

        // GET: GuiaRemision
        public ActionResult Index()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.BUSQUEDA_GUIA_REMISION;


            if (this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] == null)
            {
                instanciarGuiaRemisionBusqueda();
            }

            if (this.Session[Constantes.VAR_SESSION_GUIA_LISTA] == null)
            {
                this.Session[Constantes.VAR_SESSION_GUIA_LISTA] = new List<GuiaRemision>();
            }

            GuiaRemision guiaRemisionSearch = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA];

            ViewBag.guiaRemision = guiaRemisionSearch;
            ViewBag.guiaRemisionList = this.Session[Constantes.VAR_SESSION_GUIA_LISTA];
            ViewBag.pagina = Constantes.BUSQUEDA_GUIA_REMISION;

            ViewBag.fechaMovimientoDesde = guiaRemisionSearch.fechaMovimientoDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaMovimientoHasta = guiaRemisionSearch.fechaMovimientoHasta.ToString(Constantes.formatoFecha);

            return View();
        }

        public String Search()
        {
            //Se recupera el pedido Búsqueda de la session
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA];

            /*   String[] solDesde = this.Request.Params["fechaSolicitudDesde"].Split('/');
               pedido.fechaSolicitudDesde = new DateTime(Int32.Parse(solDesde[2]), Int32.Parse(solDesde[1]), Int32.Parse(solDesde[0]));

               String[] solHasta = this.Request.Params["fechaSolicitudHasta"].Split('/');
               pedido.fechaSolicitudHasta = new DateTime(Int32.Parse(solHasta[2]), Int32.Parse(solHasta[1]), Int32.Parse(solHasta[0]), 23, 59, 59);

               String[] entregaDesde = this.Request.Params["fechaEntregaDesde"].Split('/');
               pedido.fechaEntregaDesde = new DateTime(Int32.Parse(entregaDesde[2]), Int32.Parse(entregaDesde[1]), Int32.Parse(entregaDesde[0]));

               String[] entregaHasta = this.Request.Params["fechaEntregaHasta"].Split('/');
               pedido.fechaEntregaHasta = new DateTime(Int32.Parse(entregaHasta[2]), Int32.Parse(entregaHasta[1]), Int32.Parse(entregaHasta[0]), 23, 59, 59);


               if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
               {
                   pedido.numeroPedido = 0;
               }
               else
               {
                   pedido.numeroPedido = long.Parse(this.Request.Params["numero"]);
               }

               if (this.Request.Params["numeroGrupo"] == null || this.Request.Params["numeroGrupo"].Trim().Length == 0)
               {
                   pedido.numeroGrupoPedido = 0;
               }
               else
               {
                   pedido.numeroGrupoPedido = long.Parse(this.Request.Params["numeroGrupo"]);
               }


               pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Int32.Parse(this.Request.Params["estado"]);
               pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Int32.Parse(this.Request.Params["estadoCrediticio"]);
               */
            PedidoBL pedidoBL = new PedidoBL();
            List<GuiaRemision> guiaRemisionList = new List<GuiaRemision>(); //;pedidoBL.GetPedidos(pedido);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_GUIA_LISTA] = guiaRemisionList;
            this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] = guiaRemision;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(guiaRemisionList);
            //return pedidoList.Count();
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
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadOrigen = ciudadBL.getCiudad(idCiudad);
            guiaRemision.transportista = new Transportista();
            TransportistaBL transportistaBL = new TransportistaBL();
            ciudadOrigen.transportistaList = transportistaBL.getTransportistas(idCiudad);
            guiaRemision.ciudadOrigen = ciudadOrigen;
            this.GuiaRemisionSession = guiaRemision;
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
            guiaRemision.seguimientoMovimientoAlmacenSalida = new SeguimientoMovimientoAlmacenSalida();
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
               // guiaRemision.ciudadOrigen = pedido.ciudad;

                CiudadBL ciudadBL = new CiudadBL();
                Ciudad ciudadOrigen = ciudadBL.getCiudad(pedido.ciudad.idCiudad);
                guiaRemision.ciudadOrigen = ciudadOrigen;

                guiaRemision.transportista = new Transportista();
                guiaRemision.serieDocumento = Int32.Parse(ciudadOrigen.serieGuiaRemision);
                guiaRemision.numeroDocumento = ciudadOrigen.ultimoNumeroGuiaRemision;
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

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                if (!usuario.creaGuias)
                {
                    return RedirectToAction("Login", "Account");
                }
            }



            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.MANTENIMIENTO_GUIA_REMISION;
            try
            {
                if (this.Session[Constantes.VAR_SESSION_GUIA] == null)
                {
                    return View("GuiarVacia");
                    //instanciarGuiaRemision();
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

            ViewBag.pagina = Constantes.MANTENIMIENTO_GUIA_REMISION;
            return View();
        }

        public void ChangeAtencionParcial()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
            guiaRemision.atencionParcial = Int32.Parse( this.Request.Params["atencionParcial"])==1;
            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
        }


        public void ChangeUltimaAtencionParcial()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
            guiaRemision.ultimaAtencionParcial = Int32.Parse(this.Request.Params["ultimaAtencionParcial"]) == 1;
            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
        }

        


        public String ChangeTransportista()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
         
            if (this.Request.Params["idTransportista"] == null || this.Request.Params["idTransportista"].Equals(String.Empty))
            {
                guiaRemision.transportista = new Transportista();
            }
            else
            {
                Guid idTransportista = Guid.Parse(this.Request.Params["idTransportista"]);
                guiaRemision.transportista = guiaRemision.ciudadOrigen.transportistaList.Where(t => t.idTransportista == idTransportista).FirstOrDefault();
            }
           
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


        public String Create()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());


            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            guiaRemision.usuario = usuario;


            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            movimientoAlmacenBL.InsertMovimientoAlmacenSalida(guiaRemision);
            long numeroGuiaRemision = guiaRemision.numero;
            Guid idGuiaRemision = guiaRemision.idGuiaRemision;
            int estado = (int)guiaRemision.seguimientoMovimientoAlmacenSalida.estado;
            if (continuarLuego == 1)
            {
                SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = SeguimientoPedido.estadosSeguimientoPedido.Edicion;
                estado = (int)estadosSeguimientoPedido;
                String observacion = "Se continuará editando luego";
               // updateEstadoSeguimientoPedido(idPedido, estadosSeguimientoPedido, observacion);
            }
            guiaRemision = null;
            this.GuiaRemisionSession = null;
            //usuarioBL.updatePedidoSerializado(usuario, null);
            String resultado = "{ \"codigo\":\"" + numeroGuiaRemision + "\", \"estado\":\"" + estado + "\" }";
            return resultado;
        }

        public String CreateTransportistaTemporal()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;            
            Transportista transportista = new Transportista();
            transportista.descripcion = Request["descripcion"];
            transportista.direccion = Request["direccion"];
            transportista.ruc = Request["ruc"];
            transportista.telefono = Request["telefono"];
            transportista.idTransportista = Guid.Empty;
            GuiaRemisionSession.ciudadOrigen.transportistaList.Add(transportista);
            GuiaRemisionSession.transportista = transportista;
            this.GuiaRemisionSession = guiaRemision;
            return JsonConvert.SerializeObject(transportista);
        }


        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> documentoDetalleList)
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];

            foreach (DocumentoDetalleJson documentoDetalleJson in documentoDetalleList)
            {
                DocumentoDetalle documentoDetalle = guiaRemision.pedido.documentoDetalle.Where(d => d.producto.idProducto == Guid.Parse(documentoDetalleJson.idProducto)).FirstOrDefault();
                documentoDetalle.cantidad = documentoDetalleJson.cantidad;
            }
            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
            return "{\"cantidad\":\"" + guiaRemision.pedido.documentoDetalle.Count + "\"}";
        }
    }
}