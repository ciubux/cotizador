
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class ProveedorBL
    {
        public List<Proveedor> getProveedores()
        {
            using (var dal = new ProveedorDAL())
            {
                return dal.getProveedores();
            }
        }


        public String getProveedoresBusqueda(String textoBusqueda, Guid idCiudad)
        {
            using (var proveedorDAL = new ProveedorDAL())
            {
                List<Proveedor> proveedorList = proveedorDAL.getProveedoresBusqueda(textoBusqueda, idCiudad);
                String resultado = "{\"q\":\"" + textoBusqueda + "\",\"results\":[";
                Boolean existeProveedor = false;
                foreach (Proveedor proveedor in proveedorList)
                {
                    resultado += "{\"id\":\"" + proveedor.idProveedor + "\",\"text\":\"" + proveedor.ToString() + "\"},";
                    existeProveedor = true;
                }
                if (existeProveedor)
                    resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
                else
                    resultado = resultado.Substring(0, resultado.Length) + "]}";
                return resultado;
            }
        }
        

        public Proveedor getProveedor(Guid idProveedor)
        {
            using (var proveedorDAL = new ProveedorDAL())
            {
                return proveedorDAL.getProveedor(idProveedor);
            }
        }
    }
}
