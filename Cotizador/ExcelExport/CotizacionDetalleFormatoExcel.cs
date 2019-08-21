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
    public class CotizacionDetalleFormatoExcel : CotizacionDetalleExcel
    {
        public FileStreamResult generateExcelFormato()
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

                HSSFCellStyle titleDataStaticCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleDataStaticCellStyle.CloneStyleFrom(titleDataCellStyle);
                titleDataStaticCellStyle.FillForegroundColor = HSSFColor.Grey50Percent.Index;


                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("DETALLE");


                /*Cabecera, Sub total*/
                int rTotal = filaInicioDatos + cantFilasRegistrosAdicionales;
                int cTotal = 22;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c).CellStyle = defaulCellStyle;
                    }
                }

                UtilesHelper.setValorCelda(sheet, 4, "A", "Las columnas con '*' en su cabecera son las que se considererán en la carga de productos. Las demás columnas son calculadas o es información que se obtiene de los registros del sistema.");

                int i = filaInicioDatos - 1;


                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "A", "PROV.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "SKU PROV", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "*SKU", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "IMG.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "DESCRIPCIÓN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "*UNIDAD", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "*CANT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "P. LISTA", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", "% DSCTO.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", "*P. NETO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "K", "*FLETE", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "L", "P. UNIT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "M", "TOTAL", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "N", "*OBSERVACIONES", titleDataCellStyle);

                UtilesHelper.setColumnWidth(sheet, "A", 2300);
                UtilesHelper.setColumnWidth(sheet, "B", 3500);
                UtilesHelper.setColumnWidth(sheet, "C", 2500);
                UtilesHelper.setColumnWidth(sheet, "D", 1800);
                UtilesHelper.setColumnWidth(sheet, "E", 12000);
                UtilesHelper.setColumnWidth(sheet, "F", 8000);
                UtilesHelper.setColumnWidth(sheet, "G", 2000);

                UtilesHelper.setColumnWidth(sheet, "H", 3000);
                UtilesHelper.setColumnWidth(sheet, "I", 3000);
                UtilesHelper.setColumnWidth(sheet, "J", 3000);
                UtilesHelper.setColumnWidth(sheet, "K", 3000);
                UtilesHelper.setColumnWidth(sheet, "L", 3500);
                UtilesHelper.setColumnWidth(sheet, "M", 3500);

                UtilesHelper.setColumnWidth(sheet, "N", 8000);

                i++;

                this.marcarUnidadesNuevosRegistros(sheet, i - 1, cantFilasRegistrosAdicionales, 5);

                String rowDesc = "";
                for(int j = 0; j < cantFilasRegistrosAdicionales; j++)
                {
                    rowDesc = (filaInicioDatos + j).ToString();
                    UtilesHelper.setFormulaCelda(sheet, filaInicioDatos + j, "L", "J" + rowDesc + "+" + "K" + rowDesc);
                    UtilesHelper.setFormulaCelda(sheet, filaInicioDatos + j, "M", "G" + rowDesc + "*" + "L" + rowDesc);
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

                    result.FileDownloadName = "FORMATO_CARGA_PRODUCTOS_COTIZACION.xls";

                    return result;
                }
            }
        }
    }
}