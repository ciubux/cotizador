using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;

namespace DataLayer
{
    public class ParametroDAL : DaoBase
    {
        public ParametroDAL(IDalSettings settings) : base(settings)
        {
        }

        public ParametroDAL() : this(new CotizadorSettings())
        {
        }
        
        public String getParametro(String codigo)
        {
            var objCommand = GetSqlCommand("ps_getParametro");
            InputParameterAdd.Varchar(objCommand, "codigo", codigo);
            DataTable dataTable = Execute(objCommand);

            string valor = "";
            foreach (DataRow row in dataTable.Rows)
            {
                valor = Converter.GetString(row, "valor");
            }
            return valor;
        }

        public void updateParametro(String codigo, String valor)
        {
            var objCommand = GetSqlCommand("pu_parametro");
            InputParameterAdd.Varchar(objCommand, "codigo", codigo);
            InputParameterAdd.Varchar(objCommand, "valor", valor);
            ExecuteNonQuery(objCommand);




        }
    }
}
