using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class MailService
    {
        public void enviar(List<String> destinatarios,String asunto, String mensaje, String usermail, String password, Usuario usuario)
        {
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
            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();
            cliente.Credentials =
                new System.Net.NetworkCredential(usermail, password);
            cliente.Host = Constantes.SERVER_SMPTP;
            cliente.Port = Constantes.PUERTO_SERVER_SMPTP;
            cliente.EnableSsl = true;

            try
            {
                cliente.Send(mmsg);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
             /*   Log log = new Log("Error Envío Correo: "+ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);*/
            }
        }

    }
}
