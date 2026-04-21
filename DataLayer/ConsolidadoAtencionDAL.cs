using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;
using Model.CONFIGCLASSES;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DataLayer
{
    public class ConsolidadoAtencionDAL : DaoBase
    {
        public ConsolidadoAtencionDAL(IDalSettings settings) : base(settings) { }
        public ConsolidadoAtencionDAL() : this(new CotizadorSettings()) { }

        public ConsolidadoAtencion getConsolidadoAtencion(Guid idConsolidado)
        {
            var objCommand = GetSqlCommand("ps_consolidado_atencion");
            InputParameterAdd.Guid(objCommand, "idConsolidado", idConsolidado);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable consolidadoTable = dataSet.Tables[0];
            DataTable pedidosTable = dataSet.Tables[1];
            DataTable guiasTable = dataSet.Tables[2];

            ConsolidadoAtencion obj = new ConsolidadoAtencion();

            foreach (DataRow row in consolidadoTable.Rows)
            {
                MapearObjeto(obj, row);
            }

            obj.pedidos = new List<Pedido>();
            foreach (DataRow row in pedidosTable.Rows)
            {
                Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
                pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo_pedido");
                pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                pedido.horaEntregaAdicionalDesde = Converter.GetString(row, "hora_entrega_adicional_desde");
                pedido.horaEntregaAdicionalHasta = Converter.GetString(row, "hora_entrega_adicional_hasta");
                pedido.fechaEntregaExtendida = Converter.GetDateTimeNullable(row, "fecha_entrega_extendida");
                pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");

                pedido.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                //pedido.FechaRegistro = pedido.FechaRegistro.AddHours(-5);
                pedido.stockConfirmado = Converter.GetInt(row, "stock_confirmado");
                /*if (row["fecha_programacion"] == DBNull.Value)
                    pedido.fechaProgramacion = null;
                else*/
                pedido.fechaProgramacion = Converter.GetDateTimeNullable(row, "fecha_programacion");

                pedido.observaciones = Converter.GetString(row, "observaciones");

                pedido.facturadoAnticipadamente = Converter.GetInt(row, "facturado_anticipadamente") == 1 ? true : false;

                pedido.numeroPedidoRelacionado = Converter.GetInt(row, "numero_pedido_rel");
                pedido.codigoEmpresaPedidoRelacionado = Converter.GetString(row, "codigo_empresa_pedido_rel");

                pedido.productosNextSoftHomologados = Converter.GetInt(row, "productos_homologados_nextsoft") == 1 ? true : false;
                pedido.entregaATerceros = Converter.GetInt(row, "entrega_terceros") == 1 ? true : false;
                if (pedido.entregaATerceros)
                {
                    pedido.nombreClienteTercero = Converter.GetString(row, "nombre_cliente_rel");
                }
                pedido.cliente = new Cliente();
                pedido.cliente.codigo = Converter.GetString(row, "codigo");
                pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                pedido.cliente.ruc = Converter.GetString(row, "ruc");
                pedido.cliente.nombreComercial = Converter.GetString(row, "nombre_comercial_cliente");
                pedido.cliente.tipoDocumentoIdentidad = (DocumentoVenta.TiposDocumentoIdentidad)Converter.GetInt(row, "tipo_documento_cliente");

                pedido.cliente.grupoCliente = new GrupoCliente();
                pedido.cliente.grupoCliente.nombre = Converter.GetString(row, "nombre_grupo");

                pedido.empresa = new Empresa();
                pedido.empresa.codigo = Converter.GetString(row, "codigo_empresa");

                pedido.ciudad = new Ciudad();
                pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                pedido.seguimientoPedido = new SeguimientoPedido();
                pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Converter.GetInt(row, "estado_seguimiento");
                pedido.seguimientoPedido.observacion = Converter.GetString(row, "observacion_seguimiento");
                pedido.seguimientoPedido.usuario = new Usuario();
                pedido.seguimientoPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                pedido.seguimientoPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");

                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "codigo_ubigeo");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                obj.pedidos.Add(pedido);
            }

            obj.guias = new List<GuiaRemision>();
            foreach (DataRow row in guiasTable.Rows)
            {
                GuiaRemision guia = new GuiaRemision();

                guia.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                guia.serieDocumento = Converter.GetString(row, "serie_documento");
                guia.numeroDocumento = Converter.GetLong(row, "numero_documento");
                guia.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                guia.fechaEmision = Converter.GetDateTime(row, "fecha_emision");

                guia.pedido = new Pedido();
                guia.pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                guia.pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
                guia.pedido.entregaATerceros = Converter.GetInt(row, "entrega_terceros") == 1 ? true : false;

                guia.clienteVer = new Cliente();
                guia.clienteVer.ruc = Converter.GetString(row, "ruc");
                guia.clienteVer.nombreComercial = Converter.GetString(row, "nombre_comercial_cliente");
                guia.clienteVer.codigo = Converter.GetString(row, "codigo");
                guia.clienteVer.razonSocial = Converter.GetString(row, "razon_social");
                guia.clienteVer.tipoDocumentoIdentidad = (DocumentoVenta.TiposDocumentoIdentidad)Converter.GetInt(row, "tipo_documento_cliente");

                if (guia.pedido.entregaATerceros)
                {
                    string nombreClienteRel = Converter.GetString(row, "nombre_cliente_rel");
                    guia.clienteVer.razonSocial = nombreClienteRel + " (" + guia.clienteVer.razonSocial + ")";
                }
                obj.guias.Add(guia);
            }

            return obj;
        }

        public List<ConsolidadoAtencion> getConsolidadosAtencion(ConsolidadoAtencion filtro)
        {
            var objCommand = GetSqlCommand("ps_consolidados_atencion");
            InputParameterAdd.DateTime(objCommand, "fecha", filtro.fecha);
            InputParameterAdd.Bit(objCommand, "estado", filtro.Estado == 1);
            InputParameterAdd.Guid(objCommand, "idUsuario", filtro.IdUsuarioRegistro);
            InputParameterAdd.Guid(objCommand, "idCiudad", filtro.ciudad.idCiudad);

            DataTable dataTable = Execute(objCommand);
            List<ConsolidadoAtencion> lista = new List<ConsolidadoAtencion>();

            foreach (DataRow row in dataTable.Rows)
            {
                ConsolidadoAtencion obj = new ConsolidadoAtencion();
                MapearObjeto(obj, row);
                lista.Add(obj);
            }
            return lista;
        }

        public ConsolidadoAtencion insertConsolidadoAtencion(ConsolidadoAtencion obj)
        {
            var objCommand = GetSqlCommand("pi_consolidado_atencion");
            InputParameterAdd.DateTime(objCommand, "fecha", obj.fecha);
            InputParameterAdd.Varchar(objCommand, "observaciones", obj.observaciones?.Trim());
            InputParameterAdd.Guid(objCommand, "idCiudad", obj.ciudad.idCiudad);
            InputParameterAdd.Int(objCommand, "idVehiculo", obj.vehiculo.idVehiculo);
            InputParameterAdd.Int(objCommand, "idChofer", obj.chofer.idPersonalAlmacen);
            InputParameterAdd.Int(objCommand, "idAsistente", obj.asistente.idPersonalAlmacen);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            foreach (Pedido item in obj.pedidos)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idPedido;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idsPedidos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";


            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);

            obj.idConsolidadoAtencion = (Guid)objCommand.Parameters["@newId"].Value;
            return obj;
        }

        public ConsolidadoAtencion updateConsolidadoAtencion(ConsolidadoAtencion obj)
        {
            var objCommand = GetSqlCommand("pu_consolidado_atencion");
            InputParameterAdd.Guid(objCommand, "idConsolidado", obj.idConsolidadoAtencion);
            InputParameterAdd.DateTime(objCommand, "fecha", obj.fecha);
            InputParameterAdd.Varchar(objCommand, "observaciones", obj.observaciones?.Trim());
            InputParameterAdd.Guid(objCommand, "idCiudad", obj.ciudad.idCiudad);
            InputParameterAdd.Int(objCommand, "idVehiculo", obj.vehiculo.idVehiculo);
            InputParameterAdd.Int(objCommand, "idChofer", obj.chofer.idPersonalAlmacen);
            InputParameterAdd.Int(objCommand, "idAsistente", obj.asistente.idPersonalAlmacen);
            InputParameterAdd.Bit(objCommand, "estado", obj.Estado == 1);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            foreach (Pedido item in obj.pedidos)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idPedido;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idsPedidos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            ExecuteNonQuery(objCommand);
            return obj;
        }


        public List<ConsolidadoAtencion> getConsolidadosAtencionPedidos(List<Guid> idsConsolidados)
        {
            var objCommand = GetSqlCommand("ps_consolidados_atencion_presentacion");

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            foreach (Guid item in idsConsolidados)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idsConsolidados", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable consolidadosTable = dataSet.Tables[0];
            DataTable pedidosTable = dataSet.Tables[1];

            List<ConsolidadoAtencion> lista = new List<ConsolidadoAtencion>();

            foreach (DataRow row in consolidadosTable.Rows)
            {
                ConsolidadoAtencion obj = new ConsolidadoAtencion();
                MapearObjeto(obj, row);
                lista.Add(obj);
            }

            foreach (DataRow row in pedidosTable.Rows)
            {
                Guid idConsolidado = Converter.GetGuid(row, "id_consolidado_atencion");

                ConsolidadoAtencion obj = lista.Where(c => c.idConsolidadoAtencion.Equals(idConsolidado)).FirstOrDefault();

                Pedido pedido = new Pedido(Pedido.ClasesPedido.Venta);
                pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo_pedido");
                pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                pedido.horaEntregaAdicionalDesde = Converter.GetString(row, "hora_entrega_adicional_desde");
                pedido.horaEntregaAdicionalHasta = Converter.GetString(row, "hora_entrega_adicional_hasta");
                pedido.fechaEntregaExtendida = Converter.GetDateTimeNullable(row, "fecha_entrega_extendida");
                
                pedido.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                pedido.stockConfirmado = Converter.GetInt(row, "stock_confirmado");
                pedido.fechaProgramacion = Converter.GetDateTimeNullable(row, "fecha_programacion");

                pedido.numeroPedidoRelacionado = Converter.GetInt(row, "numero_pedido_rel");
                pedido.codigoEmpresaPedidoRelacionado = Converter.GetString(row, "codigo_empresa_pedido_rel");

                pedido.entregaATerceros = Converter.GetInt(row, "entrega_terceros") == 1 ? true : false;
                if (pedido.entregaATerceros)
                {
                    pedido.nombreClienteTercero = Converter.GetString(row, "nombre_cliente_rel");
                }
                pedido.cliente = new Cliente();
                pedido.cliente.codigo = Converter.GetString(row, "codigo");
                pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                pedido.cliente.ruc = Converter.GetString(row, "ruc");
                pedido.cliente.nombreComercial = Converter.GetString(row, "nombre_comercial_cliente");
                pedido.cliente.tipoDocumentoIdentidad = (DocumentoVenta.TiposDocumentoIdentidad)Converter.GetInt(row, "tipo_documento_cliente");

                pedido.empresa = new Empresa();
                pedido.empresa.codigo = Converter.GetString(row, "codigo_empresa");

                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "codigo_ubigeo");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                obj.pedidos.Add(pedido);
            }

            return lista;
        }


        public List<ConsolidadoAtencion> getConsolidadoAtencionsReparto(List<Guid> idsConsolidados)
        {
            var objCommand = GetSqlCommand("ps_consolidados_atencion_reparto");

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            foreach (Guid item in idsConsolidados)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idsConsolidados", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable consolidadosTable = dataSet.Tables[0];
            DataTable guiasTable = dataSet.Tables[1];

            List<ConsolidadoAtencion> lista = new List<ConsolidadoAtencion>();

            foreach (DataRow row in consolidadosTable.Rows)
            {
                ConsolidadoAtencion obj = new ConsolidadoAtencion();
                MapearObjeto(obj, row);
                lista.Add(obj);
            }

            foreach (DataRow row in guiasTable.Rows)
            {
                Guid idConsolidado = Converter.GetGuid(row, "id_consolidado_atencion");

                ConsolidadoAtencion obj = lista.Where(c => c.idConsolidadoAtencion.Equals(idConsolidado)).FirstOrDefault();

                Guid idGuia = Converter.GetGuid(row, "id_movimiento_almacen");

                GuiaRemision guia = obj.guias.Where(g => g.idMovimientoAlmacen.Equals(idGuia)).FirstOrDefault();

                if (guia == null)
                {
                    guia = new GuiaRemision();
                    //DATOS DE LA GUIA
                    guia.serieDocumento = Converter.GetString(row, "serie_documento");
                    guia.numeroDocumento = Converter.GetLong(row, "numero_documento");
                    guia.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");

                    //PEDIDO
                    guia.pedido = new Pedido();
                    guia.pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                    guia.pedido.numeroPedido = Converter.GetLong(row, "numero");
                    guia.documentoDetalle = new List<DocumentoDetalle>();

                    guia.clienteVer = new Cliente();
                    guia.clienteVer.razonSocial = Converter.GetString(row, "nombre_cliente");
                    string nombreClienteRel = Converter.GetString(row, "nombre_cliente_rel");

                    if (nombreClienteRel != null && !nombreClienteRel.Equals(String.Empty)) {
                        guia.clienteVer.razonSocial = nombreClienteRel + " (" + guia.clienteVer.razonSocial + ")";
                    }


                    obj.guias.Add(guia);
                }


                DocumentoDetalle documentoDetalle = new DocumentoDetalle();
                documentoDetalle.idDocumentoDetalle = Converter.GetGuid(row, "id_movimiento_almacen_detalle");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                documentoDetalle.cantidadPorAtender = documentoDetalle.cantidad;
                documentoDetalle.cantidadPermitida = documentoDetalle.cantidad;
                documentoDetalle.cantidadGuiada = 0;
                documentoDetalle.cantidadTotalAtencion = Converter.GetInt(row, "cantidad_pedido");

                documentoDetalle.ProductoPresentacion = new ProductoPresentacion();
                documentoDetalle.ProductoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                documentoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");

                documentoDetalle.unidad = Converter.GetString(row, "unidad");
                documentoDetalle.producto = new Producto();
                documentoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                documentoDetalle.producto.sku = Converter.GetString(row, "sku");
                documentoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                documentoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                documentoDetalle.producto.descripcionLarga = Converter.GetString(row, "descripcion_larga");
                documentoDetalle.producto.ventaRestringida = (Producto.TipoVentaRestringida)Converter.GetInt(row, "descontinuado");
                documentoDetalle.producto.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");
                documentoDetalle.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia_producto");
                documentoDetalle.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor_producto");
                documentoDetalle.producto.codigoNextSoft = Converter.GetString(row, "codigo_nextsoft");
                documentoDetalle.producto.codigoFactorUnidadMP = Converter.GetString(row, "codigo_factor_unidad_mp");
                documentoDetalle.producto.codigoFactorUnidadAlternativa = Converter.GetString(row, "codigo_factor_unidad_alternativa");
                documentoDetalle.producto.codigoFactorUnidadProveedor = Converter.GetString(row, "codigo_factor_unidad_proveedor");
                documentoDetalle.producto.codigoFactorUnidadConteo = Converter.GetString(row, "codigo_factor_unidad_conteo");
                documentoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * documentoDetalle.ProductoPresentacion.Equivalencia;

                guia.documentoDetalle.Add(documentoDetalle);
            }

            return lista;
        }

        public List<GuiaRemision> GetGuiasRemisionConsolidar(ConsolidadoAtencion filtro)
        {
            var objCommand = GetSqlCommand("ps_guias_consolidar_atencion");

            InputParameterAdd.Guid(objCommand, "idConsolidado", filtro.idConsolidadoAtencion);
            InputParameterAdd.Guid(objCommand, "idUsuario", filtro.usuario.idUsuario);
            
            DataTable dataTable = Execute(objCommand);
            List<GuiaRemision> lista = new List<GuiaRemision>();

            foreach (DataRow row in dataTable.Rows)
            {
                GuiaRemision obj = new GuiaRemision();

                obj.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                obj.serieDocumento = Converter.GetString(row, "serie_documento");
                obj.numeroDocumento = Converter.GetLong(row, "numero_documento");
                obj.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                obj.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                
                obj.pedido = new Pedido();
                obj.pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                obj.pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
                obj.pedido.entregaATerceros = Converter.GetInt(row, "entrega_terceros") == 1 ? true : false;

                obj.clienteVer = new Cliente();
                obj.clienteVer.ruc = Converter.GetString(row, "ruc");
                obj.clienteVer.nombreComercial = Converter.GetString(row, "nombre_comercial_cliente");
                obj.clienteVer.codigo = Converter.GetString(row, "codigo");
                obj.clienteVer.razonSocial = Converter.GetString(row, "razon_social");
                obj.clienteVer.tipoDocumentoIdentidad = (DocumentoVenta.TiposDocumentoIdentidad)Converter.GetInt(row, "tipo_documento_cliente");

                if (obj.pedido.entregaATerceros)
                {
                    string nombreClienteRel = Converter.GetString(row, "nombre_cliente_rel");
                    obj.clienteVer.razonSocial = nombreClienteRel + " (" + obj.clienteVer.razonSocial + ")";
                }
                lista.Add(obj);
            }
            return lista;
        }


        public void insertGuiasSalida(ConsolidadoAtencion obj)
        {
            var objCommand = GetSqlCommand("pi_guias_consolidado_atencion");
            InputParameterAdd.Guid(objCommand, "idConsolidado", obj.idConsolidadoAtencion);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.usuario.idUsuario);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            foreach (GuiaRemision item in obj.guias)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idMovimientoAlmacen;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idsMovimientosAlmacen", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";


            ExecuteNonQuery(objCommand);
        }


        // Método privado para evitar repetir código de mapeo entre SELECT individual y lista
        private void MapearObjeto(ConsolidadoAtencion obj, DataRow row)
        {
            obj.idConsolidadoAtencion = Converter.GetGuid(row, "id_consolidado_atencion");
            obj.fecha = Converter.GetDateTime(row, "fecha");
            obj.observaciones = Converter.GetString(row, "observaciones");
            obj.Estado = Converter.GetBool(row, "estado") ? 1 : 0;

            obj.ciudad = new Ciudad();
            obj.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
            obj.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

            obj.vehiculo.idVehiculo = Converter.GetInt(row, "id_vehiculo");
            obj.vehiculo.placa = Converter.GetString(row, "placa_vehiculo");

            obj.chofer.idPersonalAlmacen = Converter.GetInt(row, "id_personal_almacen_chofer");
            obj.chofer.apellidoPaterno = Converter.GetString(row, "chofer_apellido_paterno");
            obj.chofer.apellidoMaterno = Converter.GetString(row, "chofer_apellido_materno");
            obj.chofer.nombres = Converter.GetString(row, "chofer_nombres");

            obj.asistente.idPersonalAlmacen = Converter.GetInt(row, "id_personal_almacen_asistente");
            obj.asistente.apellidoPaterno = Converter.GetString(row, "asistente_apellido_paterno");
            obj.asistente.apellidoMaterno = Converter.GetString(row, "asistente_apellido_materno");
            obj.asistente.nombres = Converter.GetString(row, "asistente_nombres");
        }

    }
}
