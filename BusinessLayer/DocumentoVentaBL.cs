﻿
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

        public DocumentoVenta InsertarDocumentoVenta(DocumentoVenta documentoVenta)
        {
            using (var dal = new DocumentoVentaDAL())
            {
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


        public DocumentoVenta InsertarNotaCredito(DocumentoVenta documentoVenta)
        {
            using (var dal = new DocumentoVentaDAL())
            {
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaCrédito;

            //    documentoVenta.movimientoAlmacen = new MovimientoAlmacen();
                //Se define el idMovimientoAlmacen como vacío para que tome el idventa
             //   documentoVenta.movimientoAlmacen.idMovimientoAlmacen = Guid.Empty;

                dal.InsertarDocumentoVentaNotaCreditoDebito(documentoVenta);

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
                    documentoVenta.cPE_RESPUESTA_BE.DETALLE = documentoVenta.tiposErrorValidacionString + ". " + documentoVenta.descripcionError;
                }
                return documentoVenta;
            }
        }


        public DocumentoVenta InsertarNotaDebito(DocumentoVenta documentoVenta)
        {
            using (var dal = new DocumentoVentaDAL())
            {
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaDébito;
                dal.InsertarDocumentoVentaNotaCreditoDebito(documentoVenta);

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
                    documentoVenta.cPE_RESPUESTA_BE.DETALLE = documentoVenta.tiposErrorValidacionString + ". " + documentoVenta.descripcionError;
                }
                return documentoVenta;
            }
        }

        public DocumentoVenta GetDocumentoVenta(DocumentoVenta documentoVenta)
        {
            using (var dal = new DocumentoVentaDAL())
            {
                return dal.SelectDocumentoVenta(documentoVenta);
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



        public CPE_RESPUESTA_BE procesarBoletaVenta(DocumentoVenta documentoVenta)
        {
            using (var dal = new DocumentoVentaDAL())
            {
                try
                {
                    documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.BoletaVenta;
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
                        documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.BoletaVenta;
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

        public CPE_RESPUESTA_BE procesarCPE(DocumentoVenta documentoVenta)
        {
            using (var dal = new DocumentoVentaDAL())
            {
                try
                {
                    documentoVenta = dal.SelectDocumentoVenta(documentoVenta);
                    //Se recupera el tipo de pago registrado
                    documentoVenta.tipoPago = (DocumentoVenta.TipoPago)Int32.Parse(documentoVenta.cPE_CABECERA_BE.TIP_PAG);

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
                    //Se inserta el resultado en Base de Datos
                    dal.UpdateRespuestaDocumentoVenta(documentoVenta);

                    //Si se procesa correctamente se actualiza el correlativo y los documentos internos y 
                    //Se consulta el estado del documento en SUNAT
                    if (documentoVenta.cPE_RESPUESTA_BE.CODIGO.Equals(Constantes.EOL_CPE_RESPUESTA_BE_CODIGO_OK))
                    {
                        if (documentoVenta.tipoDocumento == DocumentoVenta.TipoDocumento.Factura)
                        {
                            dal.UpdateSiguienteNumeroFactura(documentoVenta);
                        }
                        if (documentoVenta.tipoDocumento == DocumentoVenta.TipoDocumento.BoletaVenta)
                        {
                            dal.UpdateSiguienteNumeroBoleta(documentoVenta);
                        }
                        else if (documentoVenta.tipoDocumento == DocumentoVenta.TipoDocumento.NotaCrédito)
                        {
                            dal.UpdateSiguienteNumeroNotaCredito(documentoVenta);
                        }
                        else if (documentoVenta.tipoDocumento == DocumentoVenta.TipoDocumento.NotaDébito)
                        {
                            dal.UpdateSiguienteNumeroNotaDebito(documentoVenta);
                        }


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
            

            documentoVenta.rPTA_BE = client.callStateCPE(Constantes.USER_EOL, Constantes.PASSWORD_EOL, Constantes.RUC_MP, "0"+(int)documentoVenta.tipoDocumento, documentoVenta.serie, documentoVenta.numero);

            //El resultado se inserta a BD
            using (var dal = new DocumentoVentaDAL())
            {
                dal.insertEstadoDocumentoVenta(documentoVenta);
            }
        }

        public void anularDocumentoVenta(DocumentoVenta documentoVenta)
        {
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

            /*
            List<CPE_DOC_BAJA> CPE_DOC_BAJAList = new List<CPE_DOC_BAJA>();
            CPE_DOC_BAJA CPE_DOC_BAJA = new CPE_DOC_BAJA();
            CPE_DOC_BAJA.NRO_DOC = Constantes.RUC_MP;
            CPE_DOC_BAJA.TIP_DOC = "6";
            CPE_DOC_BAJA.TIP_CPE = "01";
            CPE_DOC_BAJA.FEC_EMI = documentoVenta.fechaEmision.Value.ToString(Constantes.formatoFechaCPE);
            CPE_DOC_BAJA.SERIE = documentoVenta.serie;
            CPE_DOC_BAJA.CORRELATIVO = documentoVenta.numero;
            CPE_DOC_BAJA.MTVO_BAJA = documentoVenta.comentarioAprobacionAnulacion;
            CPE_DOC_BAJAList.Add(CPE_DOC_BAJA);

           RPTA_BE[] rPTA_BEArray = client.CallRequestLow("179EB2F735F0B782C77F25426F0A1D2B0414945BA1E94B850764D722642358CB","14", Constantes.USER_EOL, Constantes.PASSWORD_EOL, CPE_DOC_BAJAList.ToArray());

          documentoVenta.rPTA_BE = rPTA_BEArray[0];            */
            //   = client.callStateCPE(Constantes.USER_EOL, Constantes.PASSWORD_EOL, Constantes.RUC_MP, "0" + (int)documentoVenta.tipoDocumento, documentoVenta.serie, documentoVenta.numero);

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

            documentoVenta.rPTA_DOC_TRIB_BE = client.callExtractCPE(Constantes.USER_EOL, Constantes.PASSWORD_EOL, Constantes.RUC_MP, documentoVenta.cPE_CABECERA_BE.TIP_CPE, documentoVenta.cPE_CABECERA_BE.SERIE, documentoVenta.cPE_CABECERA_BE.CORRELATIVO, true, true, true); 
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
