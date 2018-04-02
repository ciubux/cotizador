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
            pedido.incluidoIGV = false;
            pedido.tasaIGV = Constantes.IGV;
            //pedido.flete = 0;
           // pedido.mostrarCodigoProveedor = true;
            pedido.observaciones = String.Empty;

            pedido.usuario = (Usuario)this.Session["usuario"];
            pedido.seguimientoPedido = new SeguimientoPedido();
            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            pedido.fechaPrecios = pedido.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);

            this.Session["pedido"] = pedido;
        }




        #region CONTROLES CHOOSEN

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            Pedido pedido = (Pedido)this.Session["pedido"];
            List<Cliente> clienteList = clienteBL.getCLientesBusqueda(data, pedido.ciudad.idCiudad);
            String resultado = "{\"q\":\"" + data + "\",\"results\":[";
            Boolean existeCliente = false;
            foreach (Cliente cliente in clienteList)
            {
                resultado += "{\"id\":\"" + cliente.idCliente + "\",\"text\":\"" + cliente.ToString() + "\"},";
                existeCliente = true;
            }
            if (existeCliente)
                resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
            else
                resultado = resultado.Substring(0, resultado.Length) + "]}";
            return resultado;
        }

        public String GetCliente()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];

            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            pedido.cliente = clienteBl.getCliente(idCliente);

            String resultado = "{" +
                "\"descripcionCliente\":\"" + pedido.cliente.ToString() + "\"," +
                "\"idCliente\":\"" + pedido.cliente.idCliente + "\"," +
                "\"contacto\":\"" + pedido.cliente.contacto1 + "\"" +
                "}";
            this.Session["pedido"] = pedido;
            return resultado;
        }


        #endregion



        #region AGREGAR PRODUCTO


        public String GetProducto()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            //Se recupera el producto y se guarda en Session
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            this.Session["idProducto"] = idProducto.ToString();

            //Para recuperar el producto se envia si la sede seleccionada es provincia o no
            ProductoBL bl = new ProductoBL();
            Producto producto = bl.getProducto(idProducto, pedido.ciudad.esProvincia, pedido.incluidoIGV, pedido.cliente.idCliente);

            Decimal fleteDetalle = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.costoLista * (0) / 100));
            Decimal precioUnitario = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, fleteDetalle + producto.precioLista));

            Decimal porcentajeDescuento = 0;
            if (producto.precioNeto != null)
            {

                porcentajeDescuento = 100 - (producto.precioNeto.Value * 100 / producto.precioLista);


            }

            String jsonPrecioLista = JsonConvert.SerializeObject(producto.precioListaList);

            String resultado = "{" +
                "\"id\":\"" + producto.idProducto + "\"," +
                "\"nombre\":\"" + producto.descripcion + "\"," +
                "\"image\":\"data:image/png;base64, " + Convert.ToBase64String(producto.image) + "\"," +
                "\"unidad\":\"" + producto.unidad + "\"," +
                "\"unidad_alternativa\":\"" + producto.unidad_alternativa + "\"," +
                "\"proveedor\":\"" + producto.proveedor + "\"," +
                "\"familia\":\"" + producto.familia + "\"," +
                "\"precioUnitarioSinIGV\":\"" + producto.precioSinIgv + "\"," +
                "\"precioUnitarioAlternativoSinIGV\":\"" + producto.precioAlternativoSinIgv + "\"," +
                "\"precioLista\":\"" + producto.precioLista + "\"," +
                "\"costoSinIGV\":\"" + producto.costoSinIgv + "\"," +
                "\"costoAlternativoSinIGV\":\"" + producto.costoAlternativoSinIgv + "\"," +
                "\"fleteDetalle\":\"" + fleteDetalle + "\"," +
                "\"precioUnitario\":\"" + precioUnitario + "\"," +
                "\"porcentajeDescuento\":\"" + porcentajeDescuento + "\"," +
                "\"precioListaList\":" + jsonPrecioLista + "," +

                "\"costoLista\":\"" + producto.costoLista + "\"" +
                "}";
            return resultado;


        }


        public String AddProducto()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            Guid idProducto = Guid.Parse(this.Session["idProducto"].ToString());
            PedidoDetalle pedidoDetalle = pedido.pedidoDetalleList.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
            if (pedidoDetalle != null)
            {
                throw new System.Exception("Producto ya se encuentra en la lista");
            }

            PedidoDetalle detalle = new PedidoDetalle();
            ProductoBL productoBL = new ProductoBL();
            Producto producto = productoBL.getProducto(idProducto, pedido.ciudad.esProvincia, pedido.incluidoIGV, pedido.cliente.idCliente);
            detalle.producto = producto;

            detalle.cantidad = Int32.Parse(Request["cantidad"].ToString());
            detalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
            detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;
            detalle.observacion = Request["observacion"].ToString();
            decimal precioNeto = Decimal.Parse(Request["precio"].ToString());
            decimal costo = Decimal.Parse(Request["costo"].ToString());
            decimal flete = Decimal.Parse(Request["flete"].ToString());
            if (detalle.esPrecioAlternativo)
            {
                //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                //dado que cuando se hace get al precioNeto se recupera diviendo entre la equivalencia
                detalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto * producto.equivalencia));
            }
            else
            {
                detalle.precioNeto = precioNeto;
            }
            detalle.flete = flete;
            pedido.pedidoDetalleList.Add(detalle);

            //Calcula los montos totales de la cabecera de la cotizacion
            HelperDocumento.calcularMontosTotales(pedido);


            detalle.unidad = detalle.producto.unidad;
            //si esPrecioAlternativo  se mostrará la unidad alternativa
            if (detalle.esPrecioAlternativo)
            {
                detalle.unidad = detalle.producto.unidad_alternativa;
            }

            var nombreProducto = detalle.producto.descripcion;
           /* if (pedido.mostrarCodigoProveedor)
            {*/
                nombreProducto = detalle.producto.skuProveedor + " - " + detalle.producto.descripcion;
        //    }

          /*  if (pedido.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Ambos)
            {*/
                nombreProducto = nombreProducto + "\\n" + detalle.observacion;
            //}

            String resultado = "{" +
                "\"idProducto\":\"" + detalle.producto.idProducto + "\"," +
                "\"codigoProducto\":\"" + detalle.producto.sku + "\"," +
                "\"nombreProducto\":\"" + nombreProducto + "\"," +
                "\"unidad\":\"" + detalle.unidad + "\"," +
                "\"igv\":\"" + pedido.montoIGV.ToString() + "\", " +
                "\"subTotal\":\"" + pedido.montoSubTotal.ToString() + "\", " +
                "\"margen\":\"" + detalle.margen + "\", " +
                "\"precioUnitario\":\"" + detalle.precioUnitario + "\", " +
                "\"observacion\":\"" + detalle.observacion + "\", " +
                "\"total\":\"" + pedido.montoTotal.ToString() + "\"}";

            this.Session["pedido"] = pedido;
            return resultado;


        }

        #endregion





        /*Actualización de Campos*/
        #region ACTUALIZACION DE CAMPOS FORMULARIO


        public String ChangeIdCiudad()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            Guid idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);

            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
            pedido.cliente = new Cliente();
            //Para realizar el cambio de ciudad ningun producto debe estar agregado
            if (pedido.pedidoDetalleList.Count > 0)
            {
                // throw new Exception("No se puede cambiar de ciudad");
                return "No se puede cambiar de ciudad";
            }
            else
            {
                pedido.ciudad = ciudadNueva;
                this.Session["pedido"] = pedido;
                return "{\"idCiudad\": \"" + idCiudad + "\"}";
            }

        }

        public void ChangeNumeroReferenciaCliente()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            pedido.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
            this.Session["pedido"] = pedido;
        }

        public void ChangeDireccionEntrega()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            pedido.direccionEntrega = this.Request.Params["direccionEntrega"];
            this.Session["pedido"] = pedido;
        }

        public void ChangeContactoEntrega()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            pedido.contactoEntrega = this.Request.Params["contactoEntrega"];
            this.Session["pedido"] = pedido;
        }

        public void ChangeTelefonoContactoEntrega()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            pedido.telefonoContactoEntrega = this.Request.Params["telefonoContactoEntrega"];
            this.Session["pedido"] = pedido;
        }


        public void ChangeFechaSolicitud()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            String[] fechaSolicitud = this.Request.Params["fechaSolicitud"].Split('/');
            String[] horaSolicitud = this.Request.Params["horaSolicitud"].Split(':');
            pedido.fechaSolicitud = new DateTime(Int32.Parse(fechaSolicitud[2]), Int32.Parse(fechaSolicitud[1]), Int32.Parse(fechaSolicitud[0]), Int32.Parse(horaSolicitud[0]), Int32.Parse(horaSolicitud[1]), 0);
            this.Session["pedido"] = pedido;
        }

        public void ChangeFechaEntrega()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            String[] fechaEntrega = this.Request.Params["fechaEntrega"].Split('/');
            pedido.fechaEntrega = new DateTime(Int32.Parse(fechaEntrega[2]), Int32.Parse(fechaEntrega[1]), Int32.Parse(fechaEntrega[0]));
            this.Session["pedido"] = pedido;
        }

        public void ChangeFechaMaximaEntrega()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            String[] fechaMaximaEntrega = this.Request.Params["fechaMaximaEntrega"].Split('/');
            pedido.fechaMaximaEntrega = new DateTime(Int32.Parse(fechaMaximaEntrega[2]), Int32.Parse(fechaMaximaEntrega[1]), Int32.Parse(fechaMaximaEntrega[0]));
            this.Session["pedido"] = pedido;
        }

        public void ChangeContactoPedido()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            pedido.contactoPedido = this.Request.Params["contactoPedido"];
            this.Session["pedido"] = pedido;
        }

        public void ChangeTelefonoContactoPedido()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            pedido.telefonoContactoPedido = this.Request.Params["telefonoContactoPedido"];
            this.Session["pedido"] = pedido;
        }


        public void ChangeObservaciones()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            pedido.observaciones = this.Request.Params["observaciones"];
            this.Session["pedido"] = pedido;
        }


        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            IDocumento documento = (Pedido)this.Session["pedido"];
            List<IDocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            HelperDocumento.calcularMontosTotales(documento);
            this.Session["pedido"] = documento;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }

        #endregion


        #region CREAR/ACTUALIZAR PEDIDO

        public String Create()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session["usuario"];
            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            Pedido pedido = (Pedido)this.Session["pedido"];
            pedido.usuario = usuario;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.InsertPedido(pedido);
            long numeroPedido = pedido.numeroPedido;
            int estado = (int)pedido.seguimientoPedido.estado;
            if (continuarLuego == 1)
            {
                SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoCotizacion = SeguimientoPedido.estadosSeguimientoPedido.Edicion;
                estado = (int)estadosSeguimientoCotizacion;
                String observacion = "Se continuará editando luego";
                //updateEstadoSeguimientoCotizacion(numeroPedido, estadosSeguimientoCotizacion, observacion);
            }
            pedido = null;
            this.Session["pedido"] = null;


            usuarioBL.updateCotizacionSerializada(usuario, null);
            String resultado = "{ \"codigo\":\"" + numeroPedido + "\", \"estado\":\"" + estado + "\" }";
            return resultado;
        }





        public String Update()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session["usuario"];

            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.usuario = usuario;
            CotizacionBL bl = new CotizacionBL();
            bl.UpdateCotizacion(cotizacion);
            long codigo = cotizacion.codigo;
            int estado = (int)cotizacion.seguimientoCotizacion.estado;
            if (continuarLuego == 1)
            {
                SeguimientoCotizacion.estadosSeguimientoCotizacion estadosSeguimientoCotizacion = SeguimientoCotizacion.estadosSeguimientoCotizacion.Edicion;
                estado = (int)estadosSeguimientoCotizacion;
                String observacion = "Se continuará editando luego";
                updateEstadoSeguimientoCotizacion(codigo, estadosSeguimientoCotizacion, observacion);
            }
            cotizacion = null;
            this.Session["cotizacion"] = null;


            usuarioBL.updateCotizacionSerializada(usuario, null);
            String resultado = "{ \"codigo\":\"" + codigo + "\", \"estado\":\"" + estado + "\" }";
            return resultado;
        }



        private void updateEstadoSeguimientoCotizacion(Int64 codigo, SeguimientoCotizacion.estadosSeguimientoCotizacion estado, String observacion)
        {
            Cotizacion cotizacionSession = (Cotizacion)this.Session["cotizacion"];
            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = codigo;
            //REVISAR
            cotizacion.fechaModificacion = DateTime.Now;// cotizacionSession.fechaModificacion;
            cotizacion.seguimientoCotizacion = new SeguimientoCotizacion();
            cotizacion.seguimientoCotizacion.estado = estado;
            cotizacion.seguimientoCotizacion.observacion = observacion;
            cotizacion.usuario = (Usuario)this.Session["usuario"];
            cotizacionBL.cambiarEstadoCotizacion(cotizacion);
        }
        #endregion


    }
}