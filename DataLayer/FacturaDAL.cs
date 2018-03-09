using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class FacturaDAL : DaoBase
    {
        public FacturaDAL(IDalSettings settings) : base(settings)
        {
        }

        public FacturaDAL() : this(new CotizadorSettings())
        {
        }

        public void setFacturaStaging(FacturaStaging facturaStaging)
        {
            var objCommand = GetSqlCommand("pi_facturaStaging");
            InputParameterAdd.Int(objCommand, "numero", facturaStaging.numero);
            InputParameterAdd.Char(objCommand, "sede", facturaStaging.sede);
            InputParameterAdd.Varchar(objCommand, "tipoDocumento", facturaStaging.tipoDocumento);
            InputParameterAdd.Int(objCommand, "numeroDocumento", facturaStaging.numeroDocumento);
            InputParameterAdd.DateTime(objCommand, "fecha", facturaStaging.fecha);
            InputParameterAdd.Varchar(objCommand, "codigoCliente", facturaStaging.codigoCliente);
            InputParameterAdd.Varchar(objCommand, "ruc", facturaStaging.ruc);
            InputParameterAdd.Varchar(objCommand, "razonSocial", facturaStaging.razonSocial);
            InputParameterAdd.Decimal(objCommand, "valorVenta", facturaStaging.valorVenta);
            InputParameterAdd.Decimal(objCommand, "igv", facturaStaging.igv);
            InputParameterAdd.Decimal(objCommand, "total", facturaStaging.total);
            InputParameterAdd.Varchar(objCommand, "observacion", facturaStaging.observacion);
            InputParameterAdd.DateTime(objCommand, "fechaVencimiento", facturaStaging.fechaVencimiento);
            ExecuteNonQuery(objCommand);
        }


        public void truncateFacturaStaging()
        {
            var objCommand = GetSqlCommand("pt_facturaStaging");
            ExecuteNonQuery(objCommand);
        }

        public void mergeFacturaStaging()
        {
            var objCommand = GetSqlCommand("pu_facturaStaging");
            ExecuteNonQuery(objCommand);
        }


    }
}
