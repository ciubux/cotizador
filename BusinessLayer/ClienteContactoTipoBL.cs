
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class ClienteContactoTipoBL
    {
        public List<ClienteContactoTipo> getTipos(int estado)
        {
            using (var dal = new ClienteContactoTipoDAL())
            {
                return dal.getTipos(estado);
            }
        }

        public ClienteContactoTipo insert(ClienteContactoTipo obj)
        {
            using (var dal = new ClienteContactoTipoDAL())
            {
                return dal.insert(obj);
            }
        }

        public ClienteContactoTipo update(ClienteContactoTipo obj)
        {
            using (var dal = new ClienteContactoTipoDAL())
            {
                return dal.update(obj);
            }
        }
    }
}
