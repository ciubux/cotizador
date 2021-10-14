using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.IO;

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
    }
}
