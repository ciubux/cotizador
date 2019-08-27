using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;
using System.Data.SqlClient;

namespace DataLayer
{
    public class CompraDAL : DaoBase
    {
        public CompraDAL(IDalSettings settings) : base(settings)
        {
        }

        public CompraDAL() : this(new CotizadorSettings())
        {
        }


        public Compra SelectCompra(Compra compra, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_compra");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", compra.notaIngreso.idMovimientoAlmacen);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable ventaDataTable = dataSet.Tables[0];
            DataTable ventaDetalleDataTable = dataSet.Tables[1];
            DataTable pedidoAdjuntoDataTable = dataSet.Tables[2];

            //   DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            compra.pedido = new Pedido();
            Pedido pedido = compra.pedido;

            foreach (DataRow row in ventaDataTable.Rows)
            {
                pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                pedido.numeroPedido = Converter.GetLong(row, "numero");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo");
                pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                pedido.incluidoIGV = Converter.GetBool(row, "incluido_igv");
                pedido.montoIGV = Converter.GetDecimal(row, "igv");
                pedido.montoTotal = Converter.GetDecimal(row, "total");
                pedido.observaciones = Converter.GetString(row, "observaciones");
                pedido.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedido.montoTotal - pedido.montoIGV));
                pedido.fechaModificacion = Converter.GetDateTime(row, "fecha_modificacion");
                pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                pedido.numeroReferenciaAdicional = Converter.GetString(row, "numero_referencia_adicional");
                pedido.numeroRequerimiento = Converter.GetString(row, "numero_requerimiento");
                pedido.direccionEntrega = new DireccionEntrega();
                pedido.direccionEntrega.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega");
                pedido.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");
                pedido.direccionEntrega.contacto = Converter.GetString(row, "contacto_entrega");
                pedido.direccionEntrega.telefono = Converter.GetString(row, "telefono_contacto_entrega");
                pedido.direccionEntrega.nombre = Converter.GetString(row, "direccion_entrega_nombre");
                pedido.direccionEntrega.codigoMP = Converter.GetString(row, "direccion_entrega_codigo_mp");
                pedido.direccionEntrega.codigoCliente = Converter.GetString(row, "direccion_entrega_codigo_cliente");
                pedido.contactoPedido = Converter.GetString(row, "contacto_pedido");
                pedido.telefonoContactoPedido = Converter.GetString(row, "telefono_contacto_pedido");
                pedido.correoContactoPedido = Converter.GetString(row, "correo_contacto_pedido");
                pedido.fechaProgramacion = Converter.GetDateTime(row, "fecha_programacion");


                pedido.observacionesFactura = Converter.GetString(row, "observaciones_factura");
                pedido.observacionesGuiaRemision = Converter.GetString(row, "observaciones_guia_remision");
                pedido.tipoPedidoCompra = (Pedido.tiposPedidoCompra)Char.Parse(Converter.GetString(row, "tipo_pedido"));
                pedido.otrosCargos = Converter.GetDecimal(row, "otros_cargos");


                compra.igv = Converter.GetDecimal(row, "igv_venta");
                compra.subTotal = Converter.GetDecimal(row, "sub_total_venta");
                compra.total = Converter.GetDecimal(row, "total_venta");
                compra.idCompra = Converter.GetGuid(row, "id_Compra");


                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                pedido.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                pedido.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                pedido.documentoPago = new DocumentoCompra();
                pedido.documentoPago.serie = Converter.GetString(row, "serie_factura");
                pedido.documentoPago.numero = Converter.GetString(row, "numero_factura");

                pedido.cotizacion = new Cotizacion();
                pedido.cotizacion.codigo = Converter.GetLong(row, "cotizacion_codigo");

                pedido.proveedor = new Proveedor();
                pedido.proveedor.codigo = Converter.GetString(row, "codigo");
                pedido.proveedor.idProveedor = Converter.GetGuid(row, "id_cliente");
                pedido.proveedor.razonSocial = Converter.GetString(row, "razon_social");
                pedido.proveedor.ruc = Converter.GetString(row, "ruc");
                pedido.proveedor.ciudad = new Ciudad();
                pedido.proveedor.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad_cliente");
                pedido.proveedor.ciudad.nombre = Converter.GetString(row, "nombre_ciudad_cliente");
                pedido.proveedor.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                pedido.proveedor.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                pedido.proveedor.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                pedido.proveedor.plazoCredito = Converter.GetString(row, "plazo_credito");
                pedido.proveedor.tipoPagoFactura = (DocumentoCompra.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                pedido.proveedor.formaPagoFactura = (DocumentoCompra.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                pedido.proveedor.tipoDocumento = Converter.GetString(row, "tipo_documento");

                pedido.ciudad = new Ciudad();
                pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                pedido.usuario = new Usuario();
                pedido.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                pedido.usuario.cargo = Converter.GetString(row, "cargo");
                pedido.usuario.contacto = Converter.GetString(row, "contacto_usuario");
                pedido.usuario.email = Converter.GetString(row, "email");

                pedido.seguimientoPedido = new SeguimientoPedido();
                pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Converter.GetInt(row, "estado_seguimiento");
                pedido.seguimientoPedido.observacion = Converter.GetString(row, "observacion_seguimiento");
                pedido.seguimientoPedido.usuario = new Usuario();
                pedido.seguimientoPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                pedido.seguimientoPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");

                pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
                pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Converter.GetInt(row, "estado_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.observacion = Converter.GetString(row, "observacion_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.usuario = new Usuario();
                pedido.seguimientoCrediticioPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento_crediticio");

                compra.notaIngreso.fechaEmision = Converter.GetDateTime(row, "fecha_emision");

            }


            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in ventaDetalleDataTable.Rows)
            {
                PedidoDetalle pedidoDetalle = new PedidoDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                pedidoDetalle.producto = new Producto();

                pedidoDetalle.idPedidoDetalle = Converter.GetGuid(row, "id_pedido_detalle");
                pedidoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                pedidoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    pedidoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                }
                
                pedidoDetalle.flete = Converter.GetDecimal(row, "flete");

                pedidoDetalle.precioUnitarioOriginal = Converter.GetDecimal(row, "precio_unitario_original");

                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.ProductoPresentacion.Equivalencia;
                }
                else
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                }

                pedidoDetalle.reverseSubTotal = Converter.GetDecimal(row, "sub_total");
                pedidoDetalle.unidad = Converter.GetString(row, "unidad");

                pedidoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                pedidoDetalle.producto.sku = Converter.GetString(row, "sku");
                pedidoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                pedidoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                pedidoDetalle.producto.proveedor = Converter.GetString(row, "proveedor");

                pedidoDetalle.producto.image = Converter.GetBytes(row, "imagen");

                pedidoDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                pedidoDetalle.observacion = Converter.GetString(row, "observaciones");


                PrecioClienteProducto precioClienteProducto = new PrecioClienteProducto();

                precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto_vigente");
                precioClienteProducto.flete = Converter.GetDecimal(row, "flete_vigente");
                precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario_vigente");
                precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia_vigente");

                precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");
                precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                precioClienteProducto.cliente = new Cliente();
                precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedidoDetalle.producto.precioClienteProducto = precioClienteProducto;

                /*Revisar terminos Venta*/
                pedidoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                pedidoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }


            pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            //Detalle de la cotizacion
            foreach (DataRow row in pedidoAdjuntoDataTable.Rows)
            {
                PedidoAdjunto pedidoAdjunto = new PedidoAdjunto();
                pedidoAdjunto.idArchivoAdjunto = Converter.GetGuid(row, "id_archivo_adjunto");
                pedidoAdjunto.adjunto = Converter.GetBytes(row, "adjunto");
                pedidoAdjunto.nombre = Converter.GetString(row, "nombre");
                pedido.pedidoAdjuntoList.Add(pedidoAdjunto);
            }

            return compra;
        }



        public void UpdateCompra(Compra compra)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);

            foreach (PedidoDetalle ventaDetalle in compra.pedido.pedidoDetalleList)
            {
                this.UpdateCompraDetalle(ventaDetalle);
            }

            //Actualiza los totales de la venta
            var objCommand = GetSqlCommand("pu_compra");
            InputParameterAdd.Guid(objCommand, "idCompra", compra.idCompra);
            InputParameterAdd.Varchar(objCommand, "observaciones", compra.observaciones);
            ExecuteNonQuery(objCommand);

            this.Commit();
        }



        public void UpdateCompraDetalle(PedidoDetalle ventaDetalle)
        {
            var objCommand = GetSqlCommand("pu_compraDetalle");
            InputParameterAdd.Guid(objCommand, "idVentaDetalle", ventaDetalle.idVentaDetalle);
            InputParameterAdd.Decimal(objCommand, "precioUnitario", ventaDetalle.precioUnitario);
            InputParameterAdd.Decimal(objCommand, "precioNeto", ventaDetalle.precioNeto);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", ventaDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "flete", ventaDetalle.flete);
            InputParameterAdd.Decimal(objCommand, "subTotal", ventaDetalle.subTotal);
            ExecuteNonQuery(objCommand);

        }




        public Transaccion SelectPlantillaCompra(Compra transaccion, Usuario usuario, Guid? idProducto = null, List<Guid> idProductoList = null)
        {
            var objCommand = GetSqlCommand("ps_plantillaCompra");

            if (idProducto != null)
            {
                objCommand = GetSqlCommand("ps_plantillaCompraDescuentoGlobal");
                InputParameterAdd.Guid(objCommand, "idProducto", idProducto.Value);
            }
            else if (idProductoList != null)
            {
                DataTable tvp = new DataTable();
                tvp.Columns.Add(new DataColumn("idProductoList", typeof(Guid)));

                // populate DataTable from your List here
                foreach (var id in idProductoList)
                    tvp.Rows.Add(id);


                objCommand = GetSqlCommand("ps_plantillaCompraCargos");

                SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idProductoList", tvp);
                // these next lines are important to map the C# DataTable object to the correct SQL User Defined Type
                tvparam.SqlDbType = SqlDbType.Structured;
                tvparam.TypeName = "dbo.UniqueIdentifierList";

                //InputParameterAdd.Varchar(objCommand, "idProductoList", tvparam);
            }

            InputParameterAdd.Int(objCommand, "idDocumentoCompra", transaccion.documentoCompra.idDocumentoCompra);
            InputParameterAdd.Int(objCommand, "tipoDocumento", (int)transaccion.documentoCompra.tipoDocumento);

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);

            DataSet dataSet = ExecuteDataSet(objCommand);
            transaccion.tipoErrorCrearTransaccion = (Compra.TiposErrorCrearTransaccion)(int)objCommand.Parameters["@tipoError"].Value;
            transaccion.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            if (transaccion.tipoErrorCrearTransaccion == Compra.TiposErrorCrearTransaccion.NoExisteError)
            {
                DataTable ventaDataTable = dataSet.Tables[0];
                DataTable ventaDetalleDataTable = dataSet.Tables[1];

                //   DataTable dataTable = Execute(objCommand);
                //Datos de la cotizacion
                transaccion.pedido = new Pedido();
                Pedido pedido = transaccion.pedido;

                foreach (DataRow row in ventaDataTable.Rows)
                {
                    transaccion.cliente = new Cliente();
                    transaccion.cliente.codigo = Converter.GetString(row, "codigo");
                    transaccion.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                    transaccion.cliente.razonSocial = Converter.GetString(row, "razon_social");
                    transaccion.cliente.ruc = Converter.GetString(row, "ruc");
                    transaccion.cliente.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                    transaccion.cliente.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                    transaccion.cliente.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                    transaccion.cliente.plazoCredito = Converter.GetString(row, "plazo_credito");
                    ///REVISAR
                    transaccion.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                    transaccion.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                    transaccion.cliente.ciudad = new Ciudad();
                    transaccion.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                    transaccion.documentoCompra.serie = Converter.GetString(row, "serie");
                    transaccion.documentoCompra.numero = Converter.GetInt(row, "correlativo").ToString().PadLeft(8, '0');
                    transaccion.pedido.numeroReferenciaAdicional = Converter.GetString(row, "numero_referencia_adicional");
                    transaccion.pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                    transaccion.pedido.observacionesFactura = Converter.GetString(row, "observaciones");
                    transaccion.guiaRemision = new GuiaRemision();
                    transaccion.guiaRemision.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                    transaccion.guiaRemision.serieDocumento = Converter.GetString(row, "serie_guia_remision");
                    transaccion.guiaRemision.numeroDocumento = Converter.GetInt(row, "numero_guia_remision");
                }


                pedido.pedidoDetalleList = new List<PedidoDetalle>();
                //Detalle de la cotizacion
                foreach (DataRow row in ventaDetalleDataTable.Rows)
                {
                    PedidoDetalle pedidoDetalle = new PedidoDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                    pedidoDetalle.producto = new Producto();

                    pedidoDetalle.idPedidoDetalle = Converter.GetGuid(row, "id_pedido_detalle");
                    pedidoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                    pedidoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                    pedidoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");
                    pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                    if (pedidoDetalle.esPrecioAlternativo)
                    {
                        pedidoDetalle.ProductoPresentacion = new ProductoPresentacion();
                        pedidoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                    }


                    pedidoDetalle.flete = Converter.GetDecimal(row, "flete");

                    pedidoDetalle.precioUnitarioOriginal = Converter.GetDecimal(row, "precio_unitario_original");
                    pedidoDetalle.cantidadOriginal = Converter.GetInt(row, "cantidad");

                    //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                    pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                    pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                    //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar


                    if (pedidoDetalle.esPrecioAlternativo)
                    {
                        pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.ProductoPresentacion.Equivalencia;
                    }
                    else
                    {
                        pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                    }

                    pedidoDetalle.unidad = Converter.GetString(row, "unidad");
                    pedidoDetalle.unidadInternacional = Converter.GetString(row, "unidad_internacional");

                    pedidoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                    pedidoDetalle.producto.sku = Converter.GetString(row, "sku");
                    pedidoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                    pedidoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                    pedidoDetalle.producto.proveedor = Converter.GetString(row, "proveedor");

                    pedidoDetalle.producto.image = Converter.GetBytes(row, "imagen");

                    pedidoDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                    pedidoDetalle.observacion = Converter.GetString(row, "observaciones");


                    PrecioClienteProducto precioClienteProducto = new PrecioClienteProducto();

                    precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto_vigente");
                    precioClienteProducto.flete = Converter.GetDecimal(row, "flete_vigente");
                    precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario_vigente");
                    precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia_vigente");

                    precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");
                    precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                    precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                    precioClienteProducto.cliente = new Cliente();
                    precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                    pedidoDetalle.producto.precioClienteProducto = precioClienteProducto;

                    //Revisar
                    pedidoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                    pedidoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                    pedido.pedidoDetalleList.Add(pedidoDetalle);
                }
                pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            }
            return transaccion;
        }



        /*








        public void InsertTransaccionNotaCredito(Compra transaccionExtorno)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_ventaNotaCredito");
            InputParameterAdd.Guid(objCommand, "idUsuario", transaccionExtorno.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "serieDocumentoReferencia", transaccionExtorno.documentoReferencia.serie);
            InputParameterAdd.Varchar(objCommand, "numeroDocumentoReferencia", transaccionExtorno.documentoReferencia.numero);
            InputParameterAdd.Int(objCommand, "tipoDocumentoReferencia", (int)transaccionExtorno.documentoReferencia.tipoDocumento);
            InputParameterAdd.DateTime(objCommand, "fechaEmisionDocumentoReferencia", transaccionExtorno.documentoReferencia.fechaEmision);
            InputParameterAdd.Guid(objCommand, "idCiudad", transaccionExtorno.cliente.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", transaccionExtorno.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "observaciones", transaccionExtorno.observaciones);
            InputParameterAdd.Varchar(objCommand, "sustento", transaccionExtorno.sustento);
            InputParameterAdd.Int(objCommand, "tipoNotaCredito", (int)transaccionExtorno.tipoNotaCredito);
            InputParameterAdd.DateTime(objCommand, "fechaEmision", transaccionExtorno.documentoCompra.fechaEmision);
            


            OutputParameterAdd.UniqueIdentifier(objCommand, "idCompra");
            OutputParameterAdd.BigInt(objCommand, "numeroCompra");
            OutputParameterAdd.UniqueIdentifier(objCommand, "idDocumentoReferenciaCompra");

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);

            transaccionExtorno.idCompra = (Int)objCommand.Parameters["@idCompra"].Value;
            transaccionExtorno.numero = (Int64)objCommand.Parameters["@numeroCompra"].Value;
            transaccionExtorno.documentoReferencia.idDocumentoReferenciaCompra = (Guid)objCommand.Parameters["@idDocumentoReferenciaCompra"].Value;

            var tipoErrorValidacion = (GuiaRemisionValidacion.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            var descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            this.InsertCompraDetalle(transaccionExtorno);

            objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idCompra", transaccionExtorno.idCompra);
            InputParameterAdd.Varchar(objCommand, "observaciones", "Se crea Transacción.");
            ExecuteNonQuery(objCommand);

            this.Commit();

        }

        public void UpdateTransaccionNotaCredito(Compra transaccionExtorno)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pu_ventaNotaCredito");
            transaccionExtorno.idCompra = transaccionExtorno.documentoCompra.movimientoAlmacen.venta.idCompra;

            InputParameterAdd.Guid(objCommand, "idCompra", transaccionExtorno.idCompra);
            InputParameterAdd.Guid(objCommand, "idUsuario", transaccionExtorno.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "serieDocumentoReferencia", transaccionExtorno.documentoReferencia.serie);
            InputParameterAdd.Varchar(objCommand, "numeroDocumentoReferencia", transaccionExtorno.documentoReferencia.numero);
            InputParameterAdd.Int(objCommand, "tipoDocumentoReferencia", (int)transaccionExtorno.documentoReferencia.tipoDocumento);
            InputParameterAdd.DateTime(objCommand, "fechaEmisionDocumentoReferencia", transaccionExtorno.documentoReferencia.fechaEmision);

          
            transaccionExtorno.documentoCompra.observaciones = transaccionExtorno.observaciones;
            InputParameterAdd.Varchar(objCommand, "observaciones", transaccionExtorno.observaciones);
            InputParameterAdd.Varchar(objCommand, "sustento", transaccionExtorno.sustento);
            InputParameterAdd.Int(objCommand, "tipoNotaCredito", (int)transaccionExtorno.tipoNotaCredito);
            InputParameterAdd.DateTime(objCommand, "fechaEmision", transaccionExtorno.documentoCompra.fechaEmision);

            OutputParameterAdd.UniqueIdentifier(objCommand, "idDocumentoReferenciaCompra");

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);

            transaccionExtorno.documentoReferencia.idDocumentoReferenciaCompra = (Guid)objCommand.Parameters["@idDocumentoReferenciaCompra"].Value;

            var tipoErrorValidacion = (GuiaRemisionValidacion.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            var descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            //this.InsertCompraDetalle(transaccionExtorno);

            objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idCompra", transaccionExtorno.idCompra);
            InputParameterAdd.Varchar(objCommand, "observaciones", "Se actualiza Transacción.");
            ExecuteNonQuery(objCommand);

            this.Commit();

        }


        public void InsertCompraDetalle(Transaccion venta)
        {
            foreach (PedidoDetalle documentoDetalle in venta.pedido.pedidoDetalleList)
            {
                var objCommand = GetSqlCommand("pi_ventaDetalle");
                InputParameterAdd.Guid(objCommand, "idCompra", venta.idCompra);
                InputParameterAdd.Guid(objCommand, "idUsuario", venta.usuario.idUsuario);
                InputParameterAdd.Guid(objCommand, "idProducto", documentoDetalle.producto.idProducto);
                InputParameterAdd.Int(objCommand, "cantidad", documentoDetalle.cantidad);
                InputParameterAdd.Varchar(objCommand, "observaciones", documentoDetalle.observacion);
                InputParameterAdd.Varchar(objCommand, "unidadInternacional", documentoDetalle.unidadInternacional);
                InputParameterAdd.Decimal(objCommand, "precioUnitario", documentoDetalle.precioUnitario);
                InputParameterAdd.Int(objCommand, "equivalencia", documentoDetalle.producto.equivalencia);
                InputParameterAdd.Varchar(objCommand, "unidad", documentoDetalle.unidad);
                InputParameterAdd.Int(objCommand, "esPrecioAlternativo", documentoDetalle.esPrecioAlternativo ? 1 : 0);

                ExecuteNonQuery(objCommand);
            }
        }






        public void InsertCompraNotaDebito(Compra venta)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_ventaNotaDebito");
            InputParameterAdd.Guid(objCommand, "idUsuario", venta.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "serieDocumentoReferencia", venta.documentoReferencia.serie);
            InputParameterAdd.Varchar(objCommand, "numeroDocumentoReferencia", venta.documentoReferencia.numero);
            InputParameterAdd.Int(objCommand, "tipoDocumentoReferencia", (int)venta.documentoReferencia.tipoDocumento);
            InputParameterAdd.DateTime(objCommand, "fechaEmisionDocumentoReferencia", venta.documentoReferencia.fechaEmision);
            InputParameterAdd.Guid(objCommand, "idCiudad", venta.cliente.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", venta.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "observaciones", venta.observaciones);
            InputParameterAdd.Varchar(objCommand, "sustento", venta.sustento);
            InputParameterAdd.Int(objCommand, "tipoNotaDebito", (int)venta.tipoNotaDebito);
            InputParameterAdd.DateTime(objCommand, "fechaEmision", venta.documentoCompra.fechaEmision);



            OutputParameterAdd.UniqueIdentifier(objCommand, "idCompra");
            OutputParameterAdd.BigInt(objCommand, "numeroCompra");
            OutputParameterAdd.UniqueIdentifier(objCommand, "idDocumentoReferenciaCompra");

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);

            venta.idCompra = (Int)objCommand.Parameters["@idCompra"].Value;
            venta.numero = (Int64)objCommand.Parameters["@numeroCompra"].Value;
            venta.documentoReferencia.idDocumentoReferenciaCompra = (Guid)objCommand.Parameters["@idDocumentoReferenciaCompra"].Value;

            var tipoErrorValidacion = (GuiaRemisionValidacion.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            var descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            this.InsertCompraDetalle(venta);


            objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idCompra", venta.idCompra);
            InputParameterAdd.Varchar(objCommand, "observaciones", "Se crea Transacción.");
            ExecuteNonQuery(objCommand);

            this.Commit();

        }



        
     


   

        public Transaccion SelectNotaIngresoTransaccion(Transaccion transaccion, NotaIngreso notaIngreso, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_notaIngresoTransaccion");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", notaIngreso.idMovimientoAlmacen);

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);

            DataSet dataSet = ExecuteDataSet(objCommand);
            transaccion.tipoErrorCrearTransaccion = (Compra.TiposErrorCrearTransaccion)(int)objCommand.Parameters["@tipoError"].Value;
            transaccion.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            if (transaccion.tipoErrorCrearTransaccion == Compra.TiposErrorCrearTransaccion.NoExisteError)
            {
                DataTable ventaDataTable = dataSet.Tables[0];
                DataTable ventaDetalleDataTable = dataSet.Tables[1];

                //   DataTable dataTable = Execute(objCommand);
                //Datos de la cotizacion
                transaccion.pedido = new Pedido();
                Pedido pedido = transaccion.pedido;

                foreach (DataRow row in ventaDataTable.Rows)
                {
                    transaccion.cliente = new Cliente();
                    transaccion.cliente.codigo = Converter.GetString(row, "codigo");
                    transaccion.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                    transaccion.cliente.razonSocial = Converter.GetString(row, "razon_social");
                    transaccion.cliente.ruc = Converter.GetString(row, "ruc");
                    transaccion.cliente.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                    transaccion.cliente.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                    transaccion.cliente.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                    transaccion.cliente.plazoCredito = Converter.GetString(row, "plazo_credito");
                    transaccion.cliente.tipoPagoFactura = (DocumentoCompra.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                    transaccion.cliente.formaPagoFactura = (DocumentoCompra.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                    transaccion.cliente.ciudad = new Ciudad();
                    transaccion.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                    transaccion.documentoCompra.serie = Converter.GetString(row, "serie");
                    transaccion.documentoCompra.numero = Converter.GetInt(row, "correlativo").ToString().PadLeft(8, '0');
                    transaccion.pedido.numeroReferenciaAdicional = Converter.GetString(row, "numero_referencia_adicional");
                    transaccion.pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                    transaccion.pedido.observacionesFactura = Converter.GetString(row, "observaciones");
                    transaccion.guiaRemision = new GuiaRemision();
                    transaccion.guiaRemision.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                    transaccion.guiaRemision.serieDocumento = Converter.GetString(row, "serie_guia_remision");
                    transaccion.guiaRemision.numeroDocumento = Converter.GetInt(row, "numero_guia_remision");
                }


                pedido.pedidoDetalleList = new List<PedidoDetalle>();
                //Detalle de la cotizacion
                foreach (DataRow row in ventaDetalleDataTable.Rows)
                {
                    PedidoDetalle pedidoDetalle = new PedidoDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                    pedidoDetalle.producto = new Producto();

                    pedidoDetalle.idPedidoDetalle = Converter.GetGuid(row, "id_pedido_detalle");
                    pedidoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                    pedidoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                    pedidoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");
                    pedidoDetalle.producto.equivalencia = Convert.ToInt32(Converter.GetDecimal(row, "equivalencia"));
                    pedidoDetalle.cantidadOriginal = pedidoDetalle.cantidad;
                    pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                    pedidoDetalle.flete = Converter.GetDecimal(row, "flete");

                    pedidoDetalle.precioUnitarioOriginal = Converter.GetDecimal(row, "precio_unitario_original");

                    //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                    pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                    pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                    //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar


                    if (pedidoDetalle.esPrecioAlternativo)
                    {
                        pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.producto.equivalencia;
                    }
                    else
                    {
                        pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                    }

                    pedidoDetalle.unidad = Converter.GetString(row, "unidad");
                    pedidoDetalle.unidadInternacional = Converter.GetString(row, "unidad_internacional");

                    pedidoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                    pedidoDetalle.producto.sku = Converter.GetString(row, "sku");
                    pedidoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                    pedidoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                    pedidoDetalle.producto.proveedor = Converter.GetString(row, "proveedor");

                    pedidoDetalle.producto.image = Converter.GetBytes(row, "imagen");

                    pedidoDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                    pedidoDetalle.observacion = Converter.GetString(row, "observaciones");


                    PrecioClienteProducto precioClienteProducto = new PrecioClienteProducto();

                    precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto_vigente");
                    precioClienteProducto.flete = Converter.GetDecimal(row, "flete_vigente");
                    precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario_vigente");
                    precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia_vigente");

                    precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");
                    precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                    precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                    precioClienteProducto.cliente = new Cliente();
                    precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                    pedidoDetalle.producto.precioClienteProducto = precioClienteProducto;


                    pedidoDetalle.precioUnitarioCompra = Converter.GetDecimal(row, "precio_unitario_venta");
                    pedidoDetalle.idCompraDetalle = Converter.GetGuid(row, "id_venta_detalle");

                    pedido.pedidoDetalleList.Add(pedidoDetalle);
                }
                pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            }
            return transaccion;
        }

       



        public Compra SelectCompraConsolidada(Compra venta, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_ventaConsolidada");
            InputParameterAdd.Guid(objCommand, "idCompra", venta.idCompra);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable ventaDataTable = dataSet.Tables[0];
            DataTable ventaDetalleDataTable = dataSet.Tables[1];
            //DataTable pedidoAdjuntoDataTable = dataSet.Tables[2];


            //   DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            venta.pedido = new Pedido();
            Pedido pedido = venta.pedido;

            foreach (DataRow row in ventaDataTable.Rows)
            {
                pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                pedido.numeroPedido = Converter.GetLong(row, "numero");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo");
                pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                pedido.incluidoIGV = Converter.GetBool(row, "incluido_igv");
                pedido.montoIGV = Converter.GetDecimal(row, "igv");
                pedido.montoTotal = Converter.GetDecimal(row, "total");
                pedido.observaciones = Converter.GetString(row, "observaciones");
                pedido.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedido.montoTotal - pedido.montoIGV));
                pedido.fechaModificacion = Converter.GetDateTime(row, "fecha_modificacion");
                pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                pedido.numeroReferenciaAdicional = Converter.GetString(row, "numero_referencia_adicional");
                pedido.direccionEntrega = new DireccionEntrega();
                pedido.direccionEntrega.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega");
                pedido.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");
                pedido.direccionEntrega.contacto = Converter.GetString(row, "contacto_entrega");
                pedido.direccionEntrega.telefono = Converter.GetString(row, "telefono_contacto_entrega");
                pedido.contactoPedido = Converter.GetString(row, "contacto_pedido");
                pedido.telefonoContactoPedido = Converter.GetString(row, "telefono_contacto_pedido");
                pedido.correoContactoPedido = Converter.GetString(row, "correo_contacto_pedido");
                pedido.fechaProgramacion = Converter.GetDateTime(row, "fecha_programacion");
                pedido.observacionesFactura = Converter.GetString(row, "observaciones_factura");
                pedido.observacionesGuiaRemision = Converter.GetString(row, "observaciones_guia_remision");
                pedido.tipoPedido = (Pedido.tiposPedido)Char.Parse(Converter.GetString(row, "tipo_pedido"));
                pedido.otrosCargos = Converter.GetDecimal(row, "otros_cargos");


                venta.igv = Converter.GetDecimal(row, "igv_venta");
                venta.subTotal = Converter.GetDecimal(row, "sub_total_venta");
                venta.total = Converter.GetDecimal(row, "total_venta");
                venta.idCompra = Converter.GetGuid(row, "id_Compra");


                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                pedido.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                pedido.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                pedido.documentoCompra = new DocumentoCompra();
                pedido.documentoCompra.serie = Converter.GetString(row, "serie_factura");
                pedido.documentoCompra.numero = Converter.GetString(row, "numero_factura");

                pedido.cotizacion = new Cotizacion();
                pedido.cotizacion.codigo = Converter.GetLong(row, "cotizacion_codigo");

                pedido.cliente = new Cliente();
                pedido.cliente.codigo = Converter.GetString(row, "codigo");
                pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                pedido.cliente.ruc = Converter.GetString(row, "ruc");
                pedido.cliente.ciudad = new Ciudad();
                pedido.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad_cliente");
                pedido.cliente.ciudad.nombre = Converter.GetString(row, "nombre_ciudad_cliente");
                pedido.cliente.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                pedido.cliente.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                pedido.cliente.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                pedido.cliente.plazoCredito = Converter.GetString(row, "plazo_credito");
                pedido.cliente.tipoPagoFactura = (DocumentoCompra.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                pedido.cliente.formaPagoFactura = (DocumentoCompra.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                pedido.cliente.tipoDocumento = Converter.GetString(row, "tipo_documento");

                pedido.ciudad = new Ciudad();
                pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                pedido.usuario = new Usuario();
                pedido.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                pedido.usuario.cargo = Converter.GetString(row, "cargo");
                pedido.usuario.contacto = Converter.GetString(row, "contacto_usuario");
                pedido.usuario.email = Converter.GetString(row, "email");

          
                venta.guiaRemision.fechaEmision = Converter.GetDateTime(row, "fecha_emision");

            }


            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in ventaDetalleDataTable.Rows)
            {
                PedidoDetalle pedidoDetalle = new PedidoDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                pedidoDetalle.producto = new Producto();

                pedidoDetalle.idPedidoDetalle = Converter.GetGuid(row, "id_pedido_detalle");
                pedidoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                pedidoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.producto.equivalencia = Convert.ToInt32(Converter.GetDecimal(row, "equivalencia"));
                pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                pedidoDetalle.flete = Converter.GetDecimal(row, "flete");

                pedidoDetalle.precioUnitarioOriginal = Converter.GetDecimal(row, "precio_unitario_original");

                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.producto.equivalencia;
                    pedidoDetalle.porcentajeDescuento = 100 - ((pedidoDetalle.precioNeto * pedidoDetalle.producto.equivalencia) * 100 / pedidoDetalle.producto.precioSinIgv);
                }
                else
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                    pedidoDetalle.porcentajeDescuento = 100 - (pedidoDetalle.precioNeto * 100 / pedidoDetalle.producto.precioSinIgv);
                }               

                pedidoDetalle.unidad = Converter.GetString(row, "unidad");

                pedidoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                pedidoDetalle.producto.sku = Converter.GetString(row, "sku");
                pedidoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                pedidoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                pedidoDetalle.producto.proveedor = Converter.GetString(row, "proveedor");

                pedidoDetalle.producto.image = Converter.GetBytes(row, "imagen");

           
                pedidoDetalle.observacion = Converter.GetString(row, "observaciones");


                PrecioClienteProducto precioClienteProducto = new PrecioClienteProducto();

                pedidoDetalle.producto.precioClienteProducto = precioClienteProducto;


                pedidoDetalle.precioUnitarioCompra = Converter.GetDecimal(row, "precio_unitario_venta");
                pedidoDetalle.idCompraDetalle = Converter.GetGuid(row, "id_venta_detalle");

                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }


            pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            //Detalle de la cotizacion


            return venta;
        }




        //Se crea venta consolidada la cual tomará los datos base de la guía de remisión


        public void InsertCompraConsolidada(Compra venta)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_ventaConsolidada");
            InputParameterAdd.Guid(objCommand, "idUsuario", venta.usuario.idUsuario);
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", venta.guiaRemision.idMovimientoAlmacen);
            InputParameterAdd.Guid(objCommand, "idPedido", venta.guiaRemision.pedido.idPedido);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", venta.guiaRemision.pedido.numeroReferenciaCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaAdicional", venta.guiaRemision.pedido.numeroReferenciaAdicional);
            InputParameterAdd.Guid(objCommand, "idCiudad", venta.guiaRemision.ciudadOrigen.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", venta.guiaRemision.pedido.cliente.idCliente);
            InputParameterAdd.DateTime(objCommand, "fechaEmision", DateTime.Now);


            OutputParameterAdd.UniqueIdentifier(objCommand, "idCompra");
            OutputParameterAdd.BigInt(objCommand, "numeroCompra");

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);

            venta.idCompra = (Guid)objCommand.Parameters["@idCompra"].Value;
            venta.numero = (Int64)objCommand.Parameters["@numeroCompra"].Value;

            var tipoErrorValidacion = (GuiaRemisionValidacion.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            var descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            this.InsertCompraDetalle(venta);

            objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idCompra", venta.idCompra);
            InputParameterAdd.Varchar(objCommand, "observaciones", "Se crea Compra Consolidada.");
            ExecuteNonQuery(objCommand);

            this.Commit();
        }


        public void InsertCompraRefacturacion(Compra venta)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_ventaRefacturacion");
            InputParameterAdd.Guid(objCommand, "idUsuario", venta.guiaRemision.usuario.idUsuario);
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", venta.guiaRemision.idMovimientoAlmacen);
            InputParameterAdd.DateTime(objCommand, "fecha", DateTime.Now);
            OutputParameterAdd.UniqueIdentifier(objCommand, "idCompra");
            OutputParameterAdd.BigInt(objCommand, "numeroCompra");            
            ExecuteNonQuery(objCommand);
            venta.idCompra = (Guid)objCommand.Parameters["@idCompra"].Value;
            venta.numero = (Int64)objCommand.Parameters["@numeroCompra"].Value;
            this.Commit();
        }
        */
    }
}
