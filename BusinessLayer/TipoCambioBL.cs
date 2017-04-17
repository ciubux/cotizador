
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class TipoCambioBL
    {
        public TipoCambio getTipoCambio()
        {
            using (var tipoCambioDAL = new TipoCambioDAL())
            {
                return tipoCambioDAL.getTipoCambio();
            }
        }

        
    }
}
