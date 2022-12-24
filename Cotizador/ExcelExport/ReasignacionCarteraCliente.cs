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
   
    public class ReasignacionCarteraCliente
    {
        private HSSFCellStyle defaulCellStyle;

        private HSSFFont formLabelFont;
        private HSSFCellStyle formLabelCellStyle;
        private HSSFCellStyle boldTextCenterCellStyle;
        private HSSFCellStyle formDataCenterCellStyle;
        private HSSFCellStyle tableDataCellStyle;
        private HSSFFont titleFont;
        private HSSFCellStyle titleCellStyle;
        private HSSFCellStyle blockedDataCellStyle;
        private HSSFCellStyle titleDataCellStyle;
        private HSSFCellStyle familiaCellStyle;
        private HSSFCellStyle tableDataLastCellStyle;
        private HSSFCellStyle tableDataCenterCellStyle;
        private HSSFCellStyle tableDataCellStyleB;
        private HSSFCellStyle tableDataCenterCellStyleB;
        private HSSFCellStyle tableDataLastCenterCellStyle;
        private IDataFormat format;

        public FileStreamResult generateExcel(List<List<String>> list, List<List<String>> listSup, List<List<String>> listAsis)
        {
            
            HSSFWorkbook wb;
            //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);
            HSSFSheet sheet;
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());


                defaulCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                defaulCellStyle.FillPattern = FillPattern.SolidForeground;
                defaulCellStyle.FillForegroundColor = HSSFColor.White.Index;

                formLabelFont = (HSSFFont)wb.CreateFont();
                formLabelFont.FontHeightInPoints = (short)11;
                formLabelFont.FontName = "Arial";
                formLabelFont.Color = IndexedColors.Black.Index;
                formLabelFont.IsBold = true;
                //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
                //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
                formLabelCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                formLabelCellStyle.SetFont(formLabelFont);
                formLabelCellStyle.Alignment = HorizontalAlignment.Right;
                formLabelCellStyle.FillPattern = FillPattern.SolidForeground;
                formLabelCellStyle.FillForegroundColor = HSSFColor.White.Index;

                boldTextCenterCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                boldTextCenterCellStyle.SetFont(formLabelFont);
                boldTextCenterCellStyle.Alignment = HorizontalAlignment.Center;

                formDataCenterCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                formDataCenterCellStyle.Alignment = HorizontalAlignment.Center;

                tableDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                tableDataCellStyle.WrapText = true;
                tableDataCellStyle.VerticalAlignment = VerticalAlignment.Center;
                tableDataCellStyle.BorderLeft = BorderStyle.Thin;
                tableDataCellStyle.BorderTop = BorderStyle.Thin;
                tableDataCellStyle.BorderRight = BorderStyle.Thin;
                tableDataCellStyle.BorderBottom = BorderStyle.Thin;


                titleFont = (HSSFFont)wb.CreateFont();
                titleFont.FontHeightInPoints = (short)11;
                titleFont.FontName = "Arial";
                titleFont.Color = IndexedColors.White.Index;
                titleFont.IsBold = true;
                //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
                //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
                titleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleCellStyle.SetFont(titleFont);
                titleCellStyle.Alignment = HorizontalAlignment.Center;
                titleCellStyle.FillPattern = FillPattern.SolidForeground;
                titleCellStyle.FillForegroundColor = HSSFColor.RoyalBlue.Index;

                blockedDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                blockedDataCellStyle.Alignment = HorizontalAlignment.Center;
                blockedDataCellStyle.FillPattern = FillPattern.SolidForeground;
                blockedDataCellStyle.FillForegroundColor = HSSFColor.Black.Index;


                titleDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleDataCellStyle.CloneStyleFrom(titleCellStyle);
                titleDataCellStyle.VerticalAlignment = VerticalAlignment.Center;
                titleDataCellStyle.BorderLeft = BorderStyle.Thin;
                titleDataCellStyle.BorderTop = BorderStyle.Thin;
                titleDataCellStyle.BorderRight = BorderStyle.Thin;
                titleDataCellStyle.BorderBottom = BorderStyle.Thin;

                familiaCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                familiaCellStyle.SetFont(formLabelFont);
                familiaCellStyle.VerticalAlignment = VerticalAlignment.Center;
                familiaCellStyle.Indention = 1;

                tableDataLastCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                tableDataLastCellStyle.CloneStyleFrom(tableDataCellStyle);
                tableDataLastCellStyle.WrapText = true;
                tableDataLastCellStyle.BorderBottom = BorderStyle.Thin;

                tableDataCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                tableDataCenterCellStyle.Alignment = HorizontalAlignment.Center;

                tableDataCellStyleB = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                tableDataCellStyleB.Alignment = HorizontalAlignment.Left;
                tableDataCellStyleB.FillPattern = FillPattern.SolidForeground;
                tableDataCellStyleB.FillForegroundColor = HSSFColor.Grey25Percent.Index;

                tableDataCenterCellStyleB = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCenterCellStyle);
                tableDataCenterCellStyleB.FillPattern = FillPattern.SolidForeground;
                tableDataCenterCellStyleB.FillForegroundColor = HSSFColor.Grey25Percent.Index;

                tableDataLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataLastCellStyle);
                tableDataLastCenterCellStyle.Alignment = HorizontalAlignment.Center;


                /*************/

                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");



                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Atenciones");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (list.Count) + 4;
                int cTotal = 5 + 2;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c);
                    }
                }

                int i = 0;

                UtilesHelper.setValorCelda(sheet, 1, "A", "Cliente", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "B", "Nro. Doc.", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "C", "Sede", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "D", "Asesor", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "E", "Reasignado A:", titleCellStyle);

                UtilesHelper.setColumnWidth(sheet, "A", 8000);
                UtilesHelper.setColumnWidth(sheet, "B", 3500);
                UtilesHelper.setColumnWidth(sheet, "C", 3500);
                UtilesHelper.setColumnWidth(sheet, "D", 8000);
                UtilesHelper.setColumnWidth(sheet, "E", 8000);

                i = 2;

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (List<String> item in list)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", item.ElementAt(0), tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", item.ElementAt(1), tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "C", item.ElementAt(2), tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "D", item.ElementAt(3), tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "E", item.ElementAt(4), tableDataCellStyle);
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

                    result.FileDownloadName = "ReasignacionCartera_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }
    }
}