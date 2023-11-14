using BusinessLayer;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using DataLayer;

namespace Cotizador.Controllers
{
    public class VentaController : Controller
    {
        // GET: Venta
        public ActionResult Index()
        {
            return View();
        }


        public void iniciarEdicionVenta()
        {
            Venta ventaVer = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];
            VentaBL ventaBL = new VentaBL();
            Venta venta = new Venta();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            venta = ventaBL.GetVenta(ventaVer, usuario);
            //Temporal
            Pedido pedido = venta.pedido;
            pedido.ciudadASolicitar = new Ciudad();

            this.Session[Constantes.VAR_SESSION_VENTA] = venta;
        }


        public String Show()
        {
            VentaBL ventaBL = new VentaBL();
            Venta venta = new Venta();
            venta.guiaRemision = new GuiaRemision();
            venta.guiaRemision.idMovimientoAlmacen = Guid.Parse(Request["idMovimientoAlmacen"].ToString());
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            venta = ventaBL.GetVenta(venta, usuario);
            this.Session[Constantes.VAR_SESSION_VENTA_VER] = venta;

            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonVenta = JsonConvert.SerializeObject(venta);

            Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == venta.pedido.ciudad.idCiudad).FirstOrDefault();

            string jsonSeries = "[]";
            if (ciudad != null)
            {
                List<SerieDocumentoElectronico> serieDocumentoElectronicoList = new List<SerieDocumentoElectronico>();
                SerieDocumentoBL serieDocumentoBL = new SerieDocumentoBL();
                ciudad.serieDocumentoElectronicoList = serieDocumentoBL.getSeriesDocumento(ciudad.idCiudad, usuario.idEmpresa);
                
                jsonSeries = JsonConvert.SerializeObject(ciudad.serieDocumentoElectronicoList);

            }
            this.Session["s_cambioclientefactura_cambio"] = false;
            String json = "{\"serieDocumentoElectronicoList\":" + jsonSeries + ", \"venta\":" + jsonVenta + "}";
            return json;
        }

        public String CambiarListadoSeries()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String response = "{\"success\": 0}";

            if (usuario.creaGuias)
            {
                SerieDocumentoBL serieBL = new SerieDocumentoBL();
                GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];

                SerieDocumentoElectronico serie = serieBL.getSerieDocumento("VENTA", guiaRemision.ciudadOrigen.idCiudad, usuario.idEmpresa);

                if (serie.sedeMP != null)
                {
                    guiaRemision.serieDocumento = serie.serie;
                    guiaRemision.numeroDocumento = serie.siguienteNumeroGuiaRemision;
                    this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;

                    response = "{\"success\": 1,  \"serie\":\"" + serie.serie + "\", \"numero\":\"" + serie.siguienteNumeroGuiaRemision.ToString() + "\", \"serieNumeroString\":\"" + guiaRemision.serieNumeroGuia + "\"}";
                }
            }

            return response;
        }

        [HttpPost]
        public void CreateVentaRefacturacion()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_VER];
            Venta venta = new Venta();
            venta.guiaRemision = guiaRemision;
            VentaBL ventaBL = new VentaBL();
            ventaBL.InsertVentaRefacturacion(venta);
        }




        [HttpPost]
        public String generarVentaConsolidada(List<DocumentoDetalleJson> documentoDetalleJsonList)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            List<Guid> guiaRemisionIdList = (List<Guid>)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS];

            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_CONSOLIDADA];

            VentaBL ventaBL = new VentaBL();
            Venta venta = new Venta();
            //venta.guiaRemision = new GuiaRemision();
            venta.guiaRemision = guiaRemision;//.idMovimientoAlmacen;
            venta.usuario = usuario;
            venta.pedido = new Pedido();
            venta.pedido.numeroReferenciaCliente = guiaRemision.pedido.numeroReferenciaCliente;
            venta.pedido.numeroReferenciaAdicional = guiaRemision.pedido.numeroReferenciaAdicional;
            venta.pedido.pedidoDetalleList = new List<PedidoDetalle>();

            DocumentoVenta documentoVenta = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_RESUMEN_CONSOLIDADO];

            foreach (VentaDetalle ventaDetalle in documentoVenta.ventaDetalleList)
            {
                DocumentoDetalleJson documentoDetalleJson = documentoDetalleJsonList.Where(d => Guid.Parse(d.idProducto) == ventaDetalle.producto.idProducto).FirstOrDefault();
                PedidoDetalle pedidoDetalle = new PedidoDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                //pedidoDetalle.producto = new Producto();
                pedidoDetalle.producto = ventaDetalle.producto;
                pedidoDetalle.cantidad = documentoDetalleJson.cantidad;
                ///Importante definir como null para que se recupere del producto al momento de insertar
                pedidoDetalle.unidadInternacional = null;






                pedidoDetalle.esPrecioAlternativo = documentoDetalleJson.esUnidadAlternativa == 1;

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    pedidoDetalle.ProductoPresentacion.Equivalencia = ventaDetalle.producto.ProductoPresentacionList[0].Equivalencia;

                    
                    //////REVISAR QUE LA CANTIDAD DEBE SER SIEMPRE ENTERO
                    if (pedidoDetalle.ProductoPresentacion.Equivalencia < 1)
                    {
                        pedidoDetalle.unidad = ventaDetalle.producto.ProductoPresentacionList[0].Presentacion;
                        pedidoDetalle.cantidad = documentoDetalleJson.cantidad;
                    }
                    else
                    {
                        pedidoDetalle.unidad = ventaDetalle.producto.unidad_alternativa;
                        pedidoDetalle.cantidad = Convert.ToInt32(ventaDetalle.sumCantidadUnidadAlternativa);
                    }
                    pedidoDetalle.precioNeto = (ventaDetalle.sumPrecioUnitario * pedidoDetalle.ProductoPresentacion.Equivalencia) / pedidoDetalle.cantidad;
                }
                else
                {
                    pedidoDetalle.unidad = ventaDetalle.producto.unidad;
                    //////REVISAR QUE LA CANTIDAD DEBE SER SIEMPRE ENTERO
                    pedidoDetalle.cantidad = Convert.ToInt32(ventaDetalle.sumCantidadUnidadEstandar);
                    pedidoDetalle.precioNeto = ventaDetalle.sumPrecioUnitario / pedidoDetalle.cantidad;
                }
                venta.pedido.pedidoDetalleList.Add(pedidoDetalle);
            }

            ventaBL.procesarVenta(venta);

            ventaBL.InsertVentaConsolidada(venta);

            ventaBL.GetVentaConsolidada(venta, usuario);

            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales(venta.pedido);

            this.Session[Constantes.VAR_SESSION_VENTA_VER] = venta;

            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonVenta = JsonConvert.SerializeObject(venta);

            Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == venta.pedido.ciudad.idCiudad).FirstOrDefault();

            string jsonSeries = "[]";
            if (ciudad != null)
            {
                var serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
                jsonSeries = JsonConvert.SerializeObject(serieDocumentoElectronicoList);

            }

            this.Session["s_cambioclientefactura_cambio"] = false;

            String json = "{\"serieDocumentoElectronicoList\":" + jsonSeries + ", \"venta\":" + jsonVenta + "}";
            return json;
        }

        [HttpPost]
        public String obtenerVentaConsolidada(List<DocumentoDetalleJson> documentoDetalleJsonList)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //List<Guid> guiaRemisionIdList = (List<Guid>)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS];

            //GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_CONSOLIDADA];

            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];

            VentaBL ventaBL = new VentaBL();
            ventaBL.GetVentaConsolidada(venta, usuario);

            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales(venta.pedido);

            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonVenta = JsonConvert.SerializeObject(venta);

            Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == venta.pedido.ciudad.idCiudad).FirstOrDefault();

            string jsonSeries = "[]";
            if (ciudad != null)
            {
                var serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
                jsonSeries = JsonConvert.SerializeObject(serieDocumentoElectronicoList);
            }

            String json = "{\"serieDocumentoElectronicoList\":" + jsonSeries + ", \"venta\":" + jsonVenta + "}";
            return json;
        }






















        public void iniciarEdicionVentaConsolidada()
        {
            this.Session[Constantes.VAR_SESSION_VENTA] = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];
        }


        /*

        public String GetVentaConsolidada(String idMovimientoAlmacenList)
        {
            VentaBL ventaBL = new VentaBL();
            Venta venta = new Venta();
            venta.guiaRemision = new GuiaRemision();
            venta.guiaRemision.idMovimientoAlmacen = Guid.Parse(Request["idMovimientoAlmacen"].ToString());
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            venta = ventaBL.GetVenta(venta, usuario);
            this.Session[Constantes.VAR_SESSION_VENTA_VER] = venta;

            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonVenta = JsonConvert.SerializeObject(venta);

            Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == venta.pedido.ciudad.idCiudad).FirstOrDefault();

            string jsonSeries = "[]";
            if (ciudad != null)
            {
                var serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
                jsonSeries = JsonConvert.SerializeObject(serieDocumentoElectronicoList);

            }

            String json = "{\"serieDocumentoElectronicoList\":" + jsonSeries + ", \"venta\":" + jsonVenta + "}";
            return json;
        }
        */




        public String Descargar()
        {/*
           

            Pedido pedido = venta.pedido; ;

            //      pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            PedidoAdjunto pedidoAdjunto = pedido.pedidoAdjuntoList.Where(p => p.nombre.Equals(nombreArchivo)).FirstOrDefault();

            if (pedidoAdjunto != null)
            {
                return JsonConvert.SerializeObject(pedidoAdjunto);
            }
            else
            {
                return null;
            }*/

            String nombreArchivo = Request["nombreArchivo"].ToString();
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA];
            if (venta == null)
                venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];


            Pedido pedido = venta.pedido;

            ArchivoAdjunto archivoAdjunto = pedido.pedidoAdjuntoList.Where(p => p.nombre.Equals(nombreArchivo)).FirstOrDefault();

            if (archivoAdjunto != null)
            {
                ArchivoAdjuntoBL archivoAdjuntoBL = new ArchivoAdjuntoBL();
                archivoAdjunto = archivoAdjuntoBL.GetArchivoAdjunto(archivoAdjunto);
                return JsonConvert.SerializeObject(archivoAdjunto);
            }
            else
            {
                return null;
            }

        }

        public ActionResult Vender()
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoVenta;

                //Si no hay usuario, se dirige el logueo
                if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                    if (!usuario.creaDocumentosVenta)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                ViewBag.debug = Constantes.DEBUG;
                ViewBag.Si = Constantes.MENSAJE_SI;
                ViewBag.No = Constantes.MENSAJE_NO;
                ViewBag.IGV = Constantes.IGV;


                //Si no se está trabajando con una cotización se crea una y se agrega a la sesion

                /*if (this.Session[Constantes.VAR_SESSION_PEDIDO] == null)
                {

                    instanciarPedido();
                }*/
                Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA];
                Pedido pedido = venta.pedido;
                pedido.moneda = venta.moneda;


                int existeCliente = 0;
                if (pedido.cliente.idCliente != Guid.Empty)
                {
                    existeCliente = 1;
                }

                ViewBag.existeCliente = existeCliente;
                ViewBag.idClienteGrupo = pedido.cliente.idCliente;
                ViewBag.clienteGrupo = pedido.cliente.ToString();

                ViewBag.fechaSolicitud = pedido.fechaSolicitud.ToString(Constantes.formatoFecha);
                ViewBag.horaSolicitud = pedido.fechaSolicitud.ToString(Constantes.formatoHora);

                ViewBag.fechaEntregaDesde = pedido.fechaEntregaDesde == null ? "" : pedido.fechaEntregaDesde.Value.ToString(Constantes.formatoFecha);
                ViewBag.fechaEntregaHasta = pedido.fechaEntregaHasta == null ? "" : pedido.fechaEntregaHasta.Value.ToString(Constantes.formatoFecha);



                ViewBag.pedido = pedido;
                ViewBag.VARIACION_PRECIO_ITEM_PEDIDO = Constantes.VARIACION_PRECIO_ITEM_PEDIDO;


                ViewBag.fechaPrecios = pedido.fechaPrecios.ToString(Constantes.formatoFecha);

            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }

            ViewBag.pagina = (int)Constantes.paginas.MantenimientoVenta;
            return View();
        }


        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA];

            IDocumento documento = (Pedido)venta.pedido;
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales((Pedido)documento);
            venta.pedido = (Pedido)documento;

            this.Session[Constantes.VAR_SESSION_VENTA] = venta;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }




        public String Update()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA];
            venta.usuario = usuario;
            VentaBL bl = new VentaBL();
            bl.UpdateVenta(venta);


            long numeroPedido = venta.pedido.numeroPedido;
            String numeroPedidoString = venta.pedido.numeroPedidoString;


            var v = new { numeroPedido = numeroPedidoString };
            String resultado = JsonConvert.SerializeObject(v);

            //String resultado = "{ \"codigo\":\"" + numeroPedido + "\", \"estado\":\"" + estado + "\", \"observacion\":\"" + observacion + "\" }";
            return resultado;
        }

        /***********************************************************************/

        [HttpGet]
        public ActionResult Lista(Guid? idMovimientoAlmacen = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaVenta;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA] == null)
            {
                instanciarBusquedaVenta();
            }

            Venta objSearch = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA];
            Venta guiaRemisionSearch = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA];
            int existeCliente = 0;
            if (objSearch.guiaRemision.pedido.cliente.idCliente != Guid.Empty)
            {
                existeCliente = 1;
            }           
            
            ViewBag.Venta = objSearch;

            ViewBag.guiaRemision = guiaRemisionSearch.guiaRemision;

            ViewBag.pagina = (int)Constantes.paginas.BusquedaVenta;

            ViewBag.pedido = this.VentaSession.pedido;

            ViewBag.existeCliente = existeCliente;

            ViewBag.venta_fechaEmisionDesde = objSearch.guiaRemision.fechaEmisionDesde.ToString(Constantes.formatoFecha);
            ViewBag.venta_fechaEmisionHasta = objSearch.guiaRemision.fechaEmisionHasta.ToString(Constantes.formatoFecha);

            return View();
        }


        private void instanciarBusquedaVenta()
        {
            Venta obj = new Venta();
            obj.pedido = new Pedido();
            obj.pedido.cliente = new Cliente();
            obj.pedido.cliente.idCliente = Guid.Empty;
            obj.usuario = new Usuario();
            obj.guiaRemision = new GuiaRemision();
            obj.guiaRemision.ciudadOrigen = new Ciudad();
            obj.guiaRemision.ciudadOrigen.idCiudad = Guid.Empty;
            obj.ciudad = new Ciudad();
            obj.guiaRemision.pedido = new Pedido();
            obj.guiaRemision.pedido.cliente = new Cliente();
            obj.guiaRemision.numero = 0;
            obj.guiaRemision.fechaEmisionDesde = DateTime.Now.AddDays(-10);
            obj.guiaRemision.fechaEmisionHasta = DateTime.Now.AddDays(1);
            obj.guiaRemision.motivoTrasladoBusqueda = GuiaRemision.motivosTrasladoBusqueda.Venta;
            obj.guiaRemision.sku = String.Empty;
            obj.pedido.numeroPedido = 0;

            obj.documentoVenta = new DocumentoVenta();
            obj.documentoVenta.serie = "0";
            obj.documentoVenta.numero = "0";
            obj.documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Todos;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.usuario = usuario;
            this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA] = obj;
        }

        public String ChangeIdCiudad()
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA];

            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }
            venta.guiaRemision.ciudadOrigen.idCiudad = idCiudad;
            this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA] = venta;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";

        }

        public void ChangeInputIntPedido()
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA];
            PropertyInfo propertyInfo = venta.pedido.GetType().GetProperty(this.Request.Params["propiedad"]);
            try
            {
                propertyInfo.SetValue(venta.pedido, Int64.Parse(this.Request.Params["valor"]));
            }
            catch (Exception e)
            {
                propertyInfo.SetValue(venta.pedido, 0);
            }
            this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA] = venta;
        }


        public void ChangeInputIntGuia()
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA];
            PropertyInfo propertyInfo = venta.guiaRemision.GetType().GetProperty(this.Request.Params["propiedad"]);
            try
            {
                propertyInfo.SetValue(venta.guiaRemision, Int64.Parse(this.Request.Params["valor"]));
            }
            catch (Exception e)
            {
                propertyInfo.SetValue(venta.guiaRemision, 0);
            }
            this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA] = venta;
        }

        public void ChangeFechaEmisionDesde()
        {
            Venta obj = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA];
            String[] fecha = this.Request.Params["fechaEmisionDesde"].Split('/');
            obj.guiaRemision.fechaEmisionDesde = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA] = obj;
        }

        public void ChangeFechaEmisionHasta()
        {
            Venta obj = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA];
            String[] fecha = this.Request.Params["fechaEmisionHasta"].Split('/');
            obj.guiaRemision.fechaEmisionHasta = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA] = obj;
        }

        public void CleanBusqueda()
        {
            instanciarBusquedaVenta();
        }

        private Venta VentaSession
        {
            get
            {
                Venta obj = null;

                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {

                    case Constantes.paginas.BusquedaVenta: obj = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaVenta: this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA] = value; break;
                }
            }
        }



        public String SearchList()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaVenta;
            if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
            {
                this.VentaSession.documentoVenta.numero = "0";
            }
            else
            {
                this.VentaSession.documentoVenta.numero = this.Request.Params["numero"];
            }
            this.VentaSession.documentoVenta.tipoDocumento = (DocumentoVenta.TipoDocumento)Int32.Parse(this.Request.Params["tipoDocumento"]);

            Venta obj = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA];
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            VentaBL bL = new VentaBL();
            List<Venta> list = bL.getListVenta(obj);

            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
        }

        public String GetCliente()
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA];
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            venta.guiaRemision.pedido.cliente = clienteBl.getCliente(idCliente);
            String resultado = JsonConvert.SerializeObject(venta.guiaRemision.pedido.cliente);
            this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA] = venta;
            return resultado;
        }

        public void ChangeInputString()
        {
            Venta obj = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA];
            PropertyInfo propertyInfo = obj.guiaRemision.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj.guiaRemision, this.Request.Params["valor"]);
            this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA] = obj;
        }

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_BUSQUEDA];
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            return clienteBL.getCLientesBusqueda(data, venta.guiaRemision.ciudadOrigen.idCiudad, usuario.idUsuario);
        }

        public String ShowList()
        {
            int? rectificar_venta = null;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (usuario.rectificarVenta == true)
            {
                rectificar_venta = 1;
            }
            VentaBL ventaBL = new VentaBL();
            Venta venta = new Venta();
            venta.guiaRemision = new GuiaRemision();
            venta.guiaRemision.idMovimientoAlmacen = Guid.Parse(Request["idMovimientoAlmacen"].ToString());
            venta.idVenta = Guid.Parse(Request["idVenta"].ToString());
            venta = ventaBL.GetVentaList(venta, usuario);
            this.Session[Constantes.VAR_SESSION_VENTA_VER] = venta;            
            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonVenta = JsonConvert.SerializeObject(venta);
            dynamic jsonPermisorectificarventa = JsonConvert.DeserializeObject("{'Permiso': '" + rectificar_venta + "'}");
            
            Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == venta.pedido.ciudad.idCiudad).FirstOrDefault();

            string jsonSeries = "[]";
            if (ciudad != null)
            {
                var serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
                jsonSeries = JsonConvert.SerializeObject(serieDocumentoElectronicoList);
            }


            String json = "{\"serieDocumentoElectronicoList\":" + jsonSeries + ", \"venta\":" + jsonVenta + ",\"PermisoRectificarVenta\":" + jsonPermisorectificarventa + "}";
            return json;
        }

        public void RectificarVentaCheck()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Guid id_detalle_producto = Guid.Parse(this.Request.Params["id_detalle_producto"]);
            int estadoCheck = int.Parse(this.Request.Params["valor"]);
            VentaBL obj = new VentaBL();
            obj.rectificacfionVenta(estadoCheck, id_detalle_producto, usuario.idUsuario);
        }


        public String verModificacionDatosVenta()
        {
            VentaBL ventaBL = new VentaBL();
            Venta venta = new Venta();
            venta.idVenta = Guid.Parse(Request["idVenta"].ToString());
            venta = ventaBL.verModificacionDatos(venta);
            return JsonConvert.SerializeObject(venta);
        }

        public void updateModificarDatosVenta()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            
            Venta venta = new Venta();
            venta.usuario = new Usuario();
            venta.usuario.idUsuario =usuario.idUsuario;

            venta.idVenta = Guid.Parse(this.Request.Params["idVenta"]);
            venta.idAsistente = int.Parse(this.Request.Params["asistenteCliente"]);
            venta.idResponsableComercial = int.Parse(this.Request.Params["reponsableComercial"]);
            venta.idSupervisorComercial = int.Parse(this.Request.Params["supervisorComercial"]);
            venta.idOrigen = int.Parse(this.Request.Params["origen"]);
            PropertyInfo propertyInfo = venta.GetType().GetProperty(this.Request.Params["propiedad"]);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(venta, 1);
            }  
            VentaBL obj = new VentaBL();
            obj.modificacionDatosVenta(venta);
        }


    }
}
