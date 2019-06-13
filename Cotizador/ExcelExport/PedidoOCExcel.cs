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

                HSSFCellStyle descCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                descCellStyle.WrapText = true;

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
                titleCellStyle.FillForegroundColor = HSSFColor.SkyBlue.Index;

                //titleCellStyle.FillBackgroundColor = HSSFColor.BlueGrey.Index;

                HSSFCellStyle ocTitleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                HSSFFont ocTitleFont = (HSSFFont)wb.CreateFont();
                ocTitleFont.FontHeightInPoints = (short)28;
                ocTitleFont.FontName = "Arial";
                ocTitleFont.Color = IndexedColors.SkyBlue.Index;
                ocTitleFont.IsBold = true;
                ocTitleCellStyle.SetFont(ocTitleFont);
                ocTitleCellStyle.Alignment = HorizontalAlignment.Center;

                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                var avgCellFormate = wb.CreateDataFormat();
                var twoDecFormat = avgCellFormate.GetFormat("0.00");
                HSSFCellStyle twoDecCellStyle = (HSSFCellStyle) wb.CreateCellStyle();
                twoDecCellStyle.DataFormat = twoDecFormat;
                

                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Atenciones");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (obj.pedidoDetalleList.Count) + 40;
                int cTotal = 10;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c);
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


                UtilesHelper.setValorCelda(sheet, 2, "E", "FECHA:");
                UtilesHelper.combinarCeldas(sheet, 2, 2, "F", "G");
                UtilesHelper.setValorCelda(sheet, 2, "F", obj.FechaRegistroFormatoFecha);

                UtilesHelper.setValorCelda(sheet, 3, "E", "N° OC:");
                UtilesHelper.combinarCeldas(sheet, 3, 3, "F", "G");
                UtilesHelper.setValorCelda(sheet, 3, "F", obj.numeroPedido);

                int i = 5; 

                UtilesHelper.setValorCelda(sheet, i, "A", "CÓDIGO", titleCellStyle);
                UtilesHelper.combinarCeldas(sheet, i, i, "B", "C");
                UtilesHelper.setValorCelda(sheet, i, "B", "DESCRIPCIÓN", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "UND", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "CANT.", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "P. UNIT.", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "TOTAL", titleCellStyle);

                UtilesHelper.setColumnWidth(sheet, "A", 3000);
                UtilesHelper.setColumnWidth(sheet, "B", 6000);
                UtilesHelper.setColumnWidth(sheet, "C", 12000);
                UtilesHelper.setColumnWidth(sheet, "D", 5000);

                UtilesHelper.setColumnWidth(sheet, "F", 2850);
                UtilesHelper.setColumnWidth(sheet, "G", 2850);
                UtilesHelper.setColumnWidth(sheet, "G", 2850);

                i++;

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (PedidoDetalle det in obj.pedidoDetalleList)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", det.producto.sku);
                    UtilesHelper.combinarCeldas(sheet, i, i, "B", "C");
                    UtilesHelper.setValorCelda(sheet, i, "B", det.producto.descripcion, descCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "D", det.unidad, descCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "E", det.cantidad);
                    UtilesHelper.setValorCelda(sheet, i, "F", (double) det.precioUnitario, twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "G", (double) det.subTotal, twoDecCellStyle);

                    i++;
                }


                UtilesHelper.setValorCelda(sheet, i, "F", "SUB TOTAL");
                UtilesHelper.setValorCelda(sheet, i, "G", (double)obj.montoSubTotal, twoDecCellStyle);
                i++;

                UtilesHelper.setValorCelda(sheet, i, "F", "IGV");
                UtilesHelper.setValorCelda(sheet, i, "G", (double)obj.montoIGV, twoDecCellStyle);
                i++;

                UtilesHelper.setValorCelda(sheet, i, "F", "TOTAL");
                UtilesHelper.setValorCelda(sheet, i, "G", (double)obj.montoTotal, twoDecCellStyle);
                i++;
                


                i = i + 3;
                UtilesHelper.combinarCeldas(sheet, i, i, "A", "G");
                UtilesHelper.setValorCelda(sheet, i, "A", "Si usted tiene alguna pregunta sobre esta orden de compra, por favor, póngase en contacto con");

                i++;
                UtilesHelper.combinarCeldas(sheet, i, i, "A", "G");
                UtilesHelper.setValorCelda(sheet, i, "A", "[]");

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