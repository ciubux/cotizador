
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class FabricanteBL
    {
       
        public List<Fabricante> Listar(Guid idUsuario, int estado)
        {
            using (var dal = new FabricanteDAL())
            {
                return dal.Listar(idUsuario, estado);
            }
        }

        public Fabricante Insert(Fabricante obj)
        {
            using (var dal = new FabricanteDAL())
            {
                return dal.Insert(obj);
            }
        }

        public Fabricante Update(Fabricante obj)
        {
            using (var dal = new FabricanteDAL())
            {
                return dal.Update(obj);
            }
        }
    }
}
