
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class ListaPreciosBL
    {
        public ListaPrecios GetListaPrecios(Guid idListaPrecios, Guid idUsuario)
        {
            using (ListaPreciosDAL dal = new ListaPreciosDAL()) 
            {
                return dal.GetListaPrecios(idListaPrecios, idUsuario); 
            }
        }

        public List<ListaPrecios> GetListasPrecios(Guid idUsuario)
        {
            using (ListaPreciosDAL dal = new ListaPreciosDAL()) 
            {
                return dal.GetListasPrecios(idUsuario); 
            }
        }

        public ListaPrecios GuardarListaPrecios(ListaPrecios obj)
        {
            using (ListaPreciosDAL dal = new ListaPreciosDAL()) 
            {
                return dal.GuardarListaPrecios(obj); 
            }
        }

        public void EliminarListaPrecios(Guid idListaPrecios, Guid idUsuario)
        {
            using (ListaPreciosDAL dal = new ListaPreciosDAL()) 
            {
                dal.EliminarListaPrecios(idListaPrecios, idUsuario);
            }
        }
    }
}
