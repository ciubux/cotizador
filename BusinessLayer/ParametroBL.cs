﻿
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class ParametroBL
    { 

        public String getParametro(String codigo)
        {
            using (var dal = new ParametroDAL())
            {
                return dal.getParametro(codigo);
            }
        }


        public void updateParametro(String codigo, String valor)
        {
            using (var dal = new ParametroDAL())
            {
                dal.updateParametro(codigo, valor);
            }
        }

        public Decimal getParametroDecimal(String codigo)
        {
            Decimal valor = 0;
            using (var dal = new ParametroDAL())
            {
                string param =  dal.getParametro(codigo);
                valor = Decimal.Parse(param);
            }

            return valor;
        }
    }
}
