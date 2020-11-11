using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;

using Model;

namespace DataLayer
{
    public class ClienteContactoTipoDAL : DaoBase
    {
        public ClienteContactoTipoDAL(IDalSettings settings) : base(settings)
        {
        }

        public ClienteContactoTipoDAL() : this(new CotizadorSettings())
        {
        }

        public List<ClienteContactoTipo> getTipos(int estado)
        {
            var objCommand = GetSqlCommand("ps_cliente_contacto_tipos");
            InputParameterAdd.Int(objCommand, "estado", estado); 

            DataTable dataTable = Execute(objCommand);
            List<ClienteContactoTipo> lista = new List<ClienteContactoTipo>();

            foreach (DataRow row in dataTable.Rows)
            {
                ClienteContactoTipo obj = new ClienteContactoTipo
                {
                    idClienteContactoTipo = Converter.GetGuid(row, "id_cliente_contacto_tipo"),
                    nombre = Converter.GetString(row, "nombre"),
                    descripcion = Converter.GetString(row, "descripcion"),
                    Estado = Converter.GetInt(row, "estado")
                };
                lista.Add(obj);
            }
            return lista;
        }

        public ClienteContactoTipo insert(ClienteContactoTipo obj)
        {
            var objCommand = GetSqlCommand("pi_cliente_contacto_tipo");

            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Varchar(objCommand, "descripcion", obj.descripcion);

            OutputParameterAdd.UniqueIdentifier(objCommand, "idClienteContactoTipo");

            ExecuteNonQuery(objCommand);

            obj.idClienteContactoTipo = (Guid)objCommand.Parameters["@idClienteContactoTipo"].Value;

            return obj;
        }



        public ClienteContactoTipo update(ClienteContactoTipo obj)
        {
            var objCommand = GetSqlCommand("pu_cliente_contacto_tipo");

            InputParameterAdd.Guid(objCommand, "idClienteContactoTipo", obj.idClienteContactoTipo);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Varchar(objCommand, "descripcion", obj.descripcion);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            ExecuteNonQuery(objCommand);

            return obj;

        }
    }
}

