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
using NPOI.SS.Util;

namespace Cotizador.ExcelExport
{
    public class CotizacionDetalleFormatoExcel
    {
        public FileStreamResult generateExcel()
        {

            HSSFWorkbook wb;
            //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);
            HSSFSheet sheet;
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());

                HSSFCellStyle defaulCellStyle = (HSSFCellStyle)wb.CreateCellStyle();

               

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



                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("DETALLE");

                //HSSFSheet sheetMeta = (HSSFSheet)wb.CreateSheet("META");
                //sheetMeta.CreateRow(0).CreateCell(0);
                //sheetMeta.CreateRow(0).CreateCell(1);
                //sheetMeta.CreateRow(0).CreateCell(2);
                //UtilesHelper.setValorCelda(sheet, 1, "A", "MP", titleDataCellStyle);
                //UtilesHelper.setValorCelda(sheet, 2, "A", "Alternativa", titleDataCellStyle);
                //UtilesHelper.setValorCelda(sheet, 3, "A", "Proveedor", titleDataCellStyle);


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = 200;
                int cTotal = 6;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c).CellStyle = defaulCellStyle;
                    }
                }



                var markConstraint = DVConstraint.CreateExplicitListConstraint(new string[] { "MP", "Alternativa", "Proveedor" });
                var markColumn = new CellRangeAddressList(1, rTotal, 1, 1);
                var markdv = new HSSFDataValidation(markColumn, markConstraint);
                markdv.EmptyCellAllowed = true;
                markdv.CreateErrorBox("Valor Incorrecto", "Por favor seleccione un tipo de unidad de la lista");
                sheet.AddValidationData(markdv);

                int i = 1;

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "A", "SKU", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "UNIDAD", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "P. UNIT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "FLETE (%)", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "CANT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "OBSERVACIONES", titleDataCellStyle);

                UtilesHelper.setColumnWidth(sheet, "A", 2200);
                UtilesHelper.setColumnWidth(sheet, "B", 4000);
                UtilesHelper.setColumnWidth(sheet, "C", 3000);
                UtilesHelper.setColumnWidth(sheet, "D", 3000);
                UtilesHelper.setColumnWidth(sheet, "E", 3000);
                UtilesHelper.setColumnWidth(sheet, "F", 8000);

                i++;


                MemoryStream ms = new MemoryStream();
                using (MemoryStream tempStream = new MemoryStream())
                {
                    wb.Write(tempStream);
                    var byteArray = tempStream.ToArray();
                    ms.Write(byteArray, 0, byteArray.Length);
                    ms.Flush();
                    ms.Position = 0;
                    FileStreamResult result = new FileStreamResult(ms, "application/vnd.ms-excel");

                    result.FileDownloadName = "FORMATO_CARGA_PRODUCTOS_COTIZACION.xls";

                    return result;
                }
            }
        }
    }
}