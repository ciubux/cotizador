using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using Model;
using Model.WEB;
using MimeKit;
using Newtonsoft.Json;
using System.IO;
using static NPOI.HSSF.Util.HSSFColor;
using System.Net;

namespace BusinessLayer
{
    public class WebBl
    {
        public async Task LeerNuevasCotizacionesMail(Guid idUsuario)
        {
            ParametroDAL dal = new ParametroDAL();

            string host = "outlook.office365.com";
            int puerto = 993;
            string email = dal.getParametro("WEB_MAIL_COTIZACIONES");
            string password = dal.getParametro("WEB_MAIL_COTIZACIONES_PASSWORD");
            
            DateTime fechaDesde = new DateTime(2025, 6, 1, 0, 0, 0);
            string parteAsunto = "Solicitud de cotización Web de";

            List<CotizacionWebMail> cotizacionesWeb = await ObtenerCotizacionesMail(host, puerto, email, password, fechaDesde, parteAsunto); 

            if (cotizacionesWeb.Count > 0) {
                CotizacionDAL dalCotizacion = new CotizacionDAL();
                dalCotizacion.InsertarCotizacionesWeb(cotizacionesWeb, idUsuario);
            }
        }

        public async Task<List<CotizacionWebMail>> ObtenerCotizacionesMail(string imapHost, int imapPort, string email, string password, DateTime sinceDate, string subjectKeyword)
        {
            List<CotizacionWebMail> lista = new List<CotizacionWebMail>();
            // Usamos 'using' para asegurar que el cliente se desconecte y libere recursos
            using (var client = new ImapClient())
            {
                //try
                //{
                // Conexión y autenticacion 
                    //System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
                    client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                    await client.ConnectAsync(imapHost, imapPort, true);
                    await client.AuthenticateAsync(email, password);

                    // Buscar correos que llegaron DESPUÉS de la fecha Y que contienen el texto en el asunto
                    await client.Inbox.OpenAsync(FolderAccess.ReadOnly);
                    var searchQuery = SearchQuery.And(
                        SearchQuery.SentSince(sinceDate.Date), // .Date para ignorar la hora
                        SearchQuery.SubjectContains(subjectKeyword)
                    );

                    var uids = await client.Inbox.SearchAsync(searchQuery);

                    foreach (var uid in uids)
                    {
                        var message = await client.Inbox.GetMessageAsync(uid);
                        Console.WriteLine($"\nProcessing email: '{message.Subject}' from {message.Date}");

                        // obtener archivos adjuntos del correo
                        foreach (var attachment in message.Attachments)
                        {
                            // Nos aseguramos de que el adjunto sea un archivo json
                            if (attachment is MimePart mimePart && mimePart.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine($"  -> Archivo JSON encontrado: {mimePart.FileName}");

                                // Leer y procesar el contenido del JSON
                                using (var memoryStream = new MemoryStream())
                                {
                                    await mimePart.Content.DecodeToAsync(memoryStream);
                                    memoryStream.Position = 0; // Crucial: resetear la posición del stream

                                    using (var reader = new StreamReader(memoryStream))
                                    {
                                        string jsonText = await reader.ReadToEndAsync();
                                        String[] partesNombre = mimePart.FileName.Split('-');

                                        CotizacionWebMail cotizacion = JsonConvert.DeserializeObject<CotizacionWebMail>(jsonText);

                                        if (cotizacion != null)
                                        {
                                            cotizacion.apply_date = message.Date.DateTime;
                                            lista.Add(cotizacion);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    await client.DisconnectAsync(true);
                /*}
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Ocurrió un error: {ex.Message}");
                }*/
            }

            return lista;
        }
    }
}
