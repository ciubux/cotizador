using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;

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

        public List<ArchivoAdjunto> getListArchivoAdjunto(ArchivoAdjunto archivoAdjunto)
        {
            var objCommand = GetSqlCommand("ps_archivos_adjuntos");           
            InputParameterAdd.VarcharEmpty(objCommand, "nombre", archivoAdjunto.nombre);           
            InputParameterAdd.Varchar(objCommand, "origen", (archivoAdjunto.origenBusqueda).ToString());
            DataTable dataTable = Execute(objCommand);
            List<ArchivoAdjunto> lista = new List<ArchivoAdjunto>();
            foreach (DataRow row in dataTable.Rows)
            {
                ArchivoAdjunto obj = new ArchivoAdjunto();
                obj.usuario= new Usuario();
                obj.idArchivoAdjunto = Converter.GetGuid(row, "id_archivo_adjunto");
                obj.nombre = Converter.GetString(row, "nombre_archivo");
                obj.fechaCreacion = Converter.GetDateTime(row, "fecha_creacion");
                obj.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                lista.Add(obj);
            }
            return lista;
        }

        public List<ArchivoAdjunto> getListArchivoAdjuntoByIdRegistro(Guid idRegistro)
        {
            var objCommand = GetSqlCommand("ps_adjuntos_id_registro");           
            InputParameterAdd.Guid(objCommand, "id_registro", idRegistro);
            DataTable dataTable = Execute(objCommand);
            List<ArchivoAdjunto> lista = new List<ArchivoAdjunto>();
            foreach (DataRow row in dataTable.Rows)
            {
                ArchivoAdjunto obj = new ArchivoAdjunto();
                obj.idArchivoAdjunto = Converter.GetGuid(row, "id_archivo_adjunto");
                obj.adjunto = Converter.GetBytes(row, "adjunto");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.idCliente = Converter.GetGuid(row, "id_cliente");
                lista.Add(obj);
            }
            return lista;
        }

        public void InsertArchivoGenerico(ArchivoAdjunto obj)
        {           
            var objCommand = GetSqlCommand("pi_archivo_adjunto");
            InputParameterAdd.Guid(objCommand, "id_registro", obj.idRegistro);
            InputParameterAdd.Varchar(objCommand, "origen", obj.origen);
            InputParameterAdd.Guid(objCommand, "id_archivo_adjunto", obj.idArchivoAdjunto);
            InputParameterAdd.Guid(objCommand, "id_cliente", obj.idCliente);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre);
            InputParameterAdd.VarBinary(objCommand, "adjunto", obj.adjunto);
            InputParameterAdd.Int(objCommand, "estado", obj.estado);
            InputParameterAdd.Guid(objCommand, "id_usuario", obj.usuario.idUsuario);           
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);
            if (obj.idArchivoAdjunto == Guid.Empty)
            obj.idArchivoAdjunto = (Guid)objCommand.Parameters["@newId"].Value;

        }
       
        
        
    }
}
