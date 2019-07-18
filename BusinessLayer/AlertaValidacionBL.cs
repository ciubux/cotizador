
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class AlertaValidacionBL
    {
        

        public List<AlertaValidacion> getAlertasUsuario(Usuario usuario)
        {
            using (var dal = new AlertaValidacionDAL())
            {
                return dal.getAlertasPorUsuario(usuario);
            }
        }


        public bool validarAlerta(Guid idAlertaValidacion, Guid idUsuario)
        {
            using (var dal = new AlertaValidacionDAL())
            {
                return dal.validaAlertaValidacion(idAlertaValidacion, idAlertaValidacion);
            }
        }
    }
}
