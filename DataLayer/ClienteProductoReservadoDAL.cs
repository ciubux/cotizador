using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;
using Model.UTILES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DataLayer
{
    public class ClienteProductoReservadoDAL : DaoBase
    {
        public ClienteProductoReservadoDAL(IDalSettings settings) : base(settings)
        {
        }

        public ClienteProductoReservadoDAL() : this(new CotizadorSettings())
        {
        }


        public ClienteProductoReservado getClienteProductoReservado(Guid idClienteProductoReservado, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_cliente_producto_reservado");
            InputParameterAdd.Guid(objCommand, "idClienteProductoReservado", idClienteProductoReservado);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataTable dataTable = Execute(objCommand);
            ClienteProductoReservado obj = new ClienteProductoReservado();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idClienteProductoReservado = Converter.GetGuid(row, "id_cliente_producto_reservado");

                obj.cantidadOriginal = Converter.GetInt(row, "cantidad_original");
                obj.cantidadReserva = Converter.GetInt(row, "cantidad_reserva");
                obj.cantidadAtendida = Converter.GetInt(row, "cantidad_atendida");
                obj.unidadConteo = Converter.GetString(row, "unidad_conteo");
                obj.fechaUltimaRecarga = Converter.GetDateTime(row, "fecha_ultima_recarga");
                obj.Estado = Converter.GetInt(row, "estado");

                obj.empresa.idEmpresa = Converter.GetInt(row, "id_empresa");

                obj.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                obj.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                obj.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                obj.cliente.ruc = Converter.GetString(row, "ruc_cliente");
                obj.cliente.codigo = Converter.GetString(row, "codigo_cliente");
                obj.cliente.razonSocial = Converter.GetString(row, "razon_social_cliente");

                obj.producto.idProducto = Converter.GetGuid(row, "id_producto");
                obj.producto.sku = Converter.GetString(row, "sku_producto");
                obj.producto.descripcion = Converter.GetString(row, "descripcion_producto");

                obj.producto.unidad = Converter.GetString(row, "unidad");
                obj.producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                obj.producto.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
                obj.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                obj.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                obj.producto.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_estandar_unidad_conteo");
                obj.producto.equivalenciaUnidadAlternativaUnidadConteo = obj.producto.equivalenciaUnidadEstandarUnidadConteo / obj.producto.equivalenciaAlternativa;
                obj.producto.equivalenciaUnidadProveedorUnidadConteo = obj.producto.equivalenciaUnidadEstandarUnidadConteo * obj.producto.equivalenciaProveedor;
            }

            return obj;
        }

        public List<ClienteProductoReservado> getClienteProductosReservados(ClienteProductoReservado filtroObj)
        {
            var objCommand = GetSqlCommand("ps_cliente_productos_reservados");

            InputParameterAdd.Int(objCommand, "estado", filtroObj.Estado);
            InputParameterAdd.Guid(objCommand, "idUsuario", filtroObj.usuario.idUsuario); 

            if (filtroObj.ciudad != null && filtroObj.ciudad.idCiudad != Guid.Empty)
            {
                InputParameterAdd.Guid(objCommand, "idCiudad", filtroObj.ciudad.idCiudad);
            }
            if (filtroObj.cliente != null && filtroObj.cliente.idCliente != Guid.Empty)
            {
                InputParameterAdd.Guid(objCommand, "idCliente", filtroObj.cliente.idCliente);
            }

            DataTable dataTable = Execute(objCommand);
            List<ClienteProductoReservado> lista = new List<ClienteProductoReservado>();

            foreach (DataRow row in dataTable.Rows)
            {
                ClienteProductoReservado obj = new ClienteProductoReservado();
                obj.idClienteProductoReservado = Converter.GetGuid(row, "id_cliente_producto_reservado");

                obj.cantidadOriginal = Converter.GetInt(row, "cantidad_original");
                obj.cantidadReserva = Converter.GetInt(row, "cantidad_reserva");
                obj.cantidadAtendida = Converter.GetInt(row, "cantidad_atendida");
                obj.unidadConteo = Converter.GetString(row, "unidad_conteo");
                obj.fechaUltimaRecarga = Converter.GetDateTime(row, "fecha_ultima_recarga");
                obj.Estado = Converter.GetInt(row, "estado");

                obj.empresa.idEmpresa = Converter.GetInt(row, "id_empresa");

                obj.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                obj.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                obj.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                obj.cliente.ruc = Converter.GetString(row, "ruc_cliente");
                obj.cliente.codigo = Converter.GetString(row, "codigo_cliente");
                obj.cliente.razonSocial = Converter.GetString(row, "razon_social_cliente");

                obj.producto.idProducto = Converter.GetGuid(row, "id_producto");
                obj.producto.sku = Converter.GetString(row, "sku_producto");
                obj.producto.descripcion = Converter.GetString(row, "descripcion_producto");

                obj.producto.unidad = Converter.GetString(row, "unidad");
                obj.producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                obj.producto.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
                obj.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                obj.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                obj.producto.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_estandar_unidad_conteo");
                obj.producto.equivalenciaUnidadAlternativaUnidadConteo = obj.producto.equivalenciaUnidadEstandarUnidadConteo / obj.producto.equivalenciaAlternativa;
                obj.producto.equivalenciaUnidadProveedorUnidadConteo = obj.producto.equivalenciaUnidadEstandarUnidadConteo * obj.producto.equivalenciaProveedor;


                obj.solicitudRecargaActiva.idClienteProductoReservadoSolicitud = Converter.GetGuid(row, "id_cliente_producto_reservado_solicitud");
                obj.solicitudRecargaActiva.cantidadSolicitada = Converter.GetInt(row, "cantidad_solicitada");
                

                lista.Add(obj);
            }

            return lista;
        }

        public ClienteProductoReservado insertClienteProductoReservado(ClienteProductoReservado obj)
        {
            var objCommand = GetSqlCommand("pi_cliente_producto_reservado");

            InputParameterAdd.Guid(objCommand, "idCliente", obj.cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "idProducto", obj.producto.idProducto);
            InputParameterAdd.Guid(objCommand, "idCiudad", obj.ciudad.idCiudad);
            InputParameterAdd.Int(objCommand, "cantidadOriginal", obj.cantidadOriginal.GetValueOrDefault(0));
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            obj.idClienteProductoReservado = (Guid)objCommand.Parameters["@newId"].Value;

            return obj;
        }


        public ClienteProductoReservado updateClienteProductoReservado(ClienteProductoReservado obj)
        {
            var objCommand = GetSqlCommand("pu_cliente_producto_reservado");

            InputParameterAdd.Guid(objCommand, "idClienteProductoReservado", obj.idClienteProductoReservado);
            InputParameterAdd.Int(objCommand, "cantidadOriginal", obj.cantidadOriginal.GetValueOrDefault(0));
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);

            ExecuteNonQuery(objCommand);

            return obj;
        }

        public void InsertClientesProductosReservadosMasivo(Guid idUsuario, Guid idCiudad, List<ClienteProductoReservado> lista)
        {
            var objCommand = GetSqlCommand("pi_clientes_productos_reservados_masivo");

            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("RUC_CLIENTE", typeof(string)));
            tvp.Columns.Add(new DataColumn("ID_PRODUCTO", typeof(Guid)));
            tvp.Columns.Add(new DataColumn("CANTIDAD_ORIGINAL", typeof(int)));

            foreach (ClienteProductoReservado item in lista)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["RUC_CLIENTE"] = item.cliente.ruc;
                rowObj["ID_PRODUCTO"] = item.producto.idProducto;
                rowObj["CANTIDAD_ORIGINAL"] = item.cantidadOriginal.GetValueOrDefault(0);
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@lista", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.ClienteProductoReservadoList";

            ExecuteNonQuery(objCommand);
        }

        public void AgregarCantidadReserva(Guid idClienteProductoReservado, int cantidadAgregar, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pu_cliente_producto_reservado_agregar_reserva");

            InputParameterAdd.Guid(objCommand, "idClienteProductoReservado", idClienteProductoReservado);
            InputParameterAdd.Int(objCommand, "cantidadAgregar", cantidadAgregar);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            ExecuteNonQuery(objCommand);
        }


        public void InsertSolicitudesRecargaReserva(Guid idUsuario, List<ClienteProductoReservadoSolicitud> lista)
        {
            var objCommand = GetSqlCommand("pi_cliente_producto_reservado_solicitud_masivo");

            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID_CLIENTE_PRODUCTO_RESERVADO", typeof(Guid)));
            tvp.Columns.Add(new DataColumn("CANTIDAD_AGREGAR", typeof(int)));

            foreach (ClienteProductoReservadoSolicitud item in lista)
            {
                DataRow rowObj = tvp.NewRow();

                rowObj["ID_CLIENTE_PRODUCTO_RESERVADO"] = item.clienteProductoReservado.idClienteProductoReservado;
                rowObj["CANTIDAD_AGREGAR"] = item.cantidadSolicitada;

                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@lista", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.ClienteProductoReservadoSolicitudList"; 

            ExecuteNonQuery(objCommand);
        }

        public List<ClienteProductoReservado> SelectDatosReservaSolicitarRecarga(Guid idUsuario, List<ClienteProductoReservado> reservas)
        {
            var objCommand = GetSqlCommand("ps_datosReservasSolicitarRecarga");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID_CLIENTE_PRODUCTO_RESERVADO", typeof(Guid)));
            tvp.Columns.Add(new DataColumn("CANTIDAD_AGREGAR", typeof(int)));

            foreach (ClienteProductoReservado item in reservas)
            {
                DataRow rowObj = tvp.NewRow();

                rowObj["ID_CLIENTE_PRODUCTO_RESERVADO"] = item.idClienteProductoReservado;
                rowObj["CANTIDAD_AGREGAR"] = 0;

                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@lista", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.ClienteProductoReservadoSolicitudList";

            DataTable dataTable = Execute(objCommand);
            List<ClienteProductoReservado> lista = new List<ClienteProductoReservado>();

            foreach (DataRow row in dataTable.Rows)
            {
                ClienteProductoReservado obj = new ClienteProductoReservado();
                obj.idClienteProductoReservado = Converter.GetGuid(row, "id_cliente_producto_reservado");
                obj.cantidadOriginal = Converter.GetInt(row, "cantidad_original");
                obj.cantidadReserva = Converter.GetInt(row, "cantidad_reserva");
                obj.fechaUltimaRecarga = Converter.GetDateTime(row, "fecha_ultima_recarga");

                obj.producto = new Producto();
                obj.producto.idProducto = Converter.GetGuid(row, "id_producto");
                obj.producto.sku = Converter.GetString(row, "sku");
                obj.producto.descripcion = Converter.GetString(row, "descripcion");
                obj.producto.unidadConteo = Converter.GetString(row, "unidad_conteo");

                obj.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                obj.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                obj.producto.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_estandar_unidad_conteo");
                obj.producto.equivalenciaUnidadAlternativaUnidadConteo = obj.producto.equivalenciaUnidadEstandarUnidadConteo / obj.producto.equivalenciaAlternativa;
                obj.producto.equivalenciaUnidadProveedorUnidadConteo = obj.producto.equivalenciaUnidadEstandarUnidadConteo * obj.producto.equivalenciaProveedor;

                obj.producto.unidad = Converter.GetString(row, "unidad");
                obj.producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                obj.producto.unidadProveedor = Converter.GetString(row, "unidad_proveedor");

                obj.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                obj.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                obj.solicitudRecargaActiva.idClienteProductoReservadoSolicitud = Converter.GetGuid(row, "id_cliente_producto_reservado_solicitud_activa");
                obj.solicitudRecargaActiva.cantidadSolicitada = Converter.GetInt(row, "cantidad_solicitada_activa");

                obj.Estado = Converter.GetInt(row, "estado");


                lista.Add(obj);
            }

            return lista;
        }
    }
}

