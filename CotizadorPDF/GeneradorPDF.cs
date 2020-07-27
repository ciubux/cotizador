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
                    if (!cot.cliente.esClienteLite)
                    {
                        page.Canvas.DrawString(cot.cliente.razonSocial, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Blue), 0, y);
                        y = y + sepLine;
                        page.Canvas.DrawString("RUC: " + cot.cliente.ruc, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Blue), 0, y);
                        y = y + sepLine + 2;
                    }
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

                //Si muestra solo cantidades o cantidades con observaciones
                if (cot.considerarCantidades != Cotizacion.OpcionesConsiderarCantidades.Observaciones)
                {
                    dataTable.Columns.Add("Cant.");
                    dataTable.Columns.Add("Subtotal");
                }
                //Si es solo observaciones
                else
                {
                    dataTable.Columns.Add("Observaciones");
                    dataTable.Columns.Add("Subtotal");
                }


                
                dataTable.Columns.Add(new DataColumn("Temp1", typeof(Byte[])));
                dataTable.Columns.Add(new DataColumn("Temp2", typeof(PdfImage)));

                String observacionesDetalle = String.Empty;

                foreach (CotizacionDetalle det in cot.cotizacionDetalleList)
                {
                    String codigo = det.producto.sku.Trim();
                    String producto = det.producto.descripcion;
                    if (cot.mostrarCodigoProveedor)
                    {
                        if (det.producto.skuProveedor.IndexOf("!") < 0)
                        {
                            producto = "[" + det.producto.skuProveedor + "] " + producto;
                        }
                    }


                    
                    String presentacion = det.unidad; //Se muestra la unidad seleccionada y que se encuentra en el detalle
                    String imagen = "";
                    String precioUnitarioAnterior = "";
                    String precioUnitarioNuevo = Constantes.SIMBOLO_SOL + " " + String.Format(Constantes.formatoDecimalesPrecioNeto, det.precioUnitario);


                    String cantidad = "";
                    String subtotal = "";

                    if (cot.considerarCantidades != Cotizacion.OpcionesConsiderarCantidades.Observaciones)
                    {
                        cantidad = det.cantidad.ToString();
                        subtotal = Constantes.SIMBOLO_SOL + " " + String.Format(Constantes.formatoDosDecimales, det.subTotal);

                        if(cot.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Ambos)
                        {
                            if(det.observacion!=null)
                                producto = producto + "\n[" + det.observacion+"]";
                        }

                    }
                    //Si es solo observaciones
                    else
                    {
                        cantidad = det.observacion==null?"": det.observacion;

                        observacionesDetalle = observacionesDetalle + cantidad;

                        subtotal = Constantes.SIMBOLO_SOL + " " + String.Format(Constantes.formatoDosDecimales, det.subTotal);
                    }



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

                if (cot.considerarCantidades != Cotizacion.OpcionesConsiderarCantidades.Observaciones)
                {
                    //Cant.
                    table.Columns[5].Width = width1 * 0.10f;
                    table.Columns[5].StringFormat
                        = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                    //Subtotal
                    table.Columns[6].Width = width1 * 0.10f;
                    table.Columns[6].StringFormat
                        = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);

                }
                //Si es solo observaciones
                else
                {
                    //OBSERVACION.
                    table.Columns[5].Width = width1 * 0.20f;
                    table.Columns[5].StringFormat
                        = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                    //Subtotal
                    table.Columns[6].Width = width1 * 0.01f;
                    table.Columns[6].StringFormat
                        = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                }





                table.BeginRowLayout += new BeginRowLayoutEventHandler(table_BeginRowLayout);
                table.EndCellLayout += new EndCellLayoutEventHandler(table_EndCellLayout);

                PdfTableLayoutFormat tableLayout = new PdfTableLayoutFormat();
                tableLayout.Break = PdfLayoutBreakType.FitElement;
                tableLayout.Layout = PdfLayoutType.Paginate;

                //Si es solo cantidades


                //Si muestra solo cantidades o cantidades con observaciones
                if (cot.considerarCantidades != Cotizacion.OpcionesConsiderarCantidades.Observaciones)
                {
                    tableLayout.EndColumnIndex = table.Columns.Count - 3;
                }
                //Si es solo observaciones
                else 
                {
                    //Si no hay datos en observaciones detalle, no se considera la columna
                    if (observacionesDetalle.Trim().Length == 0)
                    {
                        tableLayout.EndColumnIndex = table.Columns.Count - 5; //- 1;
                    }
                    else
                    {
                        tableLayout.EndColumnIndex = table.Columns.Count - 4; //- 1;
                    }

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

                xPage2 = margenLeft;

                if (y < 600)
                {
                    sectionTotales = doc.Pages[countPages-1];
                    sectionObervaciones = doc.Pages[countPages-1];
                    sectionFirma = doc.Pages[countPages-1];
                    y = y + 35;
                }
                else
                {
                    SizeF size = page.Size;
                    PdfPageBase newPage = doc.Pages.Add(size, new PdfMargins(0));

                    sectionTotales = doc.Pages[countPages];
                    sectionObervaciones = doc.Pages[countPages];
                    sectionFirma = doc.Pages[countPages];
                    //Si es una nueva pagina se inicia en el margen top
                    //Y se agrega 150 porque en la siguiente instrucción se restán 150 para que no haya mucho margen entre
                    //el fin de la tabla y la observación.
                    y =  margenTop +150;
                }
                countPages = doc.Pages.Count;
                if (countPages > 1)
                {

                    y = y - 150;
                }

                
                //Si es distinto de solo observaciones
                if (cot.considerarCantidades != Cotizacion.OpcionesConsiderarCantidades.Observaciones)
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

                    String subtotal = "Subtotal: " + Constantes.SIMBOLO_SOL + ": ";
                    String montoSubtotal = String.Format(Constantes.formatoDosDecimales, cot.montoSubTotal);
                    dataTable2.Rows.Add(new object[] { subtotal, montoSubtotal });

                    String igv = "IGV 18%: " + Constantes.SIMBOLO_SOL + ": ";
                    String montoIGV = String.Format(Constantes.formatoDosDecimales, cot.montoIGV);
                    dataTable2.Rows.Add(new object[] { igv, montoIGV });

                    String total = "Total: " + Constantes.SIMBOLO_SOL + ": ";
                    String montoTotal = String.Format(Constantes.formatoDosDecimales, cot.montoTotal);
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

                String observaciones = cot.observaciones == null ? String.Empty : cot.observaciones;
                string[] stringSeparators = new string[] { "\n" };
                string[] lines = observaciones.Split(stringSeparators, StringSplitOptions.None);


                sectionObervaciones.Canvas.DrawString("* Condiciones de Pago: " + cot.textoCondicionesPago, new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                y = y + sepLine;


                if (cot.incluidoIGV)
                {
                    sectionObervaciones.Canvas.DrawString("* Los precios incluyen IGV.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                }
                else
                {
                    sectionObervaciones.Canvas.DrawString("* Los precios NO incluyen IGV.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                }
                y = y + sepLine;

                //0 es días
                if (cot.mostrarValidezOfertaEnDias == 0)
                {
                    //REVISAR TEXTO
                    sectionObervaciones.Canvas.DrawString("* Validez de la oferta: hasta por " + cot.validezOfertaEnDias + " días.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                }
                else //1 es fecha
                {
                    sectionObervaciones.Canvas.DrawString("* Validez de la oferta: hasta   " + cot.fechaLimiteValidezOferta.ToString("dd/MM/yyyy") + ".", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                }


                //Si se ha registrado fecha inicio vigencia precios
                if (cot.fechaInicioVigenciaPrecios != null)
                {


                    //Si la fecha de inicio de vigencia es IGUAL a la fecha de creación NO se indica en la cotización
                    if (cot.fechaInicioVigenciaPrecios.Value.ToString("dd/MM/yyyy").Equals(cot.fecha.ToString("dd/MM/yyyy")))
                    {

                        //Si se cuenta con fecha de fin de vigencia 
                        if (cot.fechaFinVigenciaPrecios != null)
                        {
                            y = y + sepLine;
                            sectionObervaciones.Canvas.DrawString("* Vigencia de los precios: hasta " + cot.fechaFinVigenciaPrecios.Value.ToString("dd/MM/yyyy") + ".", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                        }
                        //Si NO se cuenta con fecha fin de vigencia no se muestra nada 

                    }
                    //Si la fecha de inicio de vigencia es DISTINTA a la fecha de creación se indica en la cotización
                    else
                    {
                        y = y + sepLine;
                        //Si se cuenta con la fecha fin de vigencia de precios
                        if (cot.fechaFinVigenciaPrecios != null)
                        {
                            sectionObervaciones.Canvas.DrawString("* Vigencia de los precios: desde " + cot.fechaInicioVigenciaPrecios.Value.ToString("dd/MM/yyyy") + " hasta " + cot.fechaFinVigenciaPrecios.Value.ToString("dd/MM/yyyy") + ".", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                        }
                        else //Si no se cuenta con la fecha fin de vigencia de precios
                        {
                            sectionObervaciones.Canvas.DrawString("* Validez de los precios: desde " + cot.fechaInicioVigenciaPrecios.Value.ToString("dd/MM/yyyy") + ".", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                        }
                    }
                }
                else
                {
                    //Si se cuenta con fecha de fin de vigencia 
                    if (cot.fechaFinVigenciaPrecios != null)
                    {
                        y = y + sepLine;
                        sectionObervaciones.Canvas.DrawString("* Vigencia de los precios: hasta " + cot.fechaFinVigenciaPrecios.Value.ToString("dd/MM/yyyy") + ".", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                    }
                }

                y = y + sepLine;

                sectionObervaciones.Canvas.DrawString("* Entrega sujeta a confirmación de disponibilidad luego de recibido el pedido u orden de compra.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
                y = y + sepLine;

                sectionObervaciones.Canvas.DrawString(" No se garantiza stock debido a coyuntura excepcional de alta demanda.", new PdfFont(PdfFontFamily.Helvetica, 8f), new PdfSolidBrush(Color.Black), xPage2, y);
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


  


                String pathrootsave = AppDomain.CurrentDomain.BaseDirectory + "\\pdf\\";

                String fechaCotizacion = cot.fecha.Year + "-" + cot.fecha.Month.ToString().PadLeft(2, '0')  + "-" + cot.fecha.Day.ToString().PadLeft(2, '0');// + "-" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;
                String nombreArchivo = cot.cliente.razonSocial + " " + fechaCotizacion + " N° " + cot.codigo.ToString().PadLeft(10,'0') + ".pdf";

                nombreArchivo = nombreArchivo.Replace('&', 'Y');
                nombreArchivo = nombreArchivo.Replace(':', '.');


                doc.SaveToFile(pathrootsave+nombreArchivo);
                PDFDocumentViewer(pathrootsave+nombreArchivo);
                doc.Close();
                return nombreArchivo;
            }
            catch (Exception ex)
            {
                Log log = new Log(ex.ToString(), TipoLog.Error, cot.usuario);
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
                    float width = 0;
                    float height = 0;
                    if (image.PhysicalDimension.Width >= image.PhysicalDimension.Height)
                    {
                        float relacion = 45f / image.PhysicalDimension.Width;
                        width = 45f;
                        height = image.PhysicalDimension.Height * relacion;
                    }
                    else 
                    {
                        float relacion = 45f / image.PhysicalDimension.Height;
                        height  = 45f;
                        width = image.PhysicalDimension.Width * relacion;
                    }




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
