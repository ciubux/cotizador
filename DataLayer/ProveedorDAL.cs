using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class ProveedorDAL : DaoBase
    {
        public ProveedorDAL(IDalSettings settings) : base(settings)
        {
        }

        public ProveedorDAL() : this(new CotizadorSettings())
        {
        }

        public List<Proveedor> getProveedores()
        {
            var objCommand = GetSqlCommand("ps_getproveedores");
            DataTable dataTable = Execute(objCommand);
            List<Proveedor> lista = new List<Proveedor>();

            foreach (DataRow row in dataTable.Rows)
            {
                Proveedor obj = new Proveedor
                {
                    idProveedor = Converter.GetGuid(row, "id_proveedor"),
                    nombre = Converter.GetString(row, "nombre"),
                    codigo = Converter.GetString(row, "codigo")
                };
                lista.Add(obj);
            }
            return lista;
        }
    }
}
