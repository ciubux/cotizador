using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Data.SqlClient;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using Model.NextSoft;
using Model.ServiceSunatPadron;
using System.Reflection;

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
                obj.codigoListaProveedor = Converter.GetString(row, "codigo_lista_proveedor");
                obj.fechaInicio = Converter.GetDateTime(row, "fecha_inicio");
                obj.fechaFin = Converter.GetDateTime(row, "fecha_fin");

                obj.tipoNegociacion = Converter.GetString(row, "tipo_negociacion");
                obj.titulo = Converter.GetString(row, "titulo");
                obj.observaciones = Converter.GetString(row, "observaciones");

                obj.clienteSunat = new Model.ClienteSunat();
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

        public PrecioEspecialDetalle GetCostoEspecialVigente(Guid idCliente, int idGrupo, Guid idProducto, int idEmpresa)
        {
            var objCommand = GetSqlCommand("ps_costoEspecialVigente");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Int(objCommand, "idEmpresa", idEmpresa);

            if (idGrupo == 0)
            {
                InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            } else
            {
                InputParameterAdd.Int(objCommand, "idGrupo", idGrupo);
            }

            DataTable dataTable = Execute(objCommand);

            PrecioEspecialDetalle item = new PrecioEspecialDetalle();

            foreach (DataRow row in dataTable.Rows)
            {
                item.idPrecioEspecialDetalle = Converter.GetGuid(row, "id_precio_especial_detalle");
                item.unidadCosto = new ProductoPresentacion();
                item.unidadCosto.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                item.unidadCosto.CostoSinIGV = Converter.GetDecimal(row, "costo_unitario_mp");
                item.unidadCosto.Equivalencia = 1;
            }

            return item;
        }

        public List<PrecioEspecialDetalle> ValidarPrecios(PrecioEspecialCabecera obj)
        {
            var objCommand = GetSqlCommand("ps_validacionPrecioEspecialDetalles");
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.usuario.idUsuario);
            InputParameterAdd.VarcharEmpty(objCommand, "tipoNegociacion", obj.tipoNegociacion);

            InputParameterAdd.Int(objCommand, "idClienteSunat", obj.clienteSunat.idClienteSunat);
            InputParameterAdd.Int(objCommand, "idGrupo", obj.grupoCliente.idGrupoCliente);

            if (obj.idPrecioEspecialCabecera != null && !obj.idPrecioEspecialCabecera.Equals(Guid.Empty)) {
                InputParameterAdd.Guid(objCommand, "idPrecioEspecialCabecera", obj.idPrecioEspecialCabecera);
            }

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("SKU", typeof(string)));
            tvp.Columns.Add(new DataColumn("MONEDA", typeof(string)));
            tvp.Columns.Add(new DataColumn("PRECIO_UNITARIO", typeof(decimal)));
            tvp.Columns.Add(new DataColumn("ID_PRODUCTO_PRESENTACION_PRECIO", typeof(int)));
            tvp.Columns.Add(new DataColumn("COSTO_UNITARIO", typeof(decimal)));
            tvp.Columns.Add(new DataColumn("ID_PRODUCTO_PRESENTACION_COSTO", typeof(int)));
            tvp.Columns.Add(new DataColumn("FECHA_INICIO", typeof(DateTime)));
            tvp.Columns.Add(new DataColumn("FECHA_FIN", typeof(DateTime)));
            tvp.Columns.Add(new DataColumn("OBSERVACIONES", typeof(string)));

            foreach (PrecioEspecialDetalle item in obj.precios)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["SKU"] = item.producto.sku;
                rowObj["MONEDA"] = item.moneda.codigo;
                rowObj["PRECIO_UNITARIO"] = item.unidadPrecio.PrecioSinIGV;
                rowObj["ID_PRODUCTO_PRESENTACION_PRECIO"] = item.unidadPrecio.IdProductoPresentacion;
                rowObj["COSTO_UNITARIO"] = item.unidadCosto.CostoSinIGV;
                rowObj["ID_PRODUCTO_PRESENTACION_COSTO"] = item.unidadCosto.IdProductoPresentacion;
                rowObj["FECHA_INICIO"] = item.fechaInicio;
                rowObj["FECHA_FIN"] = item.fechaFin;
                rowObj["OBSERVACIONES"] = item.observaciones;

                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@precios", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.PrecioDetalleValidarList";


            DataTable dataTable = Execute(objCommand);

            
            

            List<PrecioEspecialDetalle> lista = new List<PrecioEspecialDetalle>();
            foreach (DataRow row in dataTable.Rows)
            {
                PrecioEspecialDetalle item = new PrecioEspecialDetalle();
                
                item.producto = new Producto();
                item.producto.idProducto = Converter.GetGuid(row, "id_producto");
                item.producto.sku = Converter.GetString(row, "SKU");
                item.producto.descripcion = Converter.GetString(row, "descripcion_producto");

                decimal precio = Converter.GetDecimal(row, "precio");
                decimal costo = Converter.GetDecimal(row, "costo");

                int eqAlternativa = Converter.GetInt(row, "equivalencia");
                int eqProveedor = Converter.GetInt(row, "equivalencia_proveedor");

                string unMP = Converter.GetString(row, "unidad");
                string unProveedor = Converter.GetString(row, "unidad_proveedor");
                string unAlternativa = Converter.GetString(row, "unidad_alternativa");

                item.unidadPrecio = new ProductoPresentacion();
                item.unidadPrecio.IdProductoPresentacion = Converter.GetInt(row, "ID_PRODUCTO_PRESENTACION_PRECIO");
                item.unidadPrecio.PrecioSinIGV = Converter.GetDecimal(row, "PRECIO_UNITARIO");

                switch(item.unidadPrecio.IdProductoPresentacion)
                {
                    case 0:
                        item.unidadPrecio.Presentacion = unMP;
                        item.unidadPrecio.PrecioOriginalSinIGV = precio;
                        item.unidadPrecio.Equivalencia = 1;
                        break;
                    case 1:
                        item.unidadPrecio.Presentacion = unAlternativa;
                        item.unidadPrecio.PrecioOriginalSinIGV = precio / ((decimal)eqAlternativa);
                        item.unidadPrecio.Equivalencia = eqAlternativa;
                        break;
                    case 2:
                        item.unidadPrecio.Presentacion = unProveedor;
                        item.unidadPrecio.PrecioOriginalSinIGV = precio * ((decimal)eqProveedor); 
                        item.unidadPrecio.Equivalencia = 1 / (decimal)eqProveedor;
                        break;
                }

                item.unidadCosto = new ProductoPresentacion();
                item.unidadCosto.IdProductoPresentacion = Converter.GetInt(row, "ID_PRODUCTO_PRESENTACION_COSTO");
                item.unidadCosto.CostoSinIGV = Converter.GetDecimal(row, "COSTO_UNITARIO");

                switch (item.unidadCosto.IdProductoPresentacion)
                {
                    case 0:
                        item.unidadCosto.Presentacion = unMP;
                        item.unidadCosto.CostoOriginalSinIGV = costo;
                        item.unidadCosto.Equivalencia = 1;
                        break;
                    case 1:
                        item.unidadCosto.Presentacion = unAlternativa;
                        item.unidadCosto.CostoOriginalSinIGV = costo / ((decimal)eqAlternativa);
                        item.unidadCosto.Equivalencia = eqAlternativa;
                        break;
                    case 2:
                        item.unidadCosto.Presentacion = unProveedor;
                        item.unidadCosto.CostoOriginalSinIGV = costo * ((decimal)eqProveedor);
                        item.unidadCosto.Equivalencia = 1 / (decimal)eqProveedor;
                        break;
                }

                item.fechaInicio = Converter.GetDateTime(row, "FECHA_INICIO");
                item.fechaFin = Converter.GetDateTime(row, "FECHA_FIN");
                item.observaciones = Converter.GetString(row, "OBSERVACIONES");

                item.moneda = new Moneda();
                item.moneda.codigo = Converter.GetString(row, "codigo_moneda");
                item.moneda.nombre = Converter.GetString(row, "nombre_moneda");
                item.moneda.simbolo = Converter.GetString(row, "simbolo_moneda");

                item.Estado = 1;

                Guid idDetalleRelacionado = Converter.GetGuid(row, "id_precio_especial_detalle_conflicto");
                
                if (idDetalleRelacionado != null && !idDetalleRelacionado.Equals(Guid.Empty))
                {
                    item.dataRelacionada = new List<string>();
                    item.dataRelacionada.Add(idDetalleRelacionado.ToString());
                    item.dataRelacionada.Add(Converter.GetString(row, "codigo_conflicto") + " - " + Converter.GetString(row, "titulo_conflicto"));
                    item.dataRelacionada.Add(Converter.GetString(row, "fecha_inicio_conflicto"));
                    item.dataRelacionada.Add(Converter.GetString(row, "fecha_fin_conflicto"));
                    item.dataRelacionada.Add(Converter.GetString(row, "simbolo_moneda_conflicto"));
                    item.dataRelacionada.Add(Converter.GetString(row, "unidad_precio_conflicto"));
                    item.dataRelacionada.Add(Converter.GetString(row, "precio_unitario_conflicto"));
                    item.dataRelacionada.Add(Converter.GetString(row, "unidad_costo_conflicto"));
                    item.dataRelacionada.Add(Converter.GetString(row, "costo_unitario_conflicto"));

                }

                lista.Add(item);
            }

            return lista;
        }

        public List<PrecioEspecialCabecera> BuscarCabecerasDetalles(PrecioEspecialCabecera obj)
        {
            var objCommand = GetSqlCommand("ps_precioEspecialCabecerasDetalles");
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.usuario.idUsuario);
            InputParameterAdd.VarcharEmpty(objCommand, "codigo", obj.codigo);
            InputParameterAdd.VarcharEmpty(objCommand, "tipoNegociacion", obj.tipoNegociacion);

            InputParameterAdd.VarcharEmpty(objCommand, "fechaVigenciaDesde", obj.fechaInicio.ToString("yyyy-MM-dd"));
            InputParameterAdd.VarcharEmpty(objCommand, "fechaVigenciaHasta", obj.fechaFin.ToString("yyyy-MM-dd"));

            InputParameterAdd.Int(objCommand, "idClienteSunat", obj.clienteSunat.idClienteSunat);
            InputParameterAdd.Int(objCommand, "idGrupo", obj.grupoCliente.idGrupoCliente);

            DataTable dataTable = Execute(objCommand);

            List<PrecioEspecialCabecera> lista = new List<PrecioEspecialCabecera>();
            PrecioEspecialCabecera cabTemp = null;

            foreach (DataRow row in dataTable.Rows)
            {
                
                PrecioEspecialCabecera item = new PrecioEspecialCabecera();
                item.idPrecioEspecialCabecera = Converter.GetGuid(row, "id_precio_especial_cabecera");

                if (cabTemp != null && cabTemp.idPrecioEspecialCabecera.Equals(item.idPrecioEspecialCabecera))
                {
                    item = cabTemp;
                } else
                {
                    if (cabTemp != null)
                    {
                        lista.Add(cabTemp);
                    }

                    item.codigo = Converter.GetString(row, "codigo");
                    item.fechaInicio = Converter.GetDateTime(row, "fecha_inicio");
                    item.fechaFin = Converter.GetDateTime(row, "fecha_fin");

                    item.tipoNegociacion = Converter.GetString(row, "tipo_negociacion");
                    item.titulo = Converter.GetString(row, "titulo");

                    item.clienteSunat = new Model.ClienteSunat();
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

                    item.precios = new List<PrecioEspecialDetalle>();
                }


                PrecioEspecialDetalle det = new PrecioEspecialDetalle();

                det.idPrecioEspecialDetalle = Converter.GetGuid(row, "id_precio_especial_detalle");

                det.producto = new Producto();
                det.producto.idProducto = Converter.GetGuid(row, "id_producto");
                det.producto.sku = Converter.GetString(row, "sku");
                det.producto.descripcion = Converter.GetString(row, "nombre_producto");

                det.fechaInicio = Converter.GetDateTime(row, "fecha_inicio_det");
                det.fechaFin = Converter.GetDateTime(row, "fecha_fin_det");
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

                det.Estado = Converter.GetInt(row, "estado_det");
                det.FechaRegistro = Converter.GetDateTime(row, "fecha_creacion_det");

                item.precios.Add(det);


                cabTemp = item;
            }

            if (cabTemp != null)
            {
                lista.Add(cabTemp);
            }

            return lista;
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

                item.clienteSunat = new Model.ClienteSunat();
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

        public List<PrecioEspecialCabecera> CabecerasDetalles(PrecioEspecialCabecera search)
        {
            var objCommand = GetSqlCommand("ps_precio_especial_cabecera_detalles");
            InputParameterAdd.Guid(objCommand, "idUsuario", search.usuario.idUsuario);

            DataTable dataTable = Execute(objCommand);

            List<PrecioEspecialCabecera> lista = new List<PrecioEspecialCabecera>();
            String codigo = "";
            PrecioEspecialCabecera obj = new PrecioEspecialCabecera();
            obj.codigo = "";

            foreach (DataRow row in dataTable.Rows)
            {
                codigo = Converter.GetString(row, "codigo");

                if (!codigo.Equals(obj.codigo))
                {
                    if (!obj.codigo.Equals(""))
                    {
                        lista.Add(obj);
                    }

                    obj = new PrecioEspecialCabecera();

                    obj.codigo = Converter.GetString(row, "codigo");
                    obj.titulo = Converter.GetString(row, "titulo");
                    obj.fechaInicio = Converter.GetDateTime(row, "fecha_inicio_cabecera");
                    obj.fechaFin = Converter.GetDateTime(row, "fecha_fin_cabecera");

                    obj.tipoNegociacion = Converter.GetString(row, "tipo_negociacion");
                    obj.codigoListaProveedor = Converter.GetString(row, "codigo_lista_proveedor");
                    obj.observaciones = Converter.GetString(row, "observaciones_cabecera");

                    obj.clienteSunat = new Model.ClienteSunat();
                    obj.clienteSunat.razonSocial = Converter.GetString(row, "razon_social");
                    obj.clienteSunat.ruc = Converter.GetString(row, "ruc");

                    obj.grupoCliente = new GrupoCliente();
                    obj.grupoCliente.codigo = Converter.GetString(row, "codigo_grupo");
                    obj.grupoCliente.nombre = Converter.GetString(row, "nombre_grupo");
                    obj.precios = new List<PrecioEspecialDetalle>();
                }

                PrecioEspecialDetalle det = new PrecioEspecialDetalle();


                det.producto = new Producto();
                det.producto.sku = Converter.GetString(row, "sku");
                det.producto.descripcion = Converter.GetString(row, "nombre_producto");

                det.fechaInicio = Converter.GetDateTime(row, "fecha_inicio");
                det.fechaFin = Converter.GetDateTime(row, "fecha_fin");
                det.observaciones = Converter.GetString(row, "observaciones");

                det.moneda = new Moneda();
                det.moneda.codigo = Converter.GetString(row, "moneda");

                det.unidadPrecio = new ProductoPresentacion();
                det.unidadPrecio.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion_precio");
                det.unidadPrecio.Presentacion = Converter.GetString(row, "unidad_precio");
                det.unidadPrecio.PrecioSinIGV = Converter.GetDecimal(row, "precio_unitario");
                det.unidadPrecio.Equivalencia = Converter.GetDecimal(row, "equivalencia_precio");

                det.unidadCosto = new ProductoPresentacion();
                det.unidadCosto.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion_costo");
                det.unidadCosto.Presentacion = Converter.GetString(row, "unidad_costo");
                det.unidadCosto.CostoSinIGV = Converter.GetDecimal(row, "costo_unitario");
                det.unidadCosto.Equivalencia = Converter.GetDecimal(row, "equivalencia_costo");

                obj.precios.Add(det);
                                
            }

            if (!obj.codigo.Equals(""))
            {
                lista.Add(obj);
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
            InputParameterAdd.Varchar(objCommand, "codigoListaProveedor", obj.codigoListaProveedor);
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
                rowObj["COSTO_UNITARIO"] = item.unidadCosto.CostoSinIGV;
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
            InputParameterAdd.Varchar(objCommand, "codigoListaProveedor", obj.codigoListaProveedor);
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

        public bool ActualizarCostosEspeciales(Guid idUsuario, DateTime fechaInicio, DateTime fechaFin)
        {
            var objCommand = GetSqlCommand("pu_actualizar_costos_especiales_ventas");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Varchar(objCommand, "fechaInicio", fechaInicio.ToString("yyyy-MM-dd"));
            InputParameterAdd.Varchar(objCommand, "fechaFin", fechaFin.ToString("yyyy-MM-dd"));

            ExecuteNonQuery(objCommand);

            return true;
        }

        public bool ActualizarTodos(Guid idUsuario, List<PrecioEspecialCabecera> listas)
        {
            var objCommand = GetSqlCommand("pi_precio_especial_fill");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("CODIGO", typeof(string)));
            tvp.Columns.Add(new DataColumn("CODIGO_LISTA_PROVEEDOR", typeof(string)));
            tvp.Columns.Add(new DataColumn("TITULO", typeof(string)));
            tvp.Columns.Add(new DataColumn("TIPO_NEGOCIACION", typeof(string)));
            tvp.Columns.Add(new DataColumn("RUC", typeof(string)));
            tvp.Columns.Add(new DataColumn("CODIGO_GRUPO", typeof(string)));
            tvp.Columns.Add(new DataColumn("FECHA_INICIO_CABECERA", typeof(DateTime)));
            tvp.Columns.Add(new DataColumn("FECHA_FIN_CABECERA", typeof(DateTime)));
            tvp.Columns.Add(new DataColumn("OBSERVACIONES_CABECERA", typeof(string)));

            tvp.Columns.Add(new DataColumn("SKU", typeof(string)));
            tvp.Columns.Add(new DataColumn("MONEDA", typeof(string)));
            tvp.Columns.Add(new DataColumn("PRECIO_ID_PRESENTACION", typeof(int)));
            tvp.Columns.Add(new DataColumn("PRECIO_UNITARIO", typeof(decimal)));
            tvp.Columns.Add(new DataColumn("COSTO_ID_PRESENTACION", typeof(int)));
            tvp.Columns.Add(new DataColumn("COSTO_UNITARIO", typeof(decimal)));
            tvp.Columns.Add(new DataColumn("FECHA_INICIO", typeof(DateTime)));
            tvp.Columns.Add(new DataColumn("FECHA_FIN", typeof(DateTime)));
            tvp.Columns.Add(new DataColumn("OBSERVACIONES", typeof(string)));

            foreach (PrecioEspecialCabecera obj in listas)
            {
                foreach (PrecioEspecialDetalle item in obj.precios)
                {
                    DataRow rowObj = tvp.NewRow();

                    rowObj["CODIGO"] = obj.codigo;
                    rowObj["CODIGO_LISTA_PROVEEDOR"] = obj.codigoListaProveedor;
                    rowObj["TITULO"] = obj.titulo;
                    rowObj["TIPO_NEGOCIACION"] = obj.tipoNegociacion;
                    rowObj["CODIGO_GRUPO"] = obj.grupoCliente.codigo;
                    rowObj["RUC"] = obj.clienteSunat.ruc;
                    rowObj["FECHA_INICIO_CABECERA"] = obj.fechaInicio;
                    rowObj["FECHA_FIN_CABECERA"] = obj.fechaFin;
                    rowObj["OBSERVACIONES_CABECERA"] = obj.observaciones;


                    rowObj["SKU"] = item.producto.sku;
                    rowObj["MONEDA"] = item.moneda.codigo;
                    rowObj["PRECIO_ID_PRESENTACION"] = item.unidadPrecio.IdProductoPresentacion;
                    rowObj["PRECIO_UNITARIO"] = item.unidadPrecio.PrecioSinIGV;
                    rowObj["COSTO_ID_PRESENTACION"] = item.unidadCosto.IdProductoPresentacion;
                    rowObj["COSTO_UNITARIO"] = item.unidadCosto.CostoSinIGV;
                    rowObj["FECHA_INICIO"] = item.fechaInicio;
                    rowObj["FECHA_FIN"] = item.fechaFin;
                    rowObj["OBSERVACIONES"] = item.observaciones;

                    tvp.Rows.Add(rowObj);
                }
            }
            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@items", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.PrecioEspecialCabeceraDetalleList";


            ExecuteNonQuery(objCommand);

            return true;
        }
    }
}
