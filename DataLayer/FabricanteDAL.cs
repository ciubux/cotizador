using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;

namespace DataLayer
{
    public class FabricanteDAL : DaoBase
    {
        public FabricanteDAL(IDalSettings settings) : base(settings)
        {
        }

        public FabricanteDAL() : this(new CotizadorSettings())
        {
        }

        public List<Fabricante> Listar(Guid idUsuario, int estado = 1)
        {
            var objCommand = GetSqlCommand("ps_fabricantes");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Int(objCommand, "estado", estado);
            DataTable dataTable = Execute(objCommand);
            List<Fabricante> lista = new List<Fabricante>();

            foreach (DataRow row in dataTable.Rows)
            {
                Fabricante obj = new Fabricante();

                obj.idFabricante = Converter.GetInt(row, "id_fabricante");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombreUsual = Converter.GetString(row, "nombre_usual");
                obj.Estado = 1;

                lista.Add(obj);
            }

            return lista;
        }

        public Fabricante Insert(Fabricante obj)
        {
            var objCommand = GetSqlCommand("pi_fabricante");

            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Varchar(objCommand, "nombreUsual", obj.nombreUsual);

            OutputParameterAdd.Int(objCommand, "idFabricante");

            ExecuteNonQuery(objCommand);

            obj.idFabricante = (int)objCommand.Parameters["@idFabricante"].Value;

            return obj;

        }

        public Fabricante Update(Fabricante obj)
        {
            var objCommand = GetSqlCommand("pu_fabricante");
            InputParameterAdd.Int(objCommand, "idFabricante", obj.idFabricante);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "nombreUsual", obj.nombreUsual);

            ExecuteNonQuery(objCommand);

            return obj;
        }
    }
}

