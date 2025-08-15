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
                lista.Add(getBasicFromRow(row));
            }

            return lista;
        }

        public List<DocumentoExterno> getDocumentosPedidoOriginal(Guid idUsuario, Guid idPedido)
        {
            var objCommand = GetSqlCommand("ps_documentosExternosPedidoOriginal");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Guid(objCommand, "idPedido", idPedido);
            
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable docsDataTable = dataSet.Tables[0];
            DataTable guiasDataTable = dataSet.Tables[1];

            List<DocumentoExterno> lista = new List<DocumentoExterno>();

            foreach (DataRow row in docsDataTable.Rows)
            {
                lista.Add(getBasicFromRow(row));
            }

            foreach (DataRow row in guiasDataTable.Rows)
            {
                Guid idGuia = Converter.GetGuid(row, "id_movimiento_almacen");
                
                foreach (DocumentoExterno doc in lista)
                {
                    if (doc.idRegistro.Equals(idGuia))
                    {
                        doc.serieRel = Converter.GetString(row, "serie_documento");
                        doc.correlativoRel = Converter.GetInt(row, "numero_documento");
                    }
                }
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

            OutputParameterAdd.UniqueIdentifier(objCommand, "idDocExterno");

            ExecuteNonQuery(objCommand);

            obj.idDocumentoExterno = (Guid)objCommand.Parameters["@idDocExterno"].Value;

            return obj;

        }

        public void InsertarLista(List<DocumentoExterno> lista, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pi_docs_externos");

            OutputParameterAdd.Int(objCommand, "idDocExterno");

            ExecuteNonQuery(objCommand);
        }

        private DocumentoExterno getBasicFromRow(DataRow row)
        {
            DocumentoExterno obj = new DocumentoExterno();

            obj.idDocumentoExterno = Converter.GetGuid(row, "id_doc_externo");
            obj.idRegistro = Converter.GetGuid(row, "id_registro");
            obj.tipo = Converter.GetString(row, "tipo");
            obj.nombre = Converter.GetString(row, "nombre");
            obj.serie = Converter.GetString(row, "serie");
            obj.correlativo = Converter.GetString(row, "correlativo");

            return obj;
        }
    }
}
