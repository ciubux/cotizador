using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class MonedaDAL : DaoBase
    {
        public MonedaDAL(IDalSettings settings) : base(settings)
        {
        }

        public MonedaDAL() : this(new CotizadorSettings())
        {
        }

        public List<Moneda> getMonedas()
        {
            var objCommand = GetSqlCommand("ps_getmonedas");
            DataTable dataTable = Execute(objCommand);
            List<Moneda> lista = new List<Moneda>();

            foreach (DataRow row in dataTable.Rows)
            {
                Moneda obj = new Moneda
                {
                    idMoneda = Converter.GetGuid(row, "id_moneda"),
                    nombre = Converter.GetString(row, "nombre"),
                    simbolo = Converter.GetString(row, "simbolo")
                };
                lista.Add(obj);
            }
            return lista;
        }
    }
}
