
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using Framework.DAL;

namespace BusinessLayer
{
    public class PrecioEspecialBL
    {
        public PrecioEspecialCabecera GetPrecioEspcial(Guid idObj, Guid idUsuario)
        {
            using (PrecioEspecialDAL dal = new PrecioEspecialDAL())
            {
                return dal.GetPrecioEspecial(idObj, idUsuario);
            }
        }

        public List<PrecioEspecialCabecera> BuscarCabeceras(PrecioEspecialCabecera obj)
        {
            using (PrecioEspecialDAL dal = new PrecioEspecialDAL())
            {
                return dal.BuscarCabeceras(obj);
            }
        }

        public List<PrecioEspecialCabecera> BuscarCabecerasDetalles(PrecioEspecialCabecera obj)
        {
            using (PrecioEspecialDAL dal = new PrecioEspecialDAL())
            {
                return dal.BuscarCabecerasDetalles(obj);
            }
        }


        public List<PrecioEspecialDetalle> ValidarDetalles(PrecioEspecialCabecera obj)
        {
            using (PrecioEspecialDAL dal = new PrecioEspecialDAL())
            {
                return dal.ValidarPrecios(obj);
            }
        }

        public PrecioEspecialCabecera InsertarCabecera(PrecioEspecialCabecera obj)
        {
            using (PrecioEspecialDAL dal = new PrecioEspecialDAL())
            {
                return dal.InsertarCabecera(obj);
            }
        }

        public PrecioEspecialCabecera ActualizarCabecera(PrecioEspecialCabecera obj)
        {
            using (PrecioEspecialDAL dal = new PrecioEspecialDAL())
            {
                return dal.ActualizarCabecera(obj);
            }
        }

        public PrecioEspecialDetalle GetCostoEspecialVigente(Guid idCliente, int idGrupo, Guid idProducto, int idEmpresa)
        {
            using (PrecioEspecialDAL dal = new PrecioEspecialDAL())
            {
                return dal.GetCostoEspecialVigente(idCliente, idGrupo, idProducto, idEmpresa);
            }
        }

        public bool ActualizarCostosEspeciales(Guid idUsuario, DateTime fechaInicio, DateTime fechaFin)
        {
            using (PrecioEspecialDAL dal = new PrecioEspecialDAL())
            {
                return dal.ActualizarCostosEspeciales(idUsuario, fechaInicio, fechaFin);
            }
        }
    }
}
