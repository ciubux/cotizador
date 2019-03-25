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
    public class CompraController : Controller
    {
        // GET: Venta
        public ActionResult Index()
        {
            return View();
        }


        public void iniciarEdicionCompra()
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
            CompraBL compraBL = new CompraBL();
            Compra compra = new Compra();
            compra.notaIngreso = new NotaIngreso();
            compra.notaIngreso.idMovimientoAlmacen = Guid.Parse(Request["idMovimientoAlmacen"].ToString());
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            compra = compraBL.GetCompra(compra, usuario);
            this.Session[Constantes.VAR_SESSION_COMPRA_VER] = compra;
            
            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonCompra = JsonConvert.SerializeObject(compra);

            Ciudad ciudad = usuario.sedesMPPedidos.Where(s => s.idCiudad == compra.pedido.ciudad.idCiudad).FirstOrDefault();

            string jsonSeries = "[]";
            if (ciudad != null)
            {
                var serieDocumentoElectronicoList = ciudad.serieDocumentoElectronicoList.OrderByDescending(x => x.esPrincipal).ToList();
                jsonSeries = JsonConvert.SerializeObject(serieDocumentoElectronicoList);

            }

            String json = "{\"serieDocumentoElectronicoList\":" + jsonSeries + ", \"compra\":" + jsonCompra + "}";
            return json;
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
            List<Guid> guiaRemisionIdList =  (List<Guid>)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS];

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
                PedidoDetalle pedidoDetalle = new PedidoDetalle(usuario.visualizaCostos,usuario.visualizaMargen);
                //pedidoDetalle.producto = new Producto();
                pedidoDetalle.producto = ventaDetalle.producto;
                pedidoDetalle.cantidad = documentoDetalleJson.cantidad;
                ///Importante definir como null para que se recupere del producto al momento de insertar
                pedidoDetalle.unidadInternacional = null; 

                pedidoDetalle.producto.equivalencia = ventaDetalle.producto.equivalencia;
                pedidoDetalle.esPrecioAlternativo = documentoDetalleJson.esUnidadAlternativa == 1;

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.unidad = ventaDetalle.producto.unidad_alternativa;
                    //////REVISAR QUE LA CANTIDAD DEBE SER SIEMPRE ENTERO
                    pedidoDetalle.cantidad = Convert.ToInt32(ventaDetalle.sumCantidadUnidadAlternativa);
                    pedidoDetalle.precioNeto = (ventaDetalle.sumPrecioUnitario * pedidoDetalle.producto.equivalencia) / pedidoDetalle.cantidad;
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

            ventaBL.GetVentaConsolidada(venta,usuario);

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





            public String Descargar()
           {

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

            ViewBag.pagina = Constantes.paginas.MantenimientoPedido;
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


    }
}