using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;

namespace DataLayer
{
    public class ArchivoDAL : DaoBase
    {
        public ArchivoDAL(IDalSettings settings) : base(settings)
        {
        }

        public ArchivoDAL() : this(new CotizadorSettings())
        {
        }
        
        public ArchivoAdjunto SelectArchivoAdjunto(ArchivoAdjunto archivoAdjunto)
        {
            var objCommand = GetSqlCommand("ps_archivoAdjunto");
            InputParameterAdd.Guid(objCommand, "idArchivoAdjunto", archivoAdjunto.idArchivoAdjunto);
            DataTable dataTable = Execute(objCommand);

            foreach (DataRow row in dataTable.Rows)
            {
                archivoAdjunto.nombre = Converter.GetString(row, "nombre");
                archivoAdjunto.adjunto = Converter.GetBytes(row, "adjunto");
            }
            return archivoAdjunto;
        }       
         

    }
}
