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
        public FileStreamResult generateExcel(List<Producto> list, Usuario usuario, String nombreSede)
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
                formLabelFont.Color = IndexedColors.White.Index;
                formLabelFont.IsBold = true;
                //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
                //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
                HSSFCellStyle formLabelCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                formLabelCellStyle.SetFont(formLabelFont);
                formLabelCellStyle.Alignment = HorizontalAlignment.Right;
                formLabelCellStyle.FillPattern = FillPattern.SolidForeground;
                formLabelCellStyle.FillForegroundColor = HSSFColor.RoyalBlue.Index;

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

                HSSFCellStyle tableDataCellStyleB = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                tableDataCellStyleB.Alignment = HorizontalAlignment.Left;
                tableDataCellStyleB.FillPattern = FillPattern.SolidForeground;
                tableDataCellStyleB.FillForegroundColor = HSSFColor.Grey25Percent.Index;

                HSSFCellStyle tableDataCenterCellStyleB = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCenterCellStyle);
                tableDataCenterCellStyleB.FillPattern = FillPattern.SolidForeground;
                tableDataCenterCellStyleB.FillForegroundColor = HSSFColor.Grey25Percent.Index;

                HSSFCellStyle tableDataLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataLastCellStyle);
                tableDataLastCenterCellStyle.Alignment = HorizontalAlignment.Center;

                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");



                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Productos");


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


                if (!nombreSede.Equals(""))
                {
                    UtilesHelper.combinarCeldas(sheet, i, i, "B", "C");
                    UtilesHelper.setValorCelda(sheet, i, "B", "Sede:", formLabelCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "D", nombreSede.ToUpper());

                    i = i + 2;
                }


                UtilesHelper.combinarCeldas(sheet, i, i + 1, "A", "A");
                UtilesHelper.combinarCeldas(sheet, i, i + 1, "B", "B");
                UtilesHelper.combinarCeldas(sheet, i, i + 1, "C", "C");
                UtilesHelper.combinarCeldas(sheet, i, i + 1, "D", "D");
                UtilesHelper.combinarCeldas(sheet, i, i + 1, "E", "E");

                UtilesHelper.combinarCeldas(sheet, i, i, "G", "H");
                UtilesHelper.combinarCeldas(sheet, i, i, "J", "K");
                UtilesHelper.combinarCeldas(sheet, i, i, "M", "N");

                UtilesHelper.setValorCelda(sheet, i, "A", "FAMILIA", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "SKU", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "Prov.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "Cod. Proveedor", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "Descripcion", titleDataCellStyle);

                UtilesHelper.setValorCelda(sheet, i, "G", "UNIDAD MAYOR", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i+1, "G", "Unidad", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i+1, "H", "Stock", titleDataCellStyle);


                UtilesHelper.setValorCelda(sheet, i, "J", "UNIDAD MP", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "J", "Unidad", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "K", "Stock", titleDataCellStyle);

                UtilesHelper.setValorCelda(sheet, i, "M", "UNIDAD MENOR", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "M", "Unidad", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "N", "Stock", titleDataCellStyle);




                UtilesHelper.setColumnWidth(sheet, "A", 10000);
                UtilesHelper.setColumnWidth(sheet, "B", 2700);
                UtilesHelper.setColumnWidth(sheet, "C", 1800);
                UtilesHelper.setColumnWidth(sheet, "D", 4200);
                UtilesHelper.setColumnWidth(sheet, "E", 15000);
                
                UtilesHelper.setColumnWidth(sheet, "F", 500);
                UtilesHelper.setColumnWidth(sheet, "G", 6000);
                UtilesHelper.setColumnWidth(sheet, "H", 2000);

                UtilesHelper.setColumnWidth(sheet, "I", 500);
                UtilesHelper.setColumnWidth(sheet, "J", 6000);
                UtilesHelper.setColumnWidth(sheet, "K", 2000);

                UtilesHelper.setColumnWidth(sheet, "L", 500);
                UtilesHelper.setColumnWidth(sheet, "M", 6000);
                UtilesHelper.setColumnWidth(sheet, "N", 2000);


                i = i + 2;

                /*  for (int iii = 0; iii<50;iii++)
                  { */
                //string familia = "";
                bool bStyle = false;
                foreach (Producto obj in list)
                {
                    //if (!familia.Equals(obj.familia))
                    //{
                    //    i++;
                    //    familia = obj.familia;
                    //    bStyle = false;
                    //    if (!familia.Trim().Equals(""))
                    //    {
                    //        UtilesHelper.setValorCelda(sheet, i, "A", obj.familia, familiaCellStyle);
                    //        i++;
                    //    }
                    //}

                    if (bStyle)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "A", obj.familia, tableDataCenterCellStyleB);
                        UtilesHelper.setValorCelda(sheet, i, "B", obj.sku, tableDataCenterCellStyleB);
                        UtilesHelper.setValorCelda(sheet, i, "C", obj.proveedor, tableDataCenterCellStyleB);
                        UtilesHelper.setValorCelda(sheet, i, "D", obj.skuProveedor, tableDataCellStyleB);
                        UtilesHelper.setValorCelda(sheet, i, "E", obj.descripcion, tableDataCellStyleB);

                        if (obj.equivalenciaProveedor > 1)
                        {
                            UtilesHelper.setValorCelda(sheet, i, "G", obj.unidadProveedor, tableDataCellStyleB);
                            UtilesHelper.setValorCelda(sheet, i, "H", "", tableDataCenterCellStyleB);
                        }

                        UtilesHelper.setValorCelda(sheet, i, "J", obj.unidad, tableDataCellStyleB);
                        UtilesHelper.setValorCelda(sheet, i, "K", "", tableDataCenterCellStyleB);

                        if (obj.equivalenciaAlternativa > 1)
                        {
                            UtilesHelper.setValorCelda(sheet, i, "M", obj.unidad_alternativa, tableDataCellStyleB);
                            UtilesHelper.setValorCelda(sheet, i, "N", "", tableDataCenterCellStyleB);
                        }

                        if (obj.equivalenciaProveedor <= 1)
                        {
                            UtilesHelper.combinarCeldas(sheet, i, i, "G", "H");
                            UtilesHelper.setValorCelda(sheet, i, "G", "", tableDataCenterCellStyleB);
                            UtilesHelper.setValorCelda(sheet, i, "H", "", tableDataCenterCellStyleB);
                        }


                        if (obj.equivalenciaAlternativa <= 1)
                        {
                            UtilesHelper.combinarCeldas(sheet, i, i, "M", "N");
                            UtilesHelper.setValorCelda(sheet, i, "M", "", tableDataCenterCellStyleB);
                            UtilesHelper.setValorCelda(sheet, i, "N", "", tableDataCenterCellStyleB);
                        }

                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "A", obj.familia, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "B", obj.sku, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "C", obj.proveedor, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "D", obj.skuProveedor, tableDataCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "E", obj.descripcion, tableDataCellStyle);

                        if (obj.equivalenciaProveedor > 1)
                        {
                            UtilesHelper.setValorCelda(sheet, i, "G", obj.unidadProveedor, tableDataCellStyle);
                            UtilesHelper.setValorCelda(sheet, i, "H", "", tableDataCenterCellStyle);
                        }

                        UtilesHelper.setValorCelda(sheet, i, "J", obj.unidad, tableDataCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "K", "", tableDataCenterCellStyle);

                        if (obj.equivalenciaAlternativa > 1)
                        {
                            UtilesHelper.setValorCelda(sheet, i, "M", obj.unidad_alternativa, tableDataCellStyle);
                            UtilesHelper.setValorCelda(sheet, i, "N", "", tableDataCenterCellStyle);
                        }

                        if (obj.equivalenciaProveedor <= 1)
                        {
                            UtilesHelper.combinarCeldas(sheet, i, i, "G", "H");
                            UtilesHelper.setValorCelda(sheet, i, "G", "", tableDataCenterCellStyle);
                            UtilesHelper.setValorCelda(sheet, i, "H", "", tableDataCenterCellStyle);
                        }


                        if (obj.equivalenciaAlternativa <= 1)
                        {
                            UtilesHelper.combinarCeldas(sheet, i, i, "M", "N");
                            UtilesHelper.setValorCelda(sheet, i, "M", "", tableDataCenterCellStyle);
                            UtilesHelper.setValorCelda(sheet, i, "N", "", tableDataCenterCellStyle);
                        }

                    }



                    if (bStyle) { bStyle = false; } else { bStyle = true; }

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

                    result.FileDownloadName = "PlantillaCargaStock" + nombreSede + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

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