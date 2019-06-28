using DataLayer;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
     public class CatalogoBL
    {

        public List<Catalogo> getCatalogoList()
        {
            using (var dal = new CatalogoDAL())
            {
                return dal.getCatalogoList();
            }
        }

        public Catalogo getCatalogoById(Catalogo idCatalogo)
        {
            using (var dal = new CatalogoDAL())
            {
                Catalogo obj = dal.getCatalogo(idCatalogo);

                return obj;
            }
        }


        public Catalogo updateCatalogo(Catalogo idCatalogo)
        {
            using (var dal = new CatalogoDAL())
            {
                Catalogo obj = dal.updateCatalogo(idCatalogo);

                return obj;
            }
        }
    }
}
