using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Model;

namespace DataLayer
{
    public class ClienteContactoDAL : DaoBase
    {
        public ClienteContactoDAL(IDalSettings settings) : base(settings)
        {
        }

        public ClienteContactoDAL() : this(new CotizadorSettings())
        {
        }

        public List<ClienteContacto> getContactos(Guid idCliente)
        {
            var objCommand = GetSqlCommand("ps_contactos_cliente");
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente); 

            DataTable dataTable = Execute(objCommand);
            List<ClienteContacto> lista = new List<ClienteContacto>();

            foreach (DataRow row in dataTable.Rows)
            {
                ClienteContacto obj = new ClienteContacto
                {
                    idClienteContacto = Converter.GetGuid(row, "id_cliente_contacto"),
                    idCliente = Converter.GetGuid(row, "id_cliente"),
                    //idClienteSunat = Converter.GetInt(row, "id_solicitante"),
                    nombre = Converter.GetString(row, "nombre"),
                    telefono = Converter.GetString(row, "telefono"),
                    cargo = Converter.GetString(row, "cargo"),
                    correo = Converter.GetString(row, "correo"),
                    tiposDescripcion = Converter.GetString(row, "tipos"),
                    esPrincipal = Converter.GetInt(row, "es_principal"),
                    aplicaRuc = Converter.GetInt(row, "aplica_ruc"),
                };

                if (obj.tiposDescripcion == null || obj.tiposDescripcion.Equals(""))
                {
                    obj.tiposDescripcion = "NO ASIGNADO";
                }

                if (obj.nombre == null) obj.nombre = "";
                if (obj.telefono == null) obj.telefono = "";
                if (obj.cargo == null) obj.cargo = "";
                if (obj.correo == null) obj.correo = "";

                lista.Add(obj);
            }
            return lista;
        }


        public List<ClienteContactoTipo> getContactoTipos(Guid idClienteContacto)
        {
            var objCommand = GetSqlCommand("ps_contacto_cliente_tipos");
            InputParameterAdd.Guid(objCommand, "idClienteContacto", idClienteContacto);

            DataTable dataTable = Execute(objCommand);
            List<ClienteContactoTipo> lista = new List<ClienteContactoTipo>();

            foreach (DataRow row in dataTable.Rows)
            {
                ClienteContactoTipo obj = new ClienteContactoTipo
                {
                    idClienteContactoTipo = Converter.GetGuid(row, "id_cliente_contacto_tipo"),
                    nombre = Converter.GetString(row, "nombre")
                };

                lista.Add(obj);
            }
            return lista;
        }

        public ClienteContacto insert(ClienteContacto obj)
        {
            var objCommand = GetSqlCommand("pi_cliente_contacto");

            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "telefono", obj.telefono);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Varchar(objCommand, "correo", obj.correo);
            InputParameterAdd.Varchar(objCommand, "cargo", obj.cargo);
            InputParameterAdd.Guid(objCommand, "idCliente", obj.idCliente);
            InputParameterAdd.Int(objCommand, "aplicaRuc", obj.aplicaRuc);
            InputParameterAdd.Int(objCommand, "esPrincipal", obj.esPrincipal);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            foreach (ClienteContactoTipo item in obj.tipos)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idClienteContactoTipo;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@tipos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            OutputParameterAdd.UniqueIdentifier(objCommand, "idContactoCliente");

            ExecuteNonQuery(objCommand);

            obj.idClienteContacto = (Guid)objCommand.Parameters["@idContactoCliente"].Value;

            return obj;
        }



        public ClienteContacto update(ClienteContacto obj)
        {
            var objCommand = GetSqlCommand("pu_cliente_contacto");

            InputParameterAdd.Guid(objCommand, "idContactoCliente", obj.idClienteContacto);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "telefono", obj.telefono);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.Varchar(objCommand, "correo", obj.correo);
            InputParameterAdd.Varchar(objCommand, "cargo", obj.cargo);
            InputParameterAdd.Guid(objCommand, "idCliente", obj.idCliente);
            InputParameterAdd.Guid(objCommand, "idClienteVista", obj.idClienteVista);
            InputParameterAdd.Int(objCommand, "aplicaRuc", obj.aplicaRuc);
            InputParameterAdd.Int(objCommand, "esPrincipal", obj.esPrincipal);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            foreach (ClienteContactoTipo item in obj.tipos)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idClienteContactoTipo;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@tipos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            ExecuteNonQuery(objCommand);

            return obj;

        }
    }
}

