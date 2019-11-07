using BusinessLayer;
using BusinessLayer.Email;
using Model;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class TasksController : Controller
    {


       
        public void EnviarEmailPedidosSinAtencion()
        {

            MailService mail = new MailService();
            try
            {
                PedidoBL pedidoBL = new PedidoBL();
                List<Pedido> pedidosNotificar = pedidoBL.EnviarEmailAlertaPedidosNoEnviados();

                

                int count = 0;
                foreach (Pedido pedido in pedidosNotificar)
                {
                    if (pedido.cliente != null)
                    {
                        var urlVerPedido = this.Url.Action("Index", "Pedido", new { idPedido = pedido.idPedido }, this.Request.Url.Scheme);
                        urlVerPedido = "http://zasmp.azurewebsites.net/Pedido?idPedido=" + pedido.idPedido.ToString();
                        PedidoSinAtencion emailTemplate = new PedidoSinAtencion();
                        emailTemplate.urlVerPedido = urlVerPedido;

                        String template = emailTemplate.BuildTemplate(pedido);
                        List<String> destinatarios = new List<String>();

                        Boolean seEnvioCorreo = false;
                        if (pedido.cliente.asistenteServicioCliente != null && pedido.cliente.asistenteServicioCliente.usuario != null
                            && pedido.cliente.asistenteServicioCliente.usuario.email != null && !pedido.cliente.asistenteServicioCliente.usuario.email.Equals(String.Empty))
                        {
                            destinatarios.Add(pedido.cliente.asistenteServicioCliente.usuario.email);
                            seEnvioCorreo = true;
                        }
                        if (pedido.cliente.responsableComercial != null && pedido.cliente.responsableComercial.usuario != null
                            && pedido.cliente.responsableComercial.usuario.email != null && !pedido.cliente.responsableComercial.usuario.email.Equals(String.Empty))
                        {
                            destinatarios.Add(pedido.cliente.responsableComercial.usuario.email);
                            seEnvioCorreo = true;
                        }
                        if (pedido.cliente.supervisorComercial != null && pedido.cliente.supervisorComercial.usuario != null
                            && pedido.cliente.supervisorComercial.usuario.email != null && !pedido.cliente.supervisorComercial.usuario.email.Equals(String.Empty))
                        {
                            destinatarios.Add(pedido.cliente.supervisorComercial.usuario.email);
                            seEnvioCorreo = true;
                        }
                        if (!pedido.usuario.email.Equals(String.Empty))
                        {
                            destinatarios.Add(pedido.usuario.email);
                            seEnvioCorreo = true;
                        }

                        if (!seEnvioCorreo)
                            destinatarios.Add("c.cornejo@mpinstitucional.com");

                        if (destinatarios.Count > 0)
                        {
                            String asunto = "El pedido " + pedido.numeroPedidoString;
                            if (pedido.seguimientoPedido.estado == SeguimientoPedido.estadosSeguimientoPedido.AtendidoParcialmente)
                            {
                                asunto = asunto + " ha sido atendido parcialmente";
                            }
                            else
                            {
                                asunto = asunto + " no ha sido atendido";
                            }

                            mail.enviar(destinatarios, asunto, template, Constantes.MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, Constantes.PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, new Usuario());
                            count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mail.enviar(new List<string> { "ti@mpinstitucional.com" }, "ERROR al revisar pedidos pendientes", ex.Message + ex.InnerException, Constantes.MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, Constantes.PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, new Usuario());
            }
        }

        public void AplicarCambiosProgramados()
        {
            LogCambioBL logCambioBL = new LogCambioBL();

            logCambioBL.aplicarLogCambios();
        }

        public void ActualizarEstadosDiario()
        {
            GlobalBL bl = new GlobalBL();

            bl.JobDiario();
        }
    }
}