
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class FacturaBL
    { 

        public void setFacturaStaging(FacturaStaging facturaStaging)
        {
            using (var facturaDAL = new FacturaDAL())
            {
                facturaDAL.setFacturaStaging(facturaStaging);
            }
        }

        public void truncateFacturaStaging()
        {
            using (var facturaDAL = new FacturaDAL())
            {
                facturaDAL.truncateFacturaStaging();
            }
        }

        public void mergeFacturaStaging()
        {
            using (var facturaDAL = new FacturaDAL())
            {
                facturaDAL.mergeFacturaStaging();
            }
        }
    }
}
