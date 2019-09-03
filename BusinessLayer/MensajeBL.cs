using System;
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



    }
}
