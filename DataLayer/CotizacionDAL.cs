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
            InputParameterAdd.DateTime(objCommand, "fecha", obj.fecha);
            InputParameterAdd.SmallInt(objCommand, "incluidoIgv", obj.incluidoIgv);
            InputParameterAdd.SmallInt(objCommand, "consideraCantidades", short.Parse((obj.considerarCantidades ? 1 : 0).ToString()));
            InputParameterAdd.Guid(objCommand, "idCliente", obj.cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "idCiudad", obj.ciudad.idCiudad);
            //porcentajeFlete
            InputParameterAdd.Decimal(objCommand, "porcentajeFlete", obj.flete);
            InputParameterAdd.Decimal(objCommand, "igv", obj.igv);
            InputParameterAdd.Decimal(objCommand, "total", obj.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", obj.observaciones);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.usuario.idUsuario);
            
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            obj.idCotizacion = (Guid)objCommand.Parameters["@newId"].Value;
        }





        public void InsertCotizacionDetalle(CotizacionDetalle cotizacionDetalle)
        {
            var objCommand = GetSqlCommand("pi_cotizacionDetalle");
            InputParameterAdd.Guid(objCommand, "idCotizacion", cotizacionDetalle.idCotizacion);
            InputParameterAdd.Guid(objCommand, "idProducto", cotizacionDetalle.producto.idProducto);
            InputParameterAdd.Int(objCommand, "cantidad", cotizacionDetalle.cantidad);
            InputParameterAdd.Decimal(objCommand, "precioLista", cotizacionDetalle.precioUnitarioSinIGV);
            InputParameterAdd.Decimal(objCommand, "equivalencia", cotizacionDetalle.producto.equivalencia);
            InputParameterAdd.Varchar(objCommand, "unidad", cotizacionDetalle.unidad);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", cotizacionDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "precioNeto", cotizacionDetalle.precio);
            InputParameterAdd.Int(objCommand, "esPrecioAlternativo", cotizacionDetalle.esPrecioAlternativo?1:0);
            InputParameterAdd.Guid(objCommand, "idUsuario", cotizacionDetalle.usuario.idUsuario);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);

            cotizacionDetalle.idCotizacionDetalle = (Guid)objCommand.Parameters["@newId"].Value;
        }
    }
}
