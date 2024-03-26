using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;

namespace DataLayer
{
    public class AreaDAL : DaoBase
    {
        public AreaDAL(IDalSettings settings) : base(settings)
        {
        }

        public AreaDAL() : this(new CotizadorSettings())
        {
        }

        public List<Area> getAreas(Guid idUsuario, int estado = 1)
        {
            var objCommand = GetSqlCommand("ps_areas");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Int(objCommand, "estado", estado);
            DataTable dataTable = Execute(objCommand);
            List<Area> lista = new List<Area>();

            foreach (DataRow row in dataTable.Rows)
            {
                Area obj = new Area();

                obj.idArea = Converter.GetInt(row, "id_area");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = 1;

                lista.Add(obj);
            }

            return lista;
        }

    }
}
