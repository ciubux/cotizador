using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Framework.DAL;
using Model;

namespace DataLayer.DALConverter
{
    public class UbigeoConvertTo : Converter
    {
        public static List<Ubigeo> Ubigeos(DataTable dataTable)
        {
            return dataTable.AsEnumerable().Select(GetUbigeo).ToList();
        }

        private static Ubigeo GetUbigeo(DataRow row)
        {
            return new Ubigeo
            {
                Id = row["id"].ToString(),
                Descripcion = row["descripcion"].ToString()
            };
        }
    }
}
