
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class DocumentoExternoBL
    {
        public List<DocumentoExterno> getDocumentosRegistro(Guid idUsuario, Guid idRegistro, String tipo)
        {
            using (DocumentoExternoDAL dal = new DocumentoExternoDAL())
            {
                List<DocumentoExterno> lista = dal.getDocumentosRegistro(idUsuario, idRegistro, tipo);
                
                return lista;
            }
        }

        public DocumentoExterno Insertar(DocumentoExterno obj)
        {
            using (DocumentoExternoDAL dal = new DocumentoExternoDAL())
            {
                return dal.Insertar(obj);
            }
        }

    }
}
