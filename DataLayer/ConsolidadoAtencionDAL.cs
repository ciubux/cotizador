using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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


            return obj;
        }

        public List<ConsolidadoAtencion> getConsolidadosAtencion(ConsolidadoAtencion filtro)
        {
            var objCommand = GetSqlCommand("ps_consolidados_atencion");
            InputParameterAdd.DateTime(objCommand, "fecha", filtro.fecha);
            InputParameterAdd.Bit(objCommand, "estado", filtro.Estado == 1);
            InputParameterAdd.Guid(objCommand, "idUsuario", filtro.IdUsuarioRegistro);

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

        // Método privado para evitar repetir código de mapeo entre SELECT individual y lista
        private void MapearObjeto(ConsolidadoAtencion obj, DataRow row)
        {
            obj.idConsolidadoAtencion = Converter.GetGuid(row, "id_consolidado_atencion");
            obj.fecha = Converter.GetDateTime(row, "fecha");
            obj.observaciones = Converter.GetString(row, "observaciones");
            obj.Estado = Converter.GetBool(row, "estado") ? 1 : 0;

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
