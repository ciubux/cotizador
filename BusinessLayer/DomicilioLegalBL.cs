
using DataLayer;
using System.Collections.Generic;
using System;
using System.IO;
using Model;

namespace BusinessLayer
{
    public class DomicilioLegalBL
    {
        public List<DomicilioLegal> getDomiciliosLegalesPorCliente(Cliente cliente)
        {
            using (var domicilioLegalDAL = new DomicilioLegalDAL())
            {
                return domicilioLegalDAL.getDomiciliosLegalesPorCliente(cliente);
            }
        }
    }
}
