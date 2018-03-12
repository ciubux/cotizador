
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class FamiliaBL
    {
        public List<Familia> getFamilias()
        {
            using (var dal = new FamiliaDAL())
            {
                return dal.getFamilias();
            }
        }
    }
}
