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
using Model.ServiceSunatPadron;
using NPOI.SS.Formula.Functions;


namespace Cotizador.ExcelExport
{
    public class PrecioEspecialCabecerasDetallesExcel
    {
        protected int cantFilasRegistrosAdicionales = 20;
        public static int filaInicioDatos { get { return 2; } }

        public FileStreamResult generateExcel(List<PrecioEspecialCabecera> lista, string tipo)
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
                sheet = (HSSFSheet)wb.CreateSheet("Lista");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                int totalRegistros = 0;
                foreach (PrecioEspecialCabecera obj in lista)
                {
                    totalRegistros = totalRegistros + obj.precios.Count;
                }

                /*Cabecera, Sub total*/
                int rTotal = totalRegistros + cantFilasRegistrosAdicionales;
                int cTotal = 32;

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

                UtilesHelper.setValorCelda(sheet, i, "A", "TIPO NEGOCIACIÓN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "CÓDIGO NEGOCIACIÓN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "TÍTULO LISTA", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "RUC", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "RAZÓN SOCIAL", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "CÓDIGO GRUPO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "GRUPO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "FECHA INICIO LISTA", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", "FECHA FIN LISTA", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", "CÓDIGO REGISTRO PROVEEDOR", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "K", "OBSERVACIONES LISTA", titleDataCellStyle);

                UtilesHelper.setValorCelda(sheet, i, "L", "SKU", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "M", "PRODUCTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "N", "MONEDA", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "O", "TIPO UNIDAD PRECIO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "P", "UNIDAD PRECIO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "Q", "PRECIO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "R", "PRECIO UND MP", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "S", "TIPO UNIDAD COSTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "T", "UNIDAD COSTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "U", "COSTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "V", "COSTO UND MP", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "W", "FECHA INICIO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "X", "FECHA FIN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "Y", "OBSERVACIONES", titleDataCellStyle);

                UtilesHelper.setColumnWidth(sheet, "A", 4500);
                UtilesHelper.setColumnWidth(sheet, "B", 4500);
                UtilesHelper.setColumnWidth(sheet, "C", 6000);
                UtilesHelper.setColumnWidth(sheet, "D", 4000);
                UtilesHelper.setColumnWidth(sheet, "E", 6000);
                UtilesHelper.setColumnWidth(sheet, "F", 3000);
                UtilesHelper.setColumnWidth(sheet, "G", 6000);
                UtilesHelper.setColumnWidth(sheet, "H", 4500);
                UtilesHelper.setColumnWidth(sheet, "I", 4500);
                UtilesHelper.setColumnWidth(sheet, "J", 5000);
                UtilesHelper.setColumnWidth(sheet, "K", 6000);

                UtilesHelper.setColumnWidth(sheet, "L", 2500);
                UtilesHelper.setColumnWidth(sheet, "M", 8000);
                UtilesHelper.setColumnWidth(sheet, "N", 1800);
                UtilesHelper.setColumnWidth(sheet, "O", 3000);
                UtilesHelper.setColumnWidth(sheet, "P", 3500);
                UtilesHelper.setColumnWidth(sheet, "Q", 2800);
                UtilesHelper.setColumnWidth(sheet, "R", 2800);
                UtilesHelper.setColumnWidth(sheet, "S", 3000);
                UtilesHelper.setColumnWidth(sheet, "T", 3500);
                UtilesHelper.setColumnWidth(sheet, "U", 2800);
                UtilesHelper.setColumnWidth(sheet, "V", 2800);
                UtilesHelper.setColumnWidth(sheet, "W", 3000);
                UtilesHelper.setColumnWidth(sheet, "X", 3000);
                UtilesHelper.setColumnWidth(sheet, "Y", 6000);

                i = filaInicioDatos;

                HSSFCellStyle tableDataDateCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                tableDataDateCellStyle.DataFormat = wb.CreateDataFormat().GetFormat("dd/MM/yyyy");

                HSSFCellStyle tableDataCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                HSSFCellStyle tableDataLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataLastCellStyle);

                HSSFCellStyle blockedTDCenterCS = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCenterCellStyle);
                blockedTDCenterCS.FillPattern = FillPattern.SolidForeground;
                blockedTDCenterCS.FillForegroundColor = HSSFColor.Grey25Percent.Index;

                HSSFCellStyle blockedTDCS = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                blockedTDCS.FillPattern = FillPattern.SolidForeground;
                blockedTDCS.FillForegroundColor = HSSFColor.Grey25Percent.Index;

                HSSFCellStyle blockedTDDateCS = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataDateCellStyle);
                blockedTDDateCS.FillPattern = FillPattern.SolidForeground;
                blockedTDDateCS.FillForegroundColor = HSSFColor.Grey25Percent.Index;

                HSSFCellStyle twoDecCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecCellStyle);
                HSSFCellStyle twoDecLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecLastCellStyle);

                HSSFCellStyle twoDecIdentCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithIndent(wb, twoDecCellStyle, 1);
                HSSFCellStyle twoDecIdentLastCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithIndent(wb, twoDecLastCellStyle, 1);

                foreach (PrecioEspecialCabecera obj in lista)
                {

                    for (int r = i - 1; r < (obj.precios.Count + i); r++)
                    {
                        var row = sheet.CreateRow(r);
                        for (int c = 0; c < cTotal; c++)
                        {
                            row.CreateCell(c).CellStyle = defaulCellStyle;
                        }
                    }

                    marcarUnidades(sheet, i - 1, obj.precios.Count, 14);
                    marcarUnidades(sheet, i - 1, obj.precios.Count, 18);

                    marcarMonedas(sheet, i - 1, obj.precios.Count, 13);

                    foreach (PrecioEspecialDetalle det in obj.precios)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "A", obj.tipoNegociacion, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "B", obj.codigo, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "C", obj.titulo, tableDataCenterCellStyle);

                        UtilesHelper.setValorCelda(sheet, i, "D", obj.clienteSunat.ruc, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "E", obj.clienteSunat.razonSocial, blockedTDCenterCS);

                        if (obj.tipoNegociacion.Equals("GRUPO") && obj.grupoCliente != null)
                        {
                            UtilesHelper.setValorCelda(sheet, i, "F", obj.grupoCliente.codigo, tableDataCenterCellStyle);
                            UtilesHelper.setValorCelda(sheet, i, "G", obj.grupoCliente.nombre, blockedTDCenterCS);
                        } else
                        {
                            UtilesHelper.setValorCelda(sheet, i, "F", "", tableDataCenterCellStyle);
                            UtilesHelper.setValorCelda(sheet, i, "G", "", blockedTDCenterCS);
                        }

                        UtilesHelper.setValorCelda(sheet, i, "H", obj.fechaInicio, blockedTDDateCS);
                        UtilesHelper.setValorCelda(sheet, i, "I", obj.fechaFin, blockedTDDateCS);
                        UtilesHelper.setValorCelda(sheet, i, "J", obj.codigoListaProveedor, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "K", obj.observaciones, tableDataCenterCellStyle);


                        UtilesHelper.setValorCelda(sheet, i, "L", det.producto.sku, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "M", det.producto.descripcion, blockedTDCenterCS);
                        UtilesHelper.setValorCelda(sheet, i, "N", det.moneda.codigo, tableDataCenterCellStyle);

                        string unidadPrecioDesc = "";
                        switch (det.unidadPrecio.IdProductoPresentacion)
                        {
                            case 0: unidadPrecioDesc = "MP"; break;
                            case 1: unidadPrecioDesc = "Alternativa"; break;
                            case 2: unidadPrecioDesc = "Proveedor"; break;
                        }

                        UtilesHelper.setValorCelda(sheet, i, "O", unidadPrecioDesc, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "P", det.unidadPrecio.Presentacion, blockedTDCS);
                        UtilesHelper.setValorCelda(sheet, i, "Q", (double)det.unidadPrecio.PrecioSinIGV, tableDataCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "R", (double)det.unidadPrecio.PrecioOriginalSinIGV, blockedTDCS);

                        string unidadCostoDesc = "";
                        switch (det.unidadCosto.IdProductoPresentacion)
                        {
                            case 0: unidadCostoDesc = "MP"; break;
                            case 1: unidadCostoDesc = "Alternativa"; break;
                            case 2: unidadCostoDesc = "Proveedor"; break;
                        }

                        UtilesHelper.setValorCelda(sheet, i, "S", unidadCostoDesc, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "T", det.unidadCosto.Presentacion, blockedTDCS);
                        UtilesHelper.setValorCelda(sheet, i, "U", (double)det.unidadCosto.CostoSinIGV, tableDataCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "V", (double)det.unidadCosto.CostoOriginalSinIGV, blockedTDCS);

                        UtilesHelper.setValorCelda(sheet, i, "W", det.fechaInicio, tableDataDateCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "X", det.fechaFin, tableDataDateCellStyle);

                        UtilesHelper.setValorCelda(sheet, i, "Y", det.observaciones, tableDataCenterCellStyle);
                        i++;
                    }
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
                UtilesHelper.setValorCelda(sheet, i, "O", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "P", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "Q", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "R", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "S", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "T", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "U", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "V", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "W", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "X", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "Y", "", lastDataCellStyle);

                MemoryStream ms = new MemoryStream();
                using (MemoryStream tempStream = new MemoryStream())
                {
                    wb.Write(tempStream);
                    var byteArray = tempStream.ToArray();
                    ms.Write(byteArray, 0, byteArray.Length);
                    ms.Flush();
                    ms.Position = 0;
                    FileStreamResult result = new FileStreamResult(ms, "application/vnd.ms-excel");

                    result.FileDownloadName = "ListaPreciosEspeciales" + tipo + DateTime.Now.ToString("yyyyMMdd")+ " .xls";

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