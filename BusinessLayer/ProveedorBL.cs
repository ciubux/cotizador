
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class ProveedorBL
    {
        public List<Proveedor> getProveedores()
        {
            using (var dal = new ProveedorDAL())
            {
                return dal.getProveedores();
            }
        }
    }
}
