
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class ClienteContactoBL
    {
        public List<ClienteContacto> getContactos(Guid idCliente)
        {
            using (var dal = new ClienteContactoDAL())
            {
                return dal.getContactos(idCliente);
            }
        }

        public ClienteContacto insert(ClienteContacto obj)
        {
            using (var dal = new ClienteContactoDAL())
            {
                return dal.insert(obj);
            }
        }

        public ClienteContacto update(ClienteContacto obj)
        {
            using (var dal = new ClienteContactoDAL())
            {
                return dal.update(obj);
            }
        }
    }
}
