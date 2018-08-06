﻿
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class SerieDocumentoBL
    {
        public List<SerieDocumentoElectronico> getSeriesDocumento(Guid idSede)
        {
            using (var dal = new SerieDocumentoDAL())
            {
                return dal.selectSeriesDocumento(idSede);
            }
        }
    }
}
