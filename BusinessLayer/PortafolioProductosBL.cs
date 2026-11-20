
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using Model.UTILES;
using NPOI.SS.UserModel;

namespace BusinessLayer
{
    public class PortafolioProductosBL
    {
        public PortafolioProductos getPortafolioProductos(Guid idPortafolio)
        {
            using (PortafolioProductosDAL dal = new PortafolioProductosDAL())
            {
                return dal.getPortafolioProductos(idPortafolio);
            }
        }

        public List<PortafolioProductos> getPortafoliosProductos(string nombre, int estado)
        {
            using (PortafolioProductosDAL dal = new PortafolioProductosDAL())
            {
                return dal.getPortafoliosProductos(nombre, estado);
            }
        }

        public PortafolioProductos insertOrUpdatePortafolioProductos(PortafolioProductos obj, List<string> skus, Guid idUsuario)
        {
            using (PortafolioProductosDAL dal = new PortafolioProductosDAL())
            {
                return dal.insertOrUpdatePortafolioProductos(obj, skus, idUsuario);
            }
        }

        public void updatePortafolioProductosUsuarios(Guid idPortafolio, Guid idUsuario, List<Guid> idsUsuarios)
        {
            using (PortafolioProductosDAL dal = new PortafolioProductosDAL())
            {
                dal.updatePortafolioProductosUsuarios(idPortafolio, idUsuario, idsUsuarios);
            }
        }

        public void updatePortafolioProductosRoles(Guid idPortafolio, Guid idUsuario, List<int> idsRoles)
        {
            using (PortafolioProductosDAL dal = new PortafolioProductosDAL())
            {
                dal.updatePortafolioProductosRoles(idPortafolio, idUsuario, idsRoles);
            }
        }
    }
}