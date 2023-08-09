
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class LogBL
    {
     
        public void insertLog(Log log)
        {
            using (var logDAL = new LogDAL())
            {
                logDAL.insertLog(log);
            }
        }

        public void insertLogWS(string tipo, string envio, string respuesta, Guid idUsuario)
        {
            using (var logDAL = new LogDAL())
            {
                logDAL.insertLogWS(tipo, envio, respuesta, idUsuario);
            }
        }
    }
}
