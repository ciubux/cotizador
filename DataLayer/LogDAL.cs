using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.IO;

namespace DataLayer
{
    public class LogDAL : DaoBase
    {
        public LogDAL(IDalSettings settings) : base(settings)
        {
        }

        public LogDAL() : this(new CotizadorSettings())
        {
        }

        public void insertLog(Log log)
        {
            var objCommand = GetSqlCommand("pi_log");
            InputParameterAdd.Varchar(objCommand, "descripcion", log.descripcion); 
            InputParameterAdd.Int(objCommand, "tipo", (int)log.tipo); 
            InputParameterAdd.Guid(objCommand, "idUsuario", log.usuario.idUsuario); 
            ExecuteNonQuery(objCommand);
        }


        public void insertLogWS(string tipo, string envio, string respuesta, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pi_log_ws");
            InputParameterAdd.Varchar(objCommand, "tipo", tipo);
            InputParameterAdd.Varchar(objCommand, "envio", envio);
            InputParameterAdd.Varchar(objCommand, "respuesta", respuesta);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            ExecuteNonQuery(objCommand);
        }

    }
}
