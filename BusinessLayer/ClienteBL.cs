
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class ClienteBL
    {
        public List<Cliente> getCLientesBusqueda(String textoBusqueda)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                return clienteDAL.getClientesBusqueda(textoBusqueda);
            }
        }

        
    }
}
