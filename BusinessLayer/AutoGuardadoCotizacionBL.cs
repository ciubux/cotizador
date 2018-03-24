
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class AutoGuardadoCotizacionBL
    {
    
        public Cotizacion guardar(Cotizacion cotizacion)
        {
            //Se agrega el id de Cotizacion y el usuario para poder recuperarla luego

            cotizacion.autoGuardadoCotizacion.idCotizacion = cotizacion.idCotizacion;
            cotizacion.autoGuardadoCotizacion.IdUsuarioEdicion = cotizacion.usuario.idUsuario;

            //Si no cuenta con ID quiere decir que se está guardando por primera vez
            if (cotizacion.autoGuardadoCotizacion.idAutoGuardadoCotizacion == Guid.Empty)
            {
                cotizacion.autoGuardadoCotizacion = this.insertar(cotizacion.autoGuardadoCotizacion);
            }
            else
            {
                this.actualizar(cotizacion.autoGuardadoCotizacion);
            } 
            return cotizacion;
        }


        private AutoGuardadoCotizacion insertar(AutoGuardadoCotizacion autoGuardadoCotizacion)
        {
            using (var dal = new AutoGuardadoCotizacionDAL())
            {
                autoGuardadoCotizacion = dal.insertar(autoGuardadoCotizacion);
            }
            return autoGuardadoCotizacion;
        }


        private void actualizar(AutoGuardadoCotizacion autoGuardadoCotizacion)
        {
            using (var dal = new AutoGuardadoCotizacionDAL())
            {
                dal.actualizar(autoGuardadoCotizacion);
            }
        }

        public AutoGuardadoCotizacion recuperar(Cotizacion cotizacion)
        {
            cotizacion.autoGuardadoCotizacion.idCotizacion = cotizacion.idCotizacion;
            cotizacion.autoGuardadoCotizacion.IdUsuarioEdicion = cotizacion.usuario.idUsuario;
            using (var dal = new AutoGuardadoCotizacionDAL())
            {
                cotizacion.autoGuardadoCotizacion = dal.recuperar(cotizacion.autoGuardadoCotizacion);
            }
            return cotizacion.autoGuardadoCotizacion;
        }



    }
}
