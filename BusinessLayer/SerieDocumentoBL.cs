
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class SerieDocumentoBL
    {
        public List<SerieDocumentoElectronico> getSeriesDocumento(Guid idSede, int idEmpresa)
        {
            using (var dal = new SerieDocumentoDAL())
            {
                return dal.selectSeriesDocumento(idSede, idEmpresa);
            }
        }

        public SerieDocumentoElectronico getSerieDocumento(String tipo, Guid idSede, int idEmpresa)
        {
            using (var dal = new SerieDocumentoDAL())
            {
                return dal.selectSerieDocumento(tipo, idSede, idEmpresa);
            }
        }

    }
}
