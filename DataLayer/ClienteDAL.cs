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

        public List<Cliente> getClientesBusqueda(String textoBusqueda)
        {
            var objCommand = GetSqlCommand("ps_getclientes_search");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            DataTable dataTable = Execute(objCommand);
            List<Cliente> clienteList = new List<Cliente>();

            foreach (DataRow row in dataTable.Rows)
            {
                Cliente cliente = new Cliente
                {
                    idCliente = Converter.GetGuid(row, "id_cliente"),
                    codigo = Converter.GetString(row, "nombre"),
                    razonSocial = Converter.GetString(row, "razon_social"),
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
    }
}
