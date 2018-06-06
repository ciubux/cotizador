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
       
            
            venta = ventaBL.GetVenta(ventaVer);
            //Temporal
            Pedido pedido = venta.pedido;
            pedido.ciudadASolicitar = new Ciudad();

            if (pedido.tipoPedido == Pedido.tiposPedido.TrasladoInterno)
            {
                pedido.ciudadASolicitar = new Ciudad
                {
                    idCiudad = pedido.ciudad.idCiudad,
                    nombre = pedido.ciudad.nombre,
                    esProvincia = pedido.ciudad.esProvincia
                };

                pedido.ciudad = pedido.cliente.ciudad;
            }

            this.Session[Constantes.VAR_SESSION_VENTA] = venta;
        }


        public String Show()
        {
            VentaBL ventaBL = new VentaBL();
            Venta venta = new Venta();
            venta.guiaRemision = new GuiaRemision();
            venta.guiaRemision.idMovimientoAlmacen = Guid.Parse(Request["idMovimientoAlmacen"].ToString());
            venta = ventaBL.GetVenta(venta);
            this.Session[Constantes.VAR_SESSION_VENTA_VER] = venta;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
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



        public String Descargar()
        {
            String nombreArchivo = Request["nombreArchivo"].ToString();
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA];
            if (venta == null)
                venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];

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

            ViewBag.pagina = Constantes.MANTENIMIENTO_PEDIDO;
            return View();
        }


        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA];

            IDocumento documento = (Pedido)venta.pedido;
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            calcularMontosTotales((Pedido)documento);
            venta.pedido = (Pedido)documento;

            this.Session[Constantes.VAR_SESSION_VENTA] = venta;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }

        private void calcularMontosTotales(Pedido pedido)
        {
            Decimal total = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedido.pedidoDetalleList.AsEnumerable().Sum(o => o.subTotal)));
            Decimal subtotal = 0;
            Decimal igv = 0;

            if (pedido.incluidoIGV)
            {
                subtotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, total / (1 + Constantes.IGV)));
                igv = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, total - subtotal));
            }
            else
            {
                subtotal = total;
                igv = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, total * Constantes.IGV));
                total = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, subtotal + igv));
            }

            pedido.montoTotal = total;
            pedido.montoSubTotal = subtotal;
            pedido.montoIGV = igv;
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