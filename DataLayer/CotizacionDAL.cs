using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class CotizacionDAL : DaoBase
    {
        public CotizacionDAL(IDalSettings settings) : base(settings)
        {
        }

        public CotizacionDAL() : this(new CotizadorSettings())
        {
        }

        public void InsertCotizacion(Cotizacion obj)
        {
            var objCommand = GetSqlCommand("pi_cotizacion");


            InputParameterAdd.Varchar(objCommand, "codigo", "0000000001");
            InputParameterAdd.Guid(objCommand, "idCiudad", obj.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", obj.idCliente);
            InputParameterAdd.Guid(objCommand, "idMoneda", obj.idMoneda);
            InputParameterAdd.Guid(objCommand, "idPrecioLista", obj.idPrecio);
            InputParameterAdd.Guid(objCommand, "idTipoCambio", obj.idTipoCambio);
            InputParameterAdd.DateTime(objCommand, "fecha", obj.fecha);
            InputParameterAdd.SmallInt(objCommand, "incluidoIgv", obj.incluidoIgv);
            InputParameterAdd.SmallInt(objCommand, "mostrarCodProveedor", obj.mostrarCodProveedor);
            InputParameterAdd.Decimal(objCommand, "flete", obj.flete);

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);

            obj.idCotizacion = (Guid)objCommand.Parameters["@newId"].Value;
        }

        public void InsertCotizacionDetalle(CotizacionDetalle obj)
        {
            var objCommand = GetSqlCommand("pi_cotizacionDetalle");
            
            InputParameterAdd.Guid(objCommand, "idCotizacion", obj.idCotizacion);
            InputParameterAdd.Guid(objCommand, "idProducto", obj.idProducto);
            InputParameterAdd.Guid(objCommand, "idPrecioLista", obj.idPrecio);

            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", obj.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "valorUnitario", obj.valorUnitario);
            InputParameterAdd.Int(objCommand, "cantidad", obj.cantidad);

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);

            obj.idCotizacionDetalle = (Guid)objCommand.Parameters["@newId"].Value;
        }
    }
}
