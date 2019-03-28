
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class DireccionEntregaBL
    {
        public List<DireccionEntrega> getDireccionesEntrega(Guid idCLiente, String ubigeo = "000000")
        {
            using (var dal = new DireccionEntregaDAL())
            {
                return dal.getDireccionesEntrega(ubigeo, idCLiente);
            }
        }
    }
}
