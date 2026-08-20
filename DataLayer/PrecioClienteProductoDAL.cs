using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.IO;
using System.Linq;

namespace DataLayer
{
    public class PrecioClienteProductoDAL : DaoBase
    {
        public PrecioClienteProductoDAL(IDalSettings settings) : base(settings)
        {
        }

        public PrecioClienteProductoDAL() : this(new CotizadorSettings())
        {
        }

        public List<PrecioClienteProducto> getPreciosPuntalesRegistrados(Guid idProducto, Guid idCliente)
        {
            var objCommand = GetSqlCommand("ps_preciosPuntualesClienteProducto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            DataTable preciosDataSet = Execute(objCommand);

            List<PrecioClienteProducto> precioListaList = new List<PrecioClienteProducto>();
            foreach (DataRow row in preciosDataSet.Rows)
            {
                PrecioClienteProducto precioLista = new PrecioClienteProducto();

                precioLista.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia_precios");

                precioLista.fechaFinVigencia = Converter.GetDateTime(row, "fecha_limite_validez_oferta");

                precioLista.moneda = Moneda.ListaMonedasFija.Where(m => m.codigo.Equals(Converter.GetString(row, "codigo_moneda"))).First();

                precioLista.unidad = Converter.GetString(row, "unidad");
                precioLista.precioNeto = Converter.GetDecimal(row, "precio_neto");
                precioLista.flete = Converter.GetDecimal(row, "flete");
                precioLista.precioUnitario = Converter.GetDecimal(row, "precio_sin_igv");

                precioLista.numeroCotizacion = Converter.GetInt(row, "numero_cotizacion").ToString().PadLeft(10, '0');
                
                precioListaList.Add(precioLista);
            }

            return precioListaList;
        }

        public List<PrecioClienteProducto> getPreciosRegistrados(Guid idProducto, Guid idCliente)
        {
            var objCommand = GetSqlCommand("ps_getprecioClienteProducto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            DataTable preciosDataSet = Execute(objCommand);

            List<PrecioClienteProducto> precioListaList = new List<PrecioClienteProducto>();
            foreach (DataRow row in preciosDataSet.Rows)
            {
                PrecioClienteProducto precioLista = new PrecioClienteProducto();

                //     producto.idProducto = Converter.GetGuid(row, "unidad");

                if (row["fecha_inicio_vigencia"] == DBNull.Value)
                {
                    precioLista.fechaInicioVigencia = null;
                }
                else
                {
                    precioLista.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                }

                if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    precioLista.fechaFinVigencia = null;
                }
                else
                {
                    precioLista.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                }

                precioLista.moneda = Moneda.ListaMonedasFija.Where(m => m.caracter.Equals(Converter.GetString(row, "moneda"))).First();

                precioLista.unidad = Converter.GetString(row, "unidad");
                precioLista.precioNeto = Converter.GetDecimal(row, "precio_neto");
                precioLista.flete = Converter.GetDecimal(row, "flete");
                precioLista.vigenciaCorregida = Converter.GetInt(row, "vigencia_corregida");
                precioLista.precioUnitario = Converter.GetDecimal(row, "precio_unitario");
                precioLista.tipoCotizacion = Converter.GetString(row, "tipo_cotizacion");
                if (row["numero_cotizacion"] == DBNull.Value)
                {
                    precioLista.numeroCotizacion = null;
                }
                else
                {
                    precioLista.numeroCotizacion = Converter.GetInt(row, "numero_cotizacion").ToString().PadLeft(10, '0');
                }

                precioListaList.Add(precioLista);
            }

            return precioListaList;
        }

        public bool agregaProductoCanastaCliente(Guid idCliente, Guid idProducto, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pi_canastaClienteProducto");

            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
           
            ExecuteNonQuery(objCommand);
            
            return true;
        }

        public bool retiraProductoCanastaCliente(Guid idCliente, Guid idProducto, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pd_canastaClienteProducto");

            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);

            ExecuteNonQuery(objCommand);

            return true;
        }
        
        public bool agregaProductoCanastaGrupoCliente(int idGrupoCliente, Guid idProducto, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pi_canastaGrupoClienteProducto");

            InputParameterAdd.Int(objCommand, "idGrupoCliente", idGrupoCliente);
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Int(objCommand, "aplicaMiembros", 1);

            ExecuteNonQuery(objCommand);

            return true;
        }


        public bool limpiaCanastaCliente(int idGrupoCliente, int aplicaMiembros)
        {
            var objCommand = GetSqlCommand("pd_limpiarCanastaGrupoCliente");

            InputParameterAdd.Int(objCommand, "idGrupoCliente", idGrupoCliente);
            InputParameterAdd.Int(objCommand, "aplicaMiembros", aplicaMiembros);

            ExecuteNonQuery(objCommand);

            return true;
        }

        public bool retiraProductoCanastaGrupoCliente(int idGrupoCliente, Guid idProducto, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pd_canastaGrupoClienteProducto");

            InputParameterAdd.Int(objCommand, "idGrupoCliente", idGrupoCliente);
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Int(objCommand, "aplicaMiembros", 1);

            ExecuteNonQuery(objCommand);

            return true;
        }

        public List<PrecioClienteProducto> getPreciosRegistradosGrupo(Guid idProducto, int idGrupoCliente)
        {
            var objCommand = GetSqlCommand("ps_getprecioGrupoClienteProducto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Int(objCommand, "idGrupoCliente", idGrupoCliente);
            DataTable preciosDataSet = Execute(objCommand);

            List<PrecioClienteProducto> precioListaList = new List<PrecioClienteProducto>();
            foreach (DataRow row in preciosDataSet.Rows)
            {
                PrecioClienteProducto precioLista = new PrecioClienteProducto();

                //     producto.idProducto = Converter.GetGuid(row, "unidad");

                if (row["fecha_inicio_vigencia"] == DBNull.Value)
                {
                    precioLista.fechaInicioVigencia = null;
                }
                else
                {
                    precioLista.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                }

                if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    precioLista.fechaFinVigencia = null;
                }
                else
                {
                    precioLista.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                }

                precioLista.moneda = Moneda.ListaMonedasFija.Where(m => m.caracter.Equals(Converter.GetString(row, "moneda"))).First();

                precioLista.unidad = Converter.GetString(row, "unidad");
                precioLista.precioNeto = Converter.GetDecimal(row, "precio_neto");
                precioLista.vigenciaCorregida = Converter.GetInt(row, "vigencia_corregida");
                precioLista.flete = Converter.GetDecimal(row, "flete");
                precioLista.precioUnitario = Converter.GetDecimal(row, "precio_unitario");
                if (row["numero_cotizacion"] == DBNull.Value)
                {
                    precioLista.numeroCotizacion = null;
                }
                else
                {
                    precioLista.numeroCotizacion = Converter.GetInt(row, "numero_cotizacion").ToString().PadLeft(10, '0');
                }

                precioListaList.Add(precioLista);
            }

            return precioListaList;
        }

        public List<PrecioClienteProducto> GetPreciosVencidosNoRenovadosPorActualizacionPrecioLista(Guid idUsuario, DateTime fechaDesde)
        {
            var objCommand = GetSqlCommand("ps_precios_cotizados_vencidos_no_renovados_actualizacion_precio_lista");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.DateTime(objCommand, "fechaDesde", fechaDesde);
            DataTable preciosDataSet = Execute(objCommand);

            List<PrecioClienteProducto> precioListaList = new List<PrecioClienteProducto>();
            foreach (DataRow row in preciosDataSet.Rows)
            {
                PrecioClienteProducto precioLista = new PrecioClienteProducto();

                if (row["fecha_inicio_vigencia"] == DBNull.Value)
                {
                    precioLista.fechaInicioVigencia = null;
                }
                else
                {
                    precioLista.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");
                }

                if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    precioLista.fechaFinVigencia = null;
                }
                else
                {
                    precioLista.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                }

                precioLista.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");
                precioLista.unidad = Converter.GetString(row, "unidad");
                precioLista.precioNeto = Converter.GetDecimal(row, "precio_neto");
                precioLista.flete = Converter.GetDecimal(row, "flete");
                precioLista.precioUnitario = Converter.GetDecimal(row, "precio_unitario");

                precioLista.producto = new Producto();
                precioLista.producto.idProducto = Converter.GetGuid(row, "id_producto");
                precioLista.producto.sku = Converter.GetString(row, "sku");
                precioLista.producto.descripcion = Converter.GetString(row, "producto_nombre");

                Guid idCliente = Converter.GetGuid(row, "id_cliente");
                if (!idCliente.Equals(Guid.Empty)) {
                    precioLista.cliente = new Cliente();
                    precioLista.cliente.idCliente = idCliente;
                    precioLista.cliente.codigo = Converter.GetString(row, "cliente_codigo");
                    precioLista.cliente.ruc = Converter.GetString(row, "ruc");
                    precioLista.cliente.razonSocial = Converter.GetString(row, "cliente_nombre");

                    precioLista.grupoCliente = null;
                }
                else
                {
                    precioLista.grupoCliente = new GrupoCliente();
                    precioLista.grupoCliente.idGrupoCliente = Converter.GetInt(row, "id_grupo_cliente");
                    precioLista.grupoCliente.codigo = Converter.GetString(row, "id_grupo_cliente");
                    precioLista.grupoCliente.nombre = Converter.GetString(row, "grupo_nombre");

                    precioLista.cliente = null;
                }

                precioLista.cotizacion = new Cotizacion();
                precioLista.cotizacion.idCotizacion = Converter.GetGuid(row, "id_cotizacion");
                precioLista.cotizacion.codigo = Converter.GetLong(row, "cotizacion_codigo");

                precioLista.moneda = Moneda.ListaMonedasFija.Where(m => m.caracter.Equals(Converter.GetString(row, "moneda"))).First();

                precioListaList.Add(precioLista);
            }

            return precioListaList;
        }
    }
}
