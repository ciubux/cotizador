using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;

namespace DataLayer
{
    public class DocumentoExternoDAL : DaoBase
    {
        public DocumentoExternoDAL(IDalSettings settings) : base(settings)
        {
        }

        public DocumentoExternoDAL() : this(new CotizadorSettings())
        {
        }

        public List<DocumentoExterno> getDocumentosRegistro(Guid idUsuario, Guid idRegistro, String tipo)
        {
            var objCommand = GetSqlCommand("ps_docs_externos");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Guid(objCommand, "idRegistro", idRegistro);
            InputParameterAdd.Varchar(objCommand, "tipo", tipo);
            DataTable dataTable = Execute(objCommand);
            List<DocumentoExterno> lista = new List<DocumentoExterno>();

            foreach (DataRow row in dataTable.Rows)
            {
                DocumentoExterno obj = new DocumentoExterno();

                obj.idDocumentoExterno = Converter.GetGuid(row, "id_doc_externo");
                obj.tipo = Converter.GetString(row, "tipo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.serie = Converter.GetString(row, "serie");
                obj.correlativo = Converter.GetString(row, "correlativo");

                lista.Add(obj);
            }

            return lista;
        }

        public DocumentoExterno Insertar(DocumentoExterno obj)
        {
            var objCommand = GetSqlCommand("pi_doc_externo");

            InputParameterAdd.Guid(objCommand, "idRegistro", obj.idRegistro);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "tipo", obj.tipo);
            InputParameterAdd.VarcharEmpty(objCommand, "nombre", obj.nombre);
            InputParameterAdd.VarcharEmpty(objCommand, "serie", obj.serie);
            InputParameterAdd.VarcharEmpty(objCommand, "correlativo", obj.correlativo);

            OutputParameterAdd.Int(objCommand, "idDocExterno");

            ExecuteNonQuery(objCommand);

            obj.idDocumentoExterno = (Guid)objCommand.Parameters["@idDocExterno"].Value;

            return obj;

        }
    }
}
