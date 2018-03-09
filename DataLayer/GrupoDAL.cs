using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class GrupoDAL : DaoBase
    {
        public GrupoDAL(IDalSettings settings) : base(settings)
        {
        }

        public GrupoDAL() : this(new CotizadorSettings())
        {
        }

    


        public List<Grupo> getGruposBusqueda(String textoBusqueda)
        {
            var objCommand = GetSqlCommand("ps_getgrupos_search");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            DataTable dataTable = Execute(objCommand);
            List<Grupo> grupoList = new List<Grupo>();

            foreach (DataRow row in dataTable.Rows)
            {
                Grupo grupo = new Grupo
                {
                    idGrupo = Converter.GetGuid(row, "id_grupo"),
                    codigo = Converter.GetString(row, "codigo"),
                    nombre = Converter.GetString(row, "nombre"),
                };
                grupoList.Add(grupo);
            }
            return grupoList;
        }

        public Grupo getGrupo(Guid idGrupo)
        {
            var objCommand = GetSqlCommand("ps_getgrupo");
            InputParameterAdd.Guid(objCommand, "idGrupo", idGrupo);
            DataTable dataTable = Execute(objCommand);
            Grupo obj = new Grupo();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idGrupo = Converter.GetGuid(row, "id_grupo");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.contacto = Converter.GetString(row, "contacto");
            }

            return obj;
        }
    }
}
