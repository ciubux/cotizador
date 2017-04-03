
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class CategoriaBL
    {
        public List<Categoria> getCategorias()
        {
            using (var dal = new CategoriaDAL())
            {
                return dal.getCategorias();
            }
        }
    }
}
