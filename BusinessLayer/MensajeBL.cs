﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DataLayer;

namespace BusinessLayer
{
    public class MensajeBL
    {



        public Mensaje insertMensaje(Mensaje obj)
        {
            using (var dal = new MensajeDAL())
            {
                return dal.insertMensaje(obj);
            }
        }

        public List<Mensaje> getMensajes(Guid idUsuario)
        {
            using (var dal = new MensajeDAL())
            {
                return dal.getMensajes(idUsuario);
            }

        }

        public List<Mensaje> getListMensajes(Mensaje mensaje)
        {
            using (var dal = new MensajeDAL())
            {
                return dal.getListMensajes(mensaje);
            }

        }
        public void updateMensajeVisto(Mensaje obj)
        {
            using (var dal = new MensajeDAL())
            {
                dal.updateMensajeVisto(obj);
            }
        }

        public Mensaje getMensajeById(Guid idMensaje)
        {
            using (var dal = new MensajeDAL())
            {
                return dal.getMensajeById(idMensaje);
            }

        }


        public Mensaje updateMensaje(Mensaje obj)
        {
            using (var dal = new MensajeDAL())
            {
                return dal.updateMensaje(obj);
            }
        }

        public void MensajeVistoRespuesta(Mensaje obj)
        {
            using (var dal = new MensajeDAL())
            {
                dal.MensajeVistoRespuesta(obj);
            }
        }

        public List<Mensaje> getHiloMensaje(Mensaje obj)
        {
            using (var dal = new MensajeDAL())
            {
               return dal.getHiloMensaje(obj);
            }
        }

        public Mensaje verificarLeido(Guid idMensaje)
        {
            using (var dal = new MensajeDAL())
            {
                return dal.verificarLeido(idMensaje);
            }
        }

        public List<Mensaje> getRespuestasUsuario(Mensaje obj, Guid usuarioSeleccionado)
        {
            using (var dal = new MensajeDAL())
            {
                return dal.getRespuestasUsuario(obj,usuarioSeleccionado);
            }
        }

        public List<Usuario> getUsuariosRespuesta(Mensaje obj)
        {
            using (var dal = new MensajeDAL())
            {
                return dal.getUsuariosRespuesta(obj);
            }
        }

    }
}
