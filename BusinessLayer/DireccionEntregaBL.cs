
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class DireccionEntregaBL
    {
        public List<DireccionEntrega> getDireccionesEntrega(Guid idClienteSunat, string ubigeo = "000000")
        {
            return new List<DireccionEntrega>();
        }


        public List<DireccionEntrega> getDireccionesEntrega(int idClienteSunat)
        {
            using (var dal = new DireccionEntregaDAL())
            {
                return dal.getDireccionesEntrega(idClienteSunat);
            }
        }

        public List<DireccionEntrega> getDireccionesAcopio(int idClienteSunat)
        {
            using (var dal = new DireccionEntregaDAL())
            {
                return dal.getDireccionesAcopio(idClienteSunat);
            }
        }

        public DireccionEntrega getDireccionEntregaPorCodigo(int codigo)
        {
            using (var dal = new DireccionEntregaDAL())
            {
                return dal.getDireccionEntregaPorCodigo(codigo);
            }
        }

        public void deleteDireccionEntrega(DireccionEntrega direccion)
        {
            using (var dal = new DireccionEntregaDAL())
            {
                dal.deleteDireccionEntrega(direccion);
            }
        }

        public void updateDireccionEntrega(DireccionEntrega direccion)
        {
            using (var dal = new DireccionEntregaDAL())
            {
                dal.updateDireccionEntrega(direccion);
            }
        }

        public void insertDireccionEntrega(DireccionEntrega direccion)
        {
            using (var dal = new DireccionEntregaDAL())
            {
                dal.insertDireccionEntrega(direccion);
            }
        }
    }
}
