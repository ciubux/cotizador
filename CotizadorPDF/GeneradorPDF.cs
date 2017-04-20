using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace cotizadorPDF
{
    public class GeneradorPDF
    {
        public void generarPDFExtended(Cotizacion cot)
        {
            int sepLine = 12;
            String pathrootsave = AppDomain.CurrentDomain.BaseDirectory + "\\pdfs\\";
            PdfDocument doc = new PdfDocument();
            PdfPageBase page = doc.Pages.Add(PdfPageSize.A4);

            PdfImage image = PdfImage.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\images\\logo.png");
            float width = 107 * 2.4f;
            float height = 16 * 2.4f;
            page.Canvas.DrawImage(image, 0, 0, width, height);

            PdfImage imageCli = PdfImage.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\images\\clientes.png");
            width = 62 * 2.4f;
            height = 70 * 2.4f;
            page.Canvas.DrawImage(imageCli, 350, 0, width, height);

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
            page.Canvas.DrawString("Ciudad.-", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine * 2;
            page.Canvas.DrawString("Atn.- ", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + 3;
            page.Canvas.DrawString("Persona ", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Blue), 40, y);
            y = y + sepLine;
            page.Canvas.DrawString("Cargo ", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Blue), 40, y);
            y = y - 3 + sepLine * 2;
            page.Canvas.DrawString("De nuestra consideración:", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine * 2;
            page.Canvas.DrawString("Por la presente, nos es grato someter a su consideración la siguiente cotización:", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine * 2;

            
            page.Canvas.DrawString("Producto", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 40, y);
            page.Canvas.DrawString("Presentación", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 150, y);
            page.Canvas.DrawString("Precio Unit.", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 225, y);
            page.Canvas.DrawString("Anterior", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 230, y + sepLine - 2);
            page.Canvas.DrawString("Nuevo", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 290, y);
            page.Canvas.DrawString("Precio Unit.", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 275, y + sepLine - 2);
            page.Canvas.DrawString("Cant.", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 325, y);
            page.Canvas.DrawString("Subtotal", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 385, y);


            y = y + sepLine * 2;

            foreach (CotizacionDetalle det in cot.detalles)
            {
                var ms = new MemoryStream(det.producto.image);
                Image img = Image.FromStream(ms);

                PdfImage imgProd = PdfImage.FromImage(img);
                width = 40;
                height = 40;
                page.Canvas.DrawImage(imgProd, 455, y, width, height);

                page.Canvas.DrawString(det.producto.sku.Trim() + "-" + det.producto.descripcion, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 0, y + 15);
                page.Canvas.DrawString(det.producto.unidad.descripcion, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 160, y + 15);
                page.Canvas.DrawString(det.moneda.simbolo + " " + det.valorUnitario, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 230, y + 15);
                page.Canvas.DrawString(det.moneda.simbolo + " " + det.valorUnitarioFinal, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 280, y + 15);
                page.Canvas.DrawString(det.cantidad.ToString(), new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 340, y + 15);
                page.Canvas.DrawString(det.moneda.simbolo + " " + det.subTotal, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 385, y + 15);

                
                y = y + 50;
            }

            y = y + 5;


            if (cot.incluidoIgv == 1)
            {
                page.Canvas.DrawString("Sub Total ", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 400, y);
                page.Canvas.DrawString(cot.moneda.simbolo + " " + cot.subtotal, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 445, y);
                y = y + sepLine;
                page.Canvas.DrawString("I.G.V. ", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 400, y);
                page.Canvas.DrawString(cot.moneda.simbolo + " " + cot.igv, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 445, y);
                y = y + sepLine;
            }

            page.Canvas.DrawString("Total ", new PdfFont(PdfFontFamily.Helvetica, 10f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 400, y);
            page.Canvas.DrawString(cot.moneda.simbolo + " " + cot.total, new PdfFont(PdfFontFamily.Helvetica, 10f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 445, y);

            y = y + sepLine * 2;
            page.Canvas.DrawString("* Los precios incluyen IGV", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString("* Condiciones de pago: al contado", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString("* Validez de los precios: 15 días", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString("* Entrega en almacén del cliente, 48 horas luego de la recepción del pedido o la orden de compra.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString("  (para productos no stockeables o primeras compras, consultar plazo)", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine * 2;
            page.Canvas.DrawString("Sin otro particular, quedamos de ustedes.)", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine * 2;
            page.Canvas.DrawString("Atentamente,", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine * 2;
            page.Canvas.DrawString("* Los precios incluyen IGV", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine * 2;
            page.Canvas.DrawString("MP INSTITUCIONAL S.A.C.", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine * 2;
            page.Canvas.DrawString(".......................(nombre)", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString(".......................(cargo)", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString(".......................(teléfono)", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString(".......................(email)", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString("www.mpinstitucional.com", new PdfFont(PdfFontFamily.Helvetica, 8f, PdfFontStyle.Underline), new PdfSolidBrush(Color.Blue), 0, y);

            /*DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second*/
            doc.SaveToFile(pathrootsave + cot.usuarioCreacion + ".pdf");
            doc.Close();
            //PDFDocumentViewer("Image.pdf");
        }

        public void generarPDF(Cotizacion cot)
        {
            int sepLine = 12;
            String pathrootsave = AppDomain.CurrentDomain.BaseDirectory + "\\pdfs\\";
            PdfDocument doc = new PdfDocument();
            PdfPageBase page = doc.Pages.Add(PdfPageSize.A4);

            PdfImage image = PdfImage.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\images\\logo.png");
            float width = 107 * 2.4f;
            float height = 16 * 2.4f;
            page.Canvas.DrawImage(image, 0, 0, width, height);

            PdfImage imageCli = PdfImage.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\images\\clientes.png");
            width = 62 * 2.4f;
            height = 70 * 2.4f;
            page.Canvas.DrawImage(imageCli, 350, 0, width, height);

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
            page.Canvas.DrawString("Ciudad.-", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine * 2;
            page.Canvas.DrawString("Atn.- ", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + 3;
            page.Canvas.DrawString("Persona ", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Blue), 40, y);
            y = y + sepLine;
            page.Canvas.DrawString("Cargo ", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Blue), 40, y);
            y = y - 3 + sepLine * 2;
            page.Canvas.DrawString("De nuestra consideración:", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine * 2;
            page.Canvas.DrawString("Por la presente, nos es grato someter a su consideración la siguiente cotización:", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine * 2;

            page.Canvas.DrawString("Cod.", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 10, y);
            page.Canvas.DrawString("Producto", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 110, y);
            page.Canvas.DrawString("Presentación", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 210, y);
            page.Canvas.DrawString("Precio Unit.", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 285, y);
            page.Canvas.DrawString("Anterior", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 290, y + sepLine - 2);
            page.Canvas.DrawString("Nuevo", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 350, y);
            page.Canvas.DrawString("Precio Unit.", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 335, y + sepLine - 2);
            page.Canvas.DrawString("Cant.", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 395, y);
            page.Canvas.DrawString("Subtotal", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 445, y);


            y = y + sepLine * 2;

            foreach (CotizacionDetalle det in cot.detalles) {
                page.Canvas.DrawString(det.producto.sku, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 0, y);
                page.Canvas.DrawString(det.producto.descripcion, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 70, y);
                page.Canvas.DrawString(det.producto.unidad.descripcion, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 200, y);
                page.Canvas.DrawString(det.moneda.simbolo + " " + det.valorUnitario, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 290, y);
                page.Canvas.DrawString(det.moneda.simbolo + " " + det.valorUnitarioFinal, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 340, y);
                page.Canvas.DrawString(det.cantidad.ToString(), new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 400, y);
                page.Canvas.DrawString(det.moneda.simbolo + " " + det.subTotal, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 445, y);

                y = y + sepLine;
            }

            y = y + 5;


            if (cot.incluidoIgv == 1) { 
                page.Canvas.DrawString("Sub Total ", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 400, y);
                page.Canvas.DrawString(cot.moneda.simbolo + " " + cot.subtotal, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 445, y);
                y = y + sepLine;
                page.Canvas.DrawString("I.G.V. ", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 400, y);
                page.Canvas.DrawString(cot.moneda.simbolo + " " + cot.igv, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 445, y);
                y = y + sepLine;
            }

            page.Canvas.DrawString("Total ", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 400, y);
            page.Canvas.DrawString(cot.moneda.simbolo + " " + cot.total, new PdfFont(PdfFontFamily.Helvetica, 9f), new PdfSolidBrush(Color.Black), 445, y);

            y = y + sepLine*2;
            page.Canvas.DrawString("* Los precios incluyen IGV", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString("* Condiciones de pago: al contado", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString("* Validez de los precios: 15 días", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString("* Entrega en almacén del cliente, 48 horas luego de la recepción del pedido o la orden de compra.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString("  (para productos no stockeables o primeras compras, consultar plazo)", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine*2;
            page.Canvas.DrawString("Sin otro particular, quedamos de ustedes.)", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine*2;
            page.Canvas.DrawString("Atentamente,", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine*2;
            page.Canvas.DrawString("* Los precios incluyen IGV", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine*2;
            page.Canvas.DrawString("MP INSTITUCIONAL S.A.C.", new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine*2;
            page.Canvas.DrawString(".......................(nombre)", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString(".......................(cargo)", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString(".......................(teléfono)", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString(".......................(email)", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), 0, y);
            y = y + sepLine;
            page.Canvas.DrawString("www.mpinstitucional.com", new PdfFont(PdfFontFamily.Helvetica, 8f, PdfFontStyle.Underline), new PdfSolidBrush(Color.Blue), 0, y);

            /*DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second*/
            doc.SaveToFile(pathrootsave + cot.usuarioCreacion + ".pdf");
            doc.Close();
            //PDFDocumentViewer("Image.pdf");
        }
        public void generar()
        {
            String pathroot = "D:\\temp\\";
            String pathrootsave = AppDomain.CurrentDomain.BaseDirectory + "\\pdfs\\";
            //Create a pdf document.
            PdfDocument doc = new PdfDocument();

            //margin
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(2.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(3.17f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;

            // Create one page
            PdfPageBase page = doc.Pages.Add(PdfPageSize.A4, margin);

            float y = 10;

            //title
            PdfBrush brush1 = PdfBrushes.Black;
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("Arial", 16f, FontStyle.Bold));
            PdfStringFormat format1 = new PdfStringFormat(PdfTextAlignment.Center);
            page.Canvas.DrawString("Country List", font1, brush1, page.Canvas.ClientSize.Width / 2, y, format1);
            y = y + font1.MeasureString("Country List", format1).Height;
            y = y + 5;

            //create data table
            PdfTable table = new PdfTable();
            table.Style.CellPadding = 2;
            table.Style.BorderPen = new PdfPen(brush1, 0.75f);
            table.Style.DefaultStyle.BackgroundBrush = PdfBrushes.SkyBlue;
            table.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("Arial", 10f));
            table.Style.AlternateStyle = new PdfCellStyle();
            table.Style.AlternateStyle.BackgroundBrush = PdfBrushes.LightYellow;
            table.Style.AlternateStyle.Font = new PdfTrueTypeFont(new Font("Arial", 10f));
            table.Style.HeaderSource = PdfHeaderSource.ColumnCaptions;
            table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.CadetBlue;
            table.Style.HeaderStyle.Font = new PdfTrueTypeFont(new Font("Arial", 11f, FontStyle.Bold));
            table.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
            table.Style.ShowHeader = true;

            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+ pathroot + "demo.mdb";
                OleDbCommand command = new OleDbCommand();
                command.CommandText
                    = " select Name, '' as Flag, Capital, Continent, Area, Population, Flag as FlagData from country ";
                command.Connection = conn;
                using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    dataTable.Columns.Add(new DataColumn("FlagImage", typeof(PdfImage)));
                    table.DataSourceType = PdfTableDataSourceType.TableDirect;
                    table.DataSource = dataTable;
                }
            }
            float width
                = page.Canvas.ClientSize.Width
                    - (table.Columns.Count + 1) * table.Style.BorderPen.Width;
            table.Columns[0].Width = width * 0.21f;
            table.Columns[0].StringFormat
                = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            table.Columns[1].Width = width * 0.10f;
            table.Columns[1].StringFormat
                = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            table.Columns[2].Width = width * 0.19f;
            table.Columns[2].StringFormat
                = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            table.Columns[3].Width = width * 0.21f;
            table.Columns[3].StringFormat
                = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            table.Columns[4].Width = width * 0.12f;
            table.Columns[4].StringFormat
                = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            table.Columns[5].Width = width * 0.17f;
            table.Columns[5].StringFormat
                = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);

            table.BeginRowLayout += new BeginRowLayoutEventHandler(table_BeginRowLayout);
            table.EndCellLayout += new EndCellLayoutEventHandler(table_EndCellLayout);

            PdfTableLayoutFormat tableLayout = new PdfTableLayoutFormat();
            tableLayout.Break = PdfLayoutBreakType.FitElement;
            tableLayout.Layout = PdfLayoutType.Paginate;
            tableLayout.EndColumnIndex = table.Columns.Count - 2 - 1;

            PdfLayoutResult result = table.Draw(page, new PointF(0, y), tableLayout);
            y = y + result.Bounds.Height + 5;

            PdfBrush brush2 = PdfBrushes.Gray;
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("Arial", 9f));
            page.Canvas.DrawString(String.Format("* {0} countries in the list.", table.Rows.Count),
                font2, brush2, 5, y);

            //Save pdf file.
            doc.SaveToFile(pathrootsave + DateTime.Now.Year+"_"+ DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".pdf");
            doc.Close();

            //Launching the Pdf file.
        //    PDFDocumentViewer("ImageTable.pdf");
        }

        void table_EndCellLayout(object sender, EndCellLayoutEventArgs args)
        {
            if (args.RowIndex < 0)
            {
                //header
                return;
            }
            if (args.CellIndex == 1)
            {
                DataTable dataTable = (sender as PdfTable).DataSource as DataTable;
                PdfImage image = dataTable.Rows[args.RowIndex][7] as PdfImage;
                float x = (args.Bounds.Width - image.PhysicalDimension.Width) / 2 + args.Bounds.X;
                float y = (args.Bounds.Height - image.PhysicalDimension.Height) / 2 + args.Bounds.Y;
                args.Graphics.DrawImage(image, x, y);
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
            byte[] imageData = dataTable.Rows[args.RowIndex][6] as byte[];
            using (MemoryStream stream = new MemoryStream(imageData))
            {
                PdfImage image = PdfImage.FromStream(stream);
                args.MinimalHeight = 4 + image.PhysicalDimension.Height;
                dataTable.Rows[args.RowIndex][7] = image;
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
