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

        public void setPrecioListaStaging(PrecioListaStaging precioListaStaging)
        {
            var objCommand = GetSqlCommand("pi_precioListaStaging");
            InputParameterAdd.Varchar(objCommand, "codigoCliente", precioListaStaging.codigoCliente);
            InputParameterAdd.DateTime(objCommand, "fechaInicioVigenciaPrecios", precioListaStaging.fechaVigenciaInicio);
            InputParameterAdd.DateTime(objCommand, "fechaVigenciaFin", precioListaStaging.fechaVigenciaFin);
            InputParameterAdd.Varchar(objCommand, "sku", precioListaStaging.sku);
            InputParameterAdd.Varchar(objCommand, "consideraCantidades", precioListaStaging.consideraCantidades);
            InputParameterAdd.Int(objCommand, "cantidad", precioListaStaging.cantidad);
            InputParameterAdd.Varchar(objCommand, "esAlternativa", precioListaStaging.esAlternativa);
            InputParameterAdd.Varchar(objCommand, "unidad", precioListaStaging.esAlternativa);
            InputParameterAdd.Varchar(objCommand, "moneda", precioListaStaging.esAlternativa);
            InputParameterAdd.Decimal(objCommand, "precioLista", precioListaStaging.precioLista);
            InputParameterAdd.Decimal(objCommand, "precioNeto", precioListaStaging.precioNeto);
            InputParameterAdd.Decimal(objCommand, "costo", precioListaStaging.costo);
            InputParameterAdd.Varchar(objCommand, "flete", precioListaStaging.flete);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", precioListaStaging.porcentajeDescuento);
            InputParameterAdd.Varchar(objCommand, "grupo", precioListaStaging.grupo);
            ExecuteNonQuery(objCommand);
        }
        
        public void truncatePrecioListaStaging()
        {
            var objCommand = GetSqlCommand("pt_precioListaStaging");
            ExecuteNonQuery(objCommand);
        }

        public void mergePrecioListaStaging()
        {
            var objCommand = GetSqlCommand("pu_precioListaStaging");
            ExecuteNonQuery(objCommand);
        }









        /*
        public List<PrecioClienteProducto> getListas()
        {
            var objCommand = GetSqlCommand("ps_getlistaprecios");
            DataTable dataTable = Execute(objCommand);
            List<PrecioClienteProducto> lista = new List<PrecioClienteProducto>();

            foreach (DataRow row in dataTable.Rows)
            {
                PrecioClienteProducto obj = new PrecioClienteProducto
                {
                    idPrecioLista = Converter.GetGuid(row, "id_precio_lista"),
                    nombre = Converter.GetString(row, "nombre"),
                    codigo = Converter.GetString(row, "codigo")
                };
                lista.Add(obj);
            }
            return lista;
        }*/
        /*
        public List<PrecioClienteProducto> getPreciosProducto(Guid idProducto, Guid idMoneda)
        {
            var objCommand = GetSqlCommand("ps_getpreciosproducto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idMoneda", idMoneda);
            DataTable dataTable = Execute(objCommand);
            List<PrecioClienteProducto> lista = new List<PrecioClienteProducto>();

            foreach (DataRow row in dataTable.Rows)
            {
                PrecioClienteProducto obj = new PrecioClienteProducto
                {
                    idPrecioLista = Converter.GetGuid(row, "id_precio_lista"),
                    nombre = Converter.GetString(row, "nombre"),
                    codigo = Converter.GetString(row, "codigo"),
                    precio = Converter.GetDecimal(row, "monto")
                };
                lista.Add(obj);
            }
            return lista;
        }*/
        /*
        public PrecioClienteProducto getPrecioProducto(Guid idProducto, Guid idPrecioLista)
        {
            var objCommand = GetSqlCommand("ps_getprecioproducto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            InputParameterAdd.Guid(objCommand, "idPrecioLista", idPrecioLista);
            DataTable dataTable = Execute(objCommand);

            PrecioClienteProducto obj = new PrecioClienteProducto();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idPrecioLista = Converter.GetGuid(row, "id_precio_lista");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.precio = Converter.GetDecimal(row, "monto");
            }
            return obj;
        }*/
    }
}
