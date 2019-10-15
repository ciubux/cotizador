
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class RubroBL
    {
        public Rubro getRubro(int idRubro)
        {
            using (var dal = new RubroDAL())
            {
                Rubro Rubro = dal.getRubro(idRubro);
                
                return Rubro;
            }
        }

        public List<Rubro> getRubros(Rubro obj)
        {
            using (var dal = new RubroDAL())
            {
                return dal.getRubros(obj);
            }
        }

        public Rubro getRubroById(int idRubro) 
        {
            using (var dal = new RubroDAL())
            {
                Rubro obj = dal.getRubro(idRubro); 

                return obj;
            }
        }

        public String getSiguienteCodigoRubro()
        {
            using (var dal = new RubroDAL())
            {
                string cod = "";
                int cant = dal.getCantidadRubros() + 1;

                cod = cant.ToString();

                while (cod.Length < 3)
                {
                    cod = "0" + cod;
                }

                return cod;
            }
        }

        public Rubro insertRubro(Rubro obj)
        {
            using (var dal = new RubroDAL())
            {
                return dal.insertRubro(obj);
            }
        }

        public Rubro updateRubro(Rubro obj)
        {
            using (var dal = new RubroDAL())
            {
                return dal.updateRubro(obj);
            }
        }
    }
}
