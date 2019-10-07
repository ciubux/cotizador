using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DataLayer;

namespace BusinessLayer
{
    public class VendedorBL
    {

        public List<Vendedor> getVendedores(Vendedor obj)
        {
            using (var dal = new VendedorDAL())
            {
                return dal.getVendedores(obj);
            }
        }

        public Vendedor getVendedorById(int idVendedor)
        {
            using (var dal = new VendedorDAL())
            {
                Vendedor obj = dal.getVendedor(idVendedor);

                return obj;
            }
        }
        public Vendedor insertVendedor(Vendedor obj)
        {
            using (var dal = new VendedorDAL())
            {
                return dal.insertVendedor(obj);
            }
        }

        public Vendedor updateVendedor(Vendedor obj)
        {
            using (var dal = new VendedorDAL())
            {
                return dal.updateVendedor(obj);
            }
        }

        public String getVendedoresBusqueda(String textoBusqueda)
        {
            using (var vendedorDAL = new VendedorDAL())
            {
                List<Vendedor> vendedorList = vendedorDAL.searchVendedores(textoBusqueda);
                String resultado = "{\"q\":\"" + textoBusqueda + "\",\"results\":[";
                Boolean existeCliente = false;
                foreach (Vendedor vendedor in vendedorList)
                {
                    resultado += "{\"id\":\"" + vendedor.supervisor.idUsuarioVendedor + "\",\"text\":\"" + "Nombre: " + vendedor.supervisor.descripcion.ToString() + " - Email: " + vendedor.supervisor.email.ToString() + "\"},";
                    existeCliente = true;
                }
                if (existeCliente)
                    resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
                else
                    resultado = resultado.Substring(0, resultado.Length) + "]}";
                return resultado;
            }
        }

        public Vendedor getSupervisor(Guid idSupervisor)
        {
            Vendedor vendedor = null;
            using (var supervisorDAL = new VendedorDAL())
            {
                vendedor = supervisorDAL.getSupervisor(idSupervisor);
            }

            return vendedor;
        }
    }
}
