using BusinessLayer;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class PedidoController : Controller
    {
        // GET: Pedido
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Pedir()
        {
            try
            {
                this.Session["pagina"] = Constantes.MANTENIMIENTO_PEDIDO;

                //Si no hay usuario, se dirige el logueo
                if (this.Session["usuario"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                ViewBag.debug = Constantes.DEBUG;
                ViewBag.Si = Constantes.MENSAJE_SI;
                ViewBag.No = Constantes.MENSAJE_NO;
                ViewBag.IGV = Constantes.IGV;


                //Si no se está trabajando con una cotización se crea una y se agrega a la sesion

                if (this.Session["pedido"] == null)
                {

                    instanciarPedido();
                }
                Pedido pedido = (Pedido)this.Session["pedido"];


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

                ViewBag.fechaEntrega = pedido.fechaEntrega.ToString(Constantes.formatoFecha);
                ViewBag.fechaMaximaEntrega = pedido.fechaMaximaEntrega.ToString(Constantes.formatoFecha);


                ViewBag.pedido = pedido;


                ViewBag.fechaPrecios = pedido.fechaPrecios.ToString(Constantes.formatoFecha);
            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session["usuario"];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }

            return View();
        }

        private void instanciarPedido()
        {
            Pedido pedido = new Pedido();
            pedido.idPedido = Guid.Empty;
            pedido.numeroPedido = 0;
            pedido.numeroGrupoPedido = null;
            pedido.cotizacion = new Cotizacion();
            pedido.ciudad = new Ciudad();
            pedido.cliente = new Cliente();
            pedido.numeroReferenciaCliente = null;
            pedido.direccionEntrega = null;
            pedido.contactoEntrega = null;
            pedido.telefonoContactoEntrega = null;
            pedido.fechaSolicitud = DateTime.Now;
            pedido.fechaEntrega = DateTime.Now;
            pedido.fechaMaximaEntrega = DateTime.Now;
            pedido.contactoPedido = String.Empty;
            pedido.telefonoContactoPedido = String.Empty;
            pedido.incluidoIgv = false;
            pedido.tasaIGV = Constantes.IGV;
            pedido.flete = 0;
            pedido.mostrarCodigoProveedor = true;
            pedido.observaciones = String.Empty;

            pedido.usuario = (Usuario)this.Session["usuario"];
            pedido.seguimientoPedido = new SeguimientoPedido();
            pedido.PedidoDetalleList = new List<PedidoDetalle>();
            pedido.fechaPrecios = pedido.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);

            this.Session["pedido"] = pedido;
        }




    }
}