using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class TipoVistaDashboardDAL : DaoBase
    {
        public TipoVistaDashboardDAL(IDalSettings settings) : base(settings)
        {
        }

        public TipoVistaDashboardDAL() : this(new CotizadorSettings())
        {
        }

        public List<TipoVistaDashboard> getTipoVistaDashboard()
        {
            var objCommand = GetSqlCommand("PS_TIPO_VISTA_DASHBOARD");
            DataTable dataTableCiudad = Execute(objCommand);


            List<TipoVistaDashboard> tipoVistaDashboardList = new List<TipoVistaDashboard>();
            foreach (DataRow row in dataTableCiudad.Rows)
            {
                TipoVistaDashboard tipoVistaDashboard = new TipoVistaDashboard
                {
                    idTipoVistaDashboard = Converter.GetInt(row, "id_tipo_vista_dashboard"),
                    nombre = Converter.GetString(row, "nombre"),
                    idTipoPadre = Converter.GetInt(row, "id_tipo_padre"),

                };
                tipoVistaDashboardList.Add(tipoVistaDashboard);
            }

            return tipoVistaDashboardList;
        }
    }
}
