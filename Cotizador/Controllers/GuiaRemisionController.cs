using BusinessLayer;
using Model;
using Model.EXCEPTION;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            guiaRemision.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            guiaRemision.pedido = new Pedido();
            guiaRemision.pedido.cliente = new Cliente();
            guiaRemision.pedido.cliente.idCliente = Guid.Empty;

            guiaRemision.fechaTrasladoDesde = DateTime.Now.AddDays(-10);
            guiaRemision.fechaTrasladoHasta = DateTime.Now.AddDays(10);

            

            this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] = guiaRemision;
        }

        public void CleanBusqueda()
        {
            instanciarGuiaRemisionBusqueda();
        }

        // GET: GuiaRemision
        public ActionResult Index()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.BUSQUEDA_GUIA_REMISION;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }            

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

            ViewBag.fechaTrasladoDesde = guiaRemisionSearch.fechaTrasladoDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaTrasladoHasta = guiaRemisionSearch.fechaTrasladoHasta.ToString(Constantes.formatoFecha);
            ViewBag.Si = Constantes.MENSAJE_SI;
            ViewBag.No = Constantes.MENSAJE_NO;

            int existeCliente = 0;
            if (guiaRemisionSearch.pedido.cliente.idCliente != Guid.Empty)
            {
                existeCliente = 1;
            }

            ViewBag.existeCliente = existeCliente;


            return View();
        }

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            return clienteBL.getCLientesBusqueda(data, guiaRemision.ciudadOrigen.idCiudad);
        }

        public String GetCliente()
        {


            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            guiaRemision.pedido.cliente = clienteBl.getCliente(idCliente);

            String resultado = JsonConvert.SerializeObject(guiaRemision.pedido.cliente);

            this.GuiaRemisionSession = guiaRemision;
            return resultado;
        }

        public String Search()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaGuiasRemision;

            //Se recupera el pedido Búsqueda de la session
            GuiaRemision guiaRemision = this.GuiaRemisionSession;

            String[] movDesde = this.Request.Params["fechaTrasladoDesde"].Split('/');
            guiaRemision.fechaTrasladoDesde = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]));

            String[] movHasta = this.Request.Params["fechaTrasladoHasta"].Split('/');
            guiaRemision.fechaTrasladoHasta = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);
             

            if (this.Request.Params["numeroDocumento"] == null || this.Request.Params["numeroDocumento"].Trim().Length == 0)
            {
                guiaRemision.numeroDocumento = 0;
            }
            else
            {
                guiaRemision.numeroDocumento = int.Parse(this.Request.Params["numeroDocumento"]);
            }
        //    guiaRemision.seguimientoMovimientoAlmacenSalida.estado = (SeguimientoMovimientoAlmacenSalida.estadosSeguimientoMovimientoAlmacenSalida)Int32.Parse(this.Request.Params["estado"]);

            /*guiaRemision.ciudadOrigen = 
             idCiudad: idCiudad,
                 idCliente: idCliente,
             */


            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
           

            List<GuiaRemision> guiaRemisionList = movimientoAlmacenBL.GetGuiasRemision(guiaRemision);
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


        public void ChangeEstaAnulado()
        {
            this.GuiaRemisionSession.estaAnulado = Int32.Parse(this.Request.Params["estaAnulado"]) == 1;
        }

        public void ChangeFechaTrasladoDesde()
        {
            String[] movDesde = this.Request.Params["fechaTrasladoDesde"].Split('/');
            this.GuiaRemisionSession.fechaTrasladoDesde = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]));            
        }

        public void ChangeFechaTraslado()
        {
            String[] movHasta = this.Request.Params["fechaTraslado"].Split('/');
            this.GuiaRemisionSession.fechaTraslado = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);
        }

        public void ChangeFechaEmision()
        {
            String[] movDesde = this.Request.Params["fechaEmision"].Split('/');
            this.GuiaRemisionSession.fechaEmision = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]));
        }

        public void ChangeFechaTrasladoHasta()
        {
            String[] movHasta = this.Request.Params["fechaTrasladoHasta"].Split('/');
            this.GuiaRemisionSession.fechaTrasladoHasta = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);
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
            guiaRemision.fechaEmision = DateTime.Now;
            guiaRemision.fechaTraslado = DateTime.Now;
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

                guiaRemision.motivoTraslado = (GuiaRemision.motivosTraslado)(char)pedido.tipoPedido;
                guiaRemision.transportista = new Transportista();
                // guiaRemision.ciudadOrigen = pedido.ciudad;

                /*  foreach (DocumentoDetalle documentoDetalle in guiaRemision.pedido.documentoDetalle)
                  {
                      documentoDetalle.cantidadPendienteAtencion = documentoDetalle.cantidad;
                      documentoDetalle.cantidadPorAtender = documentoDetalle.cantidad;
                  }*/
                guiaRemision.observaciones = String.Empty;
                if (pedido.numeroReferenciaCliente != null && !pedido.numeroReferenciaCliente.Trim().Equals(String.Empty))
                {
                    guiaRemision.observaciones = "O/C: N°"+pedido.numeroReferenciaCliente.Trim()+ " ";
                }
                guiaRemision.observaciones = guiaRemision.observaciones + pedido.observacionesGuiaRemision;

                CiudadBL ciudadBL = new CiudadBL();
                Ciudad ciudadOrigen = ciudadBL.getCiudad(pedido.ciudad.idCiudad);
                guiaRemision.ciudadOrigen = ciudadOrigen;

                guiaRemision.transportista = new Transportista();
                guiaRemision.serieDocumento = ciudadOrigen.serieGuiaRemision;
                guiaRemision.numeroDocumento = ciudadOrigen.siguienteNumeroGuiaRemision;
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
                    return View("GuiarEmpty");
                    //instanciarGuiaRemision();
                }
                GuiaRemision guiaRemision = (GuiaRemision) this.Session[Constantes.VAR_SESSION_GUIA];
                
                ViewBag.fechaTrasladotmp = guiaRemision.fechaTraslado.ToString(Constantes.formatoFecha);
                ViewBag.fechaEmisiontmp = guiaRemision.fechaEmision.ToString(Constantes.formatoFecha);
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

        public String Show()
        {
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.idMovimientoAlmacen = Guid.Parse(Request["idMovimientoAlmacen"].ToString());
            guiaRemision = movimientoAlmacenBL.GetGuiaRemision(guiaRemision);
            this.Session[Constantes.VAR_SESSION_GUIA_VER] = guiaRemision;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonGuiaRemision = JsonConvert.SerializeObject(guiaRemision);
            String json = "{\"usuario\":" + jsonUsuario + ", \"guiaRemision\":" + jsonGuiaRemision + "}";
            return json;
        }

        public ActionResult Print()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ImprimirGuiaRemision;

            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_VER];
            //Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //string jsonUsuario = JsonConvert.SerializeObject(usuario);
            //string jsonGuiaRemision = JsonConvert.SerializeObject(guiaRemision);
            //String json = "{\"usuario\":" + jsonUsuario + ", \"guiaRemision\":" + jsonGuiaRemision + "}";
            

            ViewBag.guiaRemision = guiaRemision;
            ViewBag.pagina = this.Session[Constantes.VAR_SESSION_PAGINA];
            
            return View("Print"+ guiaRemision.ciudadOrigen.sede.ToUpper().Substring(0,1));
            
        }






        #region Changes

        public void ChangeInputString()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            PropertyInfo propertyInfo = guiaRemision.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(guiaRemision, this.Request.Params["valor"]);
            this.GuiaRemisionSession = guiaRemision;
        }

        public void ChangeInputStringTransportista()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            PropertyInfo propertyInfo = guiaRemision.transportista.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(guiaRemision.transportista, this.Request.Params["valor"]);
            this.GuiaRemisionSession = guiaRemision;
        }

        public void ChangeAtencionParcial()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
            guiaRemision.atencionParcial = Int32.Parse( this.Request.Params["atencionParcial"])==1;

            if (!guiaRemision.atencionParcial)
            {
                foreach (DocumentoDetalle documentoDetalle in guiaRemision.documentoDetalle)
                {
                    documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadPendienteAtencion;
                }

            }
            


            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
        }


        public void ChangeUltimaAtencionParcial()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
            guiaRemision.ultimaAtencionParcial = Int32.Parse(this.Request.Params["ultimaAtencionParcial"]) == 1;


            if (!guiaRemision.atencionParcial && guiaRemision.ultimaAtencionParcial)
            {
                foreach (DocumentoDetalle documentoDetalle in guiaRemision.documentoDetalle)
                {
                    documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadPendienteAtencion;
                }

            }

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



        #endregion


        public String Create()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());


            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            guiaRemision.usuario = usuario;

            String error = String.Empty;
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            try
            {
                movimientoAlmacenBL.InsertMovimientoAlmacenSalida(guiaRemision);
            }
            catch (DuplicateNumberDocumentException ex)
            {
                error = "DuplicateNumberDocumentException";
            }

            long numeroGuiaRemision = guiaRemision.numero;
            Guid idGuiaRemision = guiaRemision.idMovimientoAlmacen;
            String serieNumeroGuia = guiaRemision.serieNumeroGuia;
            
            int estado = (int)guiaRemision.seguimientoMovimientoAlmacenSalida.estado;
            /*if (continuarLuego == 1)
            {
                SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = SeguimientoPedido.estadosSeguimientoPedido.Edicion;
                estado = (int)estadosSeguimientoPedido;
                String observacion = "Se continuará editando luego";
               // updateEstadoSeguimientoPedido(idPedido, estadosSeguimientoPedido, observacion);
            }
            guiaRemision = null;*/
            this.GuiaRemisionSession = null;
            //usuarioBL.updatePedidoSerializado(usuario, null);
            String resultado = "{ \"codigo\":\"" + serieNumeroGuia + "\", \"estado\":\"" + estado + "\", \"error\":\"" + error + "\" }";
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


        public String Anular()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_VER];
            guiaRemision.comentarioAnulado =  Request["comentarioAnulado"];
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            movimientoAlmacenBL.AnularMovimientoAlmacen(guiaRemision);
            return JsonConvert.SerializeObject(guiaRemision);
        }

        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> documentoDetalleList)
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];

            foreach (DocumentoDetalleJson documentoDetalleJson in documentoDetalleList)
            {
                DocumentoDetalle documentoDetalle = guiaRemision.pedido.documentoDetalle.Where(d => d.producto.idProducto == Guid.Parse(documentoDetalleJson.idProducto)).FirstOrDefault();
                documentoDetalle.cantidadPorAtender = documentoDetalleJson.cantidad;
            }
            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
            return "{\"cantidad\":\"" + guiaRemision.pedido.documentoDetalle.Count + "\"}";
        }
    }
}