
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

        public int getParametroInt(String codigo)
        {
            int valor = 0;
            using (var dal = new ParametroDAL())
            {
                string param = dal.getParametro(codigo);
                valor = int.Parse(param);
            }

            return valor;
        }

        public List<Parametro> getListParametro(Parametro param)
        {
            using (var dal = new ParametroDAL())
            {
                return dal.getListaParametro(param);
            }
        }

        public void modificarParametro(Parametro param,Usuario user)
        {
            using (var dal = new ParametroDAL())
            {
                dal.modificarParametro(param,user);
            }
        }

        public int GetDataFacturacionEmpresaEOL(string eolId)
        {
            using (var dal = new ParametroDAL())
            {
                return dal.GetDataFacturacionEmpresaEOL(eolId);
            }
        }
    }
}
