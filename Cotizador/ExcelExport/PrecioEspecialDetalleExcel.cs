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
    public class PrecioEspecialDetalleExcel
    {
        protected int cantFilasRegistrosAdicionales = 100;
        public static int filaInicioDatos { get { return 2; } }

        public FileStreamResult generateExcel(PrecioEspecialCabecera obj)
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
                sheet = (HSSFSheet)wb.CreateSheet(obj.codigo);


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (obj.precios.Count) + cantFilasRegistrosAdicionales;
                int cTotal = 24;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c).CellStyle = defaulCellStyle;
                    }
                }

                IWorkbook newWorkbook = wb;


                int i = filaInicioDatos - 1;

                UtilesHelper.setValorCelda(sheet, i, "A", "SKU", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "DESCRIPCIÓN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "MONEDA", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "TIPO UNIDAD PRECIO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "UNIDAD PRECIO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "PRECIO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "PRECIO ORIGINAL", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "TIPO UNIDAD COSTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", "UNIDAD COSTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", "COSTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "K", "COSTO ORIGINAL", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "L", "FECHA INICIO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "M", "FECHA FIN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "N", "OBSERVACIONES", titleDataCellStyle);

                UtilesHelper.setColumnWidth(sheet, "A", 2500);
                UtilesHelper.setColumnWidth(sheet, "B", 8000);
                UtilesHelper.setColumnWidth(sheet, "C", 1800);
                UtilesHelper.setColumnWidth(sheet, "D", 3000);
                UtilesHelper.setColumnWidth(sheet, "E", 3500);
                UtilesHelper.setColumnWidth(sheet, "F", 2800);
                UtilesHelper.setColumnWidth(sheet, "G", 2800);
                UtilesHelper.setColumnWidth(sheet, "H", 3000);
                UtilesHelper.setColumnWidth(sheet, "I", 3500);
                UtilesHelper.setColumnWidth(sheet, "J", 2800);
                UtilesHelper.setColumnWidth(sheet, "K", 2800);
                UtilesHelper.setColumnWidth(sheet, "L", 3000);
                UtilesHelper.setColumnWidth(sheet, "M", 3000);
                UtilesHelper.setColumnWidth(sheet, "N", 6000);

                i = filaInicioDatos;

                HSSFCellStyle tableDataCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                HSSFCellStyle tableDataLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataLastCellStyle);

                HSSFCellStyle twoDecCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecCellStyle);
                HSSFCellStyle twoDecLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecLastCellStyle);

                HSSFCellStyle twoDecIdentCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithIndent(wb, twoDecCellStyle, 1);
                HSSFCellStyle twoDecIdentLastCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithIndent(wb, twoDecLastCellStyle, 1);

                marcarUnidades(sheet, i - 1, obj.precios.Count + 10, 3);
                marcarUnidades(sheet, i - 1, obj.precios.Count + 10, 7);

                marcarMonedas(sheet, i - 1, obj.precios.Count + 10, 2);

                foreach (PrecioEspecialDetalle det in obj.precios)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", det.producto.sku, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", det.producto.descripcion, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "C", det.moneda.codigo, tableDataCenterCellStyle);

                    string unidadPrecioDesc = "";
                    switch (det.unidadPrecio.IdProductoPresentacion)
                    {
                        case 0: unidadPrecioDesc = "MP"; break;
                        case 1: unidadPrecioDesc = "Alternativa"; break;
                        case 2: unidadPrecioDesc = "Proveedor"; break;
                    }

                    UtilesHelper.setValorCelda(sheet, i, "D", unidadPrecioDesc, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "E", det.unidadPrecio.Presentacion, tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "F", (double) det.unidadPrecio.PrecioSinIGV, tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "G", (double) det.unidadPrecio.PrecioOriginalSinIGV, tableDataCellStyle);

                    string unidadCostoDesc = "";
                    switch (det.unidadCosto.IdProductoPresentacion)
                    {
                        case 0: unidadCostoDesc = "MP"; break;
                        case 1: unidadCostoDesc = "Alternativa"; break;
                        case 2: unidadCostoDesc = "Proveedor"; break;
                    }

                    UtilesHelper.setValorCelda(sheet, i, "H", unidadCostoDesc, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "I", det.unidadCosto.Presentacion, tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "J", (double) det.unidadCosto.CostoSinIGV, tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "K", (double) det.unidadCosto.CostoOriginalSinIGV, tableDataCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "L", det.fechaInicio.ToString("yyyy-MM-dd"), tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "M", det.fechaFin.ToString("yyyy-MM-dd"), tableDataCenterCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "N", det.observaciones, tableDataCenterCellStyle);
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

                UtilesHelper.setValorCelda(sheet, i, "M", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "N", "", lastDataCellStyle);


                MemoryStream ms = new MemoryStream();
                using (MemoryStream tempStream = new MemoryStream())
                {
                    wb.Write(tempStream);
                    var byteArray = tempStream.ToArray();
                    ms.Write(byteArray, 0, byteArray.Length);
                    ms.Flush();
                    ms.Position = 0;
                    FileStreamResult result = new FileStreamResult(ms, "application/vnd.ms-excel");

                    result.FileDownloadName = "ListaPreciosEspeciales_" + obj.codigo + " .xls";

                    return result;
                }
            }
        }

        protected void marcarUnidades(HSSFSheet sheet, int rowInit, int rows, int col)
        {
            var markConstraint = DVConstraint.CreateExplicitListConstraint(new string[] { "MP", "Alternativa", "Proveedor" });
            var markColumn = new CellRangeAddressList(rowInit, rowInit + rows, col, col);
            var markdv = new HSSFDataValidation(markColumn, markConstraint);
            markdv.EmptyCellAllowed = true;
            markdv.CreateErrorBox("Valor Incorrecto", "Por favor seleccione un tipo de unidad de la lista");
            sheet.AddValidationData(markdv);
        }

        protected void marcarMonedas(HSSFSheet sheet, int rowInit, int rows, int col)
        {
            var markConstraint = DVConstraint.CreateExplicitListConstraint(new string[] { "PEN", "USD" });
            var markColumn = new CellRangeAddressList(rowInit, rowInit + rows, col, col);
            var markdv = new HSSFDataValidation(markColumn, markConstraint);
            markdv.EmptyCellAllowed = true;
            markdv.CreateErrorBox("Valor Incorrecto", "Por favor seleccione una moneda de la lista");
            sheet.AddValidationData(markdv);
        }
    }
}