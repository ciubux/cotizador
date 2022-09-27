﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using Model;

namespace Cotizador.Models.OBJsFiltro
{
    public class ReportePendientesAtencionFiltro
    {
        public Guid idCiudad { get; set; }

        public Ciudad ciudad { get; set; }
        public String descripcion { get; set; }
        public String sku { get; set; }
        public String familia { get; set; }
        public String proveedor { get; set; }

        public DateTime fechaEntregaInicio { get; set; }
        public DateTime fechaEntregaFin { get; set; }

        public int idProductoPresentacion { get; set; }

        public void changeDatoParametro(string propiedad, string valor, string tipo)
        {
            PropertyInfo propertyInfo = this.GetType().GetProperty(propiedad);

            switch (tipo)
            {
                case "string":
                    propertyInfo.SetValue(this, valor);
                    break;
                case "int":
                    propertyInfo.SetValue(this, int.Parse(valor));
                    break;
                case "date":
                    if (!valor.Trim().Equals(""))
                    {
                        String[] fecha = valor.Split('/');
                        propertyInfo.SetValue(this, new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0])));
                    }
                    break;
                case "guid":
                    Guid idGuid = Guid.Parse(valor);
                    if (!valor.Trim().Equals(""))
                    {
                        propertyInfo.SetValue(this, idGuid);
                    }

                    break;
            }
        }
    }
    
}