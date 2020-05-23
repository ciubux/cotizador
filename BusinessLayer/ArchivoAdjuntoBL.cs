
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using System.Linq;
using ServiceLayer;
using BusinessLayer.Email;

namespace BusinessLayer
{
    public class ArchivoAdjuntoBL
    {
        
        public ArchivoAdjunto GetArchivoAdjunto(ArchivoAdjunto archivoAdjunto)
        {
            using (var dal = new ArchivoDAL())
            {
                archivoAdjunto = dal.SelectArchivoAdjunto(archivoAdjunto);
            }
            return archivoAdjunto;
        }

        public List<ArchivoAdjunto> getListArchivoAdjunto(ArchivoAdjunto archivoAdjunto)
        {
            using (var dal = new ArchivoDAL())
            {
                return dal.getListArchivoAdjunto(archivoAdjunto);
            }            
        }

        public List<ArchivoAdjunto> getListArchivoAdjuntoByIdRegistro(Guid idRegistro)
        {
            using (var dal = new ArchivoDAL())
            {
                return dal.getListArchivoAdjuntoByIdRegistro(idRegistro);
            }
        }
        public void InsertArchivoGenerico(ArchivoAdjunto obj)
        {
            using (var dal = new ArchivoDAL())
            {
                dal.InsertArchivoGenerico(obj);
            }
        }

    }
}
