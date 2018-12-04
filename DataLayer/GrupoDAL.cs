using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class GrupoClienteDAL : DaoBase
    {
        public GrupoClienteDAL(IDalSettings settings) : base(settings)
        {
        }

        public GrupoClienteDAL() : this(new CotizadorSettings())
        {
        }

    


        public List<GrupoCliente> getGruposBusqueda(String textoBusqueda)
        {
            var objCommand = GetSqlCommand("ps_getgrupos_search");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            DataTable dataTable = Execute(objCommand);
            List<GrupoCliente> grupoList = new List<GrupoCliente>();

            foreach (DataRow row in dataTable.Rows)
            {
                GrupoCliente grupo = new GrupoCliente
                {
                    idGrupoCliente = Converter.GetInt(row, "id_grupo"),
                    codigo = Converter.GetString(row, "codigo"),
                    nombre = Converter.GetString(row, "nombre"),
                };
                grupoList.Add(grupo);
            }
            return grupoList;
        }

        public GrupoCliente getGrupo(Guid idGrupo)
        {
            var objCommand = GetSqlCommand("ps_getgrupo");
            InputParameterAdd.Guid(objCommand, "idGrupo", idGrupo);
            DataTable dataTable = Execute(objCommand);
            GrupoCliente obj = new GrupoCliente();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idGrupoCliente = Converter.GetInt(row, "id_grupo");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                //obj.contacto = Converter.GetString(row, "contacto");
            }

            return obj;
        }


        public List<GrupoCliente> getGruposCliente()
        {
            var objCommand = GetSqlCommand("ps_gruposCliente");
            //InputParameterAdd.Guid(objCommand, "idGrupo", idGrupo);
            DataTable dataTable = Execute(objCommand);
            List<GrupoCliente> grupoClienteList = new List<GrupoCliente>();

            foreach (DataRow row in dataTable.Rows)
            {
                GrupoCliente grupoCliente = new GrupoCliente();
                grupoCliente.idGrupoCliente = Converter.GetInt(row, "id_grupo_cliente");
                grupoCliente.codigo = Converter.GetString(row, "codigo");
                grupoCliente.nombre = Converter.GetString(row, "nombre");
                grupoClienteList.Add(grupoCliente);
            }

            return grupoClienteList;
        }
    }
}
