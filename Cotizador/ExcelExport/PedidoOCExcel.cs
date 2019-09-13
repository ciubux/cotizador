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
using System.Globalization;

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
                formLabelCellStyle.FillPattern = FillPattern.SolidForeground;
                formLabelCellStyle.FillForegroundColor = HSSFColor.White.Index;


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
                titleDataCellStyle.VerticalAlignment = VerticalAlignment.Center;
                titleDataCellStyle.BorderLeft = BorderStyle.Thin;
                titleDataCellStyle.BorderTop = BorderStyle.Thin;
                titleDataCellStyle.BorderRight = BorderStyle.Thin;

                HSSFCellStyle addressTextCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                addressTextCellStyle.CloneStyleFrom(defaulCellStyle);
                addressTextCellStyle.VerticalAlignment = VerticalAlignment.Top;
                addressTextCellStyle.WrapText = true;

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

                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                var avgCellFormate = wb.CreateDataFormat();
                var twoDecFormat = avgCellFormate.GetFormat("0.00");
                HSSFCellStyle twoDecCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                twoDecCellStyle.DataFormat = twoDecFormat;
                twoDecCellStyle.VerticalAlignment = VerticalAlignment.Center;
                twoDecCellStyle.BorderLeft = BorderStyle.Thin;
                twoDecCellStyle.BorderRight = BorderStyle.Thin;


                HSSFCellStyle totalsCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsCellStyle.DataFormat = twoDecFormat;
                totalsCellStyle.WrapText = true;
                totalsCellStyle.BorderLeft = BorderStyle.Thin;
                totalsCellStyle.BorderRight = BorderStyle.Thin;
                totalsCellStyle.BorderTop = BorderStyle.Thin;
                totalsCellStyle.BorderBottom = BorderStyle.Thin;
                totalsCellStyle.Indention = 1;


                HSSFCellStyle totalsTotalCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsTotalCellStyle.CloneStyleFrom(totalsCellStyle);
                totalsTotalCellStyle.BorderTop = BorderStyle.Double;
                HSSFFont totalFont = (HSSFFont)wb.CreateFont();
                totalFont.FontHeightInPoints = (short)11;
                totalFont.FontName = "Arial";
                totalFont.IsBold = true;
                totalsTotalCellStyle.SetFont(totalFont);


                HSSFCellStyle totalsTotalLabelCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsTotalLabelCellStyle.SetFont(totalFont);
                totalsTotalLabelCellStyle.BorderTop = BorderStyle.Double;

                HSSFCellStyle tableDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                tableDataCellStyle.WrapText = true;
                tableDataCellStyle.VerticalAlignment = VerticalAlignment.Center;
                tableDataCellStyle.BorderLeft = BorderStyle.Thin;
                tableDataCellStyle.BorderRight = BorderStyle.Thin;

                HSSFCellStyle tableDataLastCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                tableDataLastCellStyle.CloneStyleFrom(tableDataCellStyle);
                tableDataLastCellStyle.WrapText = true;
                tableDataLastCellStyle.BorderBottom = BorderStyle.Thin;

                HSSFCellStyle twoDecLastCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                twoDecLastCellStyle.CloneStyleFrom(twoDecCellStyle);
                twoDecLastCellStyle.BorderBottom = BorderStyle.Thin;

                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("OC-" + obj.numeroPedido);


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (obj.pedidoDetalleList.Count) + 40;
                int cTotal = 8;

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
                anchor.Col2 = 3;

                anchor.AnchorType = AnchorType.DontMoveAndResize;

                HSSFPicture pict = drawing.CreatePicture(anchor, picInd) as HSSFPicture;
                pict.Resize(1);

                UtilesHelper.combinarCeldas(sheet, 1, 1, "E", "H");
                UtilesHelper.setValorCelda(sheet, 1, "E", "ORDEN DE COMPRA", ocTitleCellStyle);

                UtilesHelper.setRowHeight(sheet, 1, 1200);


                UtilesHelper.setValorCelda(sheet, 3, "A", "Señores");
                UtilesHelper.setValorCelda(sheet, 4, "A", obj.cliente.razonSocialSunat);
                UtilesHelper.setValorCelda(sheet, 5, "A", textInfo.ToTitleCase(obj.cliente.direccionDomicilioLegalSunat.ToLower()));
                UtilesHelper.setValorCelda(sheet, 6, "A", "Atn.- " + obj.cliente.contacto1);

                UtilesHelper.setValorCelda(sheet, 2, "F", "FECHA:", formLabelCellStyle);
                UtilesHelper.combinarCeldas(sheet, 2, 2, "G", "H");
                UtilesHelper.setValorCelda(sheet, 2, "G", obj.FechaRegistroFormatoFecha, formDataCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "H", "", formDataCenterCellStyle);

                UtilesHelper.setValorCelda(sheet, 3, "F", "N° OC:", formLabelCellStyle);
                UtilesHelper.combinarCeldas(sheet, 3, 3, "G", "H");

                string codOC = "MP" + obj.ciudad.sede + "-" + obj.numeroPedido + "-" + obj.pedidoDetalleList.ElementAt(0).producto.proveedor;
                UtilesHelper.setValorCelda(sheet, 3, "G", codOC, formDataCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, 3, "H", "", formDataCenterCellStyle);

                int i = 9;

                UtilesHelper.combinarCeldas(sheet, i, i, "A", "C");
                UtilesHelper.setValorCelda(sheet, i, "A", "GIRAR FACTURA A", titleCellStyle);


                UtilesHelper.setValorCelda(sheet, i + 1, "A", Constantes.RAZON_SOCIAL_MP);
                UtilesHelper.setValorCelda(sheet, i + 2, "A", "RUC: " + Constantes.RUC_MP);
                UtilesHelper.setValorCelda(sheet, i + 3, "A", "", addressTextCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 4, "A", "", addressTextCellStyle);
                UtilesHelper.combinarCeldas(sheet, i + 3, i + 4, "A", "C");
                UtilesHelper.setValorCelda(sheet, i + 3, "A", textInfo.ToTitleCase(Constantes.DIRECCION_MP.ToLower()));
                UtilesHelper.setValorCelda(sheet, i + 5, "A", "Teléfono: " + Constantes.TELEFONO_MP);
                UtilesHelper.setValorCelda(sheet, i + 6, "A", Constantes.WEB_MP);
                

              
                UtilesHelper.combinarCeldas(sheet, i, i, "F", "H");
                UtilesHelper.setValorCelda(sheet, i, "F", "FECHA DE ENTREGA", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "F",  obj.rangoFechasEntrega);
                UtilesHelper.setValorCelda(sheet, i + 2, "F", obj.rangoHoraEntrega);

                i = 12;
                UtilesHelper.combinarCeldas(sheet, i, i, "F", "H");
                UtilesHelper.setValorCelda(sheet, i, "F", "ENVÍE A", titleCellStyle);

                UtilesHelper.setValorCelda(sheet, i + 1, "F", "MP " + obj.ciudad.nombre);
                UtilesHelper.setValorCelda(sheet, i + 2, "F", "", addressTextCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 3, "F", "", addressTextCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 2, "F", textInfo.ToTitleCase(obj.ciudad.direccionPuntoLlegada.ToLower()));
                UtilesHelper.combinarCeldas(sheet, i + 2, i + 5, "F", "H");


                i = i + 8;

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "A", "SKU", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "SKU PROV" , titleDataCellStyle);
                UtilesHelper.combinarCeldas(sheet, i, i, "C", "D");
                UtilesHelper.setValorCelda(sheet, i, "C", "DESCRIPCIÓN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "UND", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "CANT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "P. UNIT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "TOTAL", titleDataCellStyle);

                UtilesHelper.setColumnWidth(sheet, "A", 2200);
                UtilesHelper.setColumnWidth(sheet, "B", 2900);
                UtilesHelper.setColumnWidth(sheet, "C", 4200);
                UtilesHelper.setColumnWidth(sheet, "D", 8000);
                UtilesHelper.setColumnWidth(sheet, "E", 5000);

                UtilesHelper.setColumnWidth(sheet, "G", 2900);
                UtilesHelper.setColumnWidth(sheet, "H", 3500);

                i++;

                HSSFCellStyle tableDataCenterCellStyle = (HSSFCellStyle) UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                HSSFCellStyle tableDataLastCenterCellStyle = (HSSFCellStyle) UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataLastCellStyle);

                HSSFCellStyle twoDecCenterCellStyle = (HSSFCellStyle) UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecCellStyle);
                HSSFCellStyle twoDecLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecLastCellStyle);

                HSSFCellStyle twoDecIdentCellStyle = (HSSFCellStyle) UtilesHelper.GetCloneStyleWithIndent(wb, twoDecCellStyle, 1);
                HSSFCellStyle twoDecIdentLastCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithIndent(wb, twoDecLastCellStyle, 1);

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (PedidoDetalle det in obj.pedidoDetalleList)
                {
                    UtilesHelper.setRowHeight(sheet, i, 540);
                    UtilesHelper.setValorCelda(sheet, i, "A", "", tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", "", tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "C", "", tableDataCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "D", "", tableDataCellStyle);
                    
                    UtilesHelper.setValorCelda(sheet, i, "E", "", tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "F", "", tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "G", "", tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "H", "", tableDataCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "A", det.producto.sku);
                    UtilesHelper.setValorCelda(sheet, i, "B", det.producto.skuProveedor);
                    UtilesHelper.combinarCeldas(sheet, i, i, "C", "D");
                    UtilesHelper.setValorCelda(sheet, i, "C", det.producto.descripcion);
                    UtilesHelper.setValorCelda(sheet, i, "E", det.unidad);
                    UtilesHelper.setValorCelda(sheet, i, "F", det.cantidad);
                    
                    
                    UtilesHelper.setValorCelda(sheet, i, "G", (double)det.precioUnitario, twoDecCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "H", (double)det.subTotal, twoDecIdentCellStyle);
                    
                    i++;
                }

                UtilesHelper.setValorCelda(sheet, i - 1, "A", UtilesHelper.getValorCelda(sheet, i - 1, "A"), tableDataLastCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "B", UtilesHelper.getValorCelda(sheet, i - 1, "B"), tableDataLastCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "C", UtilesHelper.getValorCelda(sheet, i - 1, "C"), tableDataLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "D", UtilesHelper.getValorCelda(sheet, i - 1, "D"), tableDataLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "E", UtilesHelper.getValorCelda(sheet, i - 1, "E"), tableDataLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "F", int.Parse(UtilesHelper.getValorCelda(sheet, i - 1, "F")), tableDataLastCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "G", double.Parse(UtilesHelper.getValorCelda(sheet, i - 1, "G")), twoDecLastCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "H", double.Parse(UtilesHelper.getValorCelda(sheet, i - 1, "H")), twoDecIdentLastCellStyle);


                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "G", "SUB TOTAL");
                UtilesHelper.setValorCelda(sheet, i, "H", (double)obj.montoSubTotal, totalsCellStyle);
                i++;

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "G", "IGV");
                UtilesHelper.setValorCelda(sheet, i, "H", (double)obj.montoIGV, totalsCellStyle);
                i++;

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "G", "TOTAL", totalsTotalLabelCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", (double)obj.montoTotal, totalsTotalCellStyle);
                i++;
                


                i = i + 3;
                UtilesHelper.combinarCeldas(sheet, i, i, "A", "H");
                UtilesHelper.setValorCelda(sheet, i, "A", "Si usted tiene alguna pregunta sobre esta orden de compra, por favor, póngase en contacto con", footerTextCellStyle);

                i++;
                UtilesHelper.combinarCeldas(sheet, i, i, "A", "H");
                UtilesHelper.setValorCelda(sheet, i, "A", obj.usuario.nombre + " | " + obj.usuario.contacto + " | E-Mail: " + obj.usuario.email, footerTextCellStyle);

                MemoryStream ms = new MemoryStream();
                using (MemoryStream tempStream = new MemoryStream())
                {
                    wb.Write(tempStream);
                    var byteArray = tempStream.ToArray();
                    ms.Write(byteArray, 0, byteArray.Length);
                    ms.Flush();
                    ms.Position = 0;
                    FileStreamResult result = new FileStreamResult(ms, "application/vnd.ms-excel");

                    result.FileDownloadName = "OC_" + codOC.Replace("-", "_") + " .xls";

                    return result;
                }
            }
        }
    }
}