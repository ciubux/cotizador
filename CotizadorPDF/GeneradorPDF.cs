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
                String formatDecimal = "{0:0.00}";
                String simboloMonedaSol = "S/";

                int sepLine = 12;
                SampleEventSourceWriter.Log.MessageMethod("This is a message.");
                PdfDocument doc = new PdfDocument();
                PdfPageBase page = doc.Pages.Add(PdfPageSize.A4);

                PdfImage image = PdfImage.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\images\\logo.png");
                float width = 107 * 2.4f;
                float height = 16 * 2.4f;
                page.Canvas.DrawImage(image, 0, 0, width, height);

                PdfImage imageCli = PdfImage.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\images\\marcas.png");
                width = 75 * 2.4f;
                height = 70 * 2.4f;
                page.Canvas.DrawImage(imageCli, 300, 0, width, height);

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

                string fecha = cot.fecha.Day + " de " + mes + " de " + cot.fecha.Year;
                page.Canvas.DrawString(cot.ciudad.nombre + " " + fecha, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine * 2;
                page.Canvas.DrawString("Señores: ", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine;
                page.Canvas.DrawString(cot.cliente.razonSocial, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Blue), 0, y);
                y = y + sepLine + 2;
                page.Canvas.DrawString("Ciudad.- " + cot.ciudad.nombre, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine * 2;
                page.Canvas.DrawString("Atn.- " + cot.contacto, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + 3 + sepLine * 2;

                page.Canvas.DrawString("De nuestra consideración:", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine * 2;
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
                    String presentacion = det.producto.unidad;
                    String imagen = "";
                    String precioUnitarioAnterior = "";
                    String precioUnitarioNuevo = simboloMonedaSol + " " + String.Format(formatDecimal, det.precio);
                    String cantidad = det.cantidad.ToString();
                    String subtotal = simboloMonedaSol + " " + String.Format(formatDecimal, det.subTotal);

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
                table.Columns[3].Width = width1 * 0.12f;
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


                if (cot.considerarCantidades)
                {
                    page.Canvas.DrawString("Subtotal: ", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 420, y);
                    page.Canvas.DrawString(simboloMonedaSol + " " + String.Format(formatDecimal, cot.montoSubTotal), new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 460, y);
                    y = y + sepLine;
                    page.Canvas.DrawString("IGV 18%: ", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 420, y);
                    page.Canvas.DrawString(simboloMonedaSol + " " + String.Format(formatDecimal, cot.montoIGV), new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 460, y);
                    y = y + sepLine;
                    page.Canvas.DrawString("Total: ", new PdfFont(PdfFontFamily.Helvetica, 10f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 420, y);
                    page.Canvas.DrawString(simboloMonedaSol + " " + String.Format(formatDecimal, cot.montoTotal), new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 460, y);
                }

                y = y + sepLine * 2;

                string[] stringSeparators = new string[] { "\n" };
                string[] lines = cot.observaciones.Split(stringSeparators, StringSplitOptions.None);

                if (cot.incluidoIgv)
                {
                    page.Canvas.DrawString("* Los precios incluyen IGV.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                }
                else
                {
                    page.Canvas.DrawString("* Los precios NO incluyen IGV.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                }
                y = y + sepLine;


                foreach (string line in lines)
                {
                    page.Canvas.DrawString(line, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                    y = y + sepLine;
                }

                y = y + sepLine;
                page.Canvas.DrawString("Sin otro particular, quedamos de ustedes.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine * 2;
                page.Canvas.DrawString("Atentamente,", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine * 2;
                page.Canvas.DrawString("MP INSTITUCIONAL S.A.C.", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine * 2;

                page.Canvas.DrawString(cot.usuario.nombre_mostrar, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine;
                page.Canvas.DrawString(cot.usuario.cargo, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine;
                page.Canvas.DrawString("Tlf. (1) 2472142 ext. " + cot.usuario.anexo_empresa + " / " + cot.usuario.celular, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine;
                page.Canvas.DrawString(cot.usuario.email, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
                y = y + sepLine;
                page.Canvas.DrawString("www.mpinstitucional.com", new PdfFont(PdfFontFamily.Helvetica, 8f, PdfFontStyle.Underline), new PdfSolidBrush(Color.Blue), 0, y);
                //page.Canvas.DrawString("www.mpinstitucional.com", new PdfFont(PdfFontFamily.Helvetica, 8f, PdfFontStyle.Underline), new PdfSolidBrush(Color.Blue), 0, y);
                PdfTextWebLink link2 = new PdfTextWebLink();
                link2.Text = "www.mpinstitucional.com";
                link2.Url = "www.mpinstitucional.com";
                link2.Font = new PdfFont(PdfFontFamily.Helvetica, 8f, PdfFontStyle.Underline);
                link2.Brush = PdfBrushes.DarkSeaGreen;
                link2.DrawTextWebLink(page.Canvas, new PointF(0, y));


                String fechaCotizacion = DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;

                String pathrootsave = AppDomain.CurrentDomain.BaseDirectory + "\\pdf\\";
                doc.SaveToFile(pathrootsave + cot.cliente.ruc + " " + fechaCotizacion + ".pdf");
                PDFDocumentViewer(pathrootsave + cot.cliente.ruc+" " + fechaCotizacion + ".psdf");
                doc.Close();
                return cot.cliente.ruc + " " + fechaCotizacion + ".pdf";
            }
            catch (Exception ee)
            {
                SampleEventSourceWriter.Log.MessageMethod("Error"+ee.ToString());
                return ee.ToString();
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
                args.MinimalHeight = 25f;//   4 + image.PhysicalDimension.Height;
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
                float x = (args.Bounds.Width - 20f) / 2 + args.Bounds.X;
                float y = (args.Bounds.Height -20f) / 2 + args.Bounds.Y;
                byte[] imageData = dataTable.Rows[args.RowIndex][7] as byte[];
                using (MemoryStream stream = new MemoryStream(imageData))
                {    
                    image = PdfImage.FromStream(stream);
                    float width = 20f;
                    float height = 20f;
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
