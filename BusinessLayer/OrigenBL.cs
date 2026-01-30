
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class OrigenBL
    {
        public Origen getOrigen(int idOrigen)
        {
            using (OrigenDAL dal = new OrigenDAL())
            {
                Origen origen = dal.getOrigen(idOrigen);
                
                return origen;
            }
        }

        public List<Origen> getOrigenes(Origen obj)
        {
            using (OrigenDAL dal = new OrigenDAL())
            {
                return dal.getOrigenes(obj);
            }
        }

        public Origen getOrigenById(int idOrigen) 
        {
            using (OrigenDAL dal = new OrigenDAL())
            {
                Origen obj = dal.getOrigen(idOrigen); 

                return obj;
            }
        }

        public Origen insertOrigen(Origen obj)
        {
            using (OrigenDAL dal = new OrigenDAL())
            {
                return dal.insertOrigen(obj);
            }
        }

        public Origen updateOrigen(Origen obj)
        {
            using (OrigenDAL dal = new OrigenDAL())
            {
                return dal.updateOrigen(obj);
            }
        }
    }
}
