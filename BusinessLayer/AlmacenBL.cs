
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class AlmacenBL
    {
        public List<Almacen> getAlmacenesSedes(Guid idCiudad, Guid idUsuario)
        {
            using (var dal = new AlmacenDAL())
            {
                return dal.getAlmacenesSedes(idCiudad, idUsuario);
            }
        }

    }
}
