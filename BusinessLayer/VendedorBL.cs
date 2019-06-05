using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DataLayer;

namespace BusinessLayer
{
    public class VendedorBL
    {

        public List<Vendedor> getVendedores(Vendedor obj)
        {
            using (var dal = new VendedorDAL())
            {
                return dal.getVendedores(obj);
            }
        }

        public Vendedor getVendedorById(int idVendedor)
        {
            using (var dal = new VendedorDAL())
            {
                Vendedor obj = dal.getVendedor(idVendedor);

                return obj;
            }
        }
        public Vendedor insertVendedor(Vendedor obj)
        {
            using (var dal = new VendedorDAL())
            {
                return dal.insertVendedor(obj);
            }
        }

        public Vendedor updateVendedor(Vendedor obj)
        {
            using (var dal = new VendedorDAL())
            {
                return dal.updateVendedor(obj);
            }
        }
    }
}
