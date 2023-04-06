﻿
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class PrecioEspecialBL
    {
        public PrecioEspecialCabecera GetPrecioEspcial(Guid idObj, Guid idUsuario)
        {
            using (var dal = new PrecioEspecialDAL())
            {
                return dal.GetPrecioEspecial(idObj, idUsuario);
            }
        }

        public List<PrecioEspecialCabecera> BuscarCabeceras(PrecioEspecialCabecera obj)
        {
            using (var dal = new PrecioEspecialDAL())
            {
                return dal.BuscarCabeceras(obj);
            }
        }

     

        public PrecioEspecialCabecera InsertarCabecera(PrecioEspecialCabecera obj)
        {
            using (var dal = new PrecioEspecialDAL())
            {
                return dal.InsertarCabecera(obj);
            }
        }

        public PrecioEspecialCabecera ActualizarCabecera(PrecioEspecialCabecera obj)
        {
            using (var dal = new PrecioEspecialDAL())
            {
                return dal.ActualizarCabecera(obj);
            }
        }
    }
}