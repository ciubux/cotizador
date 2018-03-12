using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class PrecioDAL : DaoBase
    {
        public PrecioDAL(IDalSettings settings) : base(settings)
        {
        }

        public PrecioDAL() : this(new CotizadorSettings())
        {
        }

        public void setPrecioStaging(PrecioStaging precioStaging)
        {
            var objCommand = GetSqlCommand("pi_precioStaging");
            InputParameterAdd.DateTime(objCommand, "fecha", precioStaging.fecha);
            InputParameterAdd.Varchar(objCommand, "cliente", precioStaging.codigoCliente);
            InputParameterAdd.Varchar(objCommand, "producto", precioStaging.sku);
            InputParameterAdd.Char(objCommand, "moneda", precioStaging.moneda);
            InputParameterAdd.Decimal(objCommand, "precioUnitario", precioStaging.precio);
            InputParameterAdd.Varchar(objCommand, "codigoVendedor", precioStaging.codigoVendedor);
            InputParameterAdd.Char(objCommand, "sede", precioStaging.sede);
            ExecuteNonQuery(objCommand);
        }
        
        public void truncatePrecioStaging(String sede)
        {
            var objCommand = GetSqlCommand("pt_precioStaging");
            InputParameterAdd.Char(objCommand, "sede", sede);
            ExecuteNonQuery(objCommand);
        }

        public void mergePrecioStaging()
        {
            var objCommand = GetSqlCommand("pu_precioStaging");
            ExecuteNonQuery(objCommand);
        }










        public List<PrecioLista> getListas()
        {
            var objCommand = GetSqlCommand("ps_getlistaprecios");
            DataTable dataTable = Execute(objCommand);
            List<PrecioLista> lista = new List<PrecioLista>();

            foreach (DataRow row in dataTable.Rows)
            {
                PrecioLista obj = new PrecioLista
                {
                    idPrecioLista = Converter.GetGuid(row, "id_precio_lista"),
                    nombre = Converter.GetString(row, "nombre"),
                    codigo = Converter.GetString(row, "codigo")
                };
                lista.Add(obj);
            }
            return lista;
        }
        public List<PrecioLista> getPreciosProducto(Guid idProducto, Guid idMoneda)
        {
            var objCommand = GetSqlCommand("ps_getpreciosproducto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idMoneda", idMoneda);
            DataTable dataTable = Execute(objCommand);
            List<PrecioLista> lista = new List<PrecioLista>();

            foreach (DataRow row in dataTable.Rows)
            {
                PrecioLista obj = new PrecioLista
                {
                    idPrecioLista = Converter.GetGuid(row, "id_precio_lista"),
                    nombre = Converter.GetString(row, "nombre"),
                    codigo = Converter.GetString(row, "codigo"),
                    precio = Converter.GetDecimal(row, "monto")
                };
                lista.Add(obj);
            }
            return lista;
        }

        public PrecioLista getPrecioProducto(Guid idProducto, Guid idPrecioLista)
        {
            var objCommand = GetSqlCommand("ps_getprecioproducto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idPrecioLista", idPrecioLista);
            DataTable dataTable = Execute(objCommand);

            PrecioLista obj = new PrecioLista();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idPrecioLista = Converter.GetGuid(row, "id_precio_lista");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.precio = Converter.GetDecimal(row, "monto");
            }
            return obj;
        }
    }
}
