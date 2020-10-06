using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class RubroDAL : DaoBase
    {
        public RubroDAL(IDalSettings settings) : base(settings)
        {
        }

        public RubroDAL() : this(new CotizadorSettings())
        {
        }



        public Rubro getRubro(int idRubro)
        {
            var objCommand = GetSqlCommand("ps_rubro");
            InputParameterAdd.Int(objCommand, "idRubro", idRubro);
            DataTable dataTable = Execute(objCommand);
            Rubro obj = new Rubro();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idRubro = Converter.GetInt(row, "id_rubro");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");

                int idPadre = Converter.GetInt(row, "id_rubro_padre");

                if (idPadre > 0)
                {
                    obj.padre = this.getRubro(idPadre);
                }
            }

            return obj;
        }


        public int getCantidadRubros()
        {
            var objCommand = GetSqlCommand("ps_contarRubros");
            DataTable dataTable = Execute(objCommand);
            int cantidad = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                cantidad = Converter.GetInt(row, "cantidadRubros");
            }

            return cantidad;
        }


        public List<Rubro> getRubros(Rubro sub, int idPadre = -1)
        {
            var objCommand = GetSqlCommand("ps_rubros");
            InputParameterAdd.Int(objCommand, "estado", sub.Estado);
            InputParameterAdd.Int(objCommand, "idPadre", idPadre);

            DataTable dataTable = Execute(objCommand);
            List<Rubro> lista = new List<Rubro>();

            foreach (DataRow row in dataTable.Rows)
            {
                Rubro obj = new Rubro();
                obj.idRubro = Converter.GetInt(row, "id_rubro");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");

                lista.Add(obj);
            }

            return lista;
        }


        public Rubro insertRubro(Rubro obj)
        {
            var objCommand = GetSqlCommand("pi_rubro");

            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            OutputParameterAdd.Int(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            obj.idRubro = (int)objCommand.Parameters["@newId"].Value;

            return obj;

        }


        public Rubro updateRubro(Rubro obj)
        {
            var objCommand = GetSqlCommand("pu_rubro");
            InputParameterAdd.Int(objCommand, "idRubro", obj.idRubro);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            ExecuteNonQuery(objCommand);

            return obj;

        }
    }
}
