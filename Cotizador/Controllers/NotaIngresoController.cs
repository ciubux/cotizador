﻿using BusinessLayer;
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
                    case Constantes.paginas.BusquedaGuiasRemision: notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoNotaIngreso: notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO]; break;
             //       case Constantes.paginas.BusquedaGuiasRemisionConsolidarFactura: notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_FACTURA_CONSOLIDADA]; break;
                }
                return notaIngreso;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaGuiasRemision: this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoNotaIngreso: this.Session[Constantes.VAR_SESSION_NOTA_INGRESO] = value; break;
              //      case Constantes.paginas.BusquedaGuiasRemisionConsolidarFactura: this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_FACTURA_CONSOLIDADA] = value; break;
                }
            }
        }

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            NotaIngreso notaIngreso = this.NotaIngresoSession;
            return clienteBL.getCLientesBusqueda(data, notaIngreso.ciudadOrigen.idCiudad);
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
            notaIngreso.ciudadOrigen = new Ciudad();
            notaIngreso.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            notaIngreso.pedido = new Pedido();
            notaIngreso.pedido.cliente = new Cliente();
            notaIngreso.pedido.cliente.idCliente = Guid.Empty;

            notaIngreso.fechaTrasladoDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
            notaIngreso.fechaTrasladoHasta = DateTime.Now.AddDays(0);
            notaIngreso.estaFacturado = true;

            this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] = notaIngreso;
        }

        public void CleanBusqueda()
        {
            instanciarNotaIngresoBusqueda();
        }

        public ActionResult Index(Guid? idMovimientoAlmacen = null)
        {


            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.BUSQUEDA_GUIA_REMISION;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] == null)
            {
                instanciarNotaIngresoBusqueda();
            }

            if (this.Session[Constantes.VAR_SESSION_GUIA_LISTA] == null)
            {
                this.Session[Constantes.VAR_SESSION_GUIA_LISTA] = new List<NotaIngreso>();
            }

            NotaIngreso notaIngresoSearch = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA];

            ViewBag.notaIngreso = notaIngresoSearch;
            ViewBag.notaIngresoList = this.Session[Constantes.VAR_SESSION_GUIA_LISTA];
            ViewBag.pagina = Constantes.BUSQUEDA_GUIA_REMISION;

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
            this.Session[Constantes.VAR_SESSION_GUIA_LISTA] = notaIngresoList;
            this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] = notaIngreso;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(notaIngresoList);
            //return pedidoList.Count();
        }

        #endregion

        
        #region Crear Guia

        public Boolean ConsultarSiExisteNotaIngreso()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA];
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
            notaIngreso.motivoTraslado = NotaIngreso.motivosTraslado.Venta;
            notaIngreso.transportista = new Transportista();
            notaIngreso.ciudadOrigen = new Ciudad();
            notaIngreso.ciudadOrigen.idCiudad = Guid.Empty;
            notaIngreso.pedido = new Pedido();
            notaIngreso.pedido.pedidoDetalleList = new List<PedidoDetalle>();
            notaIngreso.pedido.ciudad = new Ciudad();
            notaIngreso.pedido.ubigeoEntrega = new Ubigeo();
            notaIngreso.ciudadOrigen.transportistaList = new List<Transportista>();
            notaIngreso.seguimientoMovimientoAlmacenSalida = new SeguimientoMovimientoAlmacenSalida();
          //  notaIngreso.certificadoInscripcion = ".";
            this.Session[Constantes.VAR_SESSION_GUIA] = notaIngreso;

        }


        public void iniciarAtencionDesdePedido()
        {
            try
            {

                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];

                if (this.Session[Constantes.VAR_SESSION_GUIA] == null)
                {
                    instanciarNotaIngreso();
                }
                NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA];
                notaIngreso.pedido = pedido;

                notaIngreso.motivoTraslado = (NotaIngreso.motivosTraslado)(char)pedido.tipoPedido;
                notaIngreso.transportista = new Transportista();
                // notaIngreso.ciudadOrigen = pedido.ciudad;

                /*  foreach (DocumentoDetalle documentoDetalle in notaIngreso.pedido.documentoDetalle)
                  {
                      documentoDetalle.cantidadPendienteAtencion = documentoDetalle.cantidad;
                      documentoDetalle.cantidadPorAtender = documentoDetalle.cantidad;
                  }*/
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
                notaIngreso.ciudadOrigen = ciudadOrigen;

                notaIngreso.transportista = new Transportista();
                notaIngreso.serieDocumento = ciudadOrigen.serieNotaIngreso;
                notaIngreso.numeroDocumento = ciudadOrigen.siguienteNumeroNotaIngreso;
                TransportistaBL transportistaBL = new TransportistaBL();
                notaIngreso.ciudadOrigen.transportistaList = transportistaBL.getTransportistas(pedido.ciudad.idCiudad);

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
                    //instanciarNotaIngreso();
                }
                NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA];

                ViewBag.fechaTrasladotmp = notaIngreso.fechaTraslado.ToString(Constantes.formatoFecha);
                ViewBag.fechaEmisiontmp = notaIngreso.fechaEmision.ToString(Constantes.formatoFecha);
                ViewBag.notaIngreso = notaIngreso;
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


        public String Create()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.MANTENIMIENTO_GUIA_REMISION;

            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());


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
                error = "DuplicateNumberDocumentException";
            }

            long numeroNotaIngreso = notaIngreso.numero;
            Guid idNotaIngreso = notaIngreso.idMovimientoAlmacen;
            String serieNumeroGuia = notaIngreso.serieNumeroGuia;

            int estado = (int)notaIngreso.seguimientoMovimientoAlmacenSalida.estado;
            /*if (continuarLuego == 1)
            {
                SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = SeguimientoPedido.estadosSeguimientoPedido.Edicion;
                estado = (int)estadosSeguimientoPedido;
                String observacion = "Se continuará editando luego";
               // updateEstadoSeguimientoPedido(idPedido, estadosSeguimientoPedido, observacion);
            }
            notaIngreso = null;*/
            /*String jsonNotaIngresoValidacion = JsonConvert.SerializeObject(notaIngreso.notaIngresoValidacion);

            if(notaIngreso.notaIngresoValidacion.tipoErrorValidacion == NotaIngresoValidacion.TiposErrorValidacion.NoExisteError)
            { 

                this.NotaIngresoSession = null;
            }*/
            //usuarioBL.updatePedidoSerializado(usuario, null);




            String resultado = "";// "{ \"serieNumeroGuia\":\"" + serieNumeroGuia + "\", \"idNotaIngreso\":\"" + idNotaIngreso + "\", \"error\":\"" + error + "\",     \"notaIngresoValidacion\": " + jsonNotaIngresoValidacion + "  }";
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
            NotaIngresoSession.ciudadOrigen.transportistaList.Add(transportista);
            NotaIngresoSession.transportista = transportista;
            this.NotaIngresoSession = notaIngreso;
            return JsonConvert.SerializeObject(transportista);
        }

        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> documentoDetalleList)
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA];

            foreach (DocumentoDetalleJson documentoDetalleJson in documentoDetalleList)
            {
                DocumentoDetalle documentoDetalle = notaIngreso.pedido.documentoDetalle.Where(d => d.producto.idProducto == Guid.Parse(documentoDetalleJson.idProducto)).FirstOrDefault();
                documentoDetalle.cantidadPorAtender = documentoDetalleJson.cantidad;
            }
            this.Session[Constantes.VAR_SESSION_GUIA] = notaIngreso;
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
            this.Session[Constantes.VAR_SESSION_GUIA_VER] = notaIngreso;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonNotaIngreso = JsonConvert.SerializeObject(notaIngreso);
            String json = "{\"usuario\":" + jsonUsuario + ", \"notaIngreso\":" + jsonNotaIngreso + "}";
            return json;
        }

        public ActionResult Print()
        {
            //this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ImprimirNotaIngreso;

            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA_VER];

            ViewBag.notaIngreso = notaIngreso;
            ViewBag.pagina = this.Session[Constantes.VAR_SESSION_PAGINA];

            return View("Print" + notaIngreso.ciudadOrigen.sede.ToUpper().Substring(0, 1));

        }

        public String Anular()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA_VER];
            notaIngreso.comentarioAnulado = Request["comentarioAnulado"];
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            movimientoAlmacenBL.AnularMovimientoAlmacen(notaIngreso);
            return JsonConvert.SerializeObject(notaIngreso);
        }



        #endregion

        
        #region Changes



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
            Ciudad ciudadOrigen = ciudadBL.getCiudad(idCiudad);
            notaIngreso.transportista = new Transportista();
            TransportistaBL transportistaBL = new TransportistaBL();
            ciudadOrigen.transportistaList = transportistaBL.getTransportistas(idCiudad);
            notaIngreso.ciudadOrigen = ciudadOrigen;
            this.NotaIngresoSession = notaIngreso;
            return JsonConvert.SerializeObject(notaIngreso.ciudadOrigen);
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
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA];
            notaIngreso.atencionParcial = Int32.Parse( this.Request.Params["atencionParcial"])==1;

            if (!notaIngreso.atencionParcial)
            {
                foreach (DocumentoDetalle documentoDetalle in notaIngreso.pedido.documentoDetalle)
                {
                    documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadPendienteAtencion;
                }

            }
            


            this.Session[Constantes.VAR_SESSION_GUIA] = notaIngreso;
        }*/

        /*
        public void ChangeUltimaAtencionParcial()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA];
            notaIngreso.ultimaAtencionParcial = Int32.Parse(this.Request.Params["ultimaAtencionParcial"]) == 1;


            if (!notaIngreso.atencionParcial && notaIngreso.ultimaAtencionParcial)
            {
                foreach (DocumentoDetalle documentoDetalle in notaIngreso.documentoDetalle)
                {
                    documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadPendienteAtencion;
                }

            }

            this.Session[Constantes.VAR_SESSION_GUIA] = notaIngreso;
        }

        */


        public String ChangeTransportista()
        {
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA];
         
            if (this.Request.Params["idTransportista"] == null || this.Request.Params["idTransportista"].Equals(String.Empty))
            {
                notaIngreso.transportista = new Transportista();
            }
            else
            {
                Guid idTransportista = Guid.Parse(this.Request.Params["idTransportista"]);
                notaIngreso.transportista = notaIngreso.ciudadOrigen.transportistaList.Where(t => t.idTransportista == idTransportista).FirstOrDefault();
            }
           
            this.Session[Constantes.VAR_SESSION_GUIA] = notaIngreso;
            String jsonTransportista = JsonConvert.SerializeObject(notaIngreso.transportista);
            return jsonTransportista;
        }

        public ActionResult CancelarCreacionNotaIngreso()
        {
            this.Session[Constantes.VAR_SESSION_GUIA] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("Index");
        }


        public String Descargar()
        {
            String nombreArchivo = Request["nombreArchivo"].ToString();
            NotaIngreso notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA];
            if(notaIngreso == null)
                notaIngreso = (NotaIngreso)this.Session[Constantes.VAR_SESSION_GUIA_VER];

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