using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Data.SqlClient;

namespace DataLayer
{
    public class PrecioEspecialDAL : DaoBase
    {
        public PrecioEspecialDAL(IDalSettings settings) : base(settings)
        {
        }

        public PrecioEspecialDAL() : this(new CotizadorSettings())
        {
        }



        public PrecioEspecialCabecera GetPrecioEspecial(Guid idObj, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_precioEspecialCabecera");
            InputParameterAdd.Guid(objCommand, "idPrecioEspecialCabecera", idObj);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable cabeceraDataTable = dataSet.Tables[0];
            DataTable detalleDataTable = dataSet.Tables[1];

            PrecioEspecialCabecera obj = new PrecioEspecialCabecera();
            obj.precios = new List<PrecioEspecialDetalle>();
            foreach (DataRow row in cabeceraDataTable.Rows)
            {
                obj.idPrecioEspecialCabecera = Converter.GetGuid(row, "id_precio_especial_cabecera");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.fechaInicio = Converter.GetDateTime(row, "fecha_inicio");
                obj.fechaFin = Converter.GetDateTime(row, "fecha_fin");

                obj.tipoNegociacion = Converter.GetString(row, "tipo_negociacion");
                obj.titulo = Converter.GetString(row, "titulo");
                obj.observaciones = Converter.GetString(row, "observaciones");

                obj.clienteSunat = new ClienteSunat();
                obj.clienteSunat.idClienteSunat = Converter.GetInt(row, "id_cliente_sunat");
                obj.clienteSunat.ruc = Converter.GetString(row, "ruc");
                obj.clienteSunat.nombreComercial = Converter.GetString(row, "nombre_comercial");
                obj.clienteSunat.razonSocial = Converter.GetString(row, "razon_social");

                obj.grupoCliente = new GrupoCliente();
                obj.grupoCliente.idGrupoCliente = Converter.GetInt(row, "id_grupo");
                obj.grupoCliente.codigo = Converter.GetString(row, "codigo_grupo");
                obj.grupoCliente.nombre = Converter.GetString(row, "nombre_grupo");


                obj.Estado = Converter.GetInt(row, "estado");
                obj.FechaRegistro = Converter.GetDateTime(row, "fecha_creacion");

                obj.UsuarioRegistro = new Usuario();
                obj.UsuarioRegistro.idUsuario = Converter.GetGuid(row, "id_usuario_creacion");
                obj.UsuarioRegistro.nombre = Converter.GetString(row, "nombre_usuario_creacion");
                obj.UsuarioRegistro.email = Converter.GetString(row, "email_usuario_creacion");

                obj.FechaEdicion = Converter.GetDateTime(row, "fecha_modificacion");

            }


            foreach (DataRow row in detalleDataTable.Rows)
            {
                PrecioEspecialDetalle det = new PrecioEspecialDetalle();

                det.idPrecioEspecialDetalle = Converter.GetGuid(row, "id_precio_especial_detalle");
                
                det.producto = new Producto();
                det.producto.idProducto = Converter.GetGuid(row, "id_producto");
                det.producto.sku = Converter.GetString(row, "sku");
                det.producto.descripcion = Converter.GetString(row, "nombre_producto");

                det.fechaInicio = Converter.GetDateTime(row, "fecha_inicio");
                det.fechaFin = Converter.GetDateTime(row, "fecha_fin");
                det.observaciones = Converter.GetString(row, "observaciones");

                det.moneda = new Moneda();
                det.moneda.codigo = Converter.GetString(row, "moneda");
                det.moneda.nombre = Converter.GetString(row, "nombre_moneda");
                det.moneda.simbolo = Converter.GetString(row, "simbolo_moneda");

                det.unidadPrecio = new ProductoPresentacion();
                det.unidadPrecio.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion_precio");
                det.unidadPrecio.Presentacion = Converter.GetString(row, "unidad_precio");
                det.unidadPrecio.PrecioSinIGV = Converter.GetDecimal(row, "precio_unitario");
                det.unidadPrecio.PrecioOriginalSinIGV = Converter.GetDecimal(row, "precio_unitario_mp");
                det.unidadPrecio.Equivalencia = Converter.GetDecimal(row, "equivalencia_precio");

                det.unidadCosto = new ProductoPresentacion();
                det.unidadCosto.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion_costo");
                det.unidadCosto.Presentacion = Converter.GetString(row, "unidad_costo");
                det.unidadCosto.CostoSinIGV = Converter.GetDecimal(row, "costo_unitario");
                det.unidadCosto.CostoOriginalSinIGV = Converter.GetDecimal(row, "costo_unitario_mp");
                det.unidadCosto.Equivalencia = Converter.GetDecimal(row, "equivalencia_costo");

                det.Estado = Converter.GetInt(row, "estado");
                det.FechaRegistro = Converter.GetDateTime(row, "fecha_creacion");

                


                obj.precios.Add(det);
            }

            return obj;
        }



        public List<PrecioEspecialCabecera> BuscarCabeceras(PrecioEspecialCabecera obj)
        {
            var objCommand = GetSqlCommand("ps_precioEspecialCabeceras");
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.usuario.idUsuario);
            InputParameterAdd.VarcharEmpty(objCommand, "codigo", obj.codigo);
            InputParameterAdd.VarcharEmpty(objCommand, "tipoNegociacion", obj.tipoNegociacion);

            InputParameterAdd.VarcharEmpty(objCommand, "fechaVigenciaDesde", obj.fechaInicio.ToString("yyyy-MM-dd"));
            InputParameterAdd.VarcharEmpty(objCommand, "fechaVigenciaHasta", obj.fechaFin.ToString("yyyy-MM-dd"));

            InputParameterAdd.Int(objCommand, "idClienteSunat", obj.clienteSunat.idClienteSunat);
            InputParameterAdd.Int(objCommand, "idGrupo", obj.grupoCliente.idGrupoCliente);

            DataTable dataTable = Execute(objCommand);

            List<PrecioEspecialCabecera> lista = new List<PrecioEspecialCabecera>();

            foreach (DataRow row in dataTable.Rows)
            {
                PrecioEspecialCabecera item = new PrecioEspecialCabecera();
                item.idPrecioEspecialCabecera = Converter.GetGuid(row, "id_precio_especial_cabecera");
                item.codigo = Converter.GetString(row, "codigo");
                item.fechaInicio = Converter.GetDateTime(row, "fecha_inicio");
                item.fechaFin = Converter.GetDateTime(row, "fecha_fin");

                item.tipoNegociacion = Converter.GetString(row, "tipo_negociacion");
                item.titulo = Converter.GetString(row, "titulo");

                item.clienteSunat = new ClienteSunat();
                item.clienteSunat.idClienteSunat = Converter.GetInt(row, "id_cliente_sunat");
                item.clienteSunat.nombreComercial = Converter.GetString(row, "nombre_comercial");
                item.clienteSunat.razonSocial = Converter.GetString(row, "razon_social");
                item.clienteSunat.ruc = Converter.GetString(row, "ruc");

                item.grupoCliente = new GrupoCliente();
                item.grupoCliente.idGrupoCliente = Converter.GetInt(row, "id_grupo");
                item.grupoCliente.codigo = Converter.GetString(row, "codigo_grupo");
                item.grupoCliente.nombre = Converter.GetString(row, "nombre_grupo");


                item.Estado = Converter.GetInt(row, "estado");
                item.FechaRegistro = Converter.GetDateTime(row, "fecha_creacion");
                item.FechaEdicion = Converter.GetDateTime(row, "fecha_modificacion");

                lista.Add(item);
            }

            return lista;
        }

        /*
@precios PrecioDetalleList readonly,
        */
        public PrecioEspecialCabecera InsertarCabecera(PrecioEspecialCabecera obj)
        {
            var objCommand = GetSqlCommand("pi_precioEspecialCabecera");

            InputParameterAdd.Guid(objCommand, "idUsuario", obj.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Varchar(objCommand, "tipoNegociacion", obj.tipoNegociacion);
            InputParameterAdd.Varchar(objCommand, "titulo", obj.titulo);
            InputParameterAdd.Varchar(objCommand, "observaciones", obj.observaciones);
            InputParameterAdd.DateTime(objCommand, "fechaInicio", obj.fechaInicio);
            InputParameterAdd.DateTime(objCommand, "fechaFin", obj.fechaFin);
            InputParameterAdd.Int(objCommand, "idClienteSunat", obj.clienteSunat.idClienteSunat);
            InputParameterAdd.Int(objCommand, "idGrupo", obj.grupoCliente.idGrupoCliente);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID_PRODUCTO", typeof(Guid)));
            tvp.Columns.Add(new DataColumn("MONEDA", typeof(string)));
            tvp.Columns.Add(new DataColumn("UNIDAD_PRECIO", typeof(string)));
            tvp.Columns.Add(new DataColumn("PRECIO_UNITARIO", typeof(decimal)));
            tvp.Columns.Add(new DataColumn("EQUIVALENCIA_PRECIO", typeof(decimal)));
            tvp.Columns.Add(new DataColumn("ID_PRODUCTO_PRESENTACION_PRECIO", typeof(int)));
            tvp.Columns.Add(new DataColumn("UNIDAD_COSTO", typeof(string)));
            tvp.Columns.Add(new DataColumn("COSTO_UNITARIO", typeof(decimal)));
            tvp.Columns.Add(new DataColumn("EQUIVALENCIA_COSTO", typeof(decimal)));
            tvp.Columns.Add(new DataColumn("ID_PRODUCTO_PRESENTACION_COSTO", typeof(int)));
            tvp.Columns.Add(new DataColumn("FECHA_INICIO", typeof(DateTime)));
            tvp.Columns.Add(new DataColumn("FECHA_FIN", typeof(DateTime)));
            tvp.Columns.Add(new DataColumn("OBSERVACIONES", typeof(string)));

            foreach (PrecioEspecialDetalle item in obj.precios)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID_PRODUCTO"] = item.producto.idProducto;
                rowObj["MONEDA"] = item.moneda.codigo;
                rowObj["UNIDAD_PRECIO"] = item.unidadPrecio.Presentacion;
                rowObj["PRECIO_UNITARIO"] = item.unidadPrecio.PrecioSinIGV;
                rowObj["EQUIVALENCIA_PRECIO"] = item.unidadPrecio.Equivalencia;
                rowObj["ID_PRODUCTO_PRESENTACION_PRECIO"] = item.unidadPrecio.IdProductoPresentacion;
                rowObj["UNIDAD_COSTO"] = item.unidadCosto.Presentacion;
                rowObj["COSTO_UNITARIO"] = item.unidadCosto.PrecioSinIGV;
                rowObj["EQUIVALENCIA_COSTO"] = item.unidadCosto.Equivalencia;
                rowObj["ID_PRODUCTO_PRESENTACION_COSTO"] = item.unidadCosto.IdProductoPresentacion;
                rowObj["FECHA_INICIO"] = item.fechaInicio;
                rowObj["FECHA_FIN"] = item.fechaFin;
                rowObj["OBSERVACIONES"] = item.observaciones;

                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@precios", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.PrecioDetalleList";

            ExecuteNonQuery(objCommand);

            return obj;
        }


        public PrecioEspecialCabecera ActualizarCabecera(PrecioEspecialCabecera obj)
        {
            var objCommand = GetSqlCommand("pu_precioEspecialCabecera");

            InputParameterAdd.Guid(objCommand, "idPrecioEspecialCabecera", obj.idPrecioEspecialCabecera);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Varchar(objCommand, "tipoNegociacion", obj.tipoNegociacion);
            InputParameterAdd.Varchar(objCommand, "titulo", obj.titulo);
            InputParameterAdd.Varchar(objCommand, "observaciones", obj.observaciones);
            InputParameterAdd.DateTime(objCommand, "fechaInicio", obj.fechaInicio);
            InputParameterAdd.DateTime(objCommand, "fechaFin", obj.fechaFin);
            InputParameterAdd.Int(objCommand, "idClienteSunat", obj.clienteSunat.idClienteSunat);
            InputParameterAdd.Int(objCommand, "idGrupo", obj.grupoCliente.idGrupoCliente);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID_PRODUCTO", typeof(Guid)));
            tvp.Columns.Add(new DataColumn("MONEDA", typeof(string)));
            tvp.Columns.Add(new DataColumn("UNIDAD_PRECIO", typeof(string)));
            tvp.Columns.Add(new DataColumn("PRECIO_UNITARIO", typeof(decimal)));
            tvp.Columns.Add(new DataColumn("EQUIVALENCIA_PRECIO", typeof(decimal)));
            tvp.Columns.Add(new DataColumn("ID_PRODUCTO_PRESENTACION_PRECIO", typeof(int)));
            tvp.Columns.Add(new DataColumn("UNIDAD_COSTO", typeof(string)));
            tvp.Columns.Add(new DataColumn("COSTO_UNITARIO", typeof(decimal)));
            tvp.Columns.Add(new DataColumn("EQUIVALENCIA_COSTO", typeof(decimal)));
            tvp.Columns.Add(new DataColumn("ID_PRODUCTO_PRESENTACION_COSTO", typeof(int)));
            tvp.Columns.Add(new DataColumn("FECHA_INICIO", typeof(DateTime)));
            tvp.Columns.Add(new DataColumn("FECHA_FIN", typeof(DateTime)));
            tvp.Columns.Add(new DataColumn("OBSERVACIONES", typeof(string)));

            foreach (PrecioEspecialDetalle item in obj.precios)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID_PRODUCTO"] = item.producto.idProducto;
                rowObj["MONEDA"] = item.moneda.codigo;
                rowObj["UNIDAD_PRECIO"] = item.unidadPrecio.Presentacion;
                rowObj["PRECIO_UNITARIO"] = item.unidadPrecio.PrecioSinIGV;
                rowObj["EQUIVALENCIA_PRECIO"] = item.unidadPrecio.Equivalencia;
                rowObj["ID_PRODUCTO_PRESENTACION_PRECIO"] = item.unidadPrecio.IdProductoPresentacion;
                rowObj["UNIDAD_COSTO"] = item.unidadCosto.Presentacion;
                rowObj["COSTO_UNITARIO"] = item.unidadCosto.PrecioSinIGV;
                rowObj["EQUIVALENCIA_COSTO"] = item.unidadCosto.Equivalencia;
                rowObj["ID_PRODUCTO_PRESENTACION_COSTO"] = item.unidadCosto.IdProductoPresentacion;
                rowObj["FECHA_INICIO"] = item.fechaInicio;
                rowObj["FECHA_FIN"] = item.fechaFin;
                rowObj["OBSERVACIONES"] = item.observaciones;

                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@precios", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.PrecioDetalleList";

            ExecuteNonQuery(objCommand);

            return obj;
        }
    }
}
