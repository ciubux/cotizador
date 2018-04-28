
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using Model.DTO;
using Model.ServiceReferencePSE;

namespace BusinessLayer
{
    public class DocumentoVentaBL
    {

        public void InsertarFactura(DocumentoVenta documentoVenta)
        {
            using (var dal = new DocumentoVentaDAL())
            {
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;


                dal.InsertarDocumentoVenta(documentoVenta);
                this.getDocumentoVenta(documentoVenta);
            }
        }


        public void getDocumentoVenta(DocumentoVenta documentoVenta)
        {
            using (var dal = new DocumentoVentaDAL())
            {
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
                documentoVenta = dal.SelectDocumentoVenta(documentoVenta);

                documentoVenta.globalEnumTipoOnline = GlobalEnumTipoOnline.Normal;

                var oUser = "demo@mp.eol.pe";
                var oPass = "00144f91";


                IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = client.callProcessOnline(oUser, oPass,
                    documentoVenta.cPE_CABECERA_BE,
                    documentoVenta.cPE_DETALLE_BEList.ToArray(),
                    documentoVenta.cPE_DAT_ADIC_BEList.ToArray(),
                    documentoVenta.cPE_DOC_REF_BEList.ToArray(),
                    documentoVenta.cPE_ANTICIPO_BEList.ToArray(),
                    documentoVenta.cPE_FAC_GUIA_BEList.ToArray(),
                    documentoVenta.cPE_DOC_ASOC_BEList.ToArray(),
                    documentoVenta.globalEnumTipoOnline);

                var CODIGO = cPE_RESPUESTA_BE.CODIGO;
                var COD_ESTD_SUNAT = cPE_RESPUESTA_BE.COD_ESTD_SUNAT;
                var DESCRIPCION = cPE_RESPUESTA_BE.DESCRIPCION;
                var DETALLE = cPE_RESPUESTA_BE.DETALLE;
                var NUM_CPE = cPE_RESPUESTA_BE.NUM_CPE;

                this.callStateCPE(documentoVenta.cPE_CABECERA_BE.SERIE, documentoVenta.cPE_CABECERA_BE.CORRELATIVO);
                this.callExtractCPE(documentoVenta.cPE_CABECERA_BE.SERIE, documentoVenta.cPE_CABECERA_BE.CORRELATIVO);


                //cPE_CABECERA_BE.MNT_TOT_DCTO



            }

        }


        public void callStateCPE(String SERIE, String CORRELATIVO)
        {
            IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            //102 103 codigos de estado aceptado SUNAT

            var oUser = "demo@mp.eol.pe";
            var oPass = "00144f91";

            //  var oUser = "ADMTAXTECH";
            //    var oPass = "T@xTech2018";
            var oNroIde = "20509411671";
            var oTipCpe = "01";
            var oSerCpe = SERIE;
            var oNroCpe = CORRELATIVO;

            RPTA_BE rpta_be = client.callStateCPE(oUser, oPass, oNroIde, oTipCpe, oSerCpe, oNroCpe);

            var codigo = rpta_be.CODIGO;
            var descripcion = rpta_be.DESCRIPCION;
            var detalle = rpta_be.DETALLE;
            var estado = rpta_be.ESTADO;

        }


        public void callExtractCPE(String SERIE, String CORRELATIVO)
        {
            IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            //102 103 codigos de estado aceptado SUNAT

            var oUser = "demo@mp.eol.pe";
            var oPass = "00144f91";
            var oNroIde = "20509411671";
            var oTipCpe = "01";
            var oSerCpe = SERIE;
            var oNroCpe = CORRELATIVO;
            var oFlgXml = true;
            var oFlgPdf = true;
            var oFlgCdr = true;


            RPTA_DOC_TRIB_BE rpta_doc_trib_be = client.callExtractCPE(oUser, oPass, oNroIde, oTipCpe, oSerCpe, oNroCpe, oFlgXml, oFlgPdf, oFlgCdr);


            var cod_rpta = rpta_doc_trib_be.COD_RPTA;
            var descripcion = rpta_doc_trib_be.DESCRIPCION;
            var detalle = rpta_doc_trib_be.DETALLE;
            byte[] doc_trib_pdf = rpta_doc_trib_be.DOC_TRIB_PDF;
            var doc_trib_rpta = rpta_doc_trib_be.DOC_TRIB_RPTA;
            var doc_trib_xml = rpta_doc_trib_be.DOC_TRIB_XML;
            var num_ope = rpta_doc_trib_be.NUM_OPE;

            String pathrootsave = System.AppDomain.CurrentDomain.BaseDirectory + "\\pdf\\";
            File.WriteAllBytes(pathrootsave + "FACTURA "+ SERIE + "-"+CORRELATIVO+".pdf", doc_trib_pdf);

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
