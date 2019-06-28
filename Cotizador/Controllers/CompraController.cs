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
            Compra compraVer = (Compra)this.Session[Constantes.VAR_SESSION_COMPRA_VER];
            CompraBL compraBL = new CompraBL();
            Compra compra = new Compra();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            compra = compraBL.GetCompra(compraVer, usuario);
            //Temporal
            Pedido pedido = compra.pedido;
            pedido.ciudadASolicitar = new Ciudad();

            this.Session[Constantes.VAR_SESSION_COMPRA] = compra;
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

                pedidoDetalle.ProductoPresentacion.Equivalencia = ventaDetalle.ProductoPresentacion.Equivalencia;
                pedidoDetalle.esPrecioAlternativo = documentoDetalleJson.esUnidadAlternativa == 1;

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.unidad = ventaDetalle.producto.unidad_alternativa;
                    //////REVISAR QUE LA CANTIDAD DEBE SER SIEMPRE ENTERO
                    pedidoDetalle.cantidad = Convert.ToInt32(ventaDetalle.sumCantidadUnidadAlternativa);
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

            ventaBL.GetVentaConsolidada(venta,usuario);

            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales(venta.pedido);

            this.Session[Constantes.VAR_SESSION_COMPRA_VER] = venta;

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

            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_COMPRA_VER];

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
            this.Session[Constantes.VAR_SESSION_COMPRA] = (Venta)this.Session[Constantes.VAR_SESSION_COMPRA_VER];
        }





            public String Descargar()
           {

            String nombreArchivo = Request["nombreArchivo"].ToString();
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_COMPRA];
            if (venta == null)
                venta = (Venta)this.Session[Constantes.VAR_SESSION_COMPRA_VER];


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

        public ActionResult Comprar()
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoCompra;

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
                Compra compra = (Compra)this.Session[Constantes.VAR_SESSION_COMPRA];
                Pedido pedido = compra.pedido;
                    


                int existeCliente = 0;
                if (pedido.proveedor.idProveedor != Guid.Empty)
                {
                    existeCliente = 1;
                }

                ViewBag.existeCliente = existeCliente;
                ViewBag.idClienteGrupo = pedido.proveedor.idProveedor;
                ViewBag.clienteGrupo = pedido.proveedor.ToString();

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

            ViewBag.pagina = (int)Constantes.paginas.MantenimientoCompra;
            return View();
        }


        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            Compra compra = (Compra)this.Session[Constantes.VAR_SESSION_COMPRA];

            IDocumento documento = (Pedido)compra.pedido;
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales((Pedido)documento);
            compra.pedido = (Pedido)documento;

            this.Session[Constantes.VAR_SESSION_COMPRA] = compra;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }

       


        public String Update()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Compra compra = (Compra)this.Session[Constantes.VAR_SESSION_COMPRA];
            compra.usuario = usuario;
            CompraBL bl = new CompraBL();
            bl.UpdateCompra(compra);


            long numeroPedido = compra.pedido.numeroPedido;
            String numeroPedidoString = compra.pedido.numeroPedidoString;
        

            var v = new { numeroPedido = numeroPedidoString };
            String resultado = JsonConvert.SerializeObject(v);

            //String resultado = "{ \"codigo\":\"" + numeroPedido + "\", \"estado\":\"" + estado + "\", \"observacion\":\"" + observacion + "\" }";
            return resultado;
        }


    }
}