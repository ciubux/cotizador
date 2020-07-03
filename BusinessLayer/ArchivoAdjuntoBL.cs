
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using System.Linq;
using ServiceLayer;
using BusinessLayer.Email;
using System.Web;
using static Model.ArchivoAdjunto;

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
        public ArchivoAdjunto InsertArchivoGenerico(ArchivoAdjunto obj)
        {
            using (var dal = new ArchivoDAL())
            {
                return dal.InsertArchivoGenerico(obj);
            }
        }

        public void asociarAchivoRegistro(HttpSessionStateBase session, Guid idResgistro,ArchivoAdjuntoOrigen origen)
        {         
            String origenString= ((ArchivoAdjuntoOrigen)origen).ToString();
            List<ArchivoAdjunto> listArchivos = (List<ArchivoAdjunto>)session ["ARCHIVO_ADJUNTO_EDIT_"+ origenString];

            using (var dal = new ArchivoDAL())
            {
                dal.updateArchivoAdjunto(listArchivos, idResgistro);
            }
            session["ARCHIVO_ADJUNTO_EDIT_" + origenString] = new List<ArchivoAdjunto>();            
        }
        public void limpiarAsociarAchivoRegistro(HttpSessionStateBase session, ArchivoAdjuntoOrigen origen)
        {
            ArchivoAdjuntoBL archivoBL = new ArchivoAdjuntoBL();
            String origenString = ((ArchivoAdjuntoOrigen)origen).ToString();            
            List<ArchivoAdjunto> listArchivos = (List<ArchivoAdjunto>)session ["ARCHIVO_ADJUNTO_EDIT_" + origenString];
            List<ArchivoAdjunto> listArchivosCambiados = (List<ArchivoAdjunto>)session["ARCHIVO_ADJUNTO_CAMBIADOS_" + origen];
            
            foreach (var item in listArchivosCambiados)
            {
                if (item.estado == 1)
                {
                    item.estado = 0;
                }
                else
                {
                    item.estado = 1;
                }
                archivoBL.InsertArchivoGenerico(item);
            } 
            session["ARCHIVO_ADJUNTO_CAMBIADOS_" + origen] = new List<ArchivoAdjunto>();
            session["ARCHIVO_ADJUNTO_EDIT_" + origenString] = new List<ArchivoAdjunto>();
        }       

    }
}
