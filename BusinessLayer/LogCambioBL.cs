﻿using DataLayer;
using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class LogCambioBL
    {

        public bool insertLogCambiosPogramados(List<LogCambio> cambios)
        {
            using (var dal = new LogCambioDAL())
            {
                foreach(LogCambio cambio in cambios)
                {
                    dal.insertLogProgramado(cambio);
                }

                return true;
            }
        }

        public List<LogCambio> getCambiosAplicar()
        {
            using (var dal = new LogCambioDAL())
            {
                return dal.getCambiosAplicar();
            }
        }
        
        public bool traspasarCambios(List<LogCambio> logs)
        {
            List<bool> repiteDato = new List<bool>();

            if (logs.Count <= 0)
            {
                return false;
            }
            
            LogCampoDAL logCampoDal = new LogCampoDAL();
            string tabla = logs.ElementAt<LogCambio>(0).tabla.nombre;
            List<LogCampo> campos = logCampoDal.getCampoLogPorTabla(Producto.NOMBRE_TABLA);
            List<LogCambio> aplicados = new List<LogCambio>();

            switch (tabla)
            {
                case Producto.NOMBRE_TABLA:
                    ProductoDAL prodDal = new ProductoDAL();
                    Guid idRegistro = Guid.Parse(logs.ElementAt<LogCambio>(0).idRegistro);
                    Producto prod = prodDal.GetProductoById(idRegistro);
                    if (prod.idProducto != null && prod.idProducto != Guid.Empty)
                    {
                        aplicados = prod.aplicarCambios(logs);   
                    }
                    prod.CargaMasiva = true;
                    prod.fechaInicioVigencia = logs.ElementAt<LogCambio>(0).fechaInicioVigencia;

                    prod.usuario = new Usuario();
                    prod.usuario.idUsuario = logs.ElementAt<LogCambio>(0).idUsuarioModificacion;

                    prodDal.updateProducto(prod);
                    break;
            }
            

            using (var dal = new LogCambioDAL())
            {
                dal.traspasarCambios(aplicados);

                return true;
            }
        }

        public bool limpiarCambiosProgramados()
        {
            using (var dal = new LogCambioDAL())
            {
                dal.limpiarCambiosProgramados();

                return true;
            }
        }
    }
}
