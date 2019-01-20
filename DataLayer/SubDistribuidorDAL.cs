using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class SubDistribuidorDAL : DaoBase
    {
        public SubDistribuidorDAL(IDalSettings settings) : base(settings)
        {
        }

        public SubDistribuidorDAL() : this(new CotizadorSettings())
        {
        }



        public SubDistribuidor getSubDistribuidor(int idSubDistribuidor)
        {
            var objCommand = GetSqlCommand("ps_subdistribuidor");
            InputParameterAdd.Int(objCommand, "idSubDistribuidor", idSubDistribuidor);
            DataTable dataTable = Execute(objCommand);
            SubDistribuidor obj = new SubDistribuidor();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idSubDistribuidor = Converter.GetInt(row, "id_subdistribuidor");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");
            }

            return obj;
        }


        public List<SubDistribuidor> getSubDistribuidores(SubDistribuidor sub)
        {
            var objCommand = GetSqlCommand("ps_subdistribuidores");
            InputParameterAdd.Int(objCommand, "estado", sub.Estado);
            DataTable dataTable = Execute(objCommand);
            List<SubDistribuidor> lista = new List<SubDistribuidor>();

            foreach (DataRow row in dataTable.Rows)
            {
                SubDistribuidor obj = new SubDistribuidor();
                obj.idSubDistribuidor = Converter.GetInt(row, "id_subdistribuidor");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");

                lista.Add(obj);
            }

            return lista;
        }


        public SubDistribuidor insertSubDistribuidor(SubDistribuidor obj)
        {
            var objCommand = GetSqlCommand("pi_subdistribuidor");

            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            OutputParameterAdd.Int(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            obj.idSubDistribuidor = (int)objCommand.Parameters["@newId"].Value;

            return obj;

        }


        public SubDistribuidor updateSubDistribuidor(SubDistribuidor obj)
        {
            var objCommand = GetSqlCommand("pu_subdistribuidor");
            InputParameterAdd.Int(objCommand, "idSubDistribuidor", obj.idSubDistribuidor);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            ExecuteNonQuery(objCommand);

            return obj;

        }
    }
}
