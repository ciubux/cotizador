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
                if (pedido.cliente.idCliente != Guid.Empty || pedido.grupo.idGrupo != Guid.Empty)
                {
                    existeCliente = 1;
                }
                ViewBag.existeCliente = existeCliente;


                if (pedido.cliente.idCliente != Guid.Empty)
                {
                    ViewBag.idClienteGrupo = pedido.cliente.idCliente;
                    ViewBag.clienteGrupo = pedido.cliente.ToString();
                }
                else
                {
                    ViewBag.idClienteGrupo = pedido.grupo.idGrupo;
                    ViewBag.clienteGrupo = pedido.grupo.ToString();
                }

                ViewBag.fechaPrecios = pedido.fechaPrecios.ToString(Constantes.formatoFecha);
                ViewBag.fecha = pedido.fecha.ToString(Constantes.formatoFecha);
                ViewBag.fechaLimiteValidezOferta = pedido.fechaLimiteValidezOferta.ToString(Constantes.formatoFecha);
                ViewBag.fechaInicioVigenciaPrecios = pedido.fechaInicioVigenciaPrecios == null ? null : pedido.fechaInicioVigenciaPrecios.Value.ToString(Constantes.formatoFecha);
                ViewBag.fechaFinVigenciaPrecios = pedido.fechaFinVigenciaPrecios == null ? null : pedido.fechaFinVigenciaPrecios.Value.ToString(Constantes.formatoFecha);

                //Se agrega el viewbag numero para poder mostrar el campo vacío cuando no se está creando una cotización
                ViewBag.numero = pedido.codigo;

                ViewBag.pedido = pedido;

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
            pedido.mostrarCodigoProveedor = true;
            pedido.fecha = DateTime.Now;
            pedido.fechaInicioVigenciaPrecios = null;
            pedido.fechaFinVigenciaPrecios = null;
            pedido.fechaLimiteValidezOferta = pedido.fecha.AddDays(Constantes.PLAZO_OFERTA_DIAS);
            pedido.fechaPrecios = pedido.fecha.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);
            pedido.ciudad = new Ciudad();
            pedido.cliente = new Cliente();
            pedido.grupo = new Grupo();
            pedido.PedidoDetalleList = new List<PedidoDetalle>();
            pedido.igv = Constantes.IGV;
            pedido.flete = 0;
            pedido.validezOfertaEnDias = Constantes.PLAZO_OFERTA_DIAS;
            pedido.considerarCantidades = Pedido.OpcionesConsiderarCantidades.Observaciones;
            Usuario usuario = (Usuario)this.Session["usuario"];
            pedido.usuario = usuario;
            pedido.observaciones = Constantes.OBSERVACION;
            pedido.incluidoIgv = false;
            pedido.seguimientoPedido = new SeguimientoPedido();
            this.Session["pedido"] = pedido;
        }




    }
}