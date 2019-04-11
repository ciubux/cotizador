
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
                    documentoCompra.tipoPago = (DocumentoCompra.TipoPago)Int32.Parse(documentoCompra.cPE_CABECERA_BE.TIP_PAG);
                    documentoCompra.cPE_RESPUESTA_BE = new CPE_RESPUESTA_BE();
                    documentoCompra.cPE_RESPUESTA_BE.CODIGO = Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK;
                }
                else
                {
                    documentoCompra.cPE_RESPUESTA_BE = new CPE_RESPUESTA_BE();
                    documentoCompra.cPE_RESPUESTA_BE.CODIGO = Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_ERROR_DATA;
                    documentoCompra.cPE_RESPUESTA_BE.DETALLE = documentoCompra.tiposErrorValidacionString+ ". "+ documentoCompra.descripcionError;
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
                    documentoCompra.tipoPago = (DocumentoCompra.TipoPago)Int32.Parse(documentoCompra.cPE_CABECERA_BE.TIP_PAG);
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
                    documentoCompra.tipoPago = (DocumentoCompra.TipoPago)Int32.Parse(documentoCompra.cPE_CABECERA_BE.TIP_PAG);
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

        public DocumentoCompra GetDocumentoCompra(DocumentoCompra documentoCompra)
        {
            using (var dal = new DocumentoCompraDAL())
            {
                documentoCompra = dal.SelectDocumentoCompra(documentoCompra);
                documentoCompra.tipoPago = (DocumentoCompra.TipoPago)Int32.Parse(documentoCompra.cPE_CABECERA_BE.TIP_PAG);
                return documentoCompra;
            }
        }

        public void ActualizarEstadoDocumentosElectronicos(Usuario usuario)
        {
            using (var dal = new DocumentoCompraDAL())
            {
                List<DocumentoCompra> documentoCompraList = dal.SelectDocumentosVentaPorProcesar();

                foreach (DocumentoCompra documentoCompra in documentoCompraList)
                {
                    documentoCompra.usuario = usuario;
                    documentoCompra.tipoDocumento = DocumentoCompra.TipoDocumento.Factura;
                    this.consultarEstadoDocumentoCompra(documentoCompra);

                }
            }
        }



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
                        documentoCompra.cPE_CABECERA_BE,
                        documentoCompra.cPE_DETALLE_BEList.ToArray(),
                        documentoCompra.cPE_DAT_ADIC_BEList.ToArray(),
                        documentoCompra.cPE_DOC_REF_BEList.ToArray(),
                        documentoCompra.cPE_ANTICIPO_BEList.ToArray(),
                        documentoCompra.cPE_FAC_GUIA_BEList.ToArray(),
                        documentoCompra.cPE_DOC_ASOC_BEList.ToArray(),
                        documentoCompra.globalEnumTipoOnline);
                    documentoCompra.serie = documentoCompra.cPE_CABECERA_BE.SERIE;
                    documentoCompra.numero = documentoCompra.cPE_CABECERA_BE.CORRELATIVO;


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
        }


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
                        documentoCompra.cPE_CABECERA_BE,
                        documentoCompra.cPE_DETALLE_BEList.ToArray(),
                        documentoCompra.cPE_DAT_ADIC_BEList.ToArray(),
                        documentoCompra.cPE_DOC_REF_BEList.ToArray(),
                        documentoCompra.cPE_ANTICIPO_BEList.ToArray(),
                        documentoCompra.cPE_FAC_GUIA_BEList.ToArray(),
                        documentoCompra.cPE_DOC_ASOC_BEList.ToArray(),
                        documentoCompra.globalEnumTipoOnline);
                    documentoCompra.serie = documentoCompra.cPE_CABECERA_BE.SERIE;
                    documentoCompra.numero = documentoCompra.cPE_CABECERA_BE.CORRELATIVO;


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
                        documentoCompra.cPE_CABECERA_BE,
                        documentoCompra.cPE_DETALLE_BEList.ToArray(),
                        documentoCompra.cPE_DAT_ADIC_BEList.ToArray(),
                        documentoCompra.cPE_DOC_REF_BEList.ToArray(),
                        documentoCompra.cPE_ANTICIPO_BEList.ToArray(),
                        documentoCompra.cPE_FAC_GUIA_BEList.ToArray(),
                        documentoCompra.cPE_DOC_ASOC_BEList.ToArray(),
                        documentoCompra.globalEnumTipoOnline);
                    documentoCompra.serie = documentoCompra.cPE_CABECERA_BE.SERIE;
                    documentoCompra.numero = documentoCompra.cPE_CABECERA_BE.CORRELATIVO;


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

        public CPE_RESPUESTA_BE procesarCPE(DocumentoCompra documentoCompra,List<Guid> movimientoAlmacenIdList = null)
        {
            using (var dal = new DocumentoCompraDAL())
            {
                try
                {
                    documentoCompra = dal.SelectDocumentoCompra(documentoCompra);
                    //Se recupera el clasePedido de pago registrado
                    documentoCompra.tipoPago = (DocumentoCompra.TipoPago)Int32.Parse(documentoCompra.cPE_CABECERA_BE.TIP_PAG);

                    documentoCompra.globalEnumTipoOnline = GlobalEnumTipoOnline.Normal;
                    IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
                    Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
                    client.Endpoint.Address = new EndpointAddress(uri);

                    documentoCompra.cPE_RESPUESTA_BE = client.callProcessOnline(Constantes.USER_EOL, Constantes.PASSWORD_EOL,
                        documentoCompra.cPE_CABECERA_BE,
                        documentoCompra.cPE_DETALLE_BEList.ToArray(),
                        documentoCompra.cPE_DAT_ADIC_BEList.ToArray(),
                        documentoCompra.cPE_DOC_REF_BEList.ToArray(),
                        documentoCompra.cPE_ANTICIPO_BEList.ToArray(),
                        documentoCompra.cPE_FAC_GUIA_BEList.ToArray(),
                        documentoCompra.cPE_DOC_ASOC_BEList.ToArray(),
                        documentoCompra.globalEnumTipoOnline);
                    documentoCompra.serie = documentoCompra.cPE_CABECERA_BE.SERIE;
                    documentoCompra.numero = documentoCompra.cPE_CABECERA_BE.CORRELATIVO;
                    //Se inserta el resultado en Base de Datos
                    dal.UpdateRespuestaDocumentoCompra(documentoCompra);

                    //Si se procesa correctamente se actualiza el correlativo y los documentos internos y 
                    //Se consulta el estado del documento en SUNAT
                    if (documentoCompra.cPE_RESPUESTA_BE.CODIGO.Equals(Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK))
                    {
                        if (documentoCompra.tipoDocumento == DocumentoCompra.TipoDocumento.Factura)
                        {

                            //Si se cuenta con la lista de guias de remisión se procede a actualizar el estado d elas guías consolidadas
                            if (movimientoAlmacenIdList == null)
                            {
                                dal.UpdateSiguienteNumeroFactura(documentoCompra);
                            }
                            else
                            {
                                String idMovimientoAlmacenList = " '";

                                foreach (Guid idMovimientoAlmacen in movimientoAlmacenIdList)
                                {

                                    idMovimientoAlmacenList = idMovimientoAlmacenList + idMovimientoAlmacen + "','";
                                }

                                idMovimientoAlmacenList = idMovimientoAlmacenList.Substring(0, idMovimientoAlmacenList.Length - 2);

                                DocumentoCompra documentoCompraValidacion = dal.UpdateSiguienteNumeroFacturaConsolidada(documentoCompra, idMovimientoAlmacenList);

                                String bodyMail = @"<h3>Se generó factura Consolidada, Cliente: " + documentoCompraValidacion.cliente.razonSocialSunat + "</h3>" +
                                    "<p>Factura: " + documentoCompraValidacion.serieNumero + "(" + documentoCompraValidacion.fechaEmision.Value.ToString(Constantes.formatoFecha) + ")" + "</p>" +
                                    "<p>Sub Total Factura: " + documentoCompraValidacion.subTotal.ToString() + "</p>" +
                                    "<p>Sub Total Venta: " + documentoCompraValidacion.venta.subTotal.ToString() + "</p>" +
                                    "<p>DIFERENCIA SUB TOTALES: " + (documentoCompraValidacion.subTotal - documentoCompraValidacion.venta.subTotal) + "</p>" +
                                    "<p>Total Factura: " + documentoCompraValidacion.total.ToString() + "</p>" +
                                    "<p>Total Venta: " + documentoCompraValidacion.venta.total.ToString() + "</p>" +
                                    "<p>DIFERENCIA TOTALES: " + (documentoCompraValidacion.total - documentoCompraValidacion.venta.total) + "</p>" +
                                    "<p>ID Documento Venta: " + documentoCompraValidacion.idDocumentoCompra.ToString() + "</p>";

                                MailService mailService = new MailService();
                                mailService.enviar(new List<string>(){ Constantes.MAIL_ADMINISTRADOR, Constantes.MAIL_SOPORTE_TI }, "SE GENERÓ FACTURA CONSOLIDADA "+ documentoCompraValidacion.cliente.razonSocialSunat,
                                    bodyMail, Constantes.MAIL_COMUNICACION_FACTURAS, Constantes.PASSWORD_MAIL_COMUNICACION_FACTURAS, documentoCompra.usuario);
                            }

                            /*Se Identifica */
                            if (Int32.Parse(documentoCompra.cPE_CABECERA_BE.FRM_PAG) == (int)DocumentoCompra.FormaPago.Letra)
                            {
                                CPE_CABECERA_BE cPE_CABECERA_BE = documentoCompra.cPE_CABECERA_BE;
                                String bodyMail = @"<h3>Se generó factura " + cPE_CABECERA_BE.SERIE + "-" + cPE_CABECERA_BE.CORRELATIVO + "</h3>" +
                                    "<p>FECHA EMISIÓN: " + cPE_CABECERA_BE.FEC_EMI + "</p>"+
                                    "<p>TOTAL (S/): " + cPE_CABECERA_BE.MNT_TOT + "</p>" +
                                    "<p>FORMA DE PAGO: LETRA</p>" +
                                    "<p>CLIENTE: " + cPE_CABECERA_BE.NOM_RCT + "</p>" +
                                    "<p>RUC: " + cPE_CABECERA_BE.NRO_DOC_RCT + "</p>";

                                MailService mailService = new MailService();
                                mailService.enviar(new List<string>() { Constantes.MAIL_ADMINISTRADOR, Constantes.MAIL_SOPORTE_TI,
                                Constantes.MAIL_COBRANZAS
                                }, "SE GENERÓ FACTURA " + documentoCompra.cPE_CABECERA_BE.SERIE+"-"+ documentoCompra.cPE_CABECERA_BE.CORRELATIVO+" - FORMA DE PAGO LETRA",
                                bodyMail, Constantes.MAIL_COMUNICACION_FACTURAS,
                                Constantes.PASSWORD_MAIL_COMUNICACION_FACTURAS, documentoCompra.usuario);
                            }

                        }
                        if (documentoCompra.tipoDocumento == DocumentoCompra.TipoDocumento.BoletaVenta)
                        {
                            dal.UpdateSiguienteNumeroBoleta(documentoCompra);
                        }
                        else if (documentoCompra.tipoDocumento == DocumentoCompra.TipoDocumento.NotaCrédito)
                        {
                            dal.UpdateSiguienteNumeroNotaCredito(documentoCompra);
                        }
                        else if (documentoCompra.tipoDocumento == DocumentoCompra.TipoDocumento.NotaDébito)
                        {
                            dal.UpdateSiguienteNumeroNotaDebito(documentoCompra);
                        }


                        consultarEstadoDocumentoCompra(documentoCompra);
                    }

                    return documentoCompra.cPE_RESPUESTA_BE;

                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }

        }


        public void consultarEstadoDocumentoCompra(DocumentoCompra documentoCompra)
        {
            IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
            client.Endpoint.Address = new EndpointAddress(uri);
            

            documentoCompra.rPTA_BE = client.callStateCPE(Constantes.USER_EOL, Constantes.PASSWORD_EOL, Constantes.RUC_MP, "0"+(int)documentoCompra.tipoDocumento, documentoCompra.serie, documentoCompra.numero);

            //El resultado se inserta a BD
            using (var dal = new DocumentoCompraDAL())
            {
                dal.insertEstadoDocumentoCompra(documentoCompra);
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
            IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
            client.Endpoint.Address = new EndpointAddress(uri);

            
            List<CPE_DOC_BAJA> CPE_DOC_BAJAList = new List<CPE_DOC_BAJA>();
            CPE_DOC_BAJA CPE_DOC_BAJA = new CPE_DOC_BAJA();
            CPE_DOC_BAJA.NRO_DOC = Constantes.RUC_MP;
            CPE_DOC_BAJA.TIP_DOC = documentoCompra.cPE_CABECERA_BE.TIP_DOC_EMI;
            CPE_DOC_BAJA.TIP_CPE = documentoCompra.cPE_CABECERA_BE.TIP_CPE;
            CPE_DOC_BAJA.FEC_EMI = documentoCompra.cPE_CABECERA_BE.FEC_EMI;
            CPE_DOC_BAJA.SERIE = documentoCompra.cPE_CABECERA_BE.SERIE;
            CPE_DOC_BAJA.CORRELATIVO = documentoCompra.cPE_CABECERA_BE.CORRELATIVO;
            CPE_DOC_BAJA.MTVO_BAJA = documentoCompra.comentarioAprobacionAnulacion;
            CPE_DOC_BAJAList.Add(CPE_DOC_BAJA);

           RPTA_BE[] rPTA_BEArray = client.CallRequestLow(Constantes.CPE_CABECERA_BE_ID, Constantes.CPE_CABECERA_BE_COD_GPO, Constantes.USER_EOL, Constantes.PASSWORD_EOL, CPE_DOC_BAJAList.ToArray());


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

                body = body.Replace("{xxxx}", documentoCompra.cPE_CABECERA_BE.SERIE.ToString());
                body = body.Replace("{xxxxxxxx}", documentoCompra.cPE_CABECERA_BE.CORRELATIVO.ToString());
                body = body.Replace("{xx/xx}",DateTime.Parse(documentoCompra.cPE_CABECERA_BE.FEC_EMI).ToString("dd/MM"));

                List<string> correos = new List<string>();

                if (documentoCompra.cPE_CABECERA_BE.CORREO_ENVIO != null &&
                    documentoCompra.cPE_CABECERA_BE.CORREO_ENVIO.Length > 0
                    ) {
                    String[] correoEnvioArray  = documentoCompra.cPE_CABECERA_BE.CORREO_ENVIO.Split(';');
                    foreach (String correoEnvio in correoEnvioArray)
                    {
                        correos.Add(correoEnvio.Trim());
                    }                   
                }

                if (documentoCompra.cPE_CABECERA_BE.CORREO_COPIA != null &&
                   documentoCompra.cPE_CABECERA_BE.CORREO_COPIA.Length > 0
                   )
                {
                    String[] correoEnvioArray = documentoCompra.cPE_CABECERA_BE.CORREO_COPIA.Split(';');
                    foreach (String correoEnvio in correoEnvioArray)
                    {
                        correos.Add(correoEnvio.Trim());
                    }
                }

                if (documentoCompra.cPE_CABECERA_BE.CORREO_OCULTO != null &&
                   documentoCompra.cPE_CABECERA_BE.CORREO_OCULTO.Length > 0
                   )
                {
                    String[] correoEnvioArray = documentoCompra.cPE_CABECERA_BE.CORREO_OCULTO.Split(';');
                    foreach (String correoEnvio in correoEnvioArray)
                    {
                        correos.Add(correoEnvio.Trim());
                    }
                }

                MailService mailService = new MailService();
                mailService.enviar(correos,Constantes.ASUNTO_ANULACION_FACTURA,
                    body, Constantes.MAIL_COMUNICACION_FACTURAS, Constantes.PASSWORD_MAIL_COMUNICACION_FACTURAS, usuario);

               
            }

        }
        

        public DocumentoCompra descargarArchivoDocumentoCompra(DocumentoCompra documentoCompra)
        {
            IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
            client.Endpoint.Address = new EndpointAddress(uri);

            documentoCompra.rPTA_DOC_TRIB_BE = client.callExtractCPE(Constantes.USER_EOL, Constantes.PASSWORD_EOL, Constantes.RUC_MP, documentoCompra.cPE_CABECERA_BE.TIP_CPE, documentoCompra.cPE_CABECERA_BE.SERIE, documentoCompra.cPE_CABECERA_BE.CORRELATIVO, true, true, true); 
            /*
            String pathrootsave = System.AppDomain.CurrentDomain.BaseDirectory + "\\pdf\\";
            String nombreArchivo = "FACTURA " + documentoCompra.serie + "-" + documentoCompra.numero + ".pdf";
            File.WriteAllBytes(pathrootsave + nombreArchivo, documentoCompra.rPTA_DOC_TRIB_BE.DOC_TRIB_PDF);*/

            return documentoCompra;
        }










        public void InsertPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {

                pedido.seguimientoPedido.observacion = String.Empty;
                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Ingresado;

             /*   foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
                {
                    //pedidoDetalle.idPedido = pedido.idPedido;
                    pedidoDetalle.usuario = pedido.usuario;

                    //Si no es aprobador para que la cotización se cree como aprobada el porcentaje de descuento debe ser mayor o igual 
                    //al porcentaje Limite sin aprobacion


                 /*   if (!pedido.usuario.apruebaCotizaciones)
                    {
                        if (pedido.porcentajeDescuento > Constantes.PORCENTAJE_MAX_APROBACION)
                        {
                            cotizacion.seguimientoCotizacion.observacion = "Se aplicó un descuento superior al permitido en el detalle de la cotización";
                            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                        }
                    }
                    //Si es aprobador, no debe sobrepasar su limite de aprobación asignado
                    else
                    {
                        if (cotizacionDetalle.porcentajeDescuento > cotizacion.usuario.maximoPorcentajeDescuentoAprobacion)
                        {
                            cotizacion.seguimientoCotizacion.observacion = "Se aplicó un descuento superior al permitido en el detalle de la cotización.";
                            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                            
                        }

                    }*/

               // }
                dal.InsertPedido(pedido);
            }
        }



        public void UpdatePedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {

                pedido.seguimientoPedido.observacion = String.Empty;
                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Ingresado;

                foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
                {
                    pedidoDetalle.idPedido = pedido.idPedido;
                    //pedidoDetalle.usuario = pedido.usuario;

                    //Si no es aprobador para que la cotización se cree como aprobada el porcentaje de descuento debe ser mayor o igual 
                    //al porcentaje Limite sin aprobacion


                    /*   if (!pedido.usuario.apruebaCotizaciones)
                       {
                           if (pedido.porcentajeDescuento > Constantes.PORCENTAJE_MAX_APROBACION)
                           {
                               cotizacion.seguimientoCotizacion.observacion = "Se aplicó un descuento superior al permitido en el detalle de la cotización";
                               cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                           }
                       }
                       //Si es aprobador, no debe sobrepasar su limite de aprobación asignado
                       else
                       {
                           if (cotizacionDetalle.porcentajeDescuento > cotizacion.usuario.maximoPorcentajeDescuentoAprobacion)
                           {
                               cotizacion.seguimientoCotizacion.observacion = "Se aplicó un descuento superior al permitido en el detalle de la cotización.";
                               cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;

                           }

                       }*/

                }
                dal.UpdatePedido(pedido);
            }
        }

        public List<DocumentoCompra> GetFacturas(DocumentoCompra documentoCompra)
        {
            List<DocumentoCompra> documentoCompraList = null;
            using (var dal = new DocumentoCompraDAL())
            {/*
                //Si el usuario no es aprobador entonces solo buscará sus cotizaciones
                if (!pedido.usuario.apruebaCotizaciones)
                {
                    pedido.usuarioBusqueda = pedido.usuario;
                }*/

                documentoCompraList = dal.SelectDocumentosVenta(documentoCompra);
            }
            return documentoCompraList;
        }

        public Pedido GetPedido(Pedido pedido,Usuario usuario)
        {
            using (var dal = new PedidoDAL())
            {
                pedido = dal.SelectPedido(pedido,usuario);

            /*    if (pedido.mostrarValidezOfertaEnDias == 0)
                {
                    TimeSpan diferencia;
                    diferencia = cotizacion.fechaLimiteValidezOferta - cotizacion.fecha;
                    cotizacion.validezOfertaEnDias = diferencia.Days;
                }
                */
                foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
                {

                    if (pedidoDetalle.producto.image == null)
                    {
                        FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                        MemoryStream storeStream = new MemoryStream();
                        storeStream.SetLength(inStream.Length);
                        inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                        storeStream.Flush();
                        inStream.Close();
                        pedidoDetalle.producto.image = storeStream.GetBuffer();
                    }

                    //Si NO es recotizacion
                    if (pedido.incluidoIGV)
                    {
                        //Se agrega el IGV al precioLista
                        decimal precioSinIgv = pedidoDetalle.producto.precioSinIgv;
                        decimal precioLista = precioSinIgv + (precioSinIgv * pedido.montoIGV);
                        pedidoDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioLista));
                        //Se agrega el IGV al costoLista
                        decimal costoSinIgv = pedidoDetalle.producto.costoSinIgv;
                        decimal costoLista = costoSinIgv + (costoSinIgv * pedido.montoIGV);
                        pedidoDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costoLista));
                    }
                    else
                    {
                        //Se agrega el IGV al precioLista
                        pedidoDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedidoDetalle.producto.precioSinIgv));
                        //Se agrega el IGV al costoLista
                        pedidoDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedidoDetalle.producto.costoSinIgv));
                    }
                }
            }
            return pedido;
        }

        public void cambiarEstadoPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                dal.insertSeguimientoPedido(pedido);
            }

        }
    }
}
