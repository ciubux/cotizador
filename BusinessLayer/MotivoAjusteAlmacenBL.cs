
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class MotivoAjusteAlmacenBL
    {

        public List<MotivoAjusteAlmacen> getMotivos(MotivoAjusteAlmacen obj)
        {
            using (var dal = new MotivoAjusteAlmacenDAL())
            {
                return dal.getMotivos(obj);
            }
        }
    }
}
