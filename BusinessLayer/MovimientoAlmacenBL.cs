
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using Model.EXCEPTION;
using System.Linq;
using Model.NextSoft;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using Framework.DAL;

namespace BusinessLayer
{
    public class MovimientoAlmacenBL
    {

        public void AnularMovimientoAlmacen(MovimientoAlmacen movimientoAlmacen)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                dal.AnularMovimientoAlmacen(movimientoAlmacen);
            }
        }

        public Guid getIdDocumentoVentaRelacionado(Guid idGuiaRemision)
        {
            using (MovimientoALmacenDAL dal = new MovimientoALmacenDAL())
            {
                return dal.getIdDocumentoVentaRelacionado(idGuiaRemision);
            }
        }

        public DocumentoVenta obtenerResumenConsolidadoAtenciones(List<Guid> idMovimientoAlmacenList)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                return dal.obtenerResumenConsolidadoAtenciones(idMovimientoAlmacenList);
            }
        }

        public List<GuiaRemision> obtenerDetalleConsolidadoAtenciones(List<Guid> idMovimientoAlmacenList, Dictionary<String,int> mostrarUnidadAlternativaList = null)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                return dal.obtenerDetalleConsolidadoAtenciones(idMovimientoAlmacenList, mostrarUnidadAlternativaList);
            }
        }

        public void UpdateMarcaNoEntregado(GuiaRemision guiaRemision)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                dal.UpdateMarcaNoEntregado(guiaRemision);
            }
        }



        public async System.Threading.Tasks.Task InsertMovimientoAlmacenSalida(GuiaRemision guiaRemision) 
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                

                guiaRemision.seguimientoMovimientoAlmacenSalida.observacion = String.Empty;
                guiaRemision.seguimientoMovimientoAlmacenSalida.estado = SeguimientoMovimientoAlmacenSalida.estadosSeguimientoMovimientoAlmacenSalida.Enviado;

                Boolean existeCantidadPendienteAtencion = false;
                foreach (DocumentoDetalle documentoDetalle in guiaRemision.pedido.documentoDetalle)
                {
                    if (documentoDetalle.cantidadPendienteAtencionPermitida < documentoDetalle.cantidadPorAtender)
                    {
                        documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadPendienteAtencionPermitida;
                    }


                    if (documentoDetalle.cantidadPendienteAtencion != documentoDetalle.cantidadPorAtender)
                    {
                        guiaRemision.atencionParcial = true;
                        guiaRemision.ultimaAtencionParcial = false;
                        existeCantidadPendienteAtencion = true;
                    }

                }


                if (!existeCantidadPendienteAtencion)
                {
                    guiaRemision.atencionParcial = false;
                    guiaRemision.ultimaAtencionParcial = true;
                }


                object dataSend = ConverterMPToNextSoft.toGuia(guiaRemision);

                try
                {
                    if (guiaRemision.serieDocumento.Substring(0, 2).Equals("T0") ||
                        guiaRemision.serieDocumento.Substring(0, 2).Equals("TT") ||
                        guiaRemision.serieDocumento.Substring(0, 2).Equals("TI"))
                    {
                        ClienteBL blCliente = new ClienteBL();

                        ClienteWS wsCli = new ClienteWS();
                        wsCli.urlApi = Constantes.NEXTSOFT_API_URL;
                        wsCli.apiToken = Constantes.NEXTSOFT_API_TOKEN;

                        Cliente clie = blCliente.getCliente(guiaRemision.clienteVer.idCliente);

                        object resultCli = await wsCli.crearCliente(ConverterMPToNextSoft.toCliente(clie));


                        GuiaWS ws = new GuiaWS();
                        ws.urlApi = Constantes.NEXTSOFT_API_URL;
                        ws.apiToken = Constantes.NEXTSOFT_API_TOKEN;
                        //object resultWs = await ws.crearGuia(guiaRemision);

                        
                        int success = 0;

                        object result = await ws.crearGuia(dataSend);

                        using (var logDAL = new LogDAL())
                        {
                            logDAL.insertLogWS("NEXTSYS_REGISTRO_GUIA_REMISION", JsonConvert.SerializeObject(dataSend), JsonConvert.SerializeObject(result), guiaRemision.IdUsuarioRegistro);
                        }


                        JObject dataResult = (JObject)result;
                        int codigo = dataResult["crearguiaResult"]["codigo"].Value<int>();

                        string resultText = JsonConvert.SerializeObject(result);

                        MovimientoAlmacenBL bl = new MovimientoAlmacenBL();
                        if (codigo == 0)
                        {
                            success = 1;
                            dal.InsertMovimientoAlmacenSalida(guiaRemision);

                            bl.GuardarRespuestaNextSys(guiaRemision.idMovimientoAlmacen, success, resultText);
                        }

                        //return JsonConvert.SerializeObject(new { success = success, dataSend = dataSend, result = result });
                    } 
                    {
                        if (guiaRemision.serieDocumento.Substring(0, 2).Equals("DF"))
                        {
                            dal.InsertMovimientoAlmacenSalida(guiaRemision);
                        }
                    }
                }
                catch (DuplicateNumberDocumentException ex)
                {
                    using (var logDAL = new LogDAL())
                    {
                        logDAL.insertLogWS("NEXTSYS_REGISTRO_GUIA_REMISION", JsonConvert.SerializeObject(dataSend), JsonConvert.SerializeObject(new {error = ex.Message }), guiaRemision.IdUsuarioRegistro);
                    }
                    throw ex;
                }
            }
        }

        public GuiaRemision InsertMovimientoAlmacenSalidaDesdeGuiaDiferida(Guid idGuiaDiferida, Guid idUsuario)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                return dal.InsertMovimientoAlmacenSalidaDesdeGuiaDiferida(idGuiaDiferida, idUsuario);
            }
        }

        public void InsertMovimientoAlmacenEntrada(NotaIngreso notaIngreso)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                notaIngreso.seguimientoMovimientoAlmacenEntrada.observacion = String.Empty;
                notaIngreso.seguimientoMovimientoAlmacenEntrada.estado = SeguimientoMovimientoAlmacenEntrada.estadosSeguimientoMovimientoAlmacenEntrada.Recibido;

                Boolean existeCantidadPendienteAtencion = false;
                Boolean existeDiferenciaConCantidadesOriginales = false;
                foreach (DocumentoDetalle documentoDetalle in notaIngreso.documentoDetalle)
                {
                    if (documentoDetalle.cantidadPendienteAtencion != documentoDetalle.cantidadPorAtender)
                    {
                        existeCantidadPendienteAtencion = true;
                        break;
                    }

                    if (documentoDetalle.cantidadPendienteAtencion != documentoDetalle.cantidad)
                    {
                        existeDiferenciaConCantidadesOriginales = true;
                        break;
                    }
                }

                if (!existeCantidadPendienteAtencion)
                {
                    notaIngreso.atencionParcial = false;
                    notaIngreso.ultimaAtencionParcial = true;


                    //Si se está extornando una guía, la no existen cantidades pendientes de devolución
                    //y el motivo es devolución por Item se cambia el motivo a devolución total
                    if (!existeDiferenciaConCantidadesOriginales && notaIngreso.guiaRemisionAExtornar != null && notaIngreso.motivoExtornoGuiaRemision == NotaIngreso.MotivosExtornoGuiaRemision.DevolucionItem)
                    {
                        notaIngreso.motivoExtornoGuiaRemision = NotaIngreso.MotivosExtornoGuiaRemision.DevolucionTotal;
                    }
                }


                try
                {
                    dal.InsertMovimientoAlmacenEntrada(notaIngreso);
                }
                catch (DuplicateNumberDocumentException ex)
                {
                    throw ex;
                }
            }
        }

        public void InsertAjusteAlmacen(GuiaRemision guia)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                dal.InsertAjusteAlmacen(guia);
            }
        }

        public void UpdateAjusteEstadoAprobado(GuiaRemision guia)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                dal.UpdateAjusteEstadoAprobado(guia);
            }
        }

        public List<GuiaRemision> GetGuiasRemision(GuiaRemision guiaRemision)
        {
            List<GuiaRemision> guiaRemisionList = null;
            using (var dal = new MovimientoALmacenDAL())
            {
                guiaRemision.usuario = guiaRemision.usuario;
                guiaRemisionList = dal.SelectGuiasRemision(guiaRemision);
            }
            return guiaRemisionList;
        }

        public List<GuiaRemision> BuscarAjustesAlmacen(GuiaRemision guiaRemision)
        {
            List<GuiaRemision> guiaRemisionList = null;
            using (var dal = new MovimientoALmacenDAL())
            {
                guiaRemisionList = dal.SelectAjustesAlmacen(guiaRemision);
            }
            return guiaRemisionList;
        }

        public GuiaRemision GetAjusteAlmacen(GuiaRemision guiaRemision)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                guiaRemision = dal.SelectAjusteAlmacen(guiaRemision);
            }
            return guiaRemision;
        }

        public List<NotaIngreso> GetNotasIngreso(NotaIngreso notaIngreso)
        {
            List<NotaIngreso> notaIngresoList = null;
            using (var dal = new MovimientoALmacenDAL())
            {
                // notaIngreso.usuario = notaIngreso.usuario;
                notaIngresoList = dal.SelectNotasIngreso(notaIngreso);
            }
            return notaIngresoList;
        }

        public List<MovimientoAlmacen> GetMovimientosAlmacenExtornantes(MovimientoAlmacen movimientoAlmacen)
        {
            List<MovimientoAlmacen> movimientoAlmacenList = new List<MovimientoAlmacen>();
            using (var dal = new MovimientoALmacenDAL())
            {
                movimientoAlmacenList = dal.SelectMovimientosAlmacenExtornantes(movimientoAlmacen);
            }
            return movimientoAlmacenList;
        }

        public List<GuiaRemision> GetGuiasRemisionGrupoCliente(GuiaRemision guiaRemision)
        {
            List<GuiaRemision> guiaRemisionList = null;
            using (var dal = new MovimientoALmacenDAL())
            {
                //Si el usuario no es aprobador entonces solo buscará sus cotizaciones
                /*   if (!pedido.usuario.apruebaCotizaciones)
                   {
                       pedido.usuarioBusqueda = pedido.usuario;
                   }*/
                guiaRemision.usuario = guiaRemision.usuario;
                guiaRemisionList = dal.SelectGuiasRemisionGrupoCliente(guiaRemision);
            }
            return guiaRemisionList;
        }

        

        public GuiaRemision GetGuiaRemision(GuiaRemision guiaRemision)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                guiaRemision = dal.SelectGuiaRemision(guiaRemision);
            }
            return guiaRemision;
        }

        public NotaIngreso GetNotaIngreso(NotaIngreso notaIngreso)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                notaIngreso = dal.SelectNotaIngreso(notaIngreso);
            }
            return notaIngreso;
        }

        public void cambiarEstadoPedido(Pedido pedido)
        {
            using (var dal = new PedidoDAL())
            {
                dal.insertSeguimientoPedido(pedido);
            }

        }

        public void obtenerCantidadesPorExtornar(MovimientoAlmacen movimientoAlmacen)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                List<DocumentoDetalle> documentoDetalleList = dal.SelectMovimientoAlmacenCantidadesExtornadas(movimientoAlmacen);

                foreach (DocumentoDetalle documentoDetalle in movimientoAlmacen.documentoDetalle)
                {

                    DocumentoDetalle documentoDetalleCantidadExtornada = documentoDetalleList.Where(d => d.producto.idProducto == documentoDetalle.producto.idProducto).FirstOrDefault();
                    if (documentoDetalleCantidadExtornada != null)
                    {
                        documentoDetalle.cantidadPendienteAtencion = documentoDetalle.cantidad - documentoDetalleCantidadExtornada.cantidad;
                        documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadPendienteAtencion;
                    }
                    else
                    {
                        documentoDetalle.cantidadPendienteAtencion = documentoDetalle.cantidad;
                        documentoDetalle.cantidadPorAtender = documentoDetalle.cantidad;
                    }
                }
            }
        }

        public Guid obtenerIdDocumentoVenta(MovimientoAlmacen movimientoAlmacen)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                return dal.SelectMovimientoAlmacenIdCpeCabeceraBe(movimientoAlmacen);
            }
        }

        public void GuardarRespuestaNextSys(Guid idGuiaRemision, int success, string resultText)
        {
            using (var dal = new MovimientoALmacenDAL())
            {
                dal.GuardarRespuestaNextSys(idGuiaRemision, success, resultText);
            }
        }
    }
}
