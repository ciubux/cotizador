using BusinessLayer;
using Model;
using Spire.Pdf;
using Spire.Pdf.Annotations;
using Spire.Pdf.Graphics;
using Spire.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cotizadorPDF
{

    sealed class SampleEventSourceWriter : EventSource
    {
        public static SampleEventSourceWriter Log = new SampleEventSourceWriter();
         public void MessageMethod(string Message) { WriteEvent(2, Message); }
       

    }
    public class GeneradorPDF
    {
        public String generarPDFExtended(Cotizacion cot)
        {
            try
            {
                char pad = '0';
                int cantidadPad = 10;

                int sepLine = 12;
                SampleEventSourceWriter.Log.MessageMethod("This is a message.");
                PdfDocument doc = new PdfDocument();
                PdfPageBase page = doc.Pages.Add(PdfPageSize.A4);

                PdfImage image = PdfImage.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\images\\logo.png");
                float width = 80 * 2.4f;
                float height = 20 * 2.4f;
                page.Canvas.DrawImage(image, 0, 0, width, height);

                PdfImage imageCli = PdfImage.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\images\\proveedores.png");
                width = 75 * 2.4f;
                height = 70 * 2.4f;
                page.Canvas.DrawImage(imageCli, 335, 0, width, height);

                float y = 60;
                string mes = "";
                switch (cot.fecha.Month)
                {
                    case 1: mes = "Enero"; break;
                    case 2: mes = "Febrero"; break;
                    case 3: mes = "Marzo"; break;
                    case 4: mes = "Abril"; break;
                    case 5: mes = "Mayo"; break;
                    case 6: mes = "Junio"; break;
                    case 7: mes = "Julio"; break;
                    case 8: mes = "Agosto"; break;
                    case 9: mes = "Septiembre"; break;
                    case 10: mes = "Octubre"; break;
                    case 11: mes = "Noviembre"; break;
                    case 12: mes = "Diciembre"; break;
                }

                page.Canvas.DrawString("Número de Cotización: "+ cot.codigo.ToString().PadLeft(cantidadPad, pad), new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine;
                string fecha = cot.fecha.Day + " de " + mes + " de " + cot.fecha.Year;
                page.Canvas.DrawString(cot.ciudad.nombre + " " + fecha, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine * 2;
                page.Canvas.DrawString("Señores: ", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine;

                if (cot.cliente.idCliente != Guid.Empty)
                {
                    page.Canvas.DrawString(cot.cliente.razonSocial, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Blue), 0, y);
                    y = y + sepLine;
                    page.Canvas.DrawString("RUC: " + cot.cliente.ruc, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Blue), 0, y);
                    y = y + sepLine + 2;
                }
                else
                {
                    page.Canvas.DrawString(cot.grupo.nombre, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Blue), 0, y);
                    y = y + sepLine;
                }

                int count = doc.Pages.Count;

                page.Canvas.DrawString("Ciudad.- " + cot.ciudad.nombre, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine * 2;
                page.Canvas.DrawString("Atn.- " + cot.contacto, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + 3 + sepLine * 2;

                page.Canvas.DrawString("De nuestra consideración:", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine;
                page.Canvas.DrawString("Por la presente, nos es grato someter a su consideración la siguiente cotización:", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine * 2;

                PdfBrush brush1 = PdfBrushes.Black;
           //     PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("Helvetica", 16f, FontStyle.Bold));
          //      PdfStringFormat format1 = new PdfStringFormat(PdfTextAlignment.Center);


                PdfTable table = new PdfTable();
                table.Style.CellPadding = 2;
                table.Style.BorderPen = new PdfPen(brush1, 0.75f);
                table.Style.DefaultStyle.BackgroundBrush = PdfBrushes.White;
              //  table.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("Helvetica", 8f));
                table.Style.AlternateStyle = new PdfCellStyle();
                table.Style.AlternateStyle.BackgroundBrush = PdfBrushes.WhiteSmoke;
            //    table.Style.AlternateStyle.Font = new PdfTrueTypeFont(new Font("Helvetica", 8f));
                table.Style.HeaderSource = PdfHeaderSource.ColumnCaptions;
                //table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.CadetBlue;
                table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.CadetBlue;
             //   table.Style.HeaderStyle.Font = new PdfTrueTypeFont(new Font("Helvetica", 8f, FontStyle.Bold));
                table.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                table.Style.ShowHeader = true;


                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Cod.");
                dataTable.Columns.Add("Producto");
                dataTable.Columns.Add("Presentación");
                dataTable.Columns.Add("Imagen");
                //   dataTable.Columns.Add("Precio Unit. Anterior");
                dataTable.Columns.Add("Precio Unit.");
                dataTable.Columns.Add("Cant.");
                dataTable.Columns.Add("Subtotal");
                dataTable.Columns.Add(new DataColumn("Temp1", typeof(Byte[])));
                dataTable.Columns.Add(new DataColumn("Temp2", typeof(PdfImage)));

                foreach (CotizacionDetalle det in cot.cotizacionDetalleList)
                {
                    String codigo = det.producto.sku.Trim();
                    String producto = det.producto.descripcion;
                    String presentacion = det.unidad; //Se muestra la unidad seleccionada y que se encuentra en el detalle
                    String imagen = "";
                    String precioUnitarioAnterior = "";
                    String precioUnitarioNuevo = Constantes.simboloMonedaSol + " " + String.Format(Constantes.decimalFormat, det.precioUnitario);
                    String cantidad = det.cantidad.ToString();
                    String subtotal = Constantes.simboloMonedaSol + " " + String.Format(Constantes.decimalFormat, det.subTotal);

                    dataTable.Rows.Add(new object[] { codigo, producto, presentacion, imagen,

                  precioUnitarioNuevo, cantidad, subtotal,det.producto.image });

                }


                table.DataSource = dataTable;

                float width1
                  = page.Canvas.ClientSize.Width
                      - (table.Columns.Count + 1) * table.Style.BorderPen.Width;
                //*Cod.
                table.Columns[0].Width = width1 * 0.08f;
                table.Columns[0].StringFormat
                    = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                //Producto
                table.Columns[1].Width = width1 * 0.35f;
                table.Columns[1].StringFormat
                    = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                //Presentación
                table.Columns[2].Width = width1 * 0.15f;
                table.Columns[2].StringFormat
                    = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                //Imagen
                table.Columns[3].Width = width1 * 0.15f;
                table.Columns[3].StringFormat
                    = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                //Nuevo Valor Unit
                table.Columns[4].Width = width1 * 0.10f;
                table.Columns[4].StringFormat
                    = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                //Cant.
                table.Columns[5].Width = width1 * 0.10f;
                table.Columns[5].StringFormat
                    = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                //Subtotal
                table.Columns[6].Width = width1 * 0.10f;
                table.Columns[6].StringFormat
                    = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);


                table.BeginRowLayout += new BeginRowLayoutEventHandler(table_BeginRowLayout);
                table.EndCellLayout += new EndCellLayoutEventHandler(table_EndCellLayout);

                PdfTableLayoutFormat tableLayout = new PdfTableLayoutFormat();
                tableLayout.Break = PdfLayoutBreakType.FitElement;
                tableLayout.Layout = PdfLayoutType.Paginate;

                if (cot.considerarCantidades)
                {
                    tableLayout.EndColumnIndex = table.Columns.Count - 3; //- 1;
                }
                else
                {
                    tableLayout.EndColumnIndex = table.Columns.Count - 5;
                }

                PdfLayoutResult result = table.Draw(page, new PointF(0, y), tableLayout);
                y = y + result.Bounds.Height + 5;

                y = y + 5;

                PdfSolidBrush brushColorBlue = new PdfSolidBrush(Color.Blue);



                int countPages = doc.Pages.Count;


                int margenTop = 40;
                int margenLeft = 45;

                PdfPageBase sectionTotales = page;
                PdfPageBase sectionObervaciones = page;
                PdfPageBase sectionFirma = page;
                int xPage2 = 0;
                int xPage2a = 0;
                String reiniciarY = "";

                //Si son dos paginas entonces se obtiene la página y se obtiene cuantos registros
                //mayores a 10 son
                if (countPages == 2)
                {
                    y = margenTop;

                    if (cot.cotizacionDetalleList.Count > 10)
                    {
                        y = y + (60 * (cot.cotizacionDetalleList.Count - 10));
                    }

                    sectionTotales = doc.Pages[1];
                    sectionObervaciones = doc.Pages[1];
                    sectionFirma = doc.Pages[1];
                    xPage2 = margenLeft;
                }
                else
                {
                    if(cot.cotizacionDetalleList.Count > 5)
                    { 

                        SizeF size = page.Size;
                        PdfPageBase page2 = doc.Pages.Add(size, new PdfMargins(0));

                        switch (cot.cotizacionDetalleList.Count)
                        {
                            case 6:
                                reiniciarY = "FIRMA";
                                sectionTotales = page;
                                sectionObervaciones = page;
                                sectionFirma = page2; break;
                            case 7:
                                reiniciarY = "OBSERVACIONES";
                                sectionTotales = page;
                                sectionObervaciones = page2;
                                sectionFirma = page2; break;
                            case 8:
                                reiniciarY = "OBSERVACIONES";
                                sectionTotales = page;
                                sectionObervaciones = page2;
                                sectionFirma = page2; break;
                            case 9:
                                reiniciarY = "OBSERVACIONES";
                                sectionTotales = page;
                                sectionObervaciones = page2;
                                sectionFirma = page2; break;
                            case 10:
                                reiniciarY = "TOTALES";
                                sectionTotales = page2;
                                sectionObervaciones = page2;
                                sectionFirma = page2; break;

                        }
                    }
                }





                if (cot.considerarCantidades)
                {
                    if (reiniciarY.Equals("TOTALES"))
                    {
                        y = margenTop;
                        xPage2 = margenLeft;
                    }

                    PdfTable tableTotales = new PdfTable();
                    tableTotales.Style.CellPadding = 2;
                    tableTotales.Style.BorderPen = new PdfPen(PdfBrushes.Transparent, 0f);
                    tableTotales.Style.DefaultStyle.BorderPen = new PdfPen(PdfBrushes.Transparent, 0f);
                    tableTotales.Style.DefaultStyle.BackgroundBrush = PdfBrushes.White;
                    //   tableTotales.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font(new FontFamily(GenericFontFamilies.Serif), 9f, FontStyle.Bold));
                    // tableTotales.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("Arial", 9f, FontStyle.Bold));

                    tableTotales.Style.DefaultStyle.TextBrush = brushColorBlue;
                    tableTotales.Style.AlternateStyle = new PdfCellStyle();
                    tableTotales.Style.AlternateStyle.BackgroundBrush = PdfBrushes.White;
                    tableTotales.Style.AlternateStyle.BorderPen = new PdfPen(PdfBrushes.Transparent, 0f);
                    tableTotales.Style.AlternateStyle.TextBrush = brushColorBlue;

                    //  tableTotales.Style.AlternateStyle.Font = new PdfTrueTypeFont(new Font(new FontFamily(GenericFontFamilies.Serif), 9f, FontStyle.Bold));
                    tableTotales.Style.HeaderSource = PdfHeaderSource.ColumnCaptions;
                    //table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.CadetBlue;
                    tableTotales.Style.HeaderStyle.BackgroundBrush = PdfBrushes.White;
                    //   table.Style.HeaderStyle.Font = new PdfTrueTypeFont(new Font("Helvetica", 8f, FontStyle.Bold));
                    tableTotales.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
                    tableTotales.Style.ShowHeader = false;
                  //  tableTotales.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("Helvetica", 8f, FontStyle.Bold));
                    //tableTotales.Style. = false;


                    DataTable dataTable2 = new DataTable();
                    dataTable2.Columns.Add("Descripcion");
                    dataTable2.Columns.Add("monto");

                    String subtotal = "Subtotal: " + Constantes.simboloMonedaSol + ": ";
                    String montoSubtotal = String.Format(Constantes.decimalFormat, cot.montoSubTotal);
                    dataTable2.Rows.Add(new object[] { subtotal, montoSubtotal });

                    String igv = "IGV 18%: " + Constantes.simboloMonedaSol + ": ";
                    String montoIGV = String.Format(Constantes.decimalFormat, cot.montoIGV);
                    dataTable2.Rows.Add(new object[] { igv, montoIGV });

                    String total = "Total: " + Constantes.simboloMonedaSol + ": ";
                    String montoTotal = String.Format(Constantes.decimalFormat, cot.montoTotal);
                    dataTable2.Rows.Add(new object[] { total, montoTotal });

                    tableTotales.DataSource = dataTable2;

                    float width2   = sectionTotales.Canvas.ClientSize.Width
                      - (tableTotales.Columns.Count + 1) * tableTotales.Style.BorderPen.Width;

                    tableTotales.Columns[0].Width = width2 * 0.25f;
                    tableTotales.Columns[0].StringFormat
                        = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

                    tableTotales.Columns[1].Width = width2 * 0.20f;
                    tableTotales.Columns[1].StringFormat
                        = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);

                   // tableTotales.BeginRowLayout += new BeginRowLayoutEventHandler(table_BeginRowLayout);
                   // table.EndCellLayout += new EndCellLayoutEventHandler(table_EndCellLayout);

                    PdfTableLayoutFormat tableLayout2 = new PdfTableLayoutFormat();
                    tableLayout2.Break = PdfLayoutBreakType.FitElement;
                    tableLayout2.Layout = PdfLayoutType.Paginate;


                    tableLayout2.EndColumnIndex = tableTotales.Columns.Count-1;
                    

                    PdfLayoutResult result2 = tableTotales.Draw(sectionTotales, new PointF(420 + xPage2a, y), tableLayout2);
                    y = y + result2.Bounds.Height + 5;

                }


                if (reiniciarY.Equals("OBSERVACIONES"))
                {
                    y = margenTop;
                    xPage2 = margenLeft;
                }


                string[] stringSeparators = new string[] { "\n" };
                string[] lines = cot.observaciones.Split(stringSeparators, StringSplitOptions.None);

                if (cot.incluidoIgv)
                {
                    sectionObervaciones.Canvas.DrawString("* Los precios incluyen IGV.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                }
                else
                {
                    sectionObervaciones.Canvas.DrawString("* Los precios NO incluyen IGV.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                }
                y = y + sepLine;



                if (cot.fechaVigenciaInicio.ToString("dd/MM/yyyy").Equals(cot.fecha.ToString("dd/MM/yyyy")))
                {
                    //0 es días
                    if (cot.tipoVigencia == 0)
                    {
                        sectionObervaciones.Canvas.DrawString("* Validez de los precios por " + cot.diasVigencia + " días.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                    }
                    else //1 es fecha
                    {
                        sectionObervaciones.Canvas.DrawString("* Validez de los precios hasta   " + cot.fechaVigenciaLimite.ToString("dd/MM/yyyy") + ".", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                    }

                }
                else //Si la fecha de inicio es distinta a la fecha actual se indica en la cotización
                {
                    if (cot.tipoVigencia == 0)
                    {
                        sectionObervaciones.Canvas.DrawString("* Validez de los precios desde " + cot.fechaVigenciaLimite.ToString("dd/MM/yyyy") + " hasta por " + cot.diasVigencia + " días.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                    }
                    else //1 es fecha
                    {
                        sectionObervaciones.Canvas.DrawString("* Validez de los precios desde "+ cot.fechaVigenciaLimite.ToString("dd/MM/yyyy")+" hasta " + cot.fechaVigenciaLimite.ToString("dd/MM/yyyy") + ".", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                    }

                }

                


                y = y + sepLine;

            

                foreach (string line in lines)
                {
                    sectionObervaciones.Canvas.DrawString(line, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                    y = y + sepLine;
                }

                sectionObervaciones.Canvas.DrawString("Sin otro particular, quedamos de ustedes.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                y = y + sepLine * 2;
                sectionObervaciones.Canvas.DrawString("Atentamente,", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                y = y + sepLine * 2;
                sectionObervaciones.Canvas.DrawString("MP INSTITUCIONAL S.A.C.", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), xPage2, y);
                y = y + sepLine * 2;


                if (reiniciarY.Equals("FIRMA"))
                {
                    y = margenTop;
                    xPage2 = margenLeft;
                }

                sectionFirma.Canvas.DrawString(cot.usuario.nombre, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                y = y + sepLine;
                sectionFirma.Canvas.DrawString(cot.usuario.cargo, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                y = y + sepLine;
                sectionFirma.Canvas.DrawString(cot.usuario.contacto, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                y = y + sepLine;
                sectionFirma.Canvas.DrawString(cot.usuario.email, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                y = y + sepLine;
                sectionFirma.Canvas.DrawString("www.mpinstitucional.com", new PdfFont(PdfFontFamily.Helvetica, 8f, PdfFontStyle.Underline), new PdfSolidBrush(Color.Blue), xPage2, y);
                //page.Canvas.DrawString("www.mpinstitucional.com", new PdfFont(PdfFontFamily.Helvetica, 8f, PdfFontStyle.Underline), new PdfSolidBrush(Color.Blue), 0, y);
                PdfTextWebLink link2 = new PdfTextWebLink();
                link2.Text = "www.mpinstitucional.com";
                link2.Url = "www.mpinstitucional.com";
                link2.Font = new PdfFont(PdfFontFamily.Helvetica, 8f, PdfFontStyle.Underline);
                link2.Brush = PdfBrushes.DarkSeaGreen;
                link2.DrawTextWebLink(sectionFirma.Canvas, new PointF(xPage2, y));

                String fechaCotizacion = DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;

                String pathrootsave = AppDomain.CurrentDomain.BaseDirectory + "\\pdf\\";
                doc.SaveToFile(pathrootsave + cot.cliente.ruc + " " + fechaCotizacion + ".pdf");
                PDFDocumentViewer(pathrootsave + cot.cliente.ruc+" " + fechaCotizacion + ".psdf");
                doc.Close();
                return cot.cliente.ruc + " " + fechaCotizacion + ".pdf";
            }
            catch (Exception ex)
            {
                Log log = new Log(ex.ToString(), TipoLog.Error,cot.usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
                return ex.ToString();
            }
        }


        void table_BeginRowLayout(object sender, BeginRowLayoutEventArgs args)
        {
            if (args.RowIndex < 0)
            {
                //header
                return;
            }
            DataTable dataTable = (sender as PdfTable).DataSource as DataTable;

            byte[] imageData = dataTable.Rows[args.RowIndex][7] as byte[];
            using (MemoryStream stream = new MemoryStream(imageData))
            {                
                PdfImage image = PdfImage.FromStream(stream);
                args.MinimalHeight = 50f;//   4 + image.PhysicalDimension.Height;
                dataTable.Rows[args.RowIndex][8] = image;
            }
        }


        void table_EndCellLayout(object sender, EndCellLayoutEventArgs args)
        {
            if (args.RowIndex < 0)
            {
                //header
                return;
            }
            if (args.CellIndex == 3)
            {
                DataTable dataTable = (sender as PdfTable).DataSource as DataTable;
                PdfImage image = dataTable.Rows[args.RowIndex][8] as PdfImage;
                //   float x = (args.Bounds.Width - image.PhysicalDimension.Width) / 2 + args.Bounds.X;
                //   float y = (args.Bounds.Height - image.PhysicalDimension.Height) / 2 + args.Bounds.Y;
                float x = (args.Bounds.Width - 45f) / 2 + args.Bounds.X;
                float y = (args.Bounds.Height -45f) / 2 + args.Bounds.Y;
                byte[] imageData = dataTable.Rows[args.RowIndex][7] as byte[];
                using (MemoryStream stream = new MemoryStream(imageData))
                {    
                    image = PdfImage.FromStream(stream);
                    float width = 45f;
                    float height = 45f;
                    args.Graphics.DrawImage(image, x, y, width, height);
                }
            }
            
        }

     

        private void PDFDocumentViewer(string fileName)
        {
            try
            {
                System.Diagnostics.Process.Start(fileName);
            }
            catch { }
        }



    }
}
