using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;
using Model.DTO;
using Model.ServiceReferencePSE;

namespace DataLayer
{
    public class DocumentoCompraDAL : DaoBase
    {
        public DocumentoCompraDAL(IDalSettings settings) : base(settings)
        {
        }

        public DocumentoCompraDAL() : this(new CotizadorSettings())
        {
        }

        public void InsertarDocumentoCompra(DocumentoCompra documentoCompra)
        {
            var objCommand = GetSqlCommand("pi_documentoCompra");
            InputParameterAdd.Guid(objCommand, "idCompra", documentoCompra.compra.idCompra);
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", documentoCompra.compra.notaIngreso.idMovimientoAlmacen);
            InputParameterAdd.Int(objCommand, "tipoDocumento", (int)documentoCompra.tipoDocumento);
            InputParameterAdd.DateTime(objCommand, "fechaEmision", documentoCompra.fechaEmision);
            InputParameterAdd.DateTime(objCommand, "fechaVencimiento", documentoCompra.fechaVencimiento);
            InputParameterAdd.Int(objCommand, "tipoPago", (int)documentoCompra.tipoPago);
            InputParameterAdd.Int(objCommand, "formaPago", (int)documentoCompra.formaPago);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoCompra.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "serie", documentoCompra.serie);
            InputParameterAdd.Varchar(objCommand, "numero", documentoCompra.numero);
            ///InputParameterAdd.Guid(objCommand, "idDocumentoCompraReferencia", Guid.Empty);
            InputParameterAdd.Varchar(objCommand, "observaciones", documentoCompra.observaciones);
            InputParameterAdd.Varchar(objCommand, "codigoProveedor", documentoCompra.proveedor.codigo);
            OutputParameterAdd.Int(objCommand, "idDocumentoCompra");
            OutputParameterAdd.UniqueIdentifier(objCommand, "idCompraSalida");
            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);
            documentoCompra.idDocumentoCompra = (Int32)objCommand.Parameters["@idDocumentoCompra"].Value;
            documentoCompra.compra.idCompra = (Guid)objCommand.Parameters["@idCompraSalida"].Value;
            documentoCompra.tiposErrorValidacion = (DocumentoCompra.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            documentoCompra.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;
        }


        public DocumentoCompra SelectDocumentoCompra(DocumentoCompra documentoCompra)
        {

            var objCommand = GetSqlCommand("ps_documentoCompra");
            InputParameterAdd.Int(objCommand, "idDocumentoCompra", documentoCompra.idDocumentoCompra);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable cpeCabeceraCompraTable = dataSet.Tables[0];
            DataTable cpeDetalleCompraTable = dataSet.Tables[1];
            DataTable cpeDatAdicCompraTable = dataSet.Tables[2];
            DataTable cpeDocRefCompraTable = dataSet.Tables[3];


            //Se obtienen todas las columnas de la tabla 
            var columnasCabecera = cpeCabeceraCompraTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            var columnnasDetalle = cpeDetalleCompraTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            var columnnasDatAdic = cpeDatAdicCompraTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            var columnnasDocRef = cpeDocRefCompraTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            documentoCompra.cPE_CABECERA_COMPRA = new CPE_CABECERA_COMPRA();
            documentoCompra.cPE_DETALLE_COMPRAList = new List<CPE_DETALLE_COMPRA>();
            documentoCompra.cPE_DAT_ADIC_COMPRAList = new List<CPE_DAT_ADIC_COMPRA>();
            documentoCompra.cPE_DOC_REF_COMPRAList = new List<CPE_DOC_REF_COMPRA>();
            documentoCompra.cPE_ANTICIPO_COMPRAList = new List<CPE_ANTICIPO_COMPRA>();
            documentoCompra.cPE_FAC_GUIA_COMPRAList = new List<CPE_FAC_GUIA_COMPRA>();
            documentoCompra.cPE_DOC_ASOC_COMPRAList = new List<CPE_DOC_ASOC_COMPRA>();


            foreach (DataRow row in cpeCabeceraCompraTable.Rows)
            {

                documentoCompra.solicitadoAnulacion = Converter.GetBool(row, "SOLICITUD_ANULACION");
                documentoCompra.permiteAnulacion = Converter.GetBool(row, "permite_anulacion");


                foreach (String column in columnasCabecera)
                {


                    if (!column.ToUpper().Equals("id_cpe_cabecera_compra".ToUpper()) &&
                        !column.ToUpper().Equals("estado".ToUpper()) &&
                        !column.ToUpper().Equals("usuario_creacion".ToUpper()) &&
                        !column.ToUpper().Equals("usuario_modificacion".ToUpper()) &&
                        !column.ToUpper().Equals("fecha_creacion".ToUpper()) &&
                        !column.ToUpper().Equals("fecha_modificacion".ToUpper()) &&
                        !column.ToUpper().Equals("ESTADO_SUNAT".ToUpper()) &&
                        !column.ToUpper().Equals("CODIGO".ToUpper()) &&
                        !column.ToUpper().Equals("COD_ESTD_SUNAT".ToUpper()) &&
                        !column.ToUpper().Equals("DESCRIPCION".ToUpper()) &&
                        !column.ToUpper().Equals("DETALLE".ToUpper()) &&
                        !column.ToUpper().Equals("NUM_CPE".ToUpper()) &&
                        !column.ToUpper().Equals("SOLICITUD_ANULACION".ToUpper()) &&
                        !column.ToUpper().Equals("ENVIADO_A_EOL".ToUpper()) &&
                        !column.ToUpper().Equals("AMBIENTE_PRODUCCION".ToUpper()) &&
                        !column.ToUpper().Equals("id_compra".ToUpper()) &&
                        !column.ToUpper().Equals("COMENTARIO_SOLICITUD_ANULACION".ToUpper()) &&
                        !column.ToUpper().Equals("COMENTARIO_APROBACION_ANULACION".ToUpper()) &&
                        !column.ToUpper().Equals("permite_anulacion".ToUpper()) &&
                        !column.ToUpper().Equals("observaciones_adicionales".ToUpper()) &&
                        !column.ToUpper().Equals("fecha_solicitud_anulacion".ToUpper()) &&
                        !column.ToUpper().Equals("fecha_aprobacion_anulacion".ToUpper()) &&
                        !column.ToUpper().Equals("usuario_solicitud_anulacion".ToUpper()) &&
                        !column.ToUpper().Equals("usuario_aprobacion_anulacion".ToUpper()) &&
                        !column.ToUpper().Equals("aprobado".ToUpper()) &&
                        !column.ToUpper().Equals("usuario_aprobacion".ToUpper()) &&
                        !column.ToUpper().Equals("fecha_aprobacion".ToUpper()) &&
                        !column.ToUpper().Equals("observaciones".ToUpper())

                        )
                    {
                        documentoCompra.cPE_CABECERA_COMPRA.GetType().GetProperty(column).SetValue(documentoCompra.cPE_CABECERA_COMPRA, Converter.GetString(row, column));
                    }
                }
            }

            foreach (DataRow row in cpeDetalleCompraTable.Rows)
            {
                CPE_DETALLE_COMPRA cPE_DETALLE_COMPRA = new CPE_DETALLE_COMPRA();
                foreach (String column in columnnasDetalle)
                {
                    if (!column.ToUpper().Equals("id_cpe_detalle_compra".ToUpper()) && !column.ToUpper().Equals("id_cpe_cabecera_compra".ToUpper()) && !column.ToUpper().Equals("estado".ToUpper()))
                    {
                        cPE_DETALLE_COMPRA.GetType().GetProperty(column).SetValue(cPE_DETALLE_COMPRA, Converter.GetString(row, column));
                    }
                }
                documentoCompra.cPE_DETALLE_COMPRAList.Add(cPE_DETALLE_COMPRA);

            }


            foreach (DataRow row in cpeDatAdicCompraTable.Rows)
            {
                CPE_DAT_ADIC_COMPRA cPE_DAT_ADIC_COMPRA = new CPE_DAT_ADIC_COMPRA();
                foreach (String column in columnnasDatAdic)
                {
                    if (!column.ToUpper().Equals("id_cpe_dat_adic_compra".ToUpper()) && !column.ToUpper().Equals("id_cpe_cabecera_compra".ToUpper()))
                    {
                        cPE_DAT_ADIC_COMPRA.GetType().GetProperty(column).SetValue(cPE_DAT_ADIC_COMPRA, Converter.GetString(row, column));
                    }
                }
                documentoCompra.cPE_DAT_ADIC_COMPRAList.Add(cPE_DAT_ADIC_COMPRA);

            }

            foreach (DataRow row in cpeDocRefCompraTable.Rows)
            {
                CPE_DOC_REF_COMPRA cPE_DOC_REF_COMPRA = new CPE_DOC_REF_COMPRA();
                foreach (String column in columnnasDocRef)
                {
                    if (!column.ToUpper().Equals("id_cpe_doc_ref_compra".ToUpper()) && !column.ToUpper().Equals("id_cpe_cabecera_compra".ToUpper()))
                    {
                        cPE_DOC_REF_COMPRA.GetType().GetProperty(column).SetValue(cPE_DOC_REF_COMPRA, Converter.GetString(row, column));
                    }
                }
                documentoCompra.cPE_DOC_REF_COMPRAList.Add(cPE_DOC_REF_COMPRA);

            }



            return documentoCompra;
        }

        public List<DocumentoCompra> SelectDocumentosCompra(DocumentoCompra documentoCompra)
        {

            List<DocumentoCompra> facturaList = new List<DocumentoCompra>();

            var objCommand = GetSqlCommand("ps_documentosCompra");
            if (!documentoCompra.numero.Equals("0"))
            {
                InputParameterAdd.Varchar(objCommand, "numero", documentoCompra.numero.PadLeft(8, '0'));
            }
            else
            {
                InputParameterAdd.Varchar(objCommand, "numero", String.Empty);
            }

            InputParameterAdd.Guid(objCommand, "idProveedor", documentoCompra.proveedor.idProveedor);
            InputParameterAdd.Bit(objCommand, "buscaSedesGrupoCliente", documentoCompra.buscarSedesGrupoCliente);
            InputParameterAdd.Int(objCommand, "idGrupoCliente", documentoCompra.idGrupoCliente);
            InputParameterAdd.Guid(objCommand, "idCiudad", documentoCompra.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoCompra.usuario.idUsuario);
            InputParameterAdd.DateTime(objCommand, "fechaDesde", new DateTime(documentoCompra.fechaEmisionDesde.Year, documentoCompra.fechaEmisionDesde.Month, documentoCompra.fechaEmisionDesde.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaHasta", new DateTime(documentoCompra.fechaEmisionHasta.Year, documentoCompra.fechaEmisionHasta.Month, documentoCompra.fechaEmisionHasta.Day, 23, 59, 59));
            InputParameterAdd.Int(objCommand, "soloSolicitudAnulacion", documentoCompra.solicitadoAnulacion ? 1 : 0);
            InputParameterAdd.Int(objCommand, "estado", (int)documentoCompra.estadoDocumentoSunatBusqueda);
            InputParameterAdd.BigInt(objCommand, "numeroPedido", documentoCompra.pedido.numeroPedido);
            InputParameterAdd.BigInt(objCommand, "numeroGuiaRemision", documentoCompra.guiaRemision.numeroDocumento);
            InputParameterAdd.Int(objCommand, "tipoDocumento", (int)documentoCompra.tipoDocumento);
            InputParameterAdd.Varchar(objCommand, "sku", documentoCompra.sku);



            //   InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            DataTable dataTable = Execute(objCommand);

            List<Pedido> pedidoList = new List<Pedido>();

            foreach (DataRow row in dataTable.Rows)
            {
                documentoCompra = new DocumentoCompra();
                documentoCompra.idDocumentoCompra = Converter.GetInt(row, "id_documento_compra");
                //documentoCompra.cPE_CABECERA_BE = new CPE_CABECERA_BE();
                documentoCompra.serie = Converter.GetString(row, "SERIE");
                documentoCompra.numero = Converter.GetString(row, "CORRELATIVO");
                documentoCompra.total = Converter.GetDecimal(row, "MNT_TOT_PRC_VTA");

                documentoCompra.pedido = new Pedido();
                documentoCompra.pedido.numeroPedido = Converter.GetLong(row, "numero");



                documentoCompra.tipoDocumento = (DocumentoCompra.TipoDocumento)Converter.GetInt(row, "TIP_CPE");
                if (documentoCompra.tipoDocumento == DocumentoCompra.TipoDocumento.BoletaVenta
                    || documentoCompra.tipoDocumento == DocumentoCompra.TipoDocumento.Factura)
                {
                    documentoCompra.notaIngreso = new NotaIngreso();
                    documentoCompra.notaIngreso.serieDocumento = Converter.GetString(row, "serie_documento");
                    documentoCompra.notaIngreso.numeroDocumento = Converter.GetLong(row, "numero_documento");
                }
                else if (documentoCompra.tipoDocumento == DocumentoCompra.TipoDocumento.NotaCrédito)
                {
                    documentoCompra.guiaRemision = new GuiaRemision();
                    documentoCompra.guiaRemision.serieDocumento = Converter.GetString(row, "serie_documento");
                    documentoCompra.guiaRemision.numeroDocumento = Converter.GetLong(row, "numero_documento");
                }


                documentoCompra.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                documentoCompra.fechaVencimiento = Converter.GetDateTime(row, "fecha_vencimiento");
                documentoCompra.estadoDocumentoSunat = (DocumentoCompra.EstadosDocumentoSunat)Converter.GetInt(row, "estado");






                documentoCompra.usuario = new Usuario();
                documentoCompra.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                documentoCompra.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");

                documentoCompra.proveedor = new Proveedor();
                documentoCompra.proveedor.codigo = Converter.GetString(row, "codigo");
                documentoCompra.proveedor.idProveedor = Converter.GetGuid(row, "id_cliente");
                documentoCompra.proveedor.razonSocial = Converter.GetString(row, "razon_social");
                documentoCompra.proveedor.ruc = Converter.GetString(row, "ruc");

                documentoCompra.ciudad = new Ciudad();
                documentoCompra.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                documentoCompra.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                documentoCompra.solicitadoAnulacion = Converter.GetBool(row, "solicitud_anulacion");
                documentoCompra.comentarioSolicitudAnulacion = Converter.GetString(row, "comentario_solicitud_anulacion");
                documentoCompra.comentarioSolicitudAnulacion = documentoCompra.comentarioSolicitudAnulacion == null ? String.Empty : documentoCompra.comentarioSolicitudAnulacion;
                documentoCompra.comentarioAprobacionAnulacion = Converter.GetString(row, "comentario_aprobacion_anulacion");
                documentoCompra.comentarioAprobacionAnulacion = documentoCompra.comentarioAprobacionAnulacion == null ? String.Empty : documentoCompra.comentarioAprobacionAnulacion;

                documentoCompra.permiteAnulacion = Converter.GetBool(row, "permite_anulacion");

                facturaList.Add(documentoCompra);
            }
            return facturaList;
        }




        
        public void anularDocumentoCompra(DocumentoCompra documentoCompra)
        {
            var objCommand = GetSqlCommand("pu_solicitarAnulacionDocumentoCompra");
            InputParameterAdd.Int(objCommand, "idDocumentoCompra", documentoCompra.idDocumentoCompra);
            InputParameterAdd.Varchar(objCommand, "comentarioAnulado", documentoCompra.comentarioAnulado);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoCompra.usuario.idUsuario);
            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);

            ExecuteNonQuery(objCommand);
            documentoCompra.tipoErrorSolicitudAnulacion = (DocumentoCompra.TiposErrorSolicitudAnulacion)(int)objCommand.Parameters["@tipoError"].Value;
            documentoCompra.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;
        }

        public void aprobarAnulacionDocumentoCompra(DocumentoCompra documentoCompra)
        {
            var objCommand = GetSqlCommand("pu_aprobarAnulacionFactura");
            InputParameterAdd.Int(objCommand, "idDocumentoCompra", documentoCompra.idDocumentoCompra);
            InputParameterAdd.Varchar(objCommand, "comentarioAprobacionAnulacion", documentoCompra.comentarioAprobacionAnulacion);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoCompra.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }

        

        public void aprobarAnularDocumentoCompra(DocumentoCompra documentoCompra)
        {
            var objCommand = GetSqlCommand("pu_aprobarAnularFactura");
            InputParameterAdd.Int(objCommand, "idDocumentoCompra", documentoCompra.idDocumentoCompra);
            InputParameterAdd.Varchar(objCommand, "comentarioAnulado", documentoCompra.comentarioAnulado);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoCompra.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }

        public void UpdateRespuestaDocumentoCompra(DocumentoCompra documentoCompra)
        {
            var objCommand = GetSqlCommand("pu_documentoCompra");
            InputParameterAdd.Int(objCommand, "idDocumentoCompra", documentoCompra.idDocumentoCompra);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoCompra.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }

      


        /*
        public void InsertarDocumentoCompraNotaCreditoDebito(DocumentoCompra documentoCompra)
        {
            var objCommand = GetSqlCommand("pi_documentoCompraNotaCreditoDebito");
            InputParameterAdd.Guid(objCommand, "idVenta", documentoCompra.venta.idVenta);
            InputParameterAdd.Int(objCommand, "tipoDocumento", (int)documentoCompra.tipoDocumento);
            InputParameterAdd.DateTime(objCommand, "fechaEmision", documentoCompra.fechaEmision);
            InputParameterAdd.DateTime(objCommand, "fechaVencimiento", documentoCompra.fechaVencimiento);
            InputParameterAdd.Int(objCommand, "tipoPago", (int)documentoCompra.tipoPago);
            InputParameterAdd.Int(objCommand, "formaPago", (int)documentoCompra.formaPago);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoCompra.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "serie", documentoCompra.serie);
            InputParameterAdd.Guid(objCommand, "idDocumentoReferenciaVenta", documentoCompra.venta.documentoReferencia.idDocumentoReferenciaVenta);
            InputParameterAdd.Varchar(objCommand, "observaciones", documentoCompra.observaciones==null?"": documentoCompra.observaciones);
            InputParameterAdd.Varchar(objCommand, "codigoCliente", documentoCompra.cliente.codigo);
            InputParameterAdd.Varchar(objCommand, "observacionesUsoInterno", documentoCompra.observacionesUsoInterno);
            OutputParameterAdd.UniqueIdentifier(objCommand, "idDocumentoCompra");
            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);

            ExecuteNonQuery(objCommand);

            documentoCompra.idDocumentoCompra = (Int32)objCommand.Parameters["@idDocumentoCompra"].Value;
            documentoCompra.tiposErrorValidacion = (DocumentoCompra.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            documentoCompra.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;
        }


      

        public DocumentoCompra UpdateSiguienteNumeroFacturaConsolidada(DocumentoCompra documentoCompra, String idMovimientoAlmacenList)
        {
            var objCommand = GetSqlCommand("pu_siguienteNumeroFacturaConsolidada");

            InputParameterAdd.Guid(objCommand, "idVenta", documentoCompra.venta.idVenta);
            InputParameterAdd.Int(objCommand, "idDocumentoCompra", documentoCompra.idDocumentoCompra);
            InputParameterAdd.Varchar(objCommand, "serie", documentoCompra.serie.Substring(1, 3));
            InputParameterAdd.Guid(objCommand, "idPedido", documentoCompra.venta.pedido.idPedido);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoCompra.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "idMovimientoAlmacenList", idMovimientoAlmacenList);
            //ExecuteNonQuery(objCommand);

            DataTable dataTable = Execute(objCommand);

            DocumentoCompra documentoCompraValidacion = new DocumentoCompra();

            foreach (DataRow row in dataTable.Rows)
            {
                documentoCompraValidacion = new DocumentoCompra();
                documentoCompraValidacion.idDocumentoCompra = Converter.GetInt(row, "id_cpe_cabecera_be");
                documentoCompraValidacion.serie = Converter.GetString(row, "SERIE");
                documentoCompraValidacion.numero = Converter.GetString(row, "CORRELATIVO");
                documentoCompraValidacion.fechaEmision = Converter.GetDateTime(row, "FEC_EMI");
                documentoCompraValidacion.subTotal = Converter.GetDecimal(row, "SUB_TOTAL_CPE");
                documentoCompraValidacion.total = Converter.GetDecimal(row, "TOTAL_CPE");
                documentoCompraValidacion.venta = new Venta();
                documentoCompraValidacion.venta.subTotal = Converter.GetDecimal(row, "SUB_TOTAL_VENTA");
                documentoCompraValidacion.venta.total = Converter.GetDecimal(row, "TOTAL_VENTA");
                documentoCompraValidacion.cliente = new Cliente();
                documentoCompraValidacion.cliente.razonSocialSunat = Converter.GetString(row, "NOM_RCT");
              
            }

            return documentoCompraValidacion;

        }

        public void UpdateSiguienteNumeroBoleta(DocumentoCompra documentoCompra)
        {
            var objCommand = GetSqlCommand("pu_siguienteNumeroBoleta");

            InputParameterAdd.Guid(objCommand, "idVenta", documentoCompra.venta.idVenta);
            InputParameterAdd.Int(objCommand, "idDocumentoCompra", documentoCompra.idDocumentoCompra);
            InputParameterAdd.Varchar(objCommand, "serie", documentoCompra.serie.Substring(1, 3));
            InputParameterAdd.Guid(objCommand, "idPedido", documentoCompra.venta.pedido.idPedido);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoCompra.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }


        public void UpdateSiguienteNumeroNotaCredito(DocumentoCompra documentoCompra)
        {
            var objCommand = GetSqlCommand("pu_siguienteNumeroNotaCredito");

            InputParameterAdd.Guid(objCommand, "idVenta", documentoCompra.venta.idVenta);
            InputParameterAdd.Int(objCommand, "idDocumentoCompra", documentoCompra.idDocumentoCompra);
            InputParameterAdd.Varchar(objCommand, "serie", documentoCompra.serie.Substring(2, 2));
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoCompra.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }


        public void UpdateSiguienteNumeroNotaDebito(DocumentoCompra documentoCompra)
        {
            var objCommand = GetSqlCommand("pu_siguienteNumeroNotaDebito");
            InputParameterAdd.Guid(objCommand, "idVenta", documentoCompra.venta.idVenta);
            InputParameterAdd.Int(objCommand, "idDocumentoCompra", documentoCompra.idDocumentoCompra);
            InputParameterAdd.Varchar(objCommand, "serie", documentoCompra.serie.Substring(2, 2));
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoCompra.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }


       


        public void insertEstadoDocumentoCompra(DocumentoCompra documentoCompra)
        {
            var objCommand = GetSqlCommand("pi_documentoCompraEstado");
            InputParameterAdd.Int(objCommand, "idDocumentoCompra", documentoCompra.idDocumentoCompra);
            InputParameterAdd.Varchar(objCommand, "CODIGO", documentoCompra.rPTA_BE.CODIGO);
            InputParameterAdd.Varchar(objCommand, "DESCRIPCION", documentoCompra.rPTA_BE.DESCRIPCION);
            InputParameterAdd.Varchar(objCommand, "DETALLE", documentoCompra.rPTA_BE.DETALLE);
            InputParameterAdd.Varchar(objCommand, "ESTADO", documentoCompra.rPTA_BE.ESTADO);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoCompra.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }

        public void UpdatePedido(Pedido pedido)
        {
            var objCommand = GetSqlCommand("pu_pedido");

            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", pedido.numeroGrupoPedido); //puede ser null

            if (pedido.cotizacion.idCotizacion == Guid.Empty)
                InputParameterAdd.Guid(objCommand, "idCotizacion", null); //puede ser null
            else
                InputParameterAdd.Guid(objCommand, "idCotizacion", pedido.cotizacion.idCotizacion); //puede ser null

            InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", pedido.numeroReferenciaCliente); //puede ser null
            InputParameterAdd.Guid(objCommand, "idDireccionEntrega", pedido.direccionEntrega.idDireccionEntrega); //puede ser null
            InputParameterAdd.Varchar(objCommand, "direccionEntrega", pedido.direccionEntrega.descripcion);  //puede ser null
            InputParameterAdd.Varchar(objCommand, "contactoEntrega", pedido.direccionEntrega.contacto); //puede ser null
            InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", pedido.direccionEntrega.telefono); //puede ser null
            InputParameterAdd.DateTime(objCommand, "fechaSolicitud", pedido.fechaSolicitud);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde.Value);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta.Value);

            DateTime dtTmp = DateTime.Now;
            String[] horaEntregaDesdeArray = pedido.horaEntregaDesde.Split(':');
            DateTime horaEntregaDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaDesdeArray[0]), Int32.Parse(horaEntregaDesdeArray[1]), 0);
            String[] horaEntregaHastaArray = pedido.horaEntregaHasta.Split(':');
            DateTime horaEntregaHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaHastaArray[0]), Int32.Parse(horaEntregaHastaArray[1]), 0);
            InputParameterAdd.DateTime(objCommand, "horaEntregaDesde", horaEntregaDesde);
            InputParameterAdd.DateTime(objCommand, "horaEntregaHasta", horaEntregaHasta);
            InputParameterAdd.Varchar(objCommand, "contactoPedido", pedido.contactoPedido);  //puede ser null
            InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", pedido.telefonoContactoPedido);  //puede ser null
            InputParameterAdd.Varchar(objCommand, "correoContactoPedido", pedido.correoContactoPedido);  //puede ser null
            InputParameterAdd.SmallInt(objCommand, "incluidoIGV", (short)(pedido.incluidoIGV ? 1 : 0));
            InputParameterAdd.Decimal(objCommand, "tasaIGV", Constantes.IGV);
            InputParameterAdd.Decimal(objCommand, "igv", pedido.montoIGV);
            InputParameterAdd.Decimal(objCommand, "total", pedido.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", pedido.observaciones);  //puede ser null
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            InputParameterAdd.Varchar(objCommand, "observacionSeguimientoPedido", pedido.seguimientoPedido.observacion);
            InputParameterAdd.Varchar(objCommand, "ubigeoEntrega", pedido.ubigeoEntrega.Id);

            ExecuteNonQuery(objCommand);


            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                pedidoDetalle.idPedido = pedido.idPedido;
                this.InsertPedidoDetalle(pedidoDetalle, pedido.usuario);
            }
        }
    

        public void InsertPedidoDetalle(PedidoDetalle pedidoDetalle, Usuario usuario)
        {
            var objCommand = GetSqlCommand("pi_pedidoDetalle");
            InputParameterAdd.Guid(objCommand, "idPedido", pedidoDetalle.idPedido);
            InputParameterAdd.Guid(objCommand, "idProducto", pedidoDetalle.producto.idProducto);
            InputParameterAdd.Int(objCommand, "cantidad", pedidoDetalle.cantidad);
            //Siempre se almacena el precio sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "precioSinIGV", pedidoDetalle.producto.precioSinIgv);
            //Siempre se almacena el costo sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "costoSinIGV", pedidoDetalle.producto.costoSinIgv);
            InputParameterAdd.Decimal(objCommand, "equivalencia", pedidoDetalle.producto.equivalencia);
            InputParameterAdd.Varchar(objCommand, "unidad", pedidoDetalle.unidad);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", pedidoDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "precioNetoEquivalente", pedidoDetalle.precioNeto);
            InputParameterAdd.Int(objCommand, "esPrecioAlternativo", pedidoDetalle.esPrecioAlternativo?1:0);
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Decimal(objCommand, "flete", pedidoDetalle.flete);
            InputParameterAdd.Varchar(objCommand, "observaciones", pedidoDetalle.observacion);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);
          
            pedidoDetalle.idPedidoDetalle = (Guid)objCommand.Parameters["@newId"].Value;
        }

        public Cotizacion aprobarCotizacion(Cotizacion cotizacion)
        {
            var objCommand = GetSqlCommand("pu_aprobarCotizacion");

            InputParameterAdd.BigInt(objCommand, "codigo", cotizacion.codigo);
            InputParameterAdd.Guid(objCommand, "idUsuario", cotizacion.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)cotizacion.seguimientoCotizacion.estado);
            InputParameterAdd.Varchar(objCommand, "observacion", cotizacion.seguimientoCotizacion.observacion);

            ExecuteNonQuery(objCommand);
            return cotizacion;

        }


        public Cotizacion obtenerProductosAPartirdePreciosRegistrados(Cotizacion cotizacion, String familia, String proveedor, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_generarPlantillaCotizacion");
            InputParameterAdd.Guid(objCommand, "idCliente", cotizacion.cliente.idCliente);
            InputParameterAdd.DateTime(objCommand, "fecha", cotizacion.fechaPrecios);
            InputParameterAdd.Varchar(objCommand, "familia", familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", proveedor);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable cotizacionDataTable = dataSet.Tables[0];
            DataTable cotizacionDetalleDataTable = dataSet.Tables[1];

            //DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            foreach (DataRow row in cotizacionDataTable.Rows)
            {

       
            }


            cotizacion.cotizacionDetalleList = new List<CotizacionDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in cotizacionDetalleDataTable.Rows)
            {
                CotizacionDetalle cotizacionDetalle = new CotizacionDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                cotizacionDetalle.producto = new Producto();


                //No se cuenta con IdCotizacionDetalle
                cotizacionDetalle.cantidad = 1;
                cotizacionDetalle.producto.equivalencia = Convert.ToInt32(Converter.GetDecimal(row, "equivalencia"));
                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                cotizacionDetalle.flete = Converter.GetDecimal(row, "flete");


                cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");


    
                //if (cotizacionDetalle.esPrecioAlternativo)
               // {
                    cotizacionDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * cotizacionDetalle.producto.equivalencia;
                    cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.precioNeto * 100 / cotizacionDetalle.producto.precioSinIgv);
               

                cotizacionDetalle.flete = Converter.GetDecimal(row, "flete");
                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");
                cotizacionDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                cotizacionDetalle.producto.sku = Converter.GetString(row, "sku");
                cotizacionDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                cotizacionDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                cotizacionDetalle.producto.proveedor = Converter.GetString(row, "proveedor");
                cotizacionDetalle.producto.image = Converter.GetBytes(row, "imagen");

                
                cotizacionDetalle.observacion = null; 

                cotizacion.cotizacionDetalleList.Add(cotizacionDetalle);
            }

            //POR REVISAR
            //  cotizacion.montoTotal = cotizacion.cotizacionDetalleList.AsEnumerable().Sum(o => o.subTotal);
            //  cotizacion.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacion.montoTotal / (1 + cotizacion.igv)));
            //cotizacion.montoIGV = cotizacion.montoTotal - cotizacion.montoSubTotal;

            return cotizacion;
        }




      


        public void insertSeguimientoPedido(Pedido pedido)
        {
            var objCommand = GetSqlCommand("pi_seguimiento_pedido");

            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            InputParameterAdd.Varchar(objCommand, "observacion", pedido.seguimientoPedido.observacion);
            InputParameterAdd.DateTime(objCommand, "fechaModificacion", pedido.fechaModificacion);
         //   OutputParameterAdd.DateTime(objCommand, "fechaModificacionActual");
            ExecuteNonQuery(objCommand);

         //   DateTime fechaModifiacionActual = (DateTime)objCommand.Parameters["@fechaModificacionActual"].Value;

        }*/

    }
}
