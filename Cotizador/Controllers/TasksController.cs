using BusinessLayer;
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
using Cotizador.Email;

namespace Cotizador.Controllers
{
    public class TasksController : Controller
    {


       
        public void EnviarEmailPedidosSinAtencion()
        {
            PedidoBL pedidoBL = new PedidoBL();
            List<Pedido> pedidosNotificar = pedidoBL.EnviarEmailAlertaPedidosNoEnviados();

            MailService mail = new MailService();

            int count = 0;
            foreach (Pedido pedido in pedidosNotificar)
            {
                if (pedido.cliente != null)
                {
                    PedidoSinAtencion emailTemplate = new PedidoSinAtencion();
                    String template = emailTemplate.BuildTemplate(pedido);
                    List<String> destinatarios = new List<String>();
                    /*
                    if (pedido.cliente.asistenteServicioCliente != null)
                    {
                        destinatarios.Add(pedido.cliente.asistenteServicioCliente.usuario.email);
                    }
                    if (pedido.cliente.responsableComercial != null)
                    {
                        destinatarios.Add(pedido.cliente.responsableComercial.usuario.email);
                    }
                    */
                    destinatarios.Add("yrvingrl520@gmail.com");
                    if (destinatarios.Count > 0 && count < 2)
                    {
                        mail.enviar(destinatarios, "El pedido " + pedido.numeroPedidoString + " no ha sido atendido", template, "yrvingtest456@gmail.com", "yrvingprueba", new Usuario());
                        count++;
                    }
                }
            }
        }
        
    }
}