using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class FamiliaDAL : DaoBase
    {
        public FamiliaDAL(IDalSettings settings) : base(settings)
        {
        }

        public FamiliaDAL() : this(new CotizadorSettings())
        {
        }

        public List<Familia> getFamilias()
        {
            var objCommand = GetSqlCommand("ps_getfamilias");
            DataTable dataTable = Execute(objCommand);
            List<Familia> lista = new List<Familia>();

            foreach (DataRow row in dataTable.Rows)
            {
                Familia obj = new Familia
                {
                    nombre = Converter.GetString(row, "familia")
                };
                lista.Add(obj);
            }
            return lista;
        }
    }
}
