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
    public class CotizacionDetalleExcel
    {
        public FileStreamResult generateExcel(Cotizacion obj)
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
                sheet = (HSSFSheet)wb.CreateSheet("OC-" + obj.codigo);


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (obj.cotizacionDetalleList.Count) + 40;
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

                UtilesHelper.setValorCelda(sheet, 2, "A", "[Dirección]");
                UtilesHelper.setValorCelda(sheet, 3, "A", "[Ciudad, Estado, postal]");
                UtilesHelper.setValorCelda(sheet, 4, "A", "Telefono: (000) 000-0000");
                UtilesHelper.setValorCelda(sheet, 5, "A", "Fax: (000) 000-0000");
                UtilesHelper.setValorCelda(sheet, 6, "A", "http://mpinstitucional.com");
<<<<<<< HEAD


=======
                
                
                
>>>>>>> origin/devInvoice

                UtilesHelper.setValorCelda(sheet, 2, "F", "FECHA:", formLabelCellStyle);
                UtilesHelper.combinarCeldas(sheet, 2, 2, "G", "H");
                //UtilesHelper.setValorCelda(sheet, 2, "G", obj.FechaRegistroFormatoFecha, formDataCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "H", "", formDataCenterCellStyle);

                UtilesHelper.setValorCelda(sheet, 3, "F", "N° OC:", formLabelCellStyle);
                UtilesHelper.combinarCeldas(sheet, 3, 3, "G", "H");
                //UtilesHelper.setValorCelda(sheet, 3, "G", obj.numeroPedido, formDataCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, 3, "H", "", formDataCenterCellStyle);

                int i = 9;

                UtilesHelper.combinarCeldas(sheet, i, i, "A", "C");
                UtilesHelper.setValorCelda(sheet, i, "A", "VENDEDOR", titleCellStyle);


                UtilesHelper.setValorCelda(sheet, i + 1, "A", "MP INSTITUCIONAL S.A.C.");
                UtilesHelper.setValorCelda(sheet, i + 2, "A", "[Contacto o Departamento]");
                UtilesHelper.setValorCelda(sheet, i + 3, "A", "[Dirección]");
                UtilesHelper.setValorCelda(sheet, i + 4, "A", "[Ciudad, Estado, postal]");
                UtilesHelper.setValorCelda(sheet, i + 5, "A", "Telefono: (000) 000-0000");
                UtilesHelper.setValorCelda(sheet, i + 6, "A", "Fax: (000) 000-0000");
                
                
                UtilesHelper.combinarCeldas(sheet, i, i, "F", "H");
                UtilesHelper.setValorCelda(sheet, i, "F", "ENVÍE A", titleCellStyle);

                UtilesHelper.setValorCelda(sheet, i + 1, "F", "[Nombre]");
                UtilesHelper.setValorCelda(sheet, i + 2, "F", "[Nombre de empresa]");
                UtilesHelper.setValorCelda(sheet, i + 3, "F", "[Dirección]");
                UtilesHelper.setValorCelda(sheet, i + 4, "F", "[Ciudad, Estado, postal]");
                UtilesHelper.setValorCelda(sheet, i + 5, "F", "[Telefono]");


                i = i + 9;

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "A", "SKU", titleDataCellStyle);
<<<<<<< HEAD
                UtilesHelper.setValorCelda(sheet, i, "B", "SKU PROV" , titleDataCellStyle);
                UtilesHelper.combinarCeldas(sheet, i, i, "C", "D");
                UtilesHelper.setValorCelda(sheet, i, "C", "DESCRIPCIÓN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "UND", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "CANT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "P. UNIT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "TOTAL", titleDataCellStyle);

                UtilesHelper.setColumnWidth(sheet, "A", 2200);
                UtilesHelper.setColumnWidth(sheet, "B", 1500);
                UtilesHelper.setColumnWidth(sheet, "C", 4200);
                UtilesHelper.setColumnWidth(sheet, "D", 8000);
                UtilesHelper.setColumnWidth(sheet, "E", 5000);

                UtilesHelper.setColumnWidth(sheet, "G", 2900);
                UtilesHelper.setColumnWidth(sheet, "H", 3500);
=======
                UtilesHelper.setValorCelda(sheet, i, "B", "IMAGEN" , titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "DESCRIPCIÓN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "UNIDAD", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "CANT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "P. UNIT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "TOTAL", titleDataCellStyle);

                UtilesHelper.setColumnWidth(sheet, "A", 2200);
                UtilesHelper.setColumnWidth(sheet, "B", 1500);
                UtilesHelper.setColumnWidth(sheet, "C", 12000);
                UtilesHelper.setColumnWidth(sheet, "D", 5000);
                UtilesHelper.setColumnWidth(sheet, "E", 2000);

                UtilesHelper.setColumnWidth(sheet, "G", 2500);
                UtilesHelper.setColumnWidth(sheet, "H", 2500);
>>>>>>> origin/devInvoice

                i++;

                HSSFCellStyle tableDataCenterCellStyle = (HSSFCellStyle) UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                HSSFCellStyle tableDataLastCenterCellStyle = (HSSFCellStyle) UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataLastCellStyle);

                HSSFCellStyle twoDecCenterCellStyle = (HSSFCellStyle) UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecCellStyle);
                HSSFCellStyle twoDecLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecLastCellStyle);

                HSSFCellStyle twoDecIdentCellStyle = (HSSFCellStyle) UtilesHelper.GetCloneStyleWithIndent(wb, twoDecCellStyle, 1);
                HSSFCellStyle twoDecIdentLastCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithIndent(wb, twoDecLastCellStyle, 1);

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (CotizacionDetalle det in obj.cotizacionDetalleList)
                {
                    UtilesHelper.setRowHeight(sheet, i, 810);

                    int picDet = newWorkbook.AddPicture(det.producto.image, PictureType.PNG);
                    HSSFClientAnchor detAnchor = helper.CreateClientAnchor() as HSSFClientAnchor;
                    detAnchor.Col1 = 1;
                    detAnchor.Row1 = i-1;

                    detAnchor.AnchorType = AnchorType.DontMoveAndResize;

                    HSSFPicture pictdet = drawing.CreatePicture(detAnchor, picDet) as HSSFPicture;
                    pictdet.Resize(1);

                    UtilesHelper.setValorCelda(sheet, i, "A", det.producto.sku);
<<<<<<< HEAD
=======
                    UtilesHelper.setValorCelda(sheet, i, "C", det.producto.descripcion);
                    UtilesHelper.setValorCelda(sheet, i, "D", det.unidad);
                    UtilesHelper.setValorCelda(sheet, i, "E", det.cantidad);
                    UtilesHelper.setValorCelda(sheet, i, "F", (double) det.precioUnitario);
                    UtilesHelper.setValorCelda(sheet, i, "G", (double) det.subTotal);
>>>>>>> origin/devInvoice

                    i++;
                }

            

                MemoryStream ms = new MemoryStream();
                using (MemoryStream tempStream = new MemoryStream())
                {
                    wb.Write(tempStream);
                    var byteArray = tempStream.ToArray();
                    ms.Write(byteArray, 0, byteArray.Length);
                    ms.Flush();
                    ms.Position = 0;
                    FileStreamResult result = new FileStreamResult(ms, "application/vnd.ms-excel");

                    result.FileDownloadName = "cot_" + obj.codigo + " .xls";

                    return result;
                }
            }
        }
    }
}