using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class TipoCambioDAL : DaoBase
    {
        public TipoCambioDAL(IDalSettings settings) : base(settings)
        {
        }

        public TipoCambioDAL() : this(new CotizadorSettings())
        {
        }

        public TipoCambio getTipoCambio()
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

        
    }
}
