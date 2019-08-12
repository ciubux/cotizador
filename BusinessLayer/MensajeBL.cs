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

        public void updateMensaje(Mensaje obj)
        {
            using (var dal = new MensajeDAL())
            {
                dal.updateMensaje(obj);
            }
        }

    }
}
