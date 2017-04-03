
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class MonedaBL
    {
        public List<Moneda> getMonedas()
        {
            using (var dal = new MonedaDAL())
            {
                return dal.getMonedas();
            }
        }
    }
}
