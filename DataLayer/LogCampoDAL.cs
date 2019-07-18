using DataLayer.DALConverter;
using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class LogCampoDAL : DaoBase
    {
        public LogCampoDAL(IDalSettings settings) : base(settings)
        {
        }

        public LogCampoDAL() : this(new CotizadorSettings())
        {
        }

        public List<LogCampo> getCampoLogPorTabla(String nombreTabla)
        {

            var objCommand = GetSqlCommand("ps_campos_log");
            InputParameterAdd.Varchar(objCommand, "nombreCatalogoTabla", nombreTabla);
            DataTable dataTable = Execute(objCommand);
            List<LogCampo> lista = new List<LogCampo>();

            foreach (DataRow row in dataTable.Rows)
            {
                LogCampo obj = new LogCampo();
                obj.idCampo = Converter.GetInt(row, "id_catalogo_campo");
                obj.idTabla = Converter.GetInt(row, "id_catalogo_Tabla");
                obj.nombre = Converter.GetString(row, "campo");
                obj.puedePersistir = Converter.GetInt(row, "puede_persistir") == 1 ? true : false;

                lista.Add(obj);
            }

            return lista;
        }

        
    }
}

