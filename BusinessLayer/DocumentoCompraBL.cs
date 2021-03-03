
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using Model.DTO;
using Model.ServiceReferencePSE;
using System.ServiceModel;
using ServiceLayer;

namespace BusinessLayer
{
    public class DocumentoCompraBL
    {

        public DocumentoCompra InsertarDocumentoCompra(DocumentoCompra documentoCompra)
        {
            using (var dal = new DocumentoCompraDAL())
            {
                dal.InsertarDocumentoCompra(documentoCompra);

                if (documentoCompra.tiposErrorValidacion == DocumentoCompra.TiposErrorValidacion.NoExisteError)
                {
                    //Se recupera el documento de venta creado para poder visualizarlo
                    documentoCompra = dal.SelectDocumentoCompra(documentoCompra);
                    documentoCompra.tipoPago = (DocumentoCompra.TipoPago)Int32.Parse(documentoCompra.cPE_CABECERA_COMPRA.TIP_PAG);
                    documentoCompra.cPE_RESPUESTA_COMPRA = new CPE_RESPUESTA_COMPRA();
                    documentoCompra.cPE_RESPUESTA_COMPRA.CODIGO = Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK;
                }
                else
                {
                    documentoCompra.cPE_RESPUESTA_COMPRA = new CPE_RESPUESTA_COMPRA();
                    documentoCompra.cPE_RESPUESTA_COMPRA.CODIGO = Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_ERROR_DATA;
                    documentoCompra.cPE_RESPUESTA_COMPRA.DETALLE = documentoCompra.tiposErrorValidacionString+ ". "+ documentoCompra.descripcionError;
                }
                return documentoCompra;
            }
        }        

        
        public DocumentoCompra InsertarNotaCredito(DocumentoCompra documentoCompra)
        {
            using (var dal = new DocumentoCompraDAL())
            {
                documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.NotaCrédito;

            //    documentoCompra.movimientoAlmacen = new MovimientoAlmacen();
                //Se define el idMovimientoAlmacen como vacío para que tome el idventa
             //   documentoCompra.movimientoAlmacen.idMovimientoAlmacen = Guid.Empty;

                dal.InsertarDocumentoCompraNotaCreditoDebito(documentoCompra);

                if (documentoCompra.tiposErrorValidacion == DocumentoCompra.TiposErrorValidacion.NoExisteError)
                {
                    //Se recupera el documento de venta creado para poder visualizarlo
                    documentoCompra = dal.SelectDocumentoCompra(documentoCompra);
                    documentoCompra.tipoPago = (DocumentoCompra.TipoPago)Int32.Parse(documentoCompra.cPE_CABECERA_COMPRA.TIP_PAG);
                    documentoCompra.cPE_RESPUESTA_COMPRA = new CPE_RESPUESTA_COMPRA();
                    documentoCompra.cPE_RESPUESTA_COMPRA.CODIGO = Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK;
                }
                else
                {
                    documentoCompra.cPE_RESPUESTA_COMPRA = new CPE_RESPUESTA_COMPRA();
                    documentoCompra.cPE_RESPUESTA_COMPRA.CODIGO = Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_ERROR_DATA;
                    documentoCompra.cPE_RESPUESTA_COMPRA.DETALLE = documentoCompra.tiposErrorValidacionString + ". " + documentoCompra.descripcionError;
                }
                return documentoCompra;
            }
        }


        /*
        public DocumentoCompra InsertarNotaDebito(DocumentoCompra documentoCompra)
        {
            using (var dal = new DocumentoCompraDAL())
            {
                documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.NotaDébito;
                dal.InsertarDocumentoCompraNotaCreditoDebito(documentoCompra);

                if (documentoCompra.tiposErrorValidacion == DocumentoCompra.TiposErrorValidacion.NoExisteError)
                {
                    //Se recupera el documento de venta creado para poder visualizarlo
                    documentoCompra = dal.SelectDocumentoCompra(documentoCompra);
                    documentoCompra.tipoPago = (DocumentoCompra.TipoPago)Int32.Parse(documentoCompra.cPE_CABECERA_COMPRA.TIP_PAG);
                    documentoCompra.cPE_RESPUESTA_BE = new CPE_RESPUESTA_BE();
                    documentoCompra.cPE_RESPUESTA_BE.CODIGO = Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK;
                }
                else
                {
                    documentoCompra.cPE_RESPUESTA_BE = new CPE_RESPUESTA_BE();
                    documentoCompra.cPE_RESPUESTA_BE.CODIGO = Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_ERROR_DATA;
                    documentoCompra.cPE_RESPUESTA_BE.DETALLE = documentoCompra.tiposErrorValidacionString + ". " + documentoCompra.descripcionError;
                }
                return documentoCompra;
            }
        }
        */
        public DocumentoCompra GetDocumentoCompra(DocumentoCompra documentoCompra)
        {
            using (var dal = new DocumentoCompraDAL())
            {
                documentoCompra = dal.SelectDocumentoCompra(documentoCompra);
                documentoCompra.tipoPago = (DocumentoCompra.TipoPago)Int32.Parse(documentoCompra.cPE_CABECERA_COMPRA.TIP_PAG);
                return documentoCompra;
            }
        }
        


        /*
        public CPE_RESPUESTA_BE procesarNotaDebito(DocumentoCompra documentoCompra)
        {
            using (var dal = new DocumentoCompraDAL())
            {
                try
                {
                    documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.NotaDébito;
                    documentoCompra = dal.SelectDocumentoCompra(documentoCompra);
                    documentoCompra.globalEnumTipoOnline = GlobalEnumTipoOnline.Normal;
                    IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
                    Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
                    client.Endpoint.Address = new EndpointAddress(uri);


                    documentoCompra.cPE_RESPUESTA_BE = client.callProcessOnline(Constantes.USER_EOL, Constantes.PASSWORD_EOL,
                        documentoCompra.cPE_CABECERA_COMPRA,
                        documentoCompra.cPE_DETALLE_BEList.ToArray(),
                        documentoCompra.cPE_DAT_ADIC_BEList.ToArray(),
                        documentoCompra.cPE_DOC_REF_BEList.ToArray(),
                        documentoCompra.cPE_ANTICIPO_BEList.ToArray(),
                        documentoCompra.cPE_FAC_GUIA_BEList.ToArray(),
                        documentoCompra.cPE_DOC_ASOC_BEList.ToArray(),
                        documentoCompra.globalEnumTipoOnline);
                    documentoCompra.serie = documentoCompra.cPE_CABECERA_COMPRA.SERIE;
                    documentoCompra.numero = documentoCompra.cPE_CABECERA_COMPRA.CORRELATIVO;


                    dal.UpdateRespuestaDocumentoCompra(documentoCompra);

                    //Si se procesa correctamente se actualiza el correlativo y los documentos internos y 
                    //Se consulta el estado del documento en SUNAT
                    if (documentoCompra.cPE_RESPUESTA_BE.CODIGO.Equals(Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK))
                    {
                        dal.UpdateSiguienteNumeroNotaDebito(documentoCompra);
                        documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.NotaDébito;
                        consultarEstadoDocumentoCompra(documentoCompra);
                    }

                    return documentoCompra.cPE_RESPUESTA_BE;

                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }
        }*/

            /*
        public CPE_RESPUESTA_BE procesarNotaCredito(DocumentoCompra documentoCompra)
        {
            using (var dal = new DocumentoCompraDAL())
            {
                try
                {
                    documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.NotaCrédito;
                    documentoCompra = dal.SelectDocumentoCompra(documentoCompra);
                    documentoCompra.globalEnumTipoOnline = GlobalEnumTipoOnline.Normal;
                    IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
                    Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
                    client.Endpoint.Address = new EndpointAddress(uri);


                    documentoCompra.cPE_RESPUESTA_BE = client.callProcessOnline(Constantes.USER_EOL, Constantes.PASSWORD_EOL,
                        documentoCompra.cPE_CABECERA_COMPRA,
                        documentoCompra.cPE_DETALLE_BEList.ToArray(),
                        documentoCompra.cPE_DAT_ADIC_BEList.ToArray(),
                        documentoCompra.cPE_DOC_REF_BEList.ToArray(),
                        documentoCompra.cPE_ANTICIPO_BEList.ToArray(),
                        documentoCompra.cPE_FAC_GUIA_BEList.ToArray(),
                        documentoCompra.cPE_DOC_ASOC_BEList.ToArray(),
                        documentoCompra.globalEnumTipoOnline);
                    documentoCompra.serie = documentoCompra.cPE_CABECERA_COMPRA.SERIE;
                    documentoCompra.numero = documentoCompra.cPE_CABECERA_COMPRA.CORRELATIVO;


                    dal.UpdateRespuestaDocumentoCompra(documentoCompra);

                    //Si se procesa correctamente se actualiza el correlativo y los documentos internos y 
                    //Se consulta el estado del documento en SUNAT
                    if (documentoCompra.cPE_RESPUESTA_BE.CODIGO.Equals(Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK))
                    {
                        dal.UpdateSiguienteNumeroNotaCredito(documentoCompra);
                        documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.NotaCrédito;
                        consultarEstadoDocumentoCompra(documentoCompra);
                    }

                    return documentoCompra.cPE_RESPUESTA_BE;

                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }
        }


        public CPE_RESPUESTA_BE procesarBoletaVenta(DocumentoCompra documentoCompra)
        {
            using (var dal = new DocumentoCompraDAL())
            {
                try
                {
                    documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.BoletaVenta;
                    documentoCompra = dal.SelectDocumentoCompra(documentoCompra);
                    documentoCompra.globalEnumTipoOnline = GlobalEnumTipoOnline.Normal;
                    IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
                    Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
                    client.Endpoint.Address = new EndpointAddress(uri);


                    documentoCompra.cPE_RESPUESTA_BE = client.callProcessOnline(Constantes.USER_EOL, Constantes.PASSWORD_EOL,
                        documentoCompra.cPE_CABECERA_COMPRA,
                        documentoCompra.cPE_DETALLE_BEList.ToArray(),
                        documentoCompra.cPE_DAT_ADIC_BEList.ToArray(),
                        documentoCompra.cPE_DOC_REF_BEList.ToArray(),
                        documentoCompra.cPE_ANTICIPO_BEList.ToArray(),
                        documentoCompra.cPE_FAC_GUIA_BEList.ToArray(),
                        documentoCompra.cPE_DOC_ASOC_BEList.ToArray(),
                        documentoCompra.globalEnumTipoOnline);
                    documentoCompra.serie = documentoCompra.cPE_CABECERA_COMPRA.SERIE;
                    documentoCompra.numero = documentoCompra.cPE_CABECERA_COMPRA.CORRELATIVO;


                    dal.UpdateRespuestaDocumentoCompra(documentoCompra);

                    //Si se procesa correctamente se actualiza el correlativo y los documentos internos y 
                    //Se consulta el estado del documento en SUNAT
                    if (documentoCompra.cPE_RESPUESTA_BE.CODIGO.Equals(Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK))
                    {
                        dal.UpdateSiguienteNumeroFactura(documentoCompra);
                        documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.BoletaVenta;
                        consultarEstadoDocumentoCompra(documentoCompra);
                    }

                    return documentoCompra.cPE_RESPUESTA_BE;

                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }
        }
        */


        public CPE_RESPUESTA_COMPRA procesarCPE(DocumentoCompra documentoCompra,List<Guid> movimientoAlmacenIdList = null)
        {
            using (var dal = new DocumentoCompraDAL())
            {
                try
                {
                    documentoCompra = dal.SelectDocumentoCompra(documentoCompra);
                    //Se recupera el clasePedido de pago registrado
                    documentoCompra.tipoPago = (DocumentoCompra.TipoPago)Int32.Parse(documentoCompra.cPE_CABECERA_COMPRA.TIP_PAG);
                    documentoCompra.cPE_RESPUESTA_COMPRA = new CPE_RESPUESTA_COMPRA();
                    documentoCompra.cPE_RESPUESTA_COMPRA.CODIGO = Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK;
                    dal.UpdateRespuestaDocumentoCompra(documentoCompra);

                    /*  if (documentoCompra.tipoDocumento == DocumentoCompra.TipoDocumento.Factura)
                        {

                            //Si se cuenta con la lista de guias de remisión se procede a actualizar el estado d elas guías consolidadas
                            if (movimientoAlmacenIdList == null)
                            {
                                dal.UpdateSiguienteNumeroFactura(documentoCompra);

                                return documentoCompra.cPE_RESPUESTA_COMPRA;
                            }
                        }*/
                    documentoCompra.serie = documentoCompra.cPE_CABECERA_COMPRA.SERIE;
                    documentoCompra.numero = documentoCompra.cPE_CABECERA_COMPRA.CORRELATIVO;

                    return documentoCompra.cPE_RESPUESTA_COMPRA;
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        
        public void anularDocumentoCompra(DocumentoCompra documentoCompra)
        {
            using (var dal = new DocumentoCompraDAL())
            {
                dal.anularDocumentoCompra(documentoCompra);
            }
        }

        public void aprobarAnulacionDocumentoCompra(DocumentoCompra documentoCompra, Usuario usuario)
        {
       /*     IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
            client.Endpoint.Address = new EndpointAddress(uri);

            
            List<CPE_DOC_BAJA> CPE_DOC_BAJAList = new List<CPE_DOC_BAJA>();
            CPE_DOC_BAJA CPE_DOC_BAJA = new CPE_DOC_BAJA();
            CPE_DOC_BAJA.NRO_DOC = Constantes.RUC_MP;
            CPE_DOC_BAJA.TIP_DOC = documentoCompra.cPE_CABECERA_COMPRA.TIP_DOC_EMI;
            CPE_DOC_BAJA.TIP_CPE = documentoCompra.cPE_CABECERA_COMPRA.TIP_CPE;
            CPE_DOC_BAJA.FEC_EMI = documentoCompra.cPE_CABECERA_COMPRA.FEC_EMI;
            CPE_DOC_BAJA.SERIE = documentoCompra.cPE_CABECERA_COMPRA.SERIE;
            CPE_DOC_BAJA.CORRELATIVO = documentoCompra.cPE_CABECERA_COMPRA.CORRELATIVO;
            CPE_DOC_BAJA.MTVO_BAJA = documentoCompra.comentarioAprobacionAnulacion;
            CPE_DOC_BAJAList.Add(CPE_DOC_BAJA);

           RPTA_BE[] rPTA_BEArray = client.CallRequestLow(Constantes.cPE_CABECERA_COMPRA_ID, Constantes.cPE_CABECERA_COMPRA_COD_GPO, Constantes.USER_EOL, Constantes.PASSWORD_EOL, CPE_DOC_BAJAList.ToArray());


            documentoCompra.rPTA_BE = rPTA_BEArray[0];

            if (documentoCompra.rPTA_BE.CODIGO == Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK)
            {
                using (var dal = new DocumentoCompraDAL())
                {
                    dal.aprobarAnulacionDocumentoCompra(documentoCompra);
                }

                string body = string.Empty;
                using (StringReader reader = new StringReader(Constantes.CUERPO_CORREO_SOLICITUD_DE_BAJA))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{xxxx}", documentoCompra.cPE_CABECERA_COMPRA.SERIE.ToString());
                body = body.Replace("{xxxxxxxx}", documentoCompra.cPE_CABECERA_COMPRA.CORRELATIVO.ToString());
                body = body.Replace("{xx/xx}",DateTime.Parse(documentoCompra.cPE_CABECERA_COMPRA.FEC_EMI).ToString("dd/MM"));

                List<string> correos = new List<string>();

                if (documentoCompra.cPE_CABECERA_COMPRA.CORREO_ENVIO != null &&
                    documentoCompra.cPE_CABECERA_COMPRA.CORREO_ENVIO.Length > 0
                    ) {
                    String[] correoEnvioArray  = documentoCompra.cPE_CABECERA_COMPRA.CORREO_ENVIO.Split(';');
                    foreach (String correoEnvio in correoEnvioArray)
                    {
                        correos.Add(correoEnvio.Trim());
                    }                   
                }

                if (documentoCompra.cPE_CABECERA_COMPRA.CORREO_COPIA != null &&
                   documentoCompra.cPE_CABECERA_COMPRA.CORREO_COPIA.Length > 0
                   )
                {
                    String[] correoEnvioArray = documentoCompra.cPE_CABECERA_COMPRA.CORREO_COPIA.Split(';');
                    foreach (String correoEnvio in correoEnvioArray)
                    {
                        correos.Add(correoEnvio.Trim());
                    }
                }

                if (documentoCompra.cPE_CABECERA_COMPRA.CORREO_OCULTO != null &&
                   documentoCompra.cPE_CABECERA_COMPRA.CORREO_OCULTO.Length > 0
                   )
                {
                    String[] correoEnvioArray = documentoCompra.cPE_CABECERA_COMPRA.CORREO_OCULTO.Split(';');
                    foreach (String correoEnvio in correoEnvioArray)
                    {
                        correos.Add(correoEnvio.Trim());
                    }
                }

                MailService mailService = new MailService();
                mailService.enviar(correos,Constantes.ASUNTO_ANULACION_FACTURA,
                    body, Constantes.MAIL_COMUNICACION_FACTURAS, Constantes.PASSWORD_MAIL_COMUNICACION_FACTURAS, usuario);

               
            }
            */
        }
        

     /*   public DocumentoCompra descargarArchivoDocumentoCompra(DocumentoCompra documentoCompra)
        {
          IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
            client.Endpoint.Address = new EndpointAddress(uri);

            documentoCompra.rPTA_DOC_TRIB_BE = client.callExtractCPE(Constantes.USER_EOL, Constantes.PASSWORD_EOL, Constantes.RUC_MP, documentoCompra.cPE_CABECERA_COMPRA.TIP_CPE, documentoCompra.cPE_CABECERA_COMPRA.SERIE, documentoCompra.cPE_CABECERA_COMPRA.CORRELATIVO, true, true, true); 
      
            return documentoCompra;
        }*/

        
        public List<DocumentoCompra> GetDocumentosCompra(DocumentoCompra documentoCompra)
        {
            List<DocumentoCompra> documentoCompraList = null;
            using (var dal = new DocumentoCompraDAL())
            {
                documentoCompraList = dal.SelectDocumentosCompra(documentoCompra);
            }
            return documentoCompraList;
        }
        
    }
}
