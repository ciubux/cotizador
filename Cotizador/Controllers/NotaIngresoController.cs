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
    public class NotaIngresoController : Controller
    {

        private NotaIngreso NotaIngresoSession
        {
            get
            {

                NotaIngreso notaIngreso = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaNotasIngreso: notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoNotaIngreso: notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO]; break;
             //       case Constantes.paginas.BusquedaGuiasRemisionConsolidarFactura: notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA_FACTURA_CONSOLIDADA]; break;
                }
                return notaIngreso;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaNotasIngreso: this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoNotaIngreso: this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = value; break;
              //      case Constantes.paginas.BusquedaGuiasRemisionConsolidarFactura: this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA_FACTURA_CONSOLIDADA] = value; break;
                }
            }
        }

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            return clienteBL.getCLientesBusqueda(data, notaIngreso.ciudadDestino.idCiudad);
        }

        public String GetCliente()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            notaIngreso.pedido.cliente = clienteBl.getCliente(idCliente);
            String resultado = JsonConvert.SerializeObject(notaIngreso.pedido.cliente);
            this.NotaIngresoSession = notaIngreso;
            return resultado;
        }



        #region Busqueda Guias

        private void instanciarNotaIngresoBusqueda()
        {
            NotaIngreso notaIngreso = new NotaIngreso();
            notaIngreso.seguimientoMovimientoAlmacenSalida = new SeguimientoMovimientoAlmacenSalida();
            notaIngreso.seguimientoMovimientoAlmacenSalida.estado = SeguimientoMovimientoAlmacenSalida.estadosSeguimientoMovimientoAlmacenSalida.Enviado;
            notaIngreso.ciudadDestino = new Ciudad();
            notaIngreso.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            notaIngreso.pedido = new Pedido();
            notaIngreso.pedido.cliente = new Cliente();
            notaIngreso.pedido.cliente.idCliente = Guid.Empty;

            notaIngreso.fechaTrasladoDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
            notaIngreso.fechaTrasladoHasta = DateTime.Now.AddDays(0);
            notaIngreso.estaFacturado = true;

            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA] = notaIngreso;
        }

        public void CleanBusqueda()
        {
            instanciarNotaIngresoBusqueda();
        }

        public ActionResult Index(Guid? idMovimientoAlmacen = null)
        {


            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaGuiasRemision;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA] == null)
            {
                instanciarNotaIngresoBusqueda();
            }

            if (this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_LISTA] == null)
            {
                this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_LISTA] = new List<NotaIngreso>();
            }

            NotaIngreso notaIngresoSearch = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA];

            ViewBag.notaIngreso = notaIngresoSearch;
            ViewBag.notaIngresoList = this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_LISTA];
            ViewBag.pagina = Constantes.paginas.BusquedaGuiasRemision;

            ViewBag.fechaTrasladoDesde = notaIngresoSearch.fechaTrasladoDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaTrasladoHasta = notaIngresoSearch.fechaTrasladoHasta.ToString(Constantes.formatoFecha);
            ViewBag.Si = Constantes.MENSAJE_SI;
            ViewBag.No = Constantes.MENSAJE_NO;

            int existeCliente = 0;
            if (notaIngresoSearch.pedido.cliente.idCliente != Guid.Empty)
            {
                existeCliente = 1;
            }

            Pedido pedido = new Pedido();

            ViewBag.pedido = pedido;

            DocumentoVenta documentoVenta = new DocumentoVenta();
            ViewBag.documentoVenta = documentoVenta;

            ViewBag.existeCliente = existeCliente;


            /*     int mostrarGuia = 0;
                 if (idMovimientoAlmacen != null)
                 {
                     mostrarGuia = 1;
                 }

                 ViewBag.mostrarGuia = mostrarGuia;*/
            ViewBag.idMovimientoAlmacen = idMovimientoAlmacen;

            return View();
        }

     
        public String Search()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaNotasIngreso;

            //Se recupera el pedido Búsqueda de la session
            NotaIngreso notaIngreso = this.NotaIngresoSession;

            String[] movDesde = this.Request.Params["fechaTrasladoDesde"].Split('/');
            notaIngreso.fechaTrasladoDesde = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]));

            String[] movHasta = this.Request.Params["fechaTrasladoHasta"].Split('/');
            notaIngreso.fechaTrasladoHasta = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);


            if (this.Request.Params["numeroDocumento"] == null || this.Request.Params["numeroDocumento"].Trim().Length == 0)
            {
                notaIngreso.numeroDocumento = 0;
            }
            else
            {
                notaIngreso.numeroDocumento = int.Parse(this.Request.Params["numeroDocumento"]);
            }

            if (this.Request.Params["numeroPedido"] == null || this.Request.Params["numeroPedido"].Trim().Length == 0)
            {
                notaIngreso.pedido.numeroPedido = 0;
            }
            else
            {
                notaIngreso.pedido.numeroPedido = int.Parse(this.Request.Params["numeroPedido"]);
            }

            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();


            List<NotaIngreso> notaIngresoList = movimientoAlmacenBL.GetNotasIngreso(notaIngreso);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_LISTA] = notaIngresoList;
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA] = notaIngreso;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(notaIngresoList);
            //return pedidoList.Count();
        }

        #endregion

        
        #region Crear Guia

        public Boolean ConsultarSiExisteNotaIngreso()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
            if (notaIngreso == null)
            {
                return false;
            }
            else
                return true;
        }

        private void instanciarNotaIngreso()
        {
            NotaIngreso notaIngreso = new NotaIngreso();
            notaIngreso.fechaEmision = DateTime.Now;
            notaIngreso.fechaTraslado = DateTime.Now;
            notaIngreso.motivoTraslado = NotaIngreso.motivosTraslado.Compra;
            notaIngreso.transportista = new Transportista();
            notaIngreso.ciudadDestino = new Ciudad();
            notaIngreso.ciudadDestino.idCiudad = Guid.Empty;
            notaIngreso.pedido = new Pedido();
            notaIngreso.pedido.pedidoDetalleList = new List<PedidoDetalle>();
            notaIngreso.pedido.ciudad = new Ciudad();
            notaIngreso.pedido.ubigeoEntrega = new Ubigeo();
            notaIngreso.ciudadDestino.transportistaList = new List<Transportista>();
            notaIngreso.seguimientoMovimientoAlmacenEntrada = new SeguimientoMovimientoAlmacenEntrada();
            //  notaIngreso.certificadoInscripcion = ".";
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;

        }


        public void iniciarAtencionDesdePedido()
        {
            try
            {

                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_VER];

                if (this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] == null)
                {
                    instanciarNotaIngreso();
                }
                NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
                notaIngreso.pedido = pedido;

                notaIngreso.motivoTraslado = (NotaIngreso.motivosTraslado)(char)pedido.tipoPedidoCompra;
                notaIngreso.transportista = new Transportista();

                notaIngreso.observaciones = String.Empty;
                if (pedido.numeroReferenciaCliente != null && !pedido.numeroReferenciaCliente.Trim().Equals(String.Empty))
                {
                    if (pedido.numeroReferenciaCliente.Contains("O/C"))
                    {
                        notaIngreso.observaciones = pedido.numeroReferenciaCliente.Trim() + " ";
                    }
                    else
                    {
                        notaIngreso.observaciones = "O/C N° " + pedido.numeroReferenciaCliente.Trim() + " ";
                    }
                }
                notaIngreso.observaciones = notaIngreso.observaciones + pedido.observacionesGuiaRemision;

                CiudadBL ciudadBL = new CiudadBL();


                Ciudad ciudadOrigen = ciudadBL.getCiudad(pedido.ciudad.idCiudad);



                notaIngreso.ciudadDestino = ciudadOrigen;

                notaIngreso.transportista = new Transportista();
                notaIngreso.serieDocumento = ciudadOrigen.serieNotaIngreso;
                notaIngreso.numeroDocumento = ciudadOrigen.siguienteNumeroNotaIngreso;
                TransportistaBL transportistaBL = new TransportistaBL();
                notaIngreso.ciudadDestino.transportistaList = transportistaBL.getTransportistas(pedido.ciudad.idCiudad);

            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }

        }


        public ActionResult Ingresar()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoNotaIngreso;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (usuario == null || !usuario.creaGuias)
            {
                return RedirectToAction("Login", "Account");
            }



            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoNotaIngreso;
            try
            {
                if (this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] == null)
                {
                    return View("IngresarEmpty");
                    //instanciarNotaIngreso();
                }
                NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];

                SerieDocumentoBL serieDocumentoBL = new SerieDocumentoBL();
                notaIngreso.ciudadDestino.serieDocumentoElectronicoList = serieDocumentoBL.getSeriesDocumento(notaIngreso.ciudadDestino.idCiudad);
                notaIngreso.serieDocumentoElectronico = notaIngreso.ciudadDestino.serieDocumentoElectronicoList[0];
                notaIngreso.serieDocumento = notaIngreso.serieDocumentoElectronico.serie;
                notaIngreso.numeroDocumento = notaIngreso.serieDocumentoElectronico.siguienteNumeroNotaIngreso;

                ViewBag.fechaTrasladotmp = notaIngreso.fechaTraslado.ToString(Constantes.formatoFecha);
                ViewBag.fechaEmisiontmp = notaIngreso.fechaEmision.ToString(Constantes.formatoFecha);
                ViewBag.notaIngreso = notaIngreso;

             //   ViewBag.serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList;

            }
            catch (Exception ex)
            {
                usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }

            ViewBag.pagina = Constantes.paginas.MantenimientoNotaIngreso;
            return View();
        }


        public String Create()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoNotaIngreso;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];            
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            notaIngreso.usuario = usuario;

            String error = String.Empty;
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            try
            {
                movimientoAlmacenBL.InsertMovimientoAlmacenEntrada(notaIngreso);
            }
            catch (DuplicateNumberDocumentException ex)
            {
                error = ex.Message;
            }

            long numeroNotaIngreso = notaIngreso.numero;
            Guid idNotaIngreso = notaIngreso.idMovimientoAlmacen;
            String serieNumeroNotaIngreso = notaIngreso.serieNumeroNotaIngreso;

            int estado = (int)notaIngreso.seguimientoMovimientoAlmacenEntrada.estado;

            String jsonNotaIngresoValidacion = JsonConvert.SerializeObject(notaIngreso.notaIngresoValidacion);

            if (notaIngreso.notaIngresoValidacion.tipoErrorValidacion == NotaIngresoValidacion.TiposErrorValidacion.NoExisteError)
            {
                this.NotaIngresoSession = null;
            }

            String resultado = "{ \"serieNumeroNotaIngreso\":\"" + serieNumeroNotaIngreso + "\", \"idNotaIngreso\":\"" + idNotaIngreso + "\", \"error\":\"" + error + "\",     \"notaIngresoValidacion\": " + jsonNotaIngresoValidacion + "  }";
            return resultado;
        }

        public String CreateTransportistaTemporal()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            Transportista transportista = new Transportista();
            transportista.descripcion = Request["descripcion"];
            transportista.direccion = Request["direccion"];
            transportista.ruc = Request["ruc"];
            transportista.telefono = Request["telefono"];
            transportista.idTransportista = Guid.Empty;
            NotaIngresoSession.ciudadDestino.transportistaList.Add(transportista);
            NotaIngresoSession.transportista = transportista;
            this.NotaIngresoSession = notaIngreso;
            return JsonConvert.SerializeObject(transportista);
        }

        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> documentoDetalleList)
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];

            foreach (DocumentoDetalleJson documentoDetalleJson in documentoDetalleList)
            {
                DocumentoDetalle documentoDetalle = notaIngreso.pedido.documentoDetalle.Where(d => d.producto.idProducto == Guid.Parse(documentoDetalleJson.idProducto)).FirstOrDefault();
                documentoDetalle.cantidadPorAtender = documentoDetalleJson.cantidad;
            }
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;
            return "{\"cantidad\":\"" + notaIngreso.pedido.documentoDetalle.Count + "\"}";
        }
        #endregion


        #region Acciones en Guia

        public String Show()
        {
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            NotaIngreso notaIngreso = new NotaIngreso();
            notaIngreso.idMovimientoAlmacen = Guid.Parse(Request["idMovimientoAlmacen"].ToString());
            notaIngreso = movimientoAlmacenBL.GetNotaIngreso(notaIngreso);
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_VER] = notaIngreso;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonNotaIngreso = JsonConvert.SerializeObject(notaIngreso);
            String json = "{\"usuario\":" + jsonUsuario + ", \"notaIngreso\":" + jsonNotaIngreso + "}";
            return json;
        }

        public ActionResult Print()
        {
            //this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ImprimirNotaIngreso;

            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_VER];

            ViewBag.notaIngreso = notaIngreso;
            ViewBag.pagina = this.Session[Constantes.VAR_SESSION_PAGINA];

            return View("Print" + notaIngreso.ciudadDestino.sede.ToUpper().Substring(0, 1));

        }

        public String Anular()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_VER];
            notaIngreso.comentarioAnulado = Request["comentarioAnulado"];
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            movimientoAlmacenBL.AnularMovimientoAlmacen(notaIngreso);
            return JsonConvert.SerializeObject(notaIngreso);
        }



        #endregion


        #region Changes

        public String UpdateSerieDocumento()
        {
            String serieDocumento = this.Request.Params["serieDocumento"];
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            SerieDocumentoElectronico serieDocumentoElectronico = notaIngreso.ciudadDestino.serieDocumentoElectronicoList.Where(s => s.serie == serieDocumento).FirstOrDefault();
            notaIngreso.serieDocumento = serieDocumentoElectronico.serie;
            notaIngreso.numeroDocumento = serieDocumentoElectronico.siguienteNumeroNotaIngreso;
            return notaIngreso.numeroDocumentoString;
        }

        public void ChangeEstaAnulado()
        {
            this.NotaIngresoSession.estaAnulado = Int32.Parse(this.Request.Params["estaAnulado"]) == 1;
        }

        public void ChangeEstaFacturado()
        {
            this.NotaIngresoSession.estaFacturado = Int32.Parse(this.Request.Params["estaFacturado"]) == 1;
        }

        public void ChangeFechaTrasladoDesde()
        {
            String[] movDesde = this.Request.Params["fechaTrasladoDesde"].Split('/');
            this.NotaIngresoSession.fechaTrasladoDesde = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]));
        }

        public void ChangeFechaTraslado()
        {
            String[] movHasta = this.Request.Params["fechaTraslado"].Split('/');
            this.NotaIngresoSession.fechaTraslado = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);
        }

        public void ChangeFechaEmision()
        {
            String[] movDesde = this.Request.Params["fechaEmision"].Split('/');
            this.NotaIngresoSession.fechaEmision = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]));
        }

        public void ChangeFechaTrasladoHasta()
        {
            String[] movHasta = this.Request.Params["fechaTrasladoHasta"].Split('/');
            this.NotaIngresoSession.fechaTrasladoHasta = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);
        }


        public String ChangeIdCiudad()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadDestino = ciudadBL.getCiudad(idCiudad);
            notaIngreso.transportista = new Transportista();
            TransportistaBL transportistaBL = new TransportistaBL();
            ciudadDestino.transportistaList = transportistaBL.getTransportistas(idCiudad);
            notaIngreso.ciudadDestino = ciudadDestino;
            this.NotaIngresoSession = notaIngreso;
            return JsonConvert.SerializeObject(notaIngreso.ciudadDestino);
        }

        public void ChangeInputString()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            PropertyInfo propertyInfo = notaIngreso.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(notaIngreso, this.Request.Params["valor"]);
            this.NotaIngresoSession = notaIngreso;
        }

        public void ChangeInputStringTransportista()
        {
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            PropertyInfo propertyInfo = notaIngreso.transportista.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(notaIngreso.transportista, this.Request.Params["valor"]);
            this.NotaIngresoSession = notaIngreso;
        }

       /* public void ChangeAtencionParcial()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
            notaIngreso.atencionParcial = Int32.Parse( this.Request.Params["atencionParcial"])==1;

            if (!notaIngreso.atencionParcial)
            {
                foreach (DocumentoDetalle documentoDetalle in notaIngreso.pedido.documentoDetalle)
                {
                    documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadPendienteAtencion;
                }

            }
            


            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;
        }*/

        /*
        public void ChangeUltimaAtencionParcial()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
            notaIngreso.ultimaAtencionParcial = Int32.Parse(this.Request.Params["ultimaAtencionParcial"]) == 1;


            if (!notaIngreso.atencionParcial && notaIngreso.ultimaAtencionParcial)
            {
                foreach (DocumentoDetalle documentoDetalle in notaIngreso.documentoDetalle)
                {
                    documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadPendienteAtencion;
                }

            }

            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;
        }

        */


        public String ChangeTransportista()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
         
            if (this.Request.Params["idTransportista"] == null || this.Request.Params["idTransportista"].Equals(String.Empty))
            {
                notaIngreso.transportista = new Transportista();
            }
            else
            {
                Guid idTransportista = Guid.Parse(this.Request.Params["idTransportista"]);
                notaIngreso.transportista = notaIngreso.ciudadDestino.transportistaList.Where(t => t.idTransportista == idTransportista).FirstOrDefault();
            }
           
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = notaIngreso;
            String jsonTransportista = JsonConvert.SerializeObject(notaIngreso.transportista);
            return jsonTransportista;
        }

        public ActionResult CancelarCreacionNotaIngreso()
        {
            this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("Index");
        }


        public String Descargar()
        {
            String nombreArchivo = Request["nombreArchivo"].ToString();
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO];
            if(notaIngreso == null)
                notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_VER];

            Pedido pedido = notaIngreso.pedido; ;

            //      pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            PedidoAdjunto pedidoAdjunto = pedido.pedidoAdjuntoList.Where(p => p.nombre.Equals(nombreArchivo)).FirstOrDefault();

            if (pedidoAdjunto != null)
            {
                return JsonConvert.SerializeObject(pedidoAdjunto);
            }
            else
            {
                return null;
            }

        }



        #endregion



    }
}