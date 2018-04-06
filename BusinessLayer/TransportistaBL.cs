
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class TransportistaBL
    {
        public List<Transportista> getTransportistas(Guid idCiudad)
        {
            using (var dal = new TransportistaDAL())
            {
                return dal.getTransportistas(idCiudad);
            }
        }
    }
}
