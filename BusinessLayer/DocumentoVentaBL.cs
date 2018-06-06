
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using Model.DTO;
using Model.ServiceReferencePSE;
using System.ServiceModel;

namespace BusinessLayer
{
    public class DocumentoVentaBL
    {

        public DocumentoVenta InsertarFactura(DocumentoVenta documentoVenta)
        {
            using (var dal = new DocumentoVentaDAL())
            {
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
                dal.InsertarDocumentoVenta(documentoVenta);

                if (documentoVenta.tiposErrorValidacion == DocumentoVenta.TiposErrorValidacion.NoExisteError)
                {
                    //Se recupera el documento de venta creado para poder visualizarlo
                    documentoVenta = dal.SelectDocumentoVenta(documentoVenta);
                    documentoVenta.tipoPago = (DocumentoVenta.TipoPago)Int32.Parse(documentoVenta.cPE_CABECERA_BE.TIP_PAG);
                    documentoVenta.cPE_RESPUESTA_BE = new CPE_RESPUESTA_BE();
                    documentoVenta.cPE_RESPUESTA_BE.CODIGO = Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK;

                    

                }
                else
                {
                    documentoVenta.cPE_RESPUESTA_BE = new CPE_RESPUESTA_BE();
                    documentoVenta.cPE_RESPUESTA_BE.CODIGO = Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_ERROR_DATA;
                    documentoVenta.cPE_RESPUESTA_BE.DETALLE = documentoVenta.tiposErrorValidacionString+ ". "+ documentoVenta.descripcionError;
                }
                return documentoVenta;
            }
        }


        public void ActualizarEstadoDocumentosElectronicos(Usuario usuario)
        {
            using (var dal = new DocumentoVentaDAL())
            {
                List<DocumentoVenta> documentoVentaList = dal.SelectDocumentosVentaPorProcesar();

                foreach (DocumentoVenta documentoVenta in documentoVentaList)
                {
                    documentoVenta.usuario = usuario;
                    documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
                    this.consultarEstadoDocumentoVenta(documentoVenta);

                }
            }
        }



        public CPE_RESPUESTA_BE procesarNotaCredito(DocumentoVenta documentoVenta)
        {
            using (var dal = new DocumentoVentaDAL())
            {
                try
                {
                    documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaCrédito;
                    documentoVenta = dal.SelectDocumentoVenta(documentoVenta);
                    documentoVenta.globalEnumTipoOnline = GlobalEnumTipoOnline.Normal;
                    IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
                    Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
                    client.Endpoint.Address = new EndpointAddress(uri);


                    documentoVenta.cPE_RESPUESTA_BE = client.callProcessOnline(Constantes.USER_EOL, Constantes.PASSWORD_EOL,
                        documentoVenta.cPE_CABECERA_BE,
                        documentoVenta.cPE_DETALLE_BEList.ToArray(),
                        documentoVenta.cPE_DAT_ADIC_BEList.ToArray(),
                        documentoVenta.cPE_DOC_REF_BEList.ToArray(),
                        documentoVenta.cPE_ANTICIPO_BEList.ToArray(),
                        documentoVenta.cPE_FAC_GUIA_BEList.ToArray(),
                        documentoVenta.cPE_DOC_ASOC_BEList.ToArray(),
                        documentoVenta.globalEnumTipoOnline);
                    documentoVenta.serie = documentoVenta.cPE_CABECERA_BE.SERIE;
                    documentoVenta.numero = documentoVenta.cPE_CABECERA_BE.CORRELATIVO;


                    dal.UpdateRespuestaDocumentoVenta(documentoVenta);

                    //Si se procesa correctamente se actualiza el correlativo y los documentos internos y 
                    //Se consulta el estado del documento en SUNAT
                    if (documentoVenta.cPE_RESPUESTA_BE.CODIGO.Equals(Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK))
                    {
                        dal.UpdateSiguienteNumeroFactura(documentoVenta);
                        documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaCrédito;
                        consultarEstadoDocumentoVenta(documentoVenta);
                    }

                    return documentoVenta.cPE_RESPUESTA_BE;

                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }
        }



        public CPE_RESPUESTA_BE procesarFactura(DocumentoVenta documentoVenta)
        {
            using (var dal = new DocumentoVentaDAL())
            {
                try
                {
                    documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
                    documentoVenta = dal.SelectDocumentoVenta(documentoVenta);

                    //Se recupera el tipo de pago registrado
                    documentoVenta.tipoPago = (DocumentoVenta.TipoPago)Int32.Parse(documentoVenta.cPE_CABECERA_BE.TIP_PAG);

                    documentoVenta.globalEnumTipoOnline = GlobalEnumTipoOnline.Normal;
                    IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
                    Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
                    client.Endpoint.Address = new EndpointAddress(uri);

                //    documentoVenta.cPE_DAT_ADIC_BEList = new List<CPE_DAT_ADIC_BE>();

              /*      CPE_DAT_ADIC_BE observaciones = new CPE_DAT_ADIC_BE();
                    observaciones.COD_TIP_ADIC_SUNAT = "159";
                    observaciones.NUM_LIN_ADIC_SUNAT = "159";
                    observaciones.TXT_DESC_ADIC_SUNAT = documentoVenta.observaciones;

                    CPE_DAT_ADIC_BE codigoCliente = new CPE_DAT_ADIC_BE();
                    codigoCliente.COD_TIP_ADIC_SUNAT = "21";
                    codigoCliente.NUM_LIN_ADIC_SUNAT = "21";
                    codigoCliente.TXT_DESC_ADIC_SUNAT = documentoVenta.cliente.codigo;*/

              /*      documentoVenta.cPE_DAT_ADIC_BEList.Add(codigoCliente);
                    documentoVenta.cPE_DAT_ADIC_BEList.Add(observaciones);*/

                    //documentoVenta.cPE_DETALLE_BEList = documentoVenta.cPE_DETALLE_BEList.or

                    documentoVenta.cPE_RESPUESTA_BE = client.callProcessOnline(Constantes.USER_EOL, Constantes.PASSWORD_EOL,
                        documentoVenta.cPE_CABECERA_BE,
                        documentoVenta.cPE_DETALLE_BEList.ToArray(),
                        documentoVenta.cPE_DAT_ADIC_BEList.ToArray(),
                        documentoVenta.cPE_DOC_REF_BEList.ToArray(),
                        documentoVenta.cPE_ANTICIPO_BEList.ToArray(),
                        documentoVenta.cPE_FAC_GUIA_BEList.ToArray(),
                        documentoVenta.cPE_DOC_ASOC_BEList.ToArray(),
                        documentoVenta.globalEnumTipoOnline);
                    documentoVenta.serie = documentoVenta.cPE_CABECERA_BE.SERIE;
                    documentoVenta.numero = documentoVenta.cPE_CABECERA_BE.CORRELATIVO;
                    //Se inserta el resultado en Base de Datos
                    dal.UpdateRespuestaDocumentoVenta(documentoVenta);

                    //Si se procesa correctamente se actualiza el correlativo y los documentos internos y 
                    //Se consulta el estado del documento en SUNAT
                    if (documentoVenta.cPE_RESPUESTA_BE.CODIGO.Equals(Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK))
                    {
                        dal.UpdateSiguienteNumeroFactura(documentoVenta);
                        documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
                        consultarEstadoDocumentoVenta(documentoVenta);
                    }

                    return documentoVenta.cPE_RESPUESTA_BE;

                }
                catch(Exception ex)
                {
                    throw ex;
                   
                }
            }

        }


        public void consultarEstadoDocumentoVenta(DocumentoVenta documentoVenta)
        {
            IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
            client.Endpoint.Address = new EndpointAddress(uri);
            

            documentoVenta.rPTA_BE = client.callStateCPE(Constantes.USER_EOL, Constantes.PASSWORD_EOL, Constantes.RUC_MP, "0" + (int)documentoVenta.tipoDocumento, documentoVenta.serie, documentoVenta.numero);

            //El resultado se inserta a BD
            using (var dal = new DocumentoVentaDAL())
            {
                dal.insertEstadoDocumentoVenta(documentoVenta);
            }
        }

        public void anularDocumentoVenta(DocumentoVenta documentoVenta)
        {
            IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
            client.Endpoint.Address = new EndpointAddress(uri);
          

          //  documentoVenta.rPTA_BE = client.callStateCPE(Constantes.USER_EOL, Constantes.PASSWORD_EOL, Constantes.RUC_MP, "0" + (int)documentoVenta.tipoDocumento, documentoVenta.serie, documentoVenta.numero);

            //El resultado se inserta a BD
            using (var dal = new DocumentoVentaDAL())
            {
                dal.anularDocumentoVenta(documentoVenta);
            }
        }

        public void aprobarAnulacionDocumentoVenta(DocumentoVenta documentoVenta)
        {
            IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
            client.Endpoint.Address = new EndpointAddress(uri);


            //  documentoVenta.rPTA_BE = client.callStateCPE(Constantes.USER_EOL, Constantes.PASSWORD_EOL, Constantes.RUC_MP, "0" + (int)documentoVenta.tipoDocumento, documentoVenta.serie, documentoVenta.numero);

            //El resultado se inserta a BD
            using (var dal = new DocumentoVentaDAL())
            {
                dal.aprobarAnulacionDocumentoVenta(documentoVenta);
            }
        }
        

        public DocumentoVenta descargarArchivoDocumentoVenta(DocumentoVenta documentoVenta)
        {
            IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            Uri uri = new Uri(Constantes.ENDPOINT_ADDRESS_EOL);
            client.Endpoint.Address = new EndpointAddress(uri);

            documentoVenta.rPTA_DOC_TRIB_BE = client.callExtractCPE(Constantes.USER_EOL, Constantes.PASSWORD_EOL, Constantes.RUC_MP, "0" + (int)documentoVenta.tipoDocumento, documentoVenta.serie, documentoVenta.numero, true, true, true); 
            /*
            String pathrootsave = System.AppDomain.CurrentDomain.BaseDirectory + "\\pdf\\";
            String nombreArchivo = "FACTURA " + documentoVenta.serie + "-" + documentoVenta.numero + ".pdf";
            File.WriteAllBytes(pathrootsave + nombreArchivo, documentoVenta.rPTA_DOC_TRIB_BE.DOC_TRIB_PDF);*/

            return documentoVenta;
        }










        public void InsertPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {

                pedido.seguimientoPedido.observacion = String.Empty;
                pedido.seguimientoPedido.estado = SeguimientoPedido.estadosSeguimientoPedido.Ingresado;

                foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
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

                }
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

                }
                dal.UpdatePedido(pedido);
            }
        }

        public List<DocumentoVenta> GetFacturas(DocumentoVenta documentoVenta)
        {
            List<DocumentoVenta> documentoVentaList = null;
            using (var dal = new DocumentoVentaDAL())
            {/*
                //Si el usuario no es aprobador entonces solo buscará sus cotizaciones
                if (!pedido.usuario.apruebaCotizaciones)
                {
                    pedido.usuarioBusqueda = pedido.usuario;
                }*/

                documentoVentaList = dal.SelectDocumentosVenta(documentoVenta);
            }
            return documentoVentaList;
        }

        public Pedido GetPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                pedido = dal.SelectPedido(pedido);

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
