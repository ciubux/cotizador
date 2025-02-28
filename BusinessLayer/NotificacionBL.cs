
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using Model.UTILES;
using NPOI.SS.UserModel;
using System.ServiceModel.Channels;

namespace BusinessLayer
{
    public class NotificacionBL
    {
        public void ProcesarNotificacionesPendeientes()
        {
            NotificacionPendienteDAL dal = new NotificacionPendienteDAL();

            List<NotificacionPendiente> lista = dal.notificacionesPendientes();
            List<Guid> procesadas = new List<Guid>();

            UsuarioDAL usuarioDal = new UsuarioDAL();
            Usuario usuarioZAS = usuarioDal.getUsuario(Constantes.IDUSUARIOZAS);

            foreach (NotificacionPendiente notificacion in lista)
            {
                switch (notificacion.codigo)
                {
                    case NotificacionPendiente.MENSAJE_ZAS_BASE:
                        

                        Mensaje mensaje = new Mensaje();
                        mensaje.titulo = notificacion.datosCorto;
                        mensaje.mensaje = notificacion.datosLargo;

                        mensaje.fechaInicioMensaje = DateTime.Now;
                        mensaje.fechaVencimientoMensaje = DateTime.Now.AddDays(7);
                        mensaje.user = usuarioZAS;
                        mensaje.importancia = "Alta";

                        String[] destinatarios = notificacion.destinatarios.Split('|');
                        mensaje.listUsuario = new List<Usuario>();
                        Usuario dest = new Usuario();    
                        foreach (string item in destinatarios)
                        {
                            dest.idUsuario = Guid.Parse(item);
                            mensaje.listUsuario.Add(dest);
                        }
                        
                        MensajeDAL mensajeDal = new MensajeDAL();
                        mensajeDal.insertMensaje(mensaje);

                        procesadas.Add(notificacion.idNotificacionPendiente);

                        break;
                }
            }

            dal.registrarNotificacionesProcesadas(procesadas, Constantes.IDUSUARIOZAS);
        }

    }
}
