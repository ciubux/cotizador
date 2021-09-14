using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class TipoCambioSunatDAL : DaoBase
    {
        public TipoCambioSunatDAL(IDalSettings settings) : base(settings)
        {
        }

        public TipoCambioSunatDAL() : this(new CotizadorSettings())
        {
        }

        public TipoCambio getTipoCambioSunat()
        {
            var objCommand = GetSqlCommand("ps_gettipocambio");
            DataTable dataTable = Execute(objCommand);
            TipoCambio tipoCambio = null;
            foreach (DataRow row in dataTable.Rows)
            {
                tipoCambio = new TipoCambio
                {
                    idTipoCambio = Converter.GetInt(row, "id_tipo_cambio"),
                    valor = Converter.GetDecimal(row, "valor")
                };
            }
            return tipoCambio;
        }

        public TipoCambioSunat Insertar(TipoCambioSunat obj)
        {
            var objCommand = GetSqlCommand("pi_tipocambiosunat");

            InputParameterAdd.DateTime(objCommand, "fecha", obj.fecha);
            InputParameterAdd.Varchar(objCommand, "codigoMoneda", obj.codigoMoneda);
            InputParameterAdd.Decimal(objCommand, "valorCompra", obj.valorSunatCompra);
            InputParameterAdd.Decimal(objCommand, "valorVenta", obj.valorSunatVenta);

            OutputParameterAdd.Int(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            obj.idTipoCambioSunat = (int)objCommand.Parameters["@newId"].Value;

            return obj;
        }

        public TipoCambioSunat GetTipoCambioHoy()
        {
            var objCommand = GetSqlCommand("ps_getLastTipoCambioSunat");
            DataTable dataTable = Execute(objCommand);
            TipoCambioSunat tipoCambio = null;
            foreach (DataRow row in dataTable.Rows)
            {
                tipoCambio = new TipoCambioSunat
                {
                    idTipoCambioSunat = Converter.GetInt(row, "id_tipo_cambio_sunat"),
                    valorSunatCompra = Converter.GetDecimal(row, "valor_sunat_compra"),
                    valorSunatVenta = Converter.GetDecimal(row, "valor_sunat_venta"),
                    fecha = Converter.GetDateTime(row, "fecha"),
                    codigoMoneda = Converter.GetString(row, "codigo_moneda")
                };
            }
            return tipoCambio;
        }
    }
}
