using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class ClienteDAL : DaoBase
    {
        public ClienteDAL(IDalSettings settings) : base(settings)
        {
        }

        public ClienteDAL() : this(new CotizadorSettings())
        {
        }

        public void setClienteStaging(ClienteStaging clienteStaging)
        {
            var objCommand = GetSqlCommand("pi_clienteStaging");
            InputParameterAdd.Varchar(objCommand, "PlazaId", clienteStaging.PlazaId);
            InputParameterAdd.Varchar(objCommand, "Plaza", clienteStaging.Plaza);
            InputParameterAdd.Varchar(objCommand, "Id", clienteStaging.codigo);
            InputParameterAdd.Varchar(objCommand, "nombre", clienteStaging.nombre);
            InputParameterAdd.Varchar(objCommand, "documento", clienteStaging.documento);
            InputParameterAdd.Varchar(objCommand, "codVe", clienteStaging.codVe);
            InputParameterAdd.Varchar(objCommand, "nombreComercial", clienteStaging.nombreComercial);
            InputParameterAdd.Varchar(objCommand, "domicilioLegal", clienteStaging.domicilioLegal);
            InputParameterAdd.Varchar(objCommand, "distrito", clienteStaging.distrito);
            InputParameterAdd.Varchar(objCommand, "direccionDespacho", clienteStaging.direccionDespacho);
            InputParameterAdd.Varchar(objCommand, "distritoDespacho", clienteStaging.distritoDespacho);
            InputParameterAdd.Varchar(objCommand, "rubro", clienteStaging.rubro);
            InputParameterAdd.Char(objCommand, "sede", clienteStaging.sede);
            ExecuteNonQuery(objCommand);
            
        }


        public void truncateClienteStaging(String sede)
        {
            var objCommand = GetSqlCommand("pt_clienteStaging");
            InputParameterAdd.Char(objCommand, "sede", sede);
            ExecuteNonQuery(objCommand);
        }

        public void mergeClienteStaging()
        {
            var objCommand = GetSqlCommand("pu_clienteStaging");
            ExecuteNonQuery(objCommand);
        }


        public List<Cliente> getClientesBusqueda(String textoBusqueda, Guid idCiudad)
        {
            var objCommand = GetSqlCommand("ps_getclientes_search");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            
            DataTable dataTable = Execute(objCommand);
            List<Cliente> clienteList = new List<Cliente>();

            foreach (DataRow row in dataTable.Rows)
            {
                Cliente cliente = new Cliente
                {
                    idCliente = Converter.GetGuid(row, "id_cliente"),
                    codigo = Converter.GetString(row, "codigo"),
                    razonSocial = Converter.GetString(row, "razon_social"),
                    nombreComercial = Converter.GetString(row, "nombre_comercial"),
                    ruc = Converter.GetString(row, "ruc"),
                    contacto1 = Converter.GetString(row, "contacto1"),
                    contacto2 = Converter.GetString(row, "contacto2")
                };
                clienteList.Add(cliente);
            }
            return clienteList;
        }

        public Cliente getCliente(Guid idCliente)
        {
            var objCommand = GetSqlCommand("ps_getcliente");
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            DataTable dataTable = Execute(objCommand);
            Cliente obj = new Cliente();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idCliente = Converter.GetGuid(row, "id_cliente");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.razonSocial = Converter.GetString(row, "razon_social");
                obj.ruc = Converter.GetString(row, "ruc");
                obj.contacto1 = Converter.GetString(row, "contacto1");
                obj.contacto2 = Converter.GetString(row, "contacto2");
            }

            return obj;
        }

        public Cliente insertCliente(Cliente cliente)
        {
            var objCommand = GetSqlCommand("pi_cliente");

            InputParameterAdd.Guid(objCommand, "idUsuario", cliente.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "razonSocial", cliente.razonSocial);
            InputParameterAdd.Varchar(objCommand, "nombreComercial", cliente.nombreComercial);
            InputParameterAdd.Varchar(objCommand, "ruc", cliente.ruc);
            InputParameterAdd.Varchar(objCommand, "contacto1", cliente.contacto1);
            InputParameterAdd.Guid(objCommand, "idCiudad", cliente.ciudad.idCiudad);

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            OutputParameterAdd.Int(objCommand, "codigoAlterno");
            ExecuteNonQuery(objCommand);

            cliente.idCliente = (Guid)objCommand.Parameters["@newId"].Value;
            cliente.codigoAlterno = (Int32)objCommand.Parameters["@codigoAlterno"].Value;

            return cliente;

        }
    }
}
