using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
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

        public List<ClienteContacto> getContactos(Guid idCLiente)
        {
            var objCommand = GetSqlCommand("ps_contactos_cliente");
            InputParameterAdd.Guid(objCommand, "idCliente", idCLiente); 

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
                    esPrincipal = Converter.GetInt(row, "es_principal"),
                    aplicaRuc = Converter.GetInt(row, "aplica_ruc"),
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

            ExecuteNonQuery(objCommand);

            return obj;

        }
    }
}

