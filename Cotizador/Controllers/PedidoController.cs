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
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.BUSQUEDA_PEDIDO;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] == null)
            {
                Pedido pedidoTmp = new Pedido();
                pedidoTmp.fechaSolicitudDesde = DateTime.Now.AddDays(-10);
                DateTime fechaHasta = DateTime.Now;
                pedidoTmp.fechaSolicitudHasta = new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);
                pedidoTmp.fechaEntregaDesde = DateTime.Now.AddDays(-10);
                DateTime fechaEntregaHasta = DateTime.Now;
                pedidoTmp.fechaEntregaHasta = new DateTime(fechaEntregaHasta.Year, fechaEntregaHasta.Month, fechaEntregaHasta.Day, 23, 59, 59);

                pedidoTmp.ciudad = new Ciudad();
                pedidoTmp.cliente = new Cliente();
                pedidoTmp.seguimientoPedido = new SeguimientoPedido();
                pedidoTmp.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Todos;
                // cotizacionTmp.cotizacionDetalleList = new List<CotizacionDetalle>();
                pedidoTmp.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                pedidoTmp.usuarioBusqueda = pedidoTmp.usuario;
                this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedidoTmp;


            }

            Pedido pedidoSearch = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];


            ViewBag.fechaSolicitudDesde = pedidoSearch.fechaSolicitudDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaSolicitudHasta = pedidoSearch.fechaSolicitudHasta.ToString(Constantes.formatoFecha);
            ViewBag.fechaEntregaDesde = pedidoSearch.fechaEntregaDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaEntregaHasta = pedidoSearch.fechaEntregaHasta.ToString(Constantes.formatoFecha);

            if (this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] == null)
            {
                this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] = new List<Pedido>();
            }

            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];
            int existeCliente = 0;
            //  if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
            if (pedido.cliente.idCliente != Guid.Empty)
            {
                existeCliente = 1;
            }

     

            ViewBag.pedido = pedido;

            ViewBag.pedidoList = this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA];
            ViewBag.existeCliente = existeCliente;

            return View();
        }


        public ActionResult Pedir()
        {
            try
            {
                this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.MANTENIMIENTO_PEDIDO;

                //Si no hay usuario, se dirige el logueo
                if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                ViewBag.debug = Constantes.DEBUG;
                ViewBag.Si = Constantes.MENSAJE_SI;
                ViewBag.No = Constantes.MENSAJE_NO;
                ViewBag.IGV = Constantes.IGV;


                //Si no se está trabajando con una cotización se crea una y se agrega a la sesion

                if (this.Session[Constantes.VAR_SESSION_PEDIDO] == null)
                {

                    instanciarPedido();
                }
                Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];


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
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
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
          //  pedido.tasaIGV = Constantes.IGV;
            //pedido.flete = 0;
           // pedido.mostrarCodigoProveedor = true;
            pedido.observaciones = String.Empty;

            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            pedido.seguimientoPedido = new SeguimientoPedido();
            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            pedido.fechaPrecios = pedido.fechaSolicitud.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);

            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }




        #region CONTROLES CHOOSEN

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];

            if ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA] == Constantes.paginas.misPedidos)
                pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];

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
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            if ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA] == Constantes.paginas.misPedidos)
                pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];

            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            pedido.cliente = clienteBl.getCliente(idCliente);

            String resultado = "{" +
                "\"descripcionCliente\":\"" + pedido.cliente.ToString() + "\"," +
                "\"idCliente\":\"" + pedido.cliente.idCliente + "\"," +
                "\"contacto\":\"" + pedido.cliente.contacto1 + "\"" +
                "}";

            if ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA] == Constantes.paginas.misPedidos)
                this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
            else
                this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
            return resultado;
        }


        #endregion



        #region AGREGAR PRODUCTO


        public String GetProducto()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
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
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
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

            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
            return resultado;


        }

        #endregion





        /*Actualización de Campos*/
        #region ACTUALIZACION DE CAMPOS FORMULARIO


        public String ChangeIdCiudad()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
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
                this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
                return "{\"idCiudad\": \"" + idCiudad + "\"}";
            }

        }

        public String ChangeIdCiudadBusqueda()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];
            Guid idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);

            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
            pedido.cliente = new Cliente();
            //Para realizar el cambio de ciudad ningun producto debe estar agregado
            pedido.ciudad = ciudadNueva;
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";


        }

        public void ChangeNumeroReferenciaCliente()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.numeroReferenciaCliente = this.Request.Params["numeroReferenciaCliente"];
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeDireccionEntrega()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.direccionEntrega = this.Request.Params["direccionEntrega"];
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeContactoEntrega()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.contactoEntrega = this.Request.Params["contactoEntrega"];
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeTelefonoContactoEntrega()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.telefonoContactoEntrega = this.Request.Params["telefonoContactoEntrega"];
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }


        public void ChangeFechaSolicitud()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            String[] fechaSolicitud = this.Request.Params["fechaSolicitud"].Split('/');
            String[] horaSolicitud = this.Request.Params["horaSolicitud"].Split(':');
            pedido.fechaSolicitud = new DateTime(Int32.Parse(fechaSolicitud[2]), Int32.Parse(fechaSolicitud[1]), Int32.Parse(fechaSolicitud[0]), Int32.Parse(horaSolicitud[0]), Int32.Parse(horaSolicitud[1]), 0);
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeFechaEntrega()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            String[] fechaEntrega = this.Request.Params["fechaEntrega"].Split('/');
            pedido.fechaEntrega = new DateTime(Int32.Parse(fechaEntrega[2]), Int32.Parse(fechaEntrega[1]), Int32.Parse(fechaEntrega[0]));
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeFechaMaximaEntrega()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            String[] fechaMaximaEntrega = this.Request.Params["fechaMaximaEntrega"].Split('/');
            pedido.fechaMaximaEntrega = new DateTime(Int32.Parse(fechaMaximaEntrega[2]), Int32.Parse(fechaMaximaEntrega[1]), Int32.Parse(fechaMaximaEntrega[0]));
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeContactoPedido()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.contactoPedido = this.Request.Params["contactoPedido"];
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }

        public void ChangeTelefonoContactoPedido()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.telefonoContactoPedido = this.Request.Params["telefonoContactoPedido"];
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }


        public void ChangeObservaciones()
        {
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.observaciones = this.Request.Params["observaciones"];
            this.Session[Constantes.VAR_SESSION_PEDIDO] = pedido;
        }


        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            IDocumento documento = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            List<IDocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            HelperDocumento.calcularMontosTotales(documento);
            this.Session[Constantes.VAR_SESSION_PEDIDO] = documento;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }

        #endregion


        #region CREAR/ACTUALIZAR PEDIDO

        public String Create()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.usuario = usuario;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.InsertPedido(pedido);
            long numeroPedido = pedido.numeroPedido;
            int estado = (int)pedido.seguimientoPedido.estado;
            if (continuarLuego == 1)
            {
                SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = SeguimientoPedido.estadosSeguimientoPedido.Edicion;
                estado = (int)estadosSeguimientoPedido;
                String observacion = "Se continuará editando luego";
                //Falta modificar
                //   updateEstadoSeguimientoPedido(numeroPedido, estadosSeguimientoPedido, observacion);
            }
            pedido = null;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = null;


            usuarioBL.updatePedidoSerializado(usuario, null);
            String resultado = "{ \"codigo\":\"" + numeroPedido + "\", \"estado\":\"" + estado + "\" }";
            return resultado;
        }





        public String Update()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            pedido.usuario = usuario;
            PedidoBL bl = new PedidoBL();
            bl.UpdatePedido(pedido);
            long codigo = pedido.numeroPedido;
            int estado = (int)pedido.seguimientoPedido.estado;
            if (continuarLuego == 1)
            {
                SeguimientoPedido.estadosSeguimientoPedido estadosSeguimientoPedido = SeguimientoPedido.estadosSeguimientoPedido.Edicion;
                estado = (int)estadosSeguimientoPedido;
                String observacion = "Se continuará editando luego";

                //Falta modificar
                //updateEstadoSeguimientoPedido(codigo, estadosSeguimientoPedido, observacion);
            }
            pedido = null;
            this.Session[Constantes.VAR_SESSION_PEDIDO] = null;


            usuarioBL.updateCotizacionSerializada(usuario, null);
            String resultado = "{ \"codigo\":\"" + codigo + "\", \"estado\":\"" + estado + "\" }";
            return resultado;
        }



        private void updateEstadoSeguimientoPedido(Int64 codigo, SeguimientoPedido.estadosSeguimientoPedido estado, String observacion)
        {
            Pedido cotizacionSession = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            PedidoBL pedidoBL = new PedidoBL();
            Pedido pedido = new Pedido();
            pedido.numeroPedido = codigo;
            //REVISAR
            pedido.fechaModificacion = DateTime.Now;// cotizacionSession.fechaModificacion;
            pedido.seguimientoPedido = new SeguimientoPedido();
            pedido.seguimientoPedido.estado = estado;
            pedido.seguimientoPedido.observacion = observacion;
            pedido.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
           //FALTA
            //pedidoBL.cambiarEstadoCotizacion(pedido);
        }
        #endregion

        public int Search()
        {
            //Se recupera el pedido Búsqueda de la session
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA];

            String[] solDesde = this.Request.Params["fechaSolicitudDesde"].Split('/');
            pedido.fechaSolicitudDesde = new DateTime(Int32.Parse(solDesde[2]), Int32.Parse(solDesde[1]), Int32.Parse(solDesde[0]));

            String[] solHasta = this.Request.Params["fechaSolicitudHasta"].Split('/');
            pedido.fechaSolicitudHasta = new DateTime(Int32.Parse(solHasta[2]), Int32.Parse(solHasta[1]), Int32.Parse(solHasta[0]),23,59,59);

            String[] entregaDesde = this.Request.Params["fechaEntregaDesde"].Split('/');
            pedido.fechaEntregaDesde = new DateTime(Int32.Parse(entregaDesde[2]), Int32.Parse(entregaDesde[1]), Int32.Parse(entregaDesde[0]));

            String[] entregaHasta = this.Request.Params["fechaEntregaHasta"].Split('/');
            pedido.fechaEntregaHasta = new DateTime(Int32.Parse(entregaHasta[2]), Int32.Parse(entregaHasta[1]), Int32.Parse(entregaHasta[0]), 23, 59, 59);


            if (this.Request.Params["numero"] == null || this.Request.Params["numero"].Trim().Length == 0)
            {
                pedido.numeroPedido = 0;
            }
            else
            {
                pedido.numeroPedido = long.Parse(this.Request.Params["numero"]);
            }

            if (this.Request.Params["numeroGrupo"] == null || this.Request.Params["numeroGrupo"].Trim().Length == 0)
            {
                pedido.numeroGrupoPedido = 0;
            }
            else
            {
                pedido.numeroGrupoPedido = long.Parse(this.Request.Params["numeroGrupo"]);
            }


            pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido) Int32.Parse(this.Request.Params["estado"]);

            PedidoBL pedidoBL = new PedidoBL();
            List<Pedido> pedidoList = pedidoBL.GetPedidos(pedido);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_PEDIDO_LISTA] = pedidoList;
            this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = pedido;
            //Se retorna la cantidad de elementos encontrados
            return pedidoList.Count();
        }


        public Boolean ConsultarSiExistePedido()
        {
            Pedido pedido = (Pedido)this.Session["pedido"];
            if (pedido == null)
                return false;
            else
                return true;
        }

        public String Show()
        {
            PedidoBL pedidoBL = new PedidoBL();
            Pedido pedido = new Pedido();
            pedido.idPedido = Guid.Parse(Request["idPedido"].ToString());
            pedido = pedidoBL.GetPedido(pedido);
            this.Session[Constantes.VAR_SESSION_PEDIDO_VER] = pedido;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonPedido = JsonConvert.SerializeObject(pedido);
            String json = "{\"usuario\":" + jsonUsuario + ", \"pedido\":" + jsonPedido + "}";
            return json;
        }




    }
}