using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class PrecioListaDAL : DaoBase
    {
        public PrecioListaDAL(IDalSettings settings) : base(settings)
        {
        }

        public PrecioListaDAL() : this(new CotizadorSettings())
        {
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
    }
}
