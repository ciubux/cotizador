
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class SolicitanteBL
    {
        public List<Solicitante> getSolicitantes(Guid idCliente)
        {
            using (var dal = new SolicitanteDAL())
            {
                return dal.getSolicitantes(idCliente);
            }
        }

        public List<Solicitante> getSolicitantesClienteSunat(int idClienteSunat)
        {
            using (var dal = new SolicitanteDAL())
            {
                return dal.getSolicitantesClienteSunat(idClienteSunat);
            }
        }
    }
}
