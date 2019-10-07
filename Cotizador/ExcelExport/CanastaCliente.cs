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
using NPOI.SS.Util;
using BusinessLayer;

using System.Web.Mvc;


namespace Cotizador.ExcelExport
{
    public class CanastaCliente
    {
        protected int cantFilasRegistrosAdicionales = 3;
        public static int filaInicioDatos { get { return 2; } }

        public FileStreamResult generateExcel(Cliente obj)
        {
            HSSFWorkbook wb;
            //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);
            HSSFSheet sheet;
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());

                HSSFCellStyle defaulCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                

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

                HSSFCellStyle lastDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                lastDataCellStyle.BorderTop = BorderStyle.Thin;
                lastDataCellStyle.FillPattern = FillPattern.SolidForeground;
                lastDataCellStyle.FillForegroundColor = HSSFColor.White.Index;


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
                titleDataCellStyle.WrapText = true;
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
                twoDecCellStyle.Indention = 1;


                HSSFCellStyle twoDecPercentageCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecCellStyle);
                twoDecPercentageCellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                twoDecPercentageCellStyle.Alignment = HorizontalAlignment.Right;

                HSSFCellStyle totalsCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsCellStyle.DataFormat = twoDecFormat;
                totalsCellStyle.WrapText = true;
                totalsCellStyle.VerticalAlignment = VerticalAlignment.Center;
                totalsCellStyle.BorderLeft = BorderStyle.Thin;
                totalsCellStyle.BorderRight = BorderStyle.Thin;
                totalsCellStyle.BorderTop = BorderStyle.Thin;
                totalsCellStyle.BorderBottom = BorderStyle.Thin;
                totalsCellStyle.Indention = 1;


                HSSFCellStyle totalsTotalCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsTotalCellStyle.CloneStyleFrom(totalsCellStyle);
                totalsTotalCellStyle.BorderTop = BorderStyle.Double;
                totalsTotalCellStyle.VerticalAlignment = VerticalAlignment.Center;
                HSSFFont totalFont = (HSSFFont)wb.CreateFont();
                totalFont.FontHeightInPoints = (short)11;
                totalFont.FontName = "Arial";
                totalFont.IsBold = true;
                totalsTotalCellStyle.SetFont(totalFont);


                HSSFCellStyle totalsLabelCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsLabelCellStyle.BorderRight = BorderStyle.Thin;
                totalsLabelCellStyle.BorderLeft = BorderStyle.Thin;
                totalsLabelCellStyle.BorderBottom = BorderStyle.Thin;
                totalsLabelCellStyle.BorderTop = BorderStyle.Thin;
                totalsLabelCellStyle.VerticalAlignment = VerticalAlignment.Center;
                totalsLabelCellStyle.Alignment = HorizontalAlignment.Right;
                totalsLabelCellStyle.Indention = 1;

                HSSFCellStyle totalsTotalLabelCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsTotalLabelCellStyle.CloneStyleFrom(totalsLabelCellStyle);
                totalsTotalLabelCellStyle.SetFont(totalFont);
                totalsTotalLabelCellStyle.BorderTop = BorderStyle.Double;
                totalsTotalLabelCellStyle.VerticalAlignment = VerticalAlignment.Center;
                totalsTotalLabelCellStyle.Alignment = HorizontalAlignment.Right;

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
                sheet = (HSSFSheet)wb.CreateSheet("COT-" + obj.codigo);


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (obj.listaPrecios.Count) + cantFilasRegistrosAdicionales;
                int cTotal = 14;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c).CellStyle = defaulCellStyle;
                    }
                }

                
                int i = filaInicioDatos - 1; 

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "A", "PROV.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "SKU", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "DESCRIPCIÓN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "IMG.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "UNIDAD", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "EQUIV.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "P. LISTA", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "% DSCTO PRECIO LISTA.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", "P. NETO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", "FLETE", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "K", "P. UNIT.", titleDataCellStyle);
              
                

                UtilesHelper.setColumnWidth(sheet, "A", 2100);
                UtilesHelper.setColumnWidth(sheet, "B", 3000);
                UtilesHelper.setColumnWidth(sheet, "C", 12000);
                UtilesHelper.setColumnWidth(sheet, "D", 2300);
                UtilesHelper.setColumnWidth(sheet, "E", 8000);
                UtilesHelper.setColumnWidth(sheet, "F", 2000);
                UtilesHelper.setColumnWidth(sheet, "G", 3000);
                UtilesHelper.setColumnWidth(sheet, "H", 3000);
                UtilesHelper.setColumnWidth(sheet, "I", 3000);
                UtilesHelper.setColumnWidth(sheet, "J", 3000);
                UtilesHelper.setColumnWidth(sheet, "K", 3000);

                if (obj.usuario.visualizaMargen)
                {
                    UtilesHelper.setValorCelda(sheet, i, "L", "% MARGEN", titleDataCellStyle);
                    UtilesHelper.setColumnWidth(sheet, "L", 3500);

                    UtilesHelper.setValorCelda(sheet, i, "M", "PERTENECE CANASTA HABITUAL", titleDataCellStyle);
                    UtilesHelper.setColumnWidth(sheet, "M", 5000);
                } else
                {
                    UtilesHelper.setValorCelda(sheet, i, "L", "PERTENECE CANASTA HABITUAL", titleDataCellStyle);
                    UtilesHelper.setColumnWidth(sheet, "L", 5000);
                }


                i = filaInicioDatos;

                HSSFCellStyle tableDataCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                HSSFCellStyle tableDataLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataLastCellStyle);

                HSSFCellStyle twoDecCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecCellStyle);
                HSSFCellStyle twoDecLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecLastCellStyle);

                HSSFCellStyle twoDecIdentCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithIndent(wb, twoDecCellStyle, 1);
                HSSFCellStyle twoDecIdentLastCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithIndent(wb, twoDecLastCellStyle, 1);

                IWorkbook newWorkbook = wb;
                HSSFCreationHelper helper = newWorkbook.GetCreationHelper() as HSSFCreationHelper;
                IDrawing drawing = sheet.CreateDrawingPatriarch();

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (DocumentoDetalle det in obj.listaPrecios)
                {
                    UtilesHelper.setRowHeight(sheet, i, 810);

                    int picDet = newWorkbook.AddPicture(det.producto.image, PictureType.PNG);
                    HSSFClientAnchor detAnchor = helper.CreateClientAnchor() as HSSFClientAnchor;
                    detAnchor.Col1 = 3;
                    detAnchor.Row1 = i - 1;
                    detAnchor.AnchorType = AnchorType.DontMoveAndResize;

                    HSSFPicture pictdet = drawing.CreatePicture(detAnchor, picDet) as HSSFPicture;
                    pictdet.Resize(1);

                    

                    UtilesHelper.setValorCelda(sheet, i, "A", det.producto.proveedor, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", det.producto.sku, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "C", det.producto.descripcion, tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "E", det.unidad, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "F", (double) det.producto.precioClienteProducto.equivalencia, twoDecCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "G", (double) det.precioLista, twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "H", (double) det.porcentajeDescuentoMostrar, twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "I", (double) det.precioNeto, twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "J", (double) det.flete, twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "K", (double) det.producto.precioClienteProducto.precioUnitario, twoDecCellStyle);


                    if (obj.usuario.visualizaMargen)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "L", (double)det.porcentajeMargenMostrar, twoDecCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "M", det.producto.precioClienteProducto.estadoCanasta ? "SI" : "NO", tableDataCenterCellStyle);
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "L", det.producto.precioClienteProducto.estadoCanasta ? "SI" : "NO", tableDataCenterCellStyle);
                    }

                    i++;
                }

                UtilesHelper.setValorCelda(sheet, i, "A", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "K", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "L", "", lastDataCellStyle);

                if (obj.usuario.visualizaMargen)
                {
                    UtilesHelper.setValorCelda(sheet, i, "M", "", lastDataCellStyle);
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

                    result.FileDownloadName = "CANASTA_CLIENTE_" + obj.codigo + " .xls";

                    return result;
                }
            }
        }

    }
}