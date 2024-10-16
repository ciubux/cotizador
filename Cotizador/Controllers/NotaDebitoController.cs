﻿using BusinessLayer;
using Model;
using Model.ServiceReferencePSE;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class NotaDebitoController : Controller
    {
   
        public ActionResult Crear()
        {
            Transaccion venta = (Transaccion)this.Session[Constantes.VAR_SESSION_NOTA_DEBITO];
            ViewBag.venta = venta;
            ViewBag.tipoNotaDebito = (int)venta.tipoNotaDebito;           

            ViewBag.fechaEmision = venta.documentoVenta.fechaEmision.Value.ToString(Constantes.formatoFecha);
            ViewBag.horaEmision = venta.documentoVenta.fechaEmision.Value.ToString(Constantes.formatoHora);  

            return View();
        }




        public String iniciarCreacion(String[] cargos)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            VentaBL ventaBL = new VentaBL();
            Transaccion transaccion = new Venta();
            
            transaccion.documentoVenta = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_FACTURA_VER];
            transaccion.documentoVenta.venta = null;
            transaccion.documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaDébito;

            transaccion.documentoVenta.idDocumentoVenta = Guid.Parse(Request["idDocumentoVenta"].ToString());
            
            transaccion.documentoReferencia = new DocumentoReferencia();
            transaccion.documentoReferencia.tipoDocumento = (DocumentoVenta.TipoDocumento)Int32.Parse(transaccion.documentoVenta.cPE_CABECERA_BE.TIP_CPE);
            String[] fechaEmisionArray = transaccion.documentoVenta.cPE_CABECERA_BE.FEC_EMI.Split('-');
            transaccion.documentoReferencia.fechaEmision = new DateTime(Int32.Parse(fechaEmisionArray[0]), Int32.Parse(fechaEmisionArray[1]), Int32.Parse(fechaEmisionArray[2]));
            transaccion.documentoReferencia.serie = transaccion.documentoVenta.cPE_CABECERA_BE.SERIE;
            transaccion.documentoReferencia.numero = transaccion.documentoVenta.cPE_CABECERA_BE.CORRELATIVO;

            DocumentoVenta.TiposNotaDebito tiposNotaDebito = (DocumentoVenta.TiposNotaDebito)Int32.Parse(Request["tipoNotaDebito"].ToString());

            if (tiposNotaDebito != DocumentoVenta.TiposNotaDebito.AumentoValor)
            {
                List<Guid> idProductoList = new List<Guid>();
                foreach (String cargo in cargos)
                {
                    idProductoList.Add(Guid.Parse(cargo));
                }
                transaccion = ventaBL.GetPlantillaVentaCargos(transaccion, usuario, idProductoList);
            }
            else
            {
                transaccion = ventaBL.GetPlantillaVenta(transaccion, usuario);
            }

            


            
            if (transaccion.tipoErrorCrearTransaccion == Venta.TiposErrorCrearTransaccion.NoExisteError)
            {
                transaccion.tipoNotaDebito = (DocumentoVenta.TiposNotaDebito)Int32.Parse(Request["tipoNotaDebito"].ToString());
                transaccion.documentoVenta.fechaEmision = DateTime.Now;

                //Temporal
                Pedido pedido = transaccion.pedido;
                pedido.ciudadASolicitar = new Ciudad();

                PedidoBL pedidoBL = new PedidoBL();
                pedidoBL.calcularMontosTotales(pedido);
                this.Session[Constantes.VAR_SESSION_NOTA_DEBITO] = transaccion;
            }
            return JsonConvert.SerializeObject(transaccion);
            
        }


        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_NOTA_DEBITO];
            IDocumento documento = (Pedido)venta.pedido;
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            PedidoBL pedidoBL = new PedidoBL();
            pedidoBL.calcularMontosTotales((Pedido)documento);
            venta.pedido = (Pedido)documento;
            this.Session[Constantes.VAR_SESSION_NOTA_DEBITO] = venta;
            return "{\"cantidad\":\"" + documento.documentoDetalle.Count + "\"}";
        }


        public void ChangeInputString()
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_NOTA_DEBITO];
            PropertyInfo propertyInfo = venta.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(venta, this.Request.Params["valor"]);
            this.Session[Constantes.VAR_SESSION_NOTA_DEBITO] = venta;
        }

        public void ChangeFechaEmision()
        {
            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_NOTA_DEBITO];
            String[] fechaEmision = this.Request.Params["fechaEmision"].Split('/');
            String[] horaEmision = this.Request.Params["horaEmision"].Split(':');
            venta.documentoVenta.fechaEmision = new DateTime(Int32.Parse(fechaEmision[2]), Int32.Parse(fechaEmision[1]), Int32.Parse(fechaEmision[0]), Int32.Parse(horaEmision[0]), Int32.Parse(horaEmision[1]), 0);
            this.Session[Constantes.VAR_SESSION_NOTA_DEBITO] = venta;
        }

        public String Create()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_NOTA_DEBITO];

                String[] fecha = this.Request.Params["fechaEmision"].Split('/');
                String[] hora = this.Request.Params["horaEmision"].Split(':');

                venta.observaciones = this.Request.Params["observaciones"];
                venta.sustento = this.Request.Params["sustento"];

                
                venta.documentoVenta.fechaEmision = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]), Int32.Parse(hora[0]), Int32.Parse(hora[1]), 0);
                ////La fecha de vencimiento es identifica a la fecha de emisión
                venta.documentoVenta.fechaVencimiento = venta.documentoVenta.fechaEmision.Value;
                ////El clasePedido de pago es al contado
                venta.documentoVenta.tipoPago = DocumentoVenta.TipoPago.Contado;
                ////La forma de Pago es Efectivo
                venta.documentoVenta.formaPago = DocumentoVenta.FormaPago.Efectivo;
                venta.documentoVenta.usuario = usuario;
                
                venta.usuario = usuario;
                VentaBL ventaBL = new VentaBL();
                ventaBL.InsertVentaNotaDebito(venta);

                //Falta Agregar validación de creación de venta

                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                //Se crea una venta solo para que pueda enviar 
                venta.documentoVenta.venta = new Venta();
                venta.documentoVenta.venta.idVenta = venta.idVenta;
                venta.documentoVenta.venta.documentoReferencia = venta.documentoReferencia;
                venta.documentoVenta.cliente = venta.cliente;
                venta.documentoVenta = documentoVentaBL.InsertarNotaDebito(venta.documentoVenta);
                venta.documentoVenta.tipoNotaDebito = (DocumentoVenta.TiposNotaDebito)Int32.Parse(venta.documentoVenta.cPE_CABECERA_BE.COD_TIP_ND);
                return JsonConvert.SerializeObject(venta.documentoVenta);
            }
            catch (Exception ex)
            {
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
                return ex.ToString();
            }
        }


        public String ConfirmarCreacion()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {
                Transaccion venta = (Transaccion)this.Session[Constantes.VAR_SESSION_NOTA_DEBITO];
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.venta = (Venta)venta;
                documentoVenta.idDocumentoVenta = Guid.Parse(this.Request.Params["idDocumentoVenta"]);
                documentoVenta.cliente = venta.cliente;
                documentoVenta.usuario = usuario;

                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaDébito;
                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = documentoVentaBL.procesarCPE(documentoVenta);

                var otmp = new
                {
                    CPE_RESPUESTA_BE = cPE_RESPUESTA_BE,
                    serieNumero = documentoVenta.serieNumero,
                    idDocumentoVenta = documentoVenta.idDocumentoVenta
                };

                return JsonConvert.SerializeObject(otmp);
            }
            catch (Exception ex)
            {
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
                return ex.ToString();
            }
        }

    }
}


