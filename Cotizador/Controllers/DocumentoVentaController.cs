using BusinessLayer;
using Model;
using Model.ServiceReferencePSE;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.IO.Compression;

namespace Cotizador.Controllers
{
    public class DocumentoVentaController : Controller
    {
        // GET: DocumentoVenta
        public ActionResult Index()
        {
            return View();
        }

        public String GenerarFactura(Guid idDocumentoVenta)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {


                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.idDocumentoVenta = idDocumentoVenta;
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.Factura;
                documentoVenta.usuario = usuario;
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


        public String GenerarNotaCredito(Guid idDocumentoVenta)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {


                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.idDocumentoVenta = idDocumentoVenta;
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaCrédito;
                documentoVenta.usuario = usuario;
                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = documentoVentaBL.procesarNotaCredito(documentoVenta);

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


        public String GenerarNotaDebito(Guid idDocumentoVenta)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {


                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.idDocumentoVenta = idDocumentoVenta;
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.NotaDébito;
                documentoVenta.usuario = usuario;
                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = documentoVentaBL.procesarNotaCredito(documentoVenta);

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


        public String GenerarBoletaVenta(Guid idDocumentoVenta)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            try
            {


                DocumentoVentaBL documentoVentaBL = new DocumentoVentaBL();
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.idDocumentoVenta = idDocumentoVenta;
                documentoVenta.tipoDocumento = DocumentoVenta.TipoDocumento.BoletaVenta;
                documentoVenta.usuario = usuario;
                CPE_RESPUESTA_BE cPE_RESPUESTA_BE = documentoVentaBL.procesarBoletaVenta(documentoVenta);

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


        public ActionResult ExportarCPEs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult exportStarsoftCPE()
        {
            String[] fechai = this.Request.Params["fechaInicio"].Split('/');
            DateTime fechaInicio = new DateTime(Int32.Parse(fechai[2]), Int32.Parse(fechai[1]), Int32.Parse(fechai[0]), 0, 0, 0);

            String[] fechaf = this.Request.Params["fechaFin"].Split('/');
            DateTime fechaFin = new DateTime(Int32.Parse(fechaf[2]), Int32.Parse(fechaf[1]), Int32.Parse(fechaf[0]), 23, 59, 59);

            DocumentoVentaBL bl = new DocumentoVentaBL();
            List<List<String>> cpes = bl.getCPEsExportStarsoft(fechaInicio, fechaFin);

            string mes = "";
            //switch (fechaInicio.Month())
            //{
            //    case 1: mes = "ENERO"; break;
            //    case 2: mes = "FEBRERO"; break;
            //    case 3: mes = "MARZO"; break;
            //    case 4: mes = "ABRIL"; break;
            //    case 5: mes = "MAYO"; break;
            //    case 6: mes = "JUNIO"; break;
            //    case 7: mes = "JULIO"; break;
            //    case 8: mes = "AGOSTO"; break;
            //    case 9: mes = "SEPTIEMBRE"; break;
            //    case 10: mes = "OCTUBRE"; break;
            //    case 11: mes = "NOVIEMBRE"; break;
            //    case 12: mes = "DICIEMBRE"; break;
            //}



            MemoryStream stream = new MemoryStream();
            int cantCPEs = 0;
            string nameFile = "";
            ZipArchiveEntry readmeEntry;

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                if (cpes.ElementAt(0).Count > 0)
                {
                    cantCPEs = cpes.ElementAt(0).Count / 3;
                    nameFile = "V_BOLETAS_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + "_ACEPTADAS_CANT_" + cantCPEs.ToString() + ".TXT";
                    readmeEntry = archive.CreateEntry(nameFile);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (String line in cpes.ElementAt(0))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }

                if (cpes.ElementAt(1).Count > 0)
                {
                    cantCPEs = cpes.ElementAt(1).Count / 3;
                    nameFile = "V_BOLETAS_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + "_ANULADAS_CANT_" + cantCPEs.ToString() + ".TXT";
                    readmeEntry = archive.CreateEntry(nameFile);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (String line in cpes.ElementAt(1))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }

                if (cpes.ElementAt(2).Count > 0)
                {
                    cantCPEs = cpes.ElementAt(2).Count / 3;
                    nameFile = "V_NOTAS_CREDITO_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + "_ACEPTADAS_CANT_" + cantCPEs.ToString() + ".TXT";
                    readmeEntry = archive.CreateEntry(nameFile);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (String line in cpes.ElementAt(2))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }

                if (cpes.ElementAt(3).Count > 0)
                {
                    cantCPEs = cpes.ElementAt(3).Count / 3;
                    nameFile = "V_NOTAS_CREDITO_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + "_ANULADAS_CANT_" + cantCPEs.ToString() + ".TXT";
                    readmeEntry = archive.CreateEntry(nameFile);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (String line in cpes.ElementAt(3))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }

                if (cpes.ElementAt(4).Count > 0)
                {
                    cantCPEs = cpes.ElementAt(4).Count / 3;
                    nameFile = "V_NOTAS_DEBITO_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + "_ACEPTADAS_CANT_" + cantCPEs.ToString() + ".TXT";
                    readmeEntry = archive.CreateEntry(nameFile);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (String line in cpes.ElementAt(4))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }

                if (cpes.ElementAt(5).Count > 0)
                {
                    cantCPEs = cpes.ElementAt(5).Count / 3;
                    nameFile = "V_NOTAS_DEBITO_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + "_ANULADAS_CANT_" + cantCPEs.ToString() + ".TXT";
                    readmeEntry = archive.CreateEntry(nameFile);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (String line in cpes.ElementAt(5))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }

                if (cpes.ElementAt(6).Count > 0)
                {
                    cantCPEs = cpes.ElementAt(6).Count / 3;
                    nameFile = "V_FACTURAS_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + "_GRAVADAS_ACEPTADAS_CANT_" + cantCPEs.ToString() + ".TXT";
                    readmeEntry = archive.CreateEntry(nameFile);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        for (int i = 0; i < 500; i++) { 
                            foreach (String line in cpes.ElementAt(6))
                            {
                                writer.WriteLine(line);
                            }
                                                                                                                                                                                                                                                                                        }


                    }
                }

                if (cpes.ElementAt(7).Count > 0)
                {
                    cantCPEs = cpes.ElementAt(7).Count / 3;
                    nameFile = "V_FACTURAS_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + "_GRAVADAS_ANULADAS_CANT_" + cantCPEs.ToString() + ".TXT";
                    readmeEntry = archive.CreateEntry(nameFile);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (String line in cpes.ElementAt(7))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }

                if (cpes.ElementAt(8).Count > 0)
                {
                    cantCPEs = cpes.ElementAt(8).Count / 3;
                    nameFile = "V_FACTURAS_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + "_GRAVADAS_CON_EXONERADAS_CANT_" + cantCPEs.ToString() + ".TXT";
                    readmeEntry = archive.CreateEntry(nameFile);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (String line in cpes.ElementAt(8))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }

                if (cpes.ElementAt(9).Count > 0)
                {
                    cantCPEs = cpes.ElementAt(9).Count / 3;
                    nameFile = "V_FACTURAS_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + "_GRAVADAS_CON_EXONERADAS_RECHAZADAS_CANT_" + cantCPEs.ToString() + ".TXT";
                    readmeEntry = archive.CreateEntry(nameFile);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (String line in cpes.ElementAt(9))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }

                if (cpes.ElementAt(10).Count > 0)
                {
                    cantCPEs = cpes.ElementAt(10).Count / 3;
                    nameFile = "V_FACTURAS_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + "_GRATUITAS_CANT_" + cantCPEs.ToString() + ".TXT";
                    readmeEntry = archive.CreateEntry(nameFile);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (String line in cpes.ElementAt(10))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }

                if (cpes.ElementAt(11).Count > 0)
                {
                    cantCPEs = cpes.ElementAt(11).Count / 3;
                    nameFile = "V_FACTURAS_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + "_GRATUITAS_ANULADAS_CANT_" + cantCPEs.ToString() + ".TXT";
                    readmeEntry = archive.CreateEntry(nameFile);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        foreach (String line in cpes.ElementAt(11))
                        {
                            writer.WriteLine(line);
                        }
                    }
                }
            }


            stream.Flush();
            stream.Position = 0;
            FileStreamResult result = new FileStreamResult(stream, "application/octet-stream");

            result.FileDownloadName = "ExportCPE_" + fechaInicio.ToString("yyyy_MM_dd") + "_" + fechaFin.ToString("yyyy_MM_dd") + ".zip";

            return result;
        }
    }
}