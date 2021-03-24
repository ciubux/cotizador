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
   
    public class PlantillaCargaStock
    {
        public FileStreamResult generateExcel(List<Producto> list, Usuario usuario)
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

                HSSFCellStyle boldTextCenterCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                boldTextCenterCellStyle.SetFont(formLabelFont);
                boldTextCenterCellStyle.Alignment = HorizontalAlignment.Center;

                HSSFCellStyle formDataCenterCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                formDataCenterCellStyle.Alignment = HorizontalAlignment.Center;
                
                HSSFCellStyle tableDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                tableDataCellStyle.WrapText = true;
                tableDataCellStyle.VerticalAlignment = VerticalAlignment.Center;
                tableDataCellStyle.BorderLeft = BorderStyle.Thin;
                tableDataCellStyle.BorderTop = BorderStyle.Thin;
                tableDataCellStyle.BorderRight = BorderStyle.Thin;
                tableDataCellStyle.BorderBottom = BorderStyle.Thin;


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

                HSSFCellStyle blockedDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                blockedDataCellStyle.Alignment = HorizontalAlignment.Center;
                blockedDataCellStyle.FillPattern = FillPattern.SolidForeground;
                blockedDataCellStyle.FillForegroundColor = HSSFColor.Black.Index;


                HSSFCellStyle titleDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleDataCellStyle.CloneStyleFrom(titleCellStyle);
                titleDataCellStyle.VerticalAlignment = VerticalAlignment.Center;
                titleDataCellStyle.BorderLeft = BorderStyle.Thin;
                titleDataCellStyle.BorderTop = BorderStyle.Thin;
                titleDataCellStyle.BorderRight = BorderStyle.Thin;
                titleDataCellStyle.BorderBottom = BorderStyle.Thin;

                HSSFCellStyle familiaCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                familiaCellStyle.SetFont(formLabelFont);
                familiaCellStyle.VerticalAlignment = VerticalAlignment.Center;
                familiaCellStyle.Indention = 1;

                HSSFCellStyle tableDataLastCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                tableDataLastCellStyle.CloneStyleFrom(tableDataCellStyle);
                tableDataLastCellStyle.WrapText = true;
                tableDataLastCellStyle.BorderBottom = BorderStyle.Thin;

                HSSFCellStyle tableDataCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                tableDataCenterCellStyle.Alignment = HorizontalAlignment.Center;

                HSSFCellStyle tableDataLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataLastCellStyle);
                tableDataLastCenterCellStyle.Alignment = HorizontalAlignment.Center;

                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");



                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Atenciones");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (list.Count) + 200;
                int cTotal = 14 + 2;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c);
                    }
                }

                int i = 1;

                UtilesHelper.combinarCeldas(sheet, i, i + 1, "A", "A");
                UtilesHelper.combinarCeldas(sheet, i, i + 1, "B", "B");
                UtilesHelper.combinarCeldas(sheet, i, i + 1, "C", "C");
                UtilesHelper.combinarCeldas(sheet, i, i + 1, "D", "D");

                UtilesHelper.combinarCeldas(sheet, i, i, "F", "G");
                UtilesHelper.combinarCeldas(sheet, i, i, "I", "J");
                UtilesHelper.combinarCeldas(sheet, i, i, "L", "M");

                UtilesHelper.setValorCelda(sheet, i, "A", "SKU", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "Prov.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "Cod. Proveedor", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "Descripcion", titleDataCellStyle);

                UtilesHelper.setValorCelda(sheet, i, "F", "UNIDAD MAYOR", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i+1, "F", "Unidad", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i+1, "G", "Stock", titleDataCellStyle);


                UtilesHelper.setValorCelda(sheet, i, "I", "UNIDAD MP", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "I", "Unidad", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "J", "Stock", titleDataCellStyle);

                UtilesHelper.setValorCelda(sheet, i, "L", "UNIDAD MENOR", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "L", "Unidad", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "M", "Stock", titleDataCellStyle);

                


                UtilesHelper.setColumnWidth(sheet, "A", 2700);
                UtilesHelper.setColumnWidth(sheet, "B", 1800);
                UtilesHelper.setColumnWidth(sheet, "C", 4200);
                UtilesHelper.setColumnWidth(sheet, "D", 15000);
                
                UtilesHelper.setColumnWidth(sheet, "E", 500);
                UtilesHelper.setColumnWidth(sheet, "F", 5000);
                UtilesHelper.setColumnWidth(sheet, "G", 2000);

                UtilesHelper.setColumnWidth(sheet, "H", 500);
                UtilesHelper.setColumnWidth(sheet, "I", 5000);
                UtilesHelper.setColumnWidth(sheet, "J", 2000);

                UtilesHelper.setColumnWidth(sheet, "K", 500);
                UtilesHelper.setColumnWidth(sheet, "L", 5000);
                UtilesHelper.setColumnWidth(sheet, "M", 2000);


                i = 3;

                /*  for (int iii = 0; iii<50;iii++)
                  { */
                string familia = "";
                foreach (Producto obj in list)
                {
                    if (!familia.Equals(obj.familia))
                    {
                        i++;
                        familia = obj.familia;
                        if (!familia.Trim().Equals(""))
                        {
                            UtilesHelper.setValorCelda(sheet, i, "A", obj.familia, familiaCellStyle);
                            i++;
                        }
                    }

                    UtilesHelper.setValorCelda(sheet, i, "A", obj.sku, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", obj.proveedor, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "C", obj.skuProveedor, tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "D", obj.descripcion, tableDataCellStyle);

                    if (obj.equivalenciaProveedor > 1)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "F", obj.unidadProveedor, tableDataCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "G", "", tableDataCenterCellStyle);
                    } else
                    {
                        UtilesHelper.combinarCeldas(sheet, i, i, "F", "G");
                        UtilesHelper.setValorCelda(sheet, i, "F", "", blockedDataCellStyle);
                    }

                    UtilesHelper.setValorCelda(sheet, i, "I", obj.unidad_alternativa, tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "J", "", tableDataCenterCellStyle);

                    if (obj.equivalenciaAlternativa > 1)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "L", obj.unidad_alternativa, tableDataCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "M", "", tableDataCenterCellStyle);
                    }
                    else
                    {
                        UtilesHelper.combinarCeldas(sheet, i, i, "L", "M");
                        UtilesHelper.setValorCelda(sheet, i, "L", "", blockedDataCellStyle);
                    }

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

                    result.FileDownloadName = "PlantilalCargaStock_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }

        private void setDataValidationSINO(HSSFSheet sheet, int rowLimit, int column)
        {
            string[] options = { "SI", "NO" };
            var markConstraintA = DVConstraint.CreateExplicitListConstraint(options);
            var markColumnA = new CellRangeAddressList(1, 1 + rowLimit, column, column);
            var markdvA = new HSSFDataValidation(markColumnA, markConstraintA);
            markdvA.EmptyCellAllowed = true;
            markdvA.CreateErrorBox("Valor Incorrecto", "Por favor seleccione un tipo de la lista");
            sheet.AddValidationData(markdvA);
        }
    }
}