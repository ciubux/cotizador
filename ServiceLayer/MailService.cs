using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EASendMail;
using System.Net.Mail;
using System.Net;

namespace ServiceLayer
{
    public class MailService
    {
        public void enviar(List<String> destinatarios,String asunto, String mensaje, String usermail, String password, Usuario usuario)
        {

            //SmtpMail oMail = new SmtpMail("TryIt");

            //// Your Hotmail email address

            //oMail.From = usermail;
            //// Set recipient email address
            //foreach (String destinatario in destinatarios)
            //    oMail.To.Add(destinatario);

            //// Set email subject
            //oMail.Subject = asunto;
            //// Set email body
            //oMail.HtmlBody = mensaje;
            //oMail.Priority = MailPriority.High;

            //// Hotmail SMTP server address
            //SmtpServer oServer = new SmtpServer(Constantes.SERVER_SMPTP);

            //// Hotmail user authentication should use your
            //// email address as the user name.
            //oServer.User = usermail;
            //oServer.Password = password;

            //// Set 587 port, if you want to use 25 port, please change 587 to 25
            //oServer.Port = Constantes.PUERTO_SERVER_SMPTP;

            //// detect SSL/TLS connection automatically
            //oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

            //SmtpClient oSmtp = new SmtpClient();
            //oSmtp.SendMail(oServer, oMail);

            
            System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();
            foreach(String destinatario in destinatarios)
                mmsg.To.Add(destinatario);
            mmsg.Subject = asunto;
            mmsg.SubjectEncoding = System.Text.Encoding.UTF8;
            mmsg.Body = mensaje;
            mmsg.BodyEncoding = System.Text.Encoding.UTF8;
            mmsg.IsBodyHtml = true; 
            mmsg.From = new System.Net.Mail.MailAddress(usermail);
            mmsg.Priority = System.Net.Mail.MailPriority.High;
            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                using (System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient())
                {
                    cliente.Credentials =
                        new System.Net.NetworkCredential(usermail, password);
                    cliente.Host = Constantes.SERVER_SMPTP;
                    cliente.Port = Constantes.PUERTO_SERVER_SMPTP;
                    cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
                    cliente.EnableSsl = true;
                    //cliente. .ConnectType = SmtpConnectType.ConnectSSLAuto;
                    //cliente.TargetName = "STARTTLS/smtp.office365.com";
                    cliente.Send(mmsg);
                }
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                int error = 1;
                //Log log = new Log("Error Envío Correo: "+ex.ToString(), TipoLog.Error, usuario);
                //LogBL logBL = new LogBL();
                //logBL.insertLog(log);
            }
        }

    }
}




