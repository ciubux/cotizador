using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class CiudadDAL : DaoBase
    {
        public CiudadDAL(IDalSettings settings) : base(settings)
        {
        }

        public CiudadDAL() : this(new CotizadorSettings())
        {
        }

        public List<Ciudad> getCiudades()
        {
            var objCommand = GetSqlCommand("ps_getCiudades");
            DataTable dataTable = Execute(objCommand);
            List<Ciudad> ciudadList = new List<Ciudad>();

            foreach (DataRow row in dataTable.Rows)
            {
                Ciudad ciudad = new Ciudad
                {
                    idCiudad = Converter.GetGuid(row, "id_ciudad"),
                    nombre = Converter.GetString(row, "nombre"),
                    orden = Converter.GetInt(row, "orden")
                };
                ciudadList.Add(ciudad);
            }
            return ciudadList;
        }
    }
}
