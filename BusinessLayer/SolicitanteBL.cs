
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class SolicitanteBL
    {
        public List<Solicitante> getSolicitantes(Guid idCLiente)
        {
            using (var dal = new SolicitanteDAL())
            {
                return dal.getSolicitantes(idCLiente);
            }
        }
    }
}
