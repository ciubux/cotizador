using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class OrigenDAL : DaoBase
    {
        public OrigenDAL(IDalSettings settings) : base(settings)
        {
        }
        public OrigenDAL() : this(new CotizadorSettings())
        {
        }



        public Origen getOrigen(int idOrigen)
        {
            var objCommand = GetSqlCommand("ps_origen");
            InputParameterAdd.Int(objCommand, "idOrigen", idOrigen);
            DataTable dataTable = Execute(objCommand);
            Origen obj = new Origen();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idOrigen = Converter.GetInt(row, "id_origen");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");
            }

            return obj;
        }


        public List<Origen> getOrigenes(Origen origen)
        {
            var objCommand = GetSqlCommand("ps_origenes");
            InputParameterAdd.Int(objCommand, "estado", origen.Estado);
            DataTable dataTable = Execute(objCommand);
            List<Origen> lista = new List<Origen>();

            foreach (DataRow row in dataTable.Rows)
            {
                Origen obj = new Origen();
                obj.idOrigen = Converter.GetInt(row, "id_origen");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");
                lista.Add(obj);
            }

            return lista;
        }


        public Origen insertOrigen(Origen obj)
        {
            var objCommand = GetSqlCommand("pi_origen");

            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            OutputParameterAdd.Int(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            obj.idOrigen = (int)objCommand.Parameters["@newId"].Value;

            return obj;

        }


        public Origen updateOrigen(Origen obj)
        {
            var objCommand = GetSqlCommand("pu_origen");
            InputParameterAdd.Int(objCommand, "idOrigen", obj.idOrigen);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            ExecuteNonQuery(objCommand);

            return obj;

        }
    }
}
