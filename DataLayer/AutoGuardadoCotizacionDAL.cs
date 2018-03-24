using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;

namespace DataLayer
{
    public class AutoGuardadoCotizacionDAL : DaoBase
    {
        public AutoGuardadoCotizacionDAL(IDalSettings settings) : base(settings)
        {
        }

        public AutoGuardadoCotizacionDAL() : this(new CotizadorSettings())
        {
        }


        public AutoGuardadoCotizacion insertar(AutoGuardadoCotizacion autoGuardadoCotizacion)
        {
            var objCommand = GetSqlCommand("pi_autoGuardadoCotizacion");
            InputParameterAdd.Guid(objCommand, "idUsuario", autoGuardadoCotizacion.IdUsuarioEdicion);
            InputParameterAdd.Guid(objCommand, "idCotizacion", autoGuardadoCotizacion.idCotizacion);
            InputParameterAdd.Varchar(objCommand, "cotizacionSerializada", autoGuardadoCotizacion.cotizacionSerializada);
            OutputParameterAdd.UniqueIdentifier(objCommand, "idAutoGuardadoCotizacion");
            ExecuteNonQuery(objCommand);
            autoGuardadoCotizacion.idAutoGuardadoCotizacion = (Guid)objCommand.Parameters["@idAutoGuardadoCotizacion"].Value;
            return autoGuardadoCotizacion;
        }

        public void actualizar(AutoGuardadoCotizacion autoGuardadoCotizacion)
        {
            var objCommand = GetSqlCommand("pu_autoGuardadoCotizacion");
            InputParameterAdd.Guid(objCommand, "idUsuario", autoGuardadoCotizacion.IdUsuarioEdicion);
            InputParameterAdd.Guid(objCommand, "idCotizacion", autoGuardadoCotizacion.idCotizacion);
            InputParameterAdd.Varchar(objCommand, "cotizacionSerializada", autoGuardadoCotizacion.cotizacionSerializada);
            InputParameterAdd.Guid(objCommand, "idAutoGuardadoCotizacion", autoGuardadoCotizacion.idAutoGuardadoCotizacion);
            ExecuteNonQuery(objCommand);
        }

        public AutoGuardadoCotizacion recuperar(AutoGuardadoCotizacion autoGuardadoCotizacion)
        {
            var objCommand = GetSqlCommand("ps_autoGuardadoCotizacion");
            InputParameterAdd.Guid(objCommand, "idUsuario", autoGuardadoCotizacion.IdUsuarioEdicion);
            InputParameterAdd.Guid(objCommand, "idCotizacion", autoGuardadoCotizacion.idCotizacion);
            OutputParameterAdd.Varchar(objCommand, "cotizacionSerializada",-1 );
            OutputParameterAdd.UniqueIdentifier(objCommand, "idAutoGuardadoCotizacion");
            ExecuteNonQuery(objCommand);

            //if (objCommand.Parameters["@cotizacionSerializada"].SqlValue.GetType != )
            try
            {
                autoGuardadoCotizacion.cotizacionSerializada = (String)objCommand.Parameters["@cotizacionSerializada"].Value;
                autoGuardadoCotizacion.idAutoGuardadoCotizacion = (Guid)objCommand.Parameters["@idAutoGuardadoCotizacion"].Value;
            }
            catch (Exception e)
            {
                autoGuardadoCotizacion.cotizacionSerializada = null;
                autoGuardadoCotizacion.idAutoGuardadoCotizacion = Guid.Empty;
            }
               
               

            return autoGuardadoCotizacion;
        }





    }
}
