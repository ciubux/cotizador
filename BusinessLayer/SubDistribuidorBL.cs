
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class SubDistribuidorBL
    {
        public SubDistribuidor getSubDistribuidor(int idSubDistribuidor)
        {
            using (var dal = new SubDistribuidorDAL())
            {
                SubDistribuidor SubDistribuidor = dal.getSubDistribuidor(idSubDistribuidor);
                
                return SubDistribuidor;
            }
        }

        public List<SubDistribuidor> getSubDistribuidores(SubDistribuidor obj)
        {
            using (var dal = new SubDistribuidorDAL())
            {
                return dal.getSubDistribuidores(obj);
            }
        }

        public SubDistribuidor getSubDistribuidorById(int idSubDistribuidor) 
        {
            using (var dal = new SubDistribuidorDAL())
            {
                SubDistribuidor obj = dal.getSubDistribuidor(idSubDistribuidor); 

                return obj;
            }
        }

        public SubDistribuidor insertSubDistribuidor(SubDistribuidor obj)
        {
            using (var dal = new SubDistribuidorDAL())
            {
                return dal.insertSubDistribuidor(obj);
            }
        }

        public SubDistribuidor updateSubDistribuidor(SubDistribuidor obj)
        {
            using (var dal = new SubDistribuidorDAL())
            {
                return dal.updateSubDistribuidor(obj);
            }
        }
    }
}
