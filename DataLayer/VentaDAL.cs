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
    public class VentaDAL : DaoBase
    {
        public VentaDAL(IDalSettings settings) : base(settings)
        {
        }

        public VentaDAL() : this(new CotizadorSettings())
        {
        }

        public void InsertTransaccionNotaCredito(Transaccion transaccionExtorno)
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
            InputParameterAdd.DateTime(objCommand, "fechaEmision", transaccionExtorno.documentoVenta.fechaEmision);



            OutputParameterAdd.UniqueIdentifier(objCommand, "idVenta");
            OutputParameterAdd.BigInt(objCommand, "numeroVenta");
            OutputParameterAdd.UniqueIdentifier(objCommand, "idDocumentoReferenciaVenta");

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);

            transaccionExtorno.idVenta = (Guid)objCommand.Parameters["@idVenta"].Value;
            transaccionExtorno.numero = (Int64)objCommand.Parameters["@numeroVenta"].Value;
            transaccionExtorno.documentoReferencia.idDocumentoReferenciaVenta = (Guid)objCommand.Parameters["@idDocumentoReferenciaVenta"].Value;

            var tipoErrorValidacion = (GuiaRemisionValidacion.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            var descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            this.InsertVentaDetalle(transaccionExtorno);

            objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idVenta", transaccionExtorno.idVenta);
            InputParameterAdd.Varchar(objCommand, "observaciones", "Se crea Transacción.");
            ExecuteNonQuery(objCommand);

            this.Commit();

        }

        public void UpdateTransaccionNotaCredito(Transaccion transaccionExtorno)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pu_ventaNotaCredito");
            transaccionExtorno.idVenta = transaccionExtorno.documentoVenta.movimientoAlmacen.venta.idVenta;

            InputParameterAdd.Guid(objCommand, "idVenta", transaccionExtorno.idVenta);
            InputParameterAdd.Guid(objCommand, "idUsuario", transaccionExtorno.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "serieDocumentoReferencia", transaccionExtorno.documentoReferencia.serie);
            InputParameterAdd.Varchar(objCommand, "numeroDocumentoReferencia", transaccionExtorno.documentoReferencia.numero);
            InputParameterAdd.Int(objCommand, "tipoDocumentoReferencia", (int)transaccionExtorno.documentoReferencia.tipoDocumento);
            InputParameterAdd.DateTime(objCommand, "fechaEmisionDocumentoReferencia", transaccionExtorno.documentoReferencia.fechaEmision);

            /*
            InputParameterAdd.Guid(objCommand, "idCiudad", transaccionExtorno.cliente.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", transaccionExtorno.cliente.idCliente);
            */
            transaccionExtorno.documentoVenta.observaciones = transaccionExtorno.observaciones;
            InputParameterAdd.Varchar(objCommand, "observaciones", transaccionExtorno.observaciones);
            InputParameterAdd.Varchar(objCommand, "sustento", transaccionExtorno.sustento);
            InputParameterAdd.Int(objCommand, "tipoNotaCredito", (int)transaccionExtorno.tipoNotaCredito);
            InputParameterAdd.DateTime(objCommand, "fechaEmision", transaccionExtorno.documentoVenta.fechaEmision);

            OutputParameterAdd.UniqueIdentifier(objCommand, "idDocumentoReferenciaVenta");

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);

            transaccionExtorno.documentoReferencia.idDocumentoReferenciaVenta = (Guid)objCommand.Parameters["@idDocumentoReferenciaVenta"].Value;

            var tipoErrorValidacion = (GuiaRemisionValidacion.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            var descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            //this.InsertVentaDetalle(transaccionExtorno);

            objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idVenta", transaccionExtorno.idVenta);
            InputParameterAdd.Varchar(objCommand, "observaciones", "Se actualiza Transacción.");
            ExecuteNonQuery(objCommand);

            this.Commit();

        }


        public void InsertVentaDetalle(Transaccion venta)
        {
            foreach (PedidoDetalle documentoDetalle in venta.pedido.pedidoDetalleList)
            {
                var objCommand = GetSqlCommand("pi_ventaDetalle");
                InputParameterAdd.Guid(objCommand, "idVenta", venta.idVenta);
                InputParameterAdd.Guid(objCommand, "idUsuario", venta.usuario.idUsuario);
                InputParameterAdd.Guid(objCommand, "idProducto", documentoDetalle.producto.idProducto);
                InputParameterAdd.Int(objCommand, "cantidad", documentoDetalle.cantidad);
                InputParameterAdd.Varchar(objCommand, "observaciones", documentoDetalle.observacion);
                InputParameterAdd.Varchar(objCommand, "unidadInternacional", documentoDetalle.unidadInternacional);
                InputParameterAdd.Decimal(objCommand, "precioUnitario", documentoDetalle.precioUnitario);
                if (documentoDetalle.esPrecioAlternativo)
                    InputParameterAdd.Decimal(objCommand, "equivalencia", documentoDetalle.ProductoPresentacion.Equivalencia);
                else
                    InputParameterAdd.Decimal(objCommand, "equivalencia", 1);
                InputParameterAdd.Varchar(objCommand, "unidad", documentoDetalle.unidad);
                InputParameterAdd.Int(objCommand, "esPrecioAlternativo", documentoDetalle.esPrecioAlternativo ? 1 : 0);

                ExecuteNonQuery(objCommand);
            }
        }






        public void InsertVentaNotaDebito(Venta venta)
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
            InputParameterAdd.DateTime(objCommand, "fechaEmision", venta.documentoVenta.fechaEmision);



            OutputParameterAdd.UniqueIdentifier(objCommand, "idVenta");
            OutputParameterAdd.BigInt(objCommand, "numeroVenta");
            OutputParameterAdd.UniqueIdentifier(objCommand, "idDocumentoReferenciaVenta");

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);

            venta.idVenta = (Guid)objCommand.Parameters["@idVenta"].Value;
            venta.numero = (Int64)objCommand.Parameters["@numeroVenta"].Value;
            venta.documentoReferencia.idDocumentoReferenciaVenta = (Guid)objCommand.Parameters["@idDocumentoReferenciaVenta"].Value;

            var tipoErrorValidacion = (GuiaRemisionValidacion.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            var descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            this.InsertVentaDetalle(venta);


            objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idVenta", venta.idVenta);
            InputParameterAdd.Varchar(objCommand, "observaciones", "Se crea Transacción.");
            ExecuteNonQuery(objCommand);

            this.Commit();

        }




        public void UpdateVenta(Venta venta)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);

            foreach (PedidoDetalle ventaDetalle in venta.pedido.pedidoDetalleList)
            {
                this.UpdateVentaDetalle(ventaDetalle);
            }

            //Actualiza los totales de la venta
            var objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idVenta", venta.idVenta);
            InputParameterAdd.Varchar(objCommand, "observaciones", venta.observaciones);
            ExecuteNonQuery(objCommand);

            this.Commit();
        }


        public void UpdateVentaDetalle(PedidoDetalle ventaDetalle)
        {
            var objCommand = GetSqlCommand("pu_ventaDetalle");
            InputParameterAdd.Guid(objCommand, "idVentaDetalle", ventaDetalle.idVentaDetalle);
            InputParameterAdd.Decimal(objCommand, "precioUnitario", ventaDetalle.precioUnitario);
            InputParameterAdd.Decimal(objCommand, "precioNeto", ventaDetalle.precioNeto);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", ventaDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "flete", ventaDetalle.flete);
            ExecuteNonQuery(objCommand);

        }




        public Transaccion SelectPlantillaVenta(Transaccion transaccion, Usuario usuario, Guid? idProducto = null, List<Guid> idProductoList = null)
        {
            var objCommand = GetSqlCommand("ps_plantillaVenta");
            /*Si se cuenta con id Producto entonces quiere decir que es un descuento global*/
            if (idProducto != null)
            {
                objCommand = GetSqlCommand("ps_plantillaVentaDescuentoGlobal");
                InputParameterAdd.Guid(objCommand, "idProducto", idProducto.Value);
            }
            else if (idProductoList != null)
            {
                DataTable tvp = new DataTable();
                tvp.Columns.Add(new DataColumn("idProductoList", typeof(Guid)));

                // populate DataTable from your List here
                foreach (var id in idProductoList)
                    tvp.Rows.Add(id);


                objCommand = GetSqlCommand("ps_plantillaVentaCargos");

                SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idProductoList", tvp);
                // these next lines are important to map the C# DataTable object to the correct SQL User Defined Type
                tvparam.SqlDbType = SqlDbType.Structured;
                tvparam.TypeName = "dbo.UniqueIdentifierList";

                //InputParameterAdd.Varchar(objCommand, "idProductoList", tvparam);
            }

            InputParameterAdd.Guid(objCommand, "idDocumentoVenta", transaccion.documentoVenta.idDocumentoVenta);
            InputParameterAdd.Int(objCommand, "tipoDocumento", (int)transaccion.documentoVenta.tipoDocumento);

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);

            DataSet dataSet = ExecuteDataSet(objCommand);
            transaccion.tipoErrorCrearTransaccion = (Venta.TiposErrorCrearTransaccion)(int)objCommand.Parameters["@tipoError"].Value;
            transaccion.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
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
                    transaccion.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                    transaccion.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                    transaccion.cliente.ciudad = new Ciudad();
                    transaccion.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                    transaccion.documentoVenta.serie = Converter.GetString(row, "serie");
                    transaccion.documentoVenta.numero = Converter.GetInt(row, "correlativo").ToString().PadLeft(8, '0');
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


                    pedidoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                    pedidoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                    pedido.pedidoDetalleList.Add(pedidoDetalle);
                }
                pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            }
            return transaccion;
        }


        public Transaccion SelectNotaIngresoTransaccion(Transaccion transaccion, NotaIngreso notaIngreso, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_notaIngresoTransaccion");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", notaIngreso.idMovimientoAlmacen);

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);

            DataSet dataSet = ExecuteDataSet(objCommand);
            transaccion.tipoErrorCrearTransaccion = (Venta.TiposErrorCrearTransaccion)(int)objCommand.Parameters["@tipoError"].Value;
            transaccion.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
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
                    transaccion.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                    transaccion.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                    transaccion.cliente.ciudad = new Ciudad();
                    transaccion.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                    transaccion.documentoVenta.serie = Converter.GetString(row, "serie");
                    transaccion.documentoVenta.numero = Converter.GetInt(row, "correlativo").ToString().PadLeft(8, '0');
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



                    pedidoDetalle.cantidadOriginal = pedidoDetalle.cantidad;

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


                    pedidoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                    pedidoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                    pedido.pedidoDetalleList.Add(pedidoDetalle);
                }
                pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            }
            return transaccion;
        }

        public Venta SelectVenta(Venta venta, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_venta");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", venta.guiaRemision.idMovimientoAlmacen);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable ventaDataTable = dataSet.Tables[0];
            DataTable ventaDetalleDataTable = dataSet.Tables[1];
            DataTable pedidoAdjuntoDataTable = dataSet.Tables[2];


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
                pedido.tipoPedido = (Pedido.tiposPedido)Char.Parse(Converter.GetString(row, "tipo_pedido"));
                pedido.otrosCargos = Converter.GetDecimal(row, "otros_cargos");


                venta.igv = Converter.GetDecimal(row, "igv_venta");
                venta.subTotal = Converter.GetDecimal(row, "sub_total_venta");
                venta.total = Converter.GetDecimal(row, "total_venta");
                venta.idVenta = Converter.GetGuid(row, "id_Venta");


                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                pedido.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                pedido.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                pedido.documentoVenta = new DocumentoVenta();
                pedido.documentoVenta.serie = Converter.GetString(row, "serie_factura");
                pedido.documentoVenta.numero = Converter.GetString(row, "numero_factura");

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
                pedido.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                pedido.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                pedido.cliente.tipoDocumento = Converter.GetString(row, "tipo_documento");

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
                pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");

                pedidoDetalle.flete = Converter.GetDecimal(row, "flete");

                pedidoDetalle.precioUnitarioOriginal = Converter.GetDecimal(row, "precio_unitario_original");

                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (pedidoDetalle.esPrecioAlternativo)
                {

                    pedidoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    pedidoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");

                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.ProductoPresentacion.Equivalencia;

                }
                else
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                }

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

            return venta;
        }



        public Venta SelectVentaConsolidada(Venta venta, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_ventaConsolidada");
            InputParameterAdd.Guid(objCommand, "idVenta", venta.idVenta);
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
                venta.idVenta = Converter.GetGuid(row, "id_Venta");


                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                pedido.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                pedido.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                pedido.documentoVenta = new DocumentoVenta();
                pedido.documentoVenta.serie = Converter.GetString(row, "serie_factura");
                pedido.documentoVenta.numero = Converter.GetString(row, "numero_factura");

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
                pedido.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                pedido.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");
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

                pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                pedidoDetalle.flete = Converter.GetDecimal(row, "flete");

                pedidoDetalle.precioUnitarioOriginal = Converter.GetDecimal(row, "precio_unitario_original");

                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    pedidoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");

                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.ProductoPresentacion.Equivalencia;
                    if (pedidoDetalle.producto.precioSinIgv != 0)
                    {
                        pedidoDetalle.porcentajeDescuento = 100 - ((pedidoDetalle.precioNeto * pedidoDetalle.ProductoPresentacion.Equivalencia) * 100 / pedidoDetalle.producto.precioSinIgv);
                    }

                }
                else
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                    if (pedidoDetalle.producto.precioSinIgv != 0)
                    {
                        pedidoDetalle.porcentajeDescuento = 100 - (pedidoDetalle.precioNeto * 100 / pedidoDetalle.producto.precioSinIgv);
                    }
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


                pedidoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                pedidoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }


            pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            //Detalle de la cotizacion
            /*         foreach (DataRow row in pedidoAdjuntoDataTable.Rows)
                     {
                         PedidoAdjunto pedidoAdjunto = new PedidoAdjunto();
                         pedidoAdjunto.idPedidoAdjunto = Converter.GetGuid(row, "id_pedido_adjunto");
                         pedidoAdjunto.adjunto = Converter.GetBytes(row, "adjunto");
                         pedidoAdjunto.nombre = Converter.GetString(row, "nombre");
                         pedido.pedidoAdjuntoList.Add(pedidoAdjunto);
                     }*/

            return venta;
        }




        /*Se crea venta consolidada la cual tomará los datos base de la guía de remisión*/


        public void InsertVentaConsolidada(Venta venta)
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


            OutputParameterAdd.UniqueIdentifier(objCommand, "idVenta");
            OutputParameterAdd.BigInt(objCommand, "numeroVenta");

            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);

            venta.idVenta = (Guid)objCommand.Parameters["@idVenta"].Value;
            venta.numero = (Int64)objCommand.Parameters["@numeroVenta"].Value;

            var tipoErrorValidacion = (GuiaRemisionValidacion.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            var descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            this.InsertVentaDetalle(venta);

            objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idVenta", venta.idVenta);
            InputParameterAdd.Varchar(objCommand, "observaciones", "Se crea Venta Consolidada.");
            ExecuteNonQuery(objCommand);

            this.Commit();
        }


        public void InsertVentaRefacturacion(Venta venta)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_ventaRefacturacion");
            InputParameterAdd.Guid(objCommand, "idUsuario", venta.guiaRemision.usuario.idUsuario);
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", venta.guiaRemision.idMovimientoAlmacen);
            InputParameterAdd.DateTime(objCommand, "fecha", DateTime.Now);
            OutputParameterAdd.UniqueIdentifier(objCommand, "idVenta");
            OutputParameterAdd.BigInt(objCommand, "numeroVenta");
            ExecuteNonQuery(objCommand);
            venta.idVenta = (Guid)objCommand.Parameters["@idVenta"].Value;
            venta.numero = (Int64)objCommand.Parameters["@numeroVenta"].Value;
            this.Commit();
        }

        public List<Venta> getListVenta(Venta venta)
        {
            var objCommand = GetSqlCommand("ps_lista_venta");
            List<Venta> lista = new List<Venta>();

            InputParameterAdd.Guid(objCommand, "idCiudad", venta.guiaRemision.ciudadOrigen.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", venta.guiaRemision.pedido.cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "idUsuario", venta.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "numeroFactura", venta.documentoVenta.numero);
            InputParameterAdd.Int(objCommand, "tipoDocumento", (int)venta.documentoVenta.tipoDocumento);
            InputParameterAdd.BigInt(objCommand, "numeroGuia", venta.guiaRemision.numero);            
            InputParameterAdd.DateTime(objCommand, "fechaTrasladoDesde", venta.guiaRemision.fechaEmisionDesde);
            InputParameterAdd.DateTime(objCommand, "fechaTrasladoHasta", venta.guiaRemision.fechaEmisionHasta);
            InputParameterAdd.BigInt(objCommand, "numeroPedido", venta.pedido.numeroPedido);
            InputParameterAdd.Varchar(objCommand, "sku", venta.guiaRemision.sku);


            DataTable dataTable = Execute(objCommand);
            foreach (DataRow row in dataTable.Rows)
            {
                Venta obj = new Venta();
                obj.documentoVenta = new DocumentoVenta();
                obj.documentoVenta.numero = Converter.GetString(row, "doc_venta");
                obj.pedido = new Pedido();
                obj.pedido.numeroPedido = Converter.GetInt(row, "numero");
                obj.ciudad = new Ciudad();
                obj.ciudad.sede = Converter.GetString(row, "sede");
                obj.cliente = new Cliente();
                obj.cliente.codigo = Converter.GetString(row, "codigo");
                obj.cliente.razonSocial = Converter.GetString(row, "razon_social");
                obj.cliente.ruc = Converter.GetString(row, "ruc");
                obj.cliente.responsableComercial.descripcion = Converter.GetString(row, "vendedor");
                obj.usuario = new Usuario();
                obj.usuario.nombre = Converter.GetString(row, "nombre");
                obj.guiaRemision = new GuiaRemision();
                obj.guiaRemision.serieDocumento = Converter.GetString(row, "mov_almacen");
                obj.guiaRemision.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");

                obj.idVenta = Converter.GetGuid(row, "id_venta");
                obj.guiaRemision.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                obj.total = Converter.GetDecimal(row, "total");


                lista.Add(obj);
            }
            return lista;
        }

        public Venta  SelectVentaList(Venta venta, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_venta_lista_detalle");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", venta.guiaRemision.idMovimientoAlmacen);
            InputParameterAdd.Guid(objCommand, "idVenta", venta.idVenta);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable ventaDataTable = dataSet.Tables[0];
            DataTable ventaDetalleDataTable = dataSet.Tables[1];
            DataTable pedidoAdjuntoDataTable = dataSet.Tables[2];
            DataTable ventaDatosDataTable = dataSet.Tables[3];
           
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
                pedido.tipoPedido = (Pedido.tiposPedido)Char.Parse(Converter.GetString(row, "tipo_pedido"));
                pedido.otrosCargos = Converter.GetDecimal(row, "otros_cargos");


                venta.igv = Converter.GetDecimal(row, "igv_venta");
                venta.subTotal = Converter.GetDecimal(row, "sub_total_venta");
                venta.total = Converter.GetDecimal(row, "total_venta");
                venta.idVenta = Converter.GetGuid(row, "id_Venta");

                venta.guiaRemision.serieDocumento = Converter.GetString(row, "mov_almacen");
                venta.guiaRemision.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                venta.guiaRemision.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");

                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                pedido.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                pedido.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                pedido.documentoVenta = new DocumentoVenta();
                pedido.documentoVenta.serie = Converter.GetString(row, "SERIE");
                pedido.documentoVenta.idDocumentoVenta = Converter.GetGuid(row, "id_cpe_cabecera_be");
                pedido.documentoVenta.numero = Converter.GetString(row, "CORRELATIVO");
                pedido.documentoVenta.fechaVencimiento = Converter.GetDateTime(row, "FEC_VCTO");
                pedido.documentoVenta.FechaRegistro = Converter.GetDateTime(row, "FEC_EMI");
                pedido.documentoVenta.horaEmision = Converter.GetDateTime(row, "HOR_EMI");


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
                pedido.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                pedido.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");


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

                pedidoDetalle.flete = Converter.GetDecimal(row, "flete");

                pedidoDetalle.precioUnitarioOriginal = Converter.GetDecimal(row, "precio_unitario_original");

                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (pedidoDetalle.esPrecioAlternativo)
                {

                    pedidoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    pedidoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");

                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.ProductoPresentacion.Equivalencia;

                }
                else
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                }

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


                pedidoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                pedidoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

               
                pedidoDetalle.excluirVenta = Converter.GetBool(row, "excluir");
                pedidoDetalle.estadoVenta = Converter.GetInt(row, "estado");

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

            foreach (DataRow row in ventaDatosDataTable.Rows)
            {                
                venta.idAsistente = Converter.GetInt(row, "id_asistente_servicio_cliente");
                venta.idOrigen = Converter.GetInt(row, "id_origen");
                venta.perteneceCanalLima = Converter.GetInt(row, "pertenece_canal_lima");
                venta.perteneceCanalPcp = Converter.GetInt(row, "pertenece_canal_pcp");
                venta.perteneceCanalProvincia = Converter.GetInt(row, "pertenece_canal_provincia");
                venta.idResponsableComercial = Converter.GetInt(row, "id_responsable_comercial");
                venta.idSupervisorComercial = Converter.GetInt(row, "id_supervisor_comercial");
                venta.perteneceCanalMultiregional = Converter.GetInt(row, "pertenece_canal_multiregional");

            }


            return venta;
        }

        public void rectificacfionVenta(int estadoCheck, Guid id_detalle_producto, Guid usuario)
        {
            var objCommand = GetSqlCommand("pu_rectificacion_venta");
            
            InputParameterAdd.Guid(objCommand, "id_venta_detalle", id_detalle_producto);
            InputParameterAdd.Guid(objCommand, "id_usuario", usuario);
            InputParameterAdd.Int(objCommand, "valor", estadoCheck);
            ExecuteNonQuery(objCommand);
        }
        
        public void modificacionDatosVenta(Venta venta)
        {
            var objCommand = GetSqlCommand("pu_modificacion_datos_venta");

            InputParameterAdd.Guid(objCommand, "id_venta", venta.idVenta);
            InputParameterAdd.Int(objCommand, "asistente_cliente", venta.idAsistente);
            InputParameterAdd.Int(objCommand, "responsable_comercial", venta.idResponsableComercial);
            InputParameterAdd.Int(objCommand, "supervisor_comercial", venta.idSupervisorComercial);
            InputParameterAdd.Int(objCommand, "origen", venta.idOrigen);
            InputParameterAdd.Int(objCommand, "canal_lima", venta.perteneceCanalLima);
            InputParameterAdd.Int(objCommand, "canal_multiregional", venta.perteneceCanalMultiregional);
            InputParameterAdd.Int(objCommand, "canal_pcp", venta.perteneceCanalPcp);
            InputParameterAdd.Int(objCommand, "canal_provincia", venta.perteneceCanalProvincia);
            InputParameterAdd.Guid(objCommand, "id_usuario", venta.usuario.idUsuario);
            ExecuteNonQuery(objCommand);           
        }

        public Venta verModificacionDatos(Venta venta)
        {
            var objCommand = GetSqlCommand("sp_modificacion_datos");            
                      
            InputParameterAdd.Guid(objCommand, "id_venta", venta.idVenta);

            DataTable dataTable = Execute(objCommand);
            Venta obj = new Venta();
            foreach (DataRow row in dataTable.Rows)
            {
                
                obj.idAsistente = Converter.GetInt(row, "id_asistente_servicio_cliente");
                obj.idOrigen = Converter.GetInt(row, "id_origen");
                obj.perteneceCanalLima = Converter.GetInt(row, "pertenece_canal_lima");
                obj.perteneceCanalPcp = Converter.GetInt(row, "pertenece_canal_pcp");
                obj.perteneceCanalProvincia = Converter.GetInt(row, "pertenece_canal_provincia");
                obj.idResponsableComercial = Converter.GetInt(row, "id_responsable_comercial");
                obj.idSupervisorComercial = Converter.GetInt(row, "id_supervisor_comercial");
                obj.perteneceCanalMultiregional = Converter.GetInt(row, "pertenece_canal_multiregional");
                
            }

            return obj;
        }

    }
}
