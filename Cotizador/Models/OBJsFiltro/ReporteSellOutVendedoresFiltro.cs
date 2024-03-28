using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

using Model;

namespace Cotizador.Models.OBJsFiltro
{
    public class ReporteSellOutVendedoresFiltro
    {
        public Guid idCiudad { get; set; }

        public Ciudad ciudad { get; set; }
        public String sku { get; set; }
        public String familia { get; set; }
        public String proveedor { get; set; }
        public String ruc { get; set; }

        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }

        public int anio { get; set; }
        public int trimestre { get; set; }

        public int idResponsableComercial { get; set; }
        public int idGrupo { get; set; }

        public int idAsistenteComercial { get; set; }

        public int idSupervisorComercial { get; set; }

        public Guid idUsuarioCreador { get; set; }

        public int incluirVentasExcluidas { get; set; }

        public void changeDatoParametro(string propiedad, string valor, string tipo)
        {
            PropertyInfo propertyInfo = this.GetType().GetProperty(propiedad);

            switch (tipo)
            {
                case "string":
                    propertyInfo.SetValue(this, valor);
                    break;
                case "int":
                    propertyInfo.SetValue(this, int.Parse(valor.Trim().Equals("") ? "0" : valor));
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