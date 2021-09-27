
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class PromocionBL
    {
        public Promocion getPromocion(Guid idPromocion)
        {
            using (PromocionDAL dal = new PromocionDAL())
            {
                Promocion obj = dal.getPromocion(idPromocion);
                
                return obj;
            }
        }

        public List<Promocion> getPromociones(Promocion obj)
        {
            using (PromocionDAL dal = new PromocionDAL())
            {
                return dal.getPromociones(obj);
            }
        }

        public Promocion insertar(Promocion obj)
        {
            using (PromocionDAL dal = new PromocionDAL())
            {
                return dal.insertPromocion(obj);
            }
        }

        public Promocion editar(Promocion obj)
        {
            using (PromocionDAL dal = new PromocionDAL())
            {
                return dal.updatePromocion(obj);
            }
        }
    }
}
