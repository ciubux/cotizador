using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class AreaBL
    {
        public List<Area> getAreas(Guid idUsuario, int estado = 1)
        {
            using (AreaDAL dal = new AreaDAL())
            {
                return dal.getAreas(idUsuario, estado);
            }
        }

    }
}
