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
using Model.UTILES;

using System.Web.Mvc;

namespace Cotizador.ExcelExport
{

    public class ReporteKardexProducto
    {
        public FileStreamResult generateExcel(MovimientoKardexCabecera kardex, DateTime? fechaInicio)
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
                titleDataCellStyle.WrapText = true;
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
                sheet = (HSSFSheet)wb.CreateSheet("KARDEX");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (kardex.movimientos.Count) + 20;
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
                UtilesHelper.setValorCelda(sheet, i, "A", "SEDE", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", kardex.ciudad.nombre.ToUpper());
                UtilesHelper.setValorCelda(sheet, i, "C", "PRODUCTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", kardex.producto.descripcion);

                i++;
                UtilesHelper.setValorCelda(sheet, i, "A", "SKU", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", kardex.producto.sku);
                UtilesHelper.setValorCelda(sheet, i, "C", "UNIDAD", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", kardex.unidad);

                if (fechaInicio.HasValue)
                {
                    i++;
                    UtilesHelper.setValorCelda(sheet, i, "A", "FECHA INICIO MOVIMIENTOS", titleDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", fechaInicio.Value.ToString("dd/MM/yyyy"));
                }

                i++; i++;
                UtilesHelper.combinarCeldas(sheet, i, i + 1, "A", "A");
                UtilesHelper.combinarCeldas(sheet, i, i + 1, "B", "B");
                UtilesHelper.combinarCeldas(sheet, i, i + 1, "C", "C");
                UtilesHelper.combinarCeldas(sheet, i, i + 1, "D", "D");
                UtilesHelper.combinarCeldas(sheet, i, i + 1, "E", "E");

                UtilesHelper.combinarCeldas(sheet, i, i, "F", "G");
                UtilesHelper.combinarCeldas(sheet, i, i, "H", "I");

                UtilesHelper.setValorCelda(sheet, i, "A", "TIPO MOVIMIENTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "FECHA EMISIÓN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "NRO. MOVIMIENTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "DESTINATARIO/REMITENTE", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "UNIDAD MOVIMIENTO", titleDataCellStyle);

                UtilesHelper.setValorCelda(sheet, i, "F", "CANTIDAD MOVIMIENTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "F", "UNIDAD MOVIMIENTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "G", kardex.unidadConteo, titleDataCellStyle);

                UtilesHelper.setValorCelda(sheet, i, "H", "STOCK RESULTANTE", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "H", kardex.unidad, titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i + 1, "I", kardex.unidadConteo, titleDataCellStyle);

                //       $("#verNombreSedeKardexProductoStock").html(kardex.ciudad.nombre);
                //$("#verNombreProductoStockKardex").html(kardex.producto.descripcion);
                //$("#verCodigoProductoStockKardex").html(kardex.producto.sku);
                //$(".verUnidadMostrarStockKardex").html(kardex.unidad);
                //$("#verUnidadConteoStockKardex").html(kardex.unidadConteo);
                //$("#verUnidadConteoMovimientoKardex").html(kardex.unidadConteo);

                UtilesHelper.setColumnWidth(sheet, "A", 5000);
                UtilesHelper.setColumnWidth(sheet, "B", 4000);
                UtilesHelper.setColumnWidth(sheet, "C", 5500);
                UtilesHelper.setColumnWidth(sheet, "D", 15000);
                UtilesHelper.setColumnWidth(sheet, "E", 9000);

                UtilesHelper.setColumnWidth(sheet, "F", 4500);
                UtilesHelper.setColumnWidth(sheet, "G", 4500);
                UtilesHelper.setColumnWidth(sheet, "H", 4500);
                UtilesHelper.setColumnWidth(sheet, "I", 4500);


                i++; i++;

                /*  for (int iii = 0; iii<50;iii++)
                  { */
                //string familia = "";
                bool bStyle = false;
                foreach (MovimientoKardexDetalle obj in kardex.movimientos)
                {
                    //if (bStyle)
                    //{
                    //    UtilesHelper.setValorCelda(sheet, i, "A", obj.familia, tableDataCenterCellStyleB);
                    //    UtilesHelper.setValorCelda(sheet, i, "B", obj.sku, tableDataCenterCellStyleB);

                    //}
                    //else
                    //{
                    UtilesHelper.setValorCelda(sheet, i, "A", obj.TipoMovimientoDesc, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", obj.FechaDesc, tableDataCenterCellStyle);
                    if (obj.tipoMovimiento == 1 || obj.tipoMovimiento == 2)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "C", obj.NumeroDocumentoDesc, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "D", obj.razonSocialCliente + " " + obj.TipoDocumentoClienteDesc + " " + obj.nroDocumentoCliente, tableDataCenterCellStyle);
                    } else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "C", "", tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "D", "", tableDataCenterCellStyle);
                    }

                    UtilesHelper.setValorCelda(sheet, i, "E", obj.unidadMovimiento, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "F", obj.cantidadMovimiento, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "G", obj.cantidadConteo, tableDataCenterCellStyle);

                    if (!fechaInicio.HasValue)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "H", (double)obj.stockUnidad, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "I", obj.stockConteo, tableDataCenterCellStyle);
                    } else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "H", "", tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "I", "", tableDataCenterCellStyle);
                    }


                    //}



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

                    result.FileDownloadName = "KardexProductoSede_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }
    }
}