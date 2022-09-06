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

    public class ReporteStockPendienteAtencion
    {
        public FileStreamResult generateExcel(List<List<String>> lista, Ciudad ciudad, Producto producto, DateTime fechaInicio, DateTime fechaFin, int idProductoPresentacion)
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


                var avgCellFormate = wb.CreateDataFormat();
                var twoDecFormat = avgCellFormate.GetFormat("0.00");
                HSSFCellStyle twoDecCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                twoDecCellStyle.DataFormat = twoDecFormat;
                twoDecCellStyle.VerticalAlignment = VerticalAlignment.Center;
                twoDecCellStyle.BorderLeft = BorderStyle.Thin;
                twoDecCellStyle.BorderRight = BorderStyle.Thin;
                twoDecCellStyle.BorderTop = BorderStyle.Thin;
                twoDecCellStyle.BorderBottom = BorderStyle.Thin;
                twoDecCellStyle.Indention = 1;

                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("KARDEX");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (lista.Count) + 20;
                int cTotal = 16 + 2;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c);
                    }
                }

                string unidad = "";

                switch(idProductoPresentacion)
                {
                    case 0: unidad = producto.unidad; break;
                    case 1: unidad = producto.unidad_alternativa; break;
                    case 2: unidad = producto.unidadProveedor; break;
                    case 3: unidad = producto.unidadConteo; break;
                }

                int i = 1;
                UtilesHelper.setValorCelda(sheet, i, "A", "SEDE", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", ciudad.nombre.ToUpper());
                UtilesHelper.setValorCelda(sheet, i, "C", "PRODUCTO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", producto.descripcion);

                i++; i++;
                UtilesHelper.setValorCelda(sheet, i, "A", "SKU", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", producto.sku);
                UtilesHelper.setValorCelda(sheet, i, "C", "UNIDAD", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", unidad);

                i++; i++;
                UtilesHelper.setValorCelda(sheet, i, "A", "FECHA ENTREGA", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", fechaInicio.ToString("dd/MM/yyyy") + " - " + fechaFin.ToString("dd/MM/yyyy"));
                

                i++; i++;


                UtilesHelper.setValorCelda(sheet, i, "A", "SEDE", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "NUMERO PEDIDO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "TIPO PEDIDO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "SUB TIPO PEDIDO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "CODIGO CLIENTE", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "NOMBRE CLIENTE", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "GRUPO CLIENTE", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "FECHA PEDIDO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", "FECHA ENTREGA", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", "CANT. PEDIDO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "K", "CANT. ATENTIDA", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "L", "CANT. PENDIENTE", titleDataCellStyle);


                UtilesHelper.setColumnWidth(sheet, "A", 4000);
                UtilesHelper.setColumnWidth(sheet, "B", 3000);
                UtilesHelper.setColumnWidth(sheet, "C", 4000);
                UtilesHelper.setColumnWidth(sheet, "D", 4000);
                UtilesHelper.setColumnWidth(sheet, "E", 4000);
                UtilesHelper.setColumnWidth(sheet, "F", 12000);

                UtilesHelper.setColumnWidth(sheet, "G", 6000);
                UtilesHelper.setColumnWidth(sheet, "H", 4000);
                UtilesHelper.setColumnWidth(sheet, "I", 6000);
                UtilesHelper.setColumnWidth(sheet, "J", 3000);
                UtilesHelper.setColumnWidth(sheet, "K", 3000);
                UtilesHelper.setColumnWidth(sheet, "L", 3000);


                i++; 


                foreach (List<String> item in lista)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", item.ElementAt(0), tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", item.ElementAt(1), tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "C", item.ElementAt(2), tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "D", item.ElementAt(3), tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "E", item.ElementAt(4), tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "F", item.ElementAt(5), tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "G", item.ElementAt(6), tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "H", item.ElementAt(7), tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "I", item.ElementAt(8) + " - " + item.ElementAt(9), tableDataCellStyle);

                    decimal cantidadPedido = decimal.Parse(item.ElementAt(10));
                    decimal cantidadAtendida = decimal.Parse(item.ElementAt(11));
                    decimal cantidadPendiente = decimal.Parse(item.ElementAt(12));

                    switch (idProductoPresentacion)
                    {
                        case 0: 
                            cantidadPedido = cantidadPedido / producto.equivalenciaUnidadEstandarUnidadConteo;
                            cantidadAtendida = cantidadAtendida / producto.equivalenciaUnidadEstandarUnidadConteo;
                            cantidadPendiente = cantidadPendiente / producto.equivalenciaUnidadEstandarUnidadConteo;
                            break;
                        case 1:
                            cantidadPedido = cantidadPedido / (producto.equivalenciaUnidadEstandarUnidadConteo / producto.equivalenciaAlternativa);
                            cantidadAtendida = cantidadAtendida / (producto.equivalenciaUnidadEstandarUnidadConteo / producto.equivalenciaAlternativa);
                            cantidadPendiente = cantidadPendiente / (producto.equivalenciaUnidadEstandarUnidadConteo / producto.equivalenciaAlternativa);
                            break;
                        case 2:
                            cantidadPedido = cantidadPedido / (producto.equivalenciaUnidadEstandarUnidadConteo * producto.equivalenciaProveedor);
                            cantidadAtendida = cantidadAtendida / (producto.equivalenciaUnidadEstandarUnidadConteo * producto.equivalenciaProveedor);
                            cantidadPendiente = cantidadPendiente / (producto.equivalenciaUnidadEstandarUnidadConteo * producto.equivalenciaProveedor);
                            break;
                    }

                    UtilesHelper.setValorCelda(sheet, i, "J", (double) cantidadPedido, twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "K", (double)cantidadAtendida, twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "L", (double)cantidadPendiente, twoDecCellStyle);
                    
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

                    result.FileDownloadName = "PedidosPendienteAtencionProducto_" + producto.sku + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }
    }
}