using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Model.MASTERS
{
    public static class HelperMaster
    {
        public static void ChangePropertyData(this object obj, string propiedad, string valor, string tipo)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propiedad);

            switch (tipo)
            {
                case "string":
                    propertyInfo.SetValue(obj, valor);
                    break;
                case "date":
                    if (!valor.Trim().Equals(""))
                    {
                        String[] fecha = valor.Split('/');
                        propertyInfo.SetValue(obj, new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0])));
                    }
                    break;
                case "guid":
                    if (!valor.Trim().Equals(""))
                    {
                        propertyInfo.SetValue(obj, Guid.Parse(valor));
                    }
                    break;
            }
        }
    }
}
