using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using Model;
using Newtonsoft.Json;
using NPOI.HSSF.Model;
using NPOI.HSSF.Util;
using BusinessLayer;

using System.Web.Mvc;

namespace Cotizador.ExcelExport
{
    public class PedidoOCExcel
    {
        public FileStreamResult generateExcel(Pedido obj)
        {
            
            HSSFWorkbook wb;
            //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);
            HSSFSheet sheet;
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());

                HSSFCellStyle defaulCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                defaulCellStyle.FillPattern = FillPattern.SolidForeground;
                defaulCellStyle.FillForegroundColor = HSSFColor.White.Index;

                HSSFFont formLabelFont = (HSSFFont)wb.CreateFont();
                formLabelFont.FontHeightInPoints = (short)11;
                formLabelFont.FontName = "Arial";
                formLabelFont.Color = IndexedColors.Black.Index;
                formLabelFont.IsBold = true;
                //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
                //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
                HSSFCellStyle formLabelCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                formLabelCellStyle.SetFont(formLabelFont);
                formLabelCellStyle.Alignment = HorizontalAlignment.Right;

                HSSFCellStyle formDataCenterCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                formDataCenterCellStyle.Alignment = HorizontalAlignment.Center;
                formDataCenterCellStyle.BorderLeft = BorderStyle.Thin;
                formDataCenterCellStyle.BorderTop = BorderStyle.Thin;
                formDataCenterCellStyle.BorderRight = BorderStyle.Thin;
                formDataCenterCellStyle.BorderBottom = BorderStyle.Thin;



                HSSFFont titleFont = (HSSFFont)wb.CreateFont();
                titleFont.FontHeightInPoints = (short)11;
                titleFont.FontName = "Arial";
                titleFont.Color = IndexedColors.White.Index;
                titleFont.IsBold = true;
                //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
                //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
                HSSFCellStyle titleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleCellStyle.SetFont(titleFont);
                titleCellStyle.Alignment = HorizontalAlignment.Center;
                titleCellStyle.FillPattern = FillPattern.SolidForeground;
                titleCellStyle.FillForegroundColor = HSSFColor.RoyalBlue.Index;
               

                HSSFCellStyle titleDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleDataCellStyle.CloneStyleFrom(titleCellStyle);
                titleDataCellStyle.BorderLeft = BorderStyle.Thin;
                titleDataCellStyle.BorderTop = BorderStyle.Thin;
                titleDataCellStyle.BorderRight = BorderStyle.Thin;

                HSSFCellStyle ocTitleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                HSSFFont ocTitleFont = (HSSFFont)wb.CreateFont();
                ocTitleFont.FontHeightInPoints = (short)28;
                ocTitleFont.FontName = "Arial";
                ocTitleFont.Color = IndexedColors.RoyalBlue.Index;
                ocTitleFont.IsBold = true;
                ocTitleCellStyle.SetFont(ocTitleFont);
                ocTitleCellStyle.Alignment = HorizontalAlignment.Center;
                ocTitleCellStyle.VerticalAlignment = VerticalAlignment.Center;

                HSSFCellStyle footerTextCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                footerTextCellStyle.Alignment = HorizontalAlignment.Center;

                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                var avgCellFormate = wb.CreateDataFormat();
                var twoDecFormat = avgCellFormate.GetFormat("0.00");
                HSSFCellStyle twoDecCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                twoDecCellStyle.DataFormat = twoDecFormat;
                twoDecCellStyle.BorderLeft = BorderStyle.Thin;
                twoDecCellStyle.BorderRight = BorderStyle.Thin;


                HSSFCellStyle totalsCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsCellStyle.DataFormat = twoDecFormat;
                totalsCellStyle.BorderLeft = BorderStyle.Thin;
                totalsCellStyle.BorderRight = BorderStyle.Thin;
                totalsCellStyle.BorderTop = BorderStyle.Thin;
                totalsCellStyle.BorderBottom = BorderStyle.Thin;


                HSSFCellStyle tableDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                tableDataCellStyle.WrapText = true;
                tableDataCellStyle.BorderLeft = BorderStyle.Thin;
                tableDataCellStyle.BorderRight = BorderStyle.Thin;

                HSSFCellStyle tableDataLastCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                tableDataLastCellStyle.CloneStyleFrom(tableDataCellStyle);
                tableDataLastCellStyle.BorderBottom = BorderStyle.Thin;

                HSSFCellStyle twoDecLastCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                twoDecLastCellStyle.CloneStyleFrom(twoDecCellStyle);
                twoDecLastCellStyle.BorderBottom = BorderStyle.Thin;

                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Atenciones");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (obj.pedidoDetalleList.Count) + 40;
                int cTotal = 7;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c).CellStyle = defaulCellStyle;
                    }
                }

                UtilesHelper.combinarCeldas(sheet, 1, 1, "A", "B");

                IWorkbook newWorkbook = wb;

                byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\images\\logo.png");
                int picInd = newWorkbook.AddPicture(data, PictureType.PNG);
                HSSFCreationHelper helper = newWorkbook.GetCreationHelper() as HSSFCreationHelper;
                IDrawing drawing = sheet.CreateDrawingPatriarch();
                HSSFClientAnchor anchor = helper.CreateClientAnchor() as HSSFClientAnchor;
                anchor.Col1 = 0;
                anchor.Row1 = 0;
                anchor.Col2 = 2;

                anchor.AnchorType = AnchorType.DontMoveAndResize;

                HSSFPicture pict = drawing.CreatePicture(anchor, picInd) as HSSFPicture;
                pict.Resize(1);

                UtilesHelper.combinarCeldas(sheet, 1, 1, "D", "G");
                UtilesHelper.setValorCelda(sheet, 1, "D", "ORDEN DE COMPRA", ocTitleCellStyle);

                UtilesHelper.setRowHeight(sheet, 1, 1200);

                UtilesHelper.setValorCelda(sheet, 2, "A", "[Dirección]");
                UtilesHelper.setValorCelda(sheet, 3, "A", "[Ciudad, Estado, postal]");
                UtilesHelper.setValorCelda(sheet, 4, "A", "Telefono: (000) 000-0000");
                UtilesHelper.setValorCelda(sheet, 5, "A", "Fax: (000) 000-0000");
                UtilesHelper.setValorCelda(sheet, 6, "A", "http://mpinstitucional.com");



                UtilesHelper.setValorCelda(sheet, 2, "E", "FECHA:", formLabelCellStyle);
                UtilesHelper.combinarCeldas(sheet, 2, 2, "F", "G");
                UtilesHelper.setValorCelda(sheet, 2, "F", obj.FechaRegistroFormatoFecha, formDataCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "G", "", formDataCenterCellStyle);

                UtilesHelper.setValorCelda(sheet, 3, "E", "N° OC:", formLabelCellStyle);
                UtilesHelper.combinarCeldas(sheet, 3, 3, "F", "G");
                UtilesHelper.setValorCelda(sheet, 3, "F", obj.numeroPedido, formDataCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, 3, "G", "", formDataCenterCellStyle);

                int i = 8;

                UtilesHelper.combinarCeldas(sheet, i, i, "A", "B");
                UtilesHelper.setValorCelda(sheet, i, "A", "VENDEDOR", titleCellStyle);


                UtilesHelper.setValorCelda(sheet, i + 1, "A", "MP INSTITUCIONAL S.A.C.");
                UtilesHelper.setValorCelda(sheet, i + 2, "A", "[Contacto o Departamento]");
                UtilesHelper.setValorCelda(sheet, i + 3, "A", "[Dirección]");
                UtilesHelper.setValorCelda(sheet, i + 4, "A", "[Ciudad, Estado, postal]");
                UtilesHelper.setValorCelda(sheet, i + 5, "A", "Telefono: (000) 000-0000");
                UtilesHelper.setValorCelda(sheet, i + 6, "A", "Fax: (000) 000-0000");
                
                
                UtilesHelper.combinarCeldas(sheet, i, i, "E", "G");
                UtilesHelper.setValorCelda(sheet, i, "E", "ENVÍE A", titleCellStyle);

                UtilesHelper.setValorCelda(sheet, i + 1, "E", "[Nombre]");
                UtilesHelper.setValorCelda(sheet, i + 2, "E", "[Nombre de empresa]");
                UtilesHelper.setValorCelda(sheet, i + 3, "E", "[Dirección]");
                UtilesHelper.setValorCelda(sheet, i + 4, "E", "[Ciudad, Estado, postal]");
                UtilesHelper.setValorCelda(sheet, i + 5, "E", "[Telefono]");


                i = i + 8; 

                UtilesHelper.setValorCelda(sheet, i, "A", "CÓDIGO", titleDataCellStyle);
                UtilesHelper.combinarCeldas(sheet, i, i, "B", "C");
                UtilesHelper.setValorCelda(sheet, i, "B", "DESCRIPCIÓN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "UND", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "CANT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "P. UNIT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "TOTAL", titleDataCellStyle);

                UtilesHelper.setColumnWidth(sheet, "A", 3000);
                UtilesHelper.setColumnWidth(sheet, "B", 6000);
                UtilesHelper.setColumnWidth(sheet, "C", 12000);
                UtilesHelper.setColumnWidth(sheet, "D", 5000);

                UtilesHelper.setColumnWidth(sheet, "F", 2900);
                UtilesHelper.setColumnWidth(sheet, "G", 3200);

                i++;

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (PedidoDetalle det in obj.pedidoDetalleList)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", "", tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", "", tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "C", "", tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "D", "", tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "E", "", tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "F", "", tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "G", "", tableDataCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "A", det.producto.sku);
                    UtilesHelper.combinarCeldas(sheet, i, i, "B", "C");
                    UtilesHelper.setValorCelda(sheet, i, "B", det.producto.descripcion);
                    UtilesHelper.setValorCelda(sheet, i, "D", det.unidad);
                    UtilesHelper.setValorCelda(sheet, i, "E", det.cantidad);
                    UtilesHelper.setValorCelda(sheet, i, "F", (double) det.precioUnitario, twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "G", (double) det.subTotal, twoDecCellStyle);

                    i++;
                }

                UtilesHelper.setValorCelda(sheet, i - 1, "A", UtilesHelper.getValorCelda(sheet, i - 1, "A"), tableDataLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "B", UtilesHelper.getValorCelda(sheet, i - 1, "B"), tableDataLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "C", UtilesHelper.getValorCelda(sheet, i - 1, "C"), tableDataLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "D", UtilesHelper.getValorCelda(sheet, i - 1, "D"), tableDataLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "E", int.Parse(UtilesHelper.getValorCelda(sheet, i - 1, "E")), tableDataLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "F", double.Parse(UtilesHelper.getValorCelda(sheet, i - 1, "F")), twoDecLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "G", double.Parse(UtilesHelper.getValorCelda(sheet, i - 1, "G")), twoDecLastCellStyle);
                


                UtilesHelper.setValorCelda(sheet, i, "F", "SUB TOTAL");
                UtilesHelper.setValorCelda(sheet, i, "G", (double)obj.montoSubTotal, totalsCellStyle);
                i++;

                UtilesHelper.setValorCelda(sheet, i, "F", "IGV");
                UtilesHelper.setValorCelda(sheet, i, "G", (double)obj.montoIGV, totalsCellStyle);
                i++;

                UtilesHelper.setValorCelda(sheet, i, "F", "TOTAL");
                UtilesHelper.setValorCelda(sheet, i, "G", (double)obj.montoTotal, totalsCellStyle);
                i++;
                


                i = i + 3;
                UtilesHelper.combinarCeldas(sheet, i, i, "A", "G");
                UtilesHelper.setValorCelda(sheet, i, "A", "Si usted tiene alguna pregunta sobre esta orden de compra, por favor, póngase en contacto con", footerTextCellStyle);

                i++;
                UtilesHelper.combinarCeldas(sheet, i, i, "A", "G");
                UtilesHelper.setValorCelda(sheet, i, "A", "[Nombre, Teléfono, E-Mail]", footerTextCellStyle);

                MemoryStream ms = new MemoryStream();
                using (MemoryStream tempStream = new MemoryStream())
                {
                    wb.Write(tempStream);
                    var byteArray = tempStream.ToArray();
                    ms.Write(byteArray, 0, byteArray.Length);
                    ms.Flush();
                    ms.Position = 0;
                    FileStreamResult result = new FileStreamResult(ms, "application/vnd.ms-excel");

                    result.FileDownloadName = "OC_" + obj.numeroPedido + " .xls";

                    return result;
                }
            }
        }
    }
}