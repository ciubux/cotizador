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
using Model.UTILES;
using Cotizador.Models.OBJsFiltro;
using Newtonsoft.Json;
using NPOI.HSSF.Model;
using NPOI.HSSF.Util;
using BusinessLayer;

using System.Web.Mvc;

namespace Cotizador.ExcelExport
{

    public class ReporteDetallesSellOutVendedor
    {
        public FileStreamResult generateExcel(List<List<String>> list, ReporteSellOutVendedoresFiltro filtro, Usuario usuario)
        {

            HSSFWorkbook wb;
            //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);
            HSSFSheet sheet;
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());


                HSSFFont titleFont = (HSSFFont)wb.CreateFont();
                HSSFCellStyle titleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                HSSFCellStyle titleDataCellStyle;

                titleFont = (HSSFFont)wb.CreateFont();
                titleFont.FontHeightInPoints = (short)11;
                titleFont.FontName = "Arial";
                titleFont.Color = IndexedColors.White.Index;
                titleFont.IsBold = true;

                titleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleCellStyle.SetFont(titleFont);
                titleCellStyle.Alignment = HorizontalAlignment.Center;
                titleCellStyle.FillPattern = FillPattern.SolidForeground;
                titleCellStyle.FillForegroundColor = HSSFColor.RoyalBlue.Index;

                titleDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleDataCellStyle.CloneStyleFrom(titleCellStyle);
                titleDataCellStyle.VerticalAlignment = VerticalAlignment.Center;
                titleDataCellStyle.BorderLeft = BorderStyle.Thin;
                titleDataCellStyle.BorderTop = BorderStyle.Thin;
                titleDataCellStyle.BorderRight = BorderStyle.Thin;
                titleDataCellStyle.BorderBottom = BorderStyle.Thin;

                var avgCellFormate = wb.CreateDataFormat();
                var twoDecFormat = avgCellFormate.GetFormat("0.00");
                HSSFCellStyle twoDecCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                twoDecCellStyle.DataFormat = twoDecFormat;
                twoDecCellStyle.VerticalAlignment = VerticalAlignment.Center;
                twoDecCellStyle.BorderLeft = BorderStyle.Thin;
                twoDecCellStyle.BorderRight = BorderStyle.Thin;
                twoDecCellStyle.Indention = 1;

                var fourDecFormat = avgCellFormate.GetFormat("0.0000");
                HSSFCellStyle fourDecCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                fourDecCellStyle.DataFormat = fourDecFormat;
                fourDecCellStyle.VerticalAlignment = VerticalAlignment.Center;
                fourDecCellStyle.BorderLeft = BorderStyle.Thin;
                fourDecCellStyle.BorderRight = BorderStyle.Thin;
                fourDecCellStyle.Indention = 1;

                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");



                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Reporte");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (list.Count) + 12;
                int cTotal = 50 + 2;

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

                /*
                UtilesHelper.setColumnWidth(sheet, "A", 3000);
                UtilesHelper.setColumnWidth(sheet, "B", 2700);
                UtilesHelper.setColumnWidth(sheet, "C", 15000);
                UtilesHelper.setColumnWidth(sheet, "D", 4200);
                UtilesHelper.setColumnWidth(sheet, "E", 1800);
                UtilesHelper.setColumnWidth(sheet, "F", 6500);
                UtilesHelper.setColumnWidth(sheet, "G", 3000);
                */

                UtilesHelper.setValorCelda(sheet, i, "A", "SEDE:", titleCellStyle);
                if (filtro.ciudad == null)
                {
                    UtilesHelper.setValorCelda(sheet, i, "B", "TODAS");
                }
                else
                {
                    UtilesHelper.setValorCelda(sheet, i, "B", filtro.ciudad.nombre);
                }

                UtilesHelper.setValorCelda(sheet, i, "E", "PROVEEDOR:", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", filtro.proveedor);

                UtilesHelper.setValorCelda(sheet, i, "H", "FAMILIA:", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", filtro.familia);
                i = i + 2;

                UtilesHelper.combinarCeldas(sheet, i, i, "B", "C");
                UtilesHelper.setValorCelda(sheet, i, "A", "RANGO FECHA:", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", filtro.fechaInicio.ToString("dd/MM/yyyy") + " - " + filtro.fechaFin.ToString("dd/MM/yyyy"));

                UtilesHelper.setValorCelda(sheet, i, "E", "AÑO:", titleCellStyle);
                switch (filtro.anio)
                {
                    case 0: UtilesHelper.setValorCelda(sheet, i, "F", "Todos"); break;
                    default: UtilesHelper.setValorCelda(sheet, i, "F", filtro.anio); break;
                }

                UtilesHelper.setValorCelda(sheet, i, "H", "TRIMESTRE:", titleCellStyle);
                switch (filtro.trimestre)
                {
                    case 0: UtilesHelper.setValorCelda(sheet, i, "I", "Todos"); break;
                    case 1: UtilesHelper.setValorCelda(sheet, i, "I", "Ene - Mar"); break;
                    case 2: UtilesHelper.setValorCelda(sheet, i, "I", "Abr - Jun"); break;
                    case 3: UtilesHelper.setValorCelda(sheet, i, "I", "Jul - Sep"); break;
                    case 4: UtilesHelper.setValorCelda(sheet, i, "I", "Oct - Dic"); break;
                }

                i = i + 2;

                UtilesHelper.setValorCelda(sheet, i, "A", "SKU:", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", filtro.sku.Equals("") ? "Todos" : filtro.sku);

                i = i + 2;

                UtilesHelper.setValorCelda(sheet, i, "A", "id_venta_detalle", titleCellStyle); /* 0 */
                UtilesHelper.setValorCelda(sheet, i, "B", "CÓDIGO RESPONSABLE COMERCIAL", titleCellStyle); /* 1 */
                UtilesHelper.setValorCelda(sheet, i, "C", "RESPONSABLE COMERCIAL", titleCellStyle); /* 2 */
                UtilesHelper.setValorCelda(sheet, i, "D", "CIUDAD", titleCellStyle); /* 3 */
                UtilesHelper.setValorCelda(sheet, i, "E", "CÓDIGO GRUPO", titleCellStyle); /* 4 */
                UtilesHelper.setValorCelda(sheet, i, "F", "GRUPO", titleCellStyle); /* 5 */
                UtilesHelper.setValorCelda(sheet, i, "G", "N° DOC CLIENTE", titleCellStyle); /* 6 */
                UtilesHelper.setValorCelda(sheet, i, "H", "CLIENTE", titleCellStyle); /* 7 */
                UtilesHelper.setValorCelda(sheet, i, "I", "CÓDIGO CLIENTE", titleCellStyle); /* 8 */
                UtilesHelper.setValorCelda(sheet, i, "J", "TIPO MOVIMIENTO", titleCellStyle); /* 9 */
                UtilesHelper.setValorCelda(sheet, i, "K", "FECHA TRANSACCIÓN", titleCellStyle); /* 10 */
                UtilesHelper.setValorCelda(sheet, i, "L", "N° PEDIDO", titleCellStyle); /* 11 */
                UtilesHelper.setValorCelda(sheet, i, "M", "N° GRUPO PEDIDO", titleCellStyle); /* 12 */
                UtilesHelper.setValorCelda(sheet, i, "N", "GUÍA", titleCellStyle); /* 13 */
                UtilesHelper.setValorCelda(sheet, i, "O", "FECHA EMISIÓN GUÍA", titleCellStyle); /* 14 */
                UtilesHelper.setValorCelda(sheet, i, "P", "FAMILIA PROD", titleCellStyle); /* 15 */
                UtilesHelper.setValorCelda(sheet, i, "Q", "PROVEEDOR", titleCellStyle); /* 16 */
                UtilesHelper.setValorCelda(sheet, i, "R", "SKU MP", titleCellStyle); /* 17 */
                UtilesHelper.setValorCelda(sheet, i, "S", "SKU PROV", titleCellStyle); /* 18 */
                UtilesHelper.setValorCelda(sheet, i, "T", "DESCRIPCIÓN PROD", titleCellStyle); /* 19 */
                UtilesHelper.setValorCelda(sheet, i, "U", "UNIDAD VENTA", titleCellStyle); /* 20 */

                UtilesHelper.setValorCelda(sheet, i, "V", "CANTIDAD", titleCellStyle); /* 21 */
                UtilesHelper.setValorCelda(sheet, i, "W", "VALOR UNIT", titleCellStyle); /* 22 */
                UtilesHelper.setValorCelda(sheet, i, "X", "SUBTOTAL", titleCellStyle); /* 23 */
                UtilesHelper.setValorCelda(sheet, i, "Y", "COSTO UNIT", titleCellStyle); /* 24 */
                UtilesHelper.setValorCelda(sheet, i, "Z", "MK UP%", titleCellStyle); /* 25 */
                UtilesHelper.setValorCelda(sheet, i, "AA", "GP S/", titleCellStyle); /* 26 */
                UtilesHelper.setValorCelda(sheet, i, "AB", "GP %", titleCellStyle); /* 27 */

                UtilesHelper.setValorCelda(sheet, i, "AC", "UNIDAD MP", titleCellStyle); /* 33 */
                UtilesHelper.setValorCelda(sheet, i, "AD", "EQUIVALENCIA MP", titleCellStyle); /* 34 */
                UtilesHelper.setValorCelda(sheet, i, "AE", "CANTIDAD MP", titleCellStyle); /* 35 */

                UtilesHelper.setValorCelda(sheet, i, "AF", "UNIDAD PROV", titleCellStyle); /* 36 */
                UtilesHelper.setValorCelda(sheet, i, "AG", "EQUIVALENCIA PROV", titleCellStyle); /* 37 */
                UtilesHelper.setValorCelda(sheet, i, "AH", "CANTIDAD PROV", titleCellStyle); /* 38 */

                UtilesHelper.setValorCelda(sheet, i, "AI", "CÓDIGO SUPERVISOR COMERCIAL", titleCellStyle); /* 28 */
                UtilesHelper.setValorCelda(sheet, i, "AJ", "SUPERVISOR COMERCIAL", titleCellStyle); /* 29 */
                UtilesHelper.setValorCelda(sheet, i, "AK", "CÓDIGO ASISTENTE COMERCIAL", titleCellStyle); /* 30 */
                UtilesHelper.setValorCelda(sheet, i, "AL", "ASISTENTE COMERCIAL", titleCellStyle); /* 31 */
                UtilesHelper.setValorCelda(sheet, i, "AM", "PEDIDO CREADO POR", titleCellStyle); /* 32 */

                i = i + 1;

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (List<String> obj in list)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", obj.ElementAt(0));
                    UtilesHelper.setValorCelda(sheet, i, "B", obj.ElementAt(1));
                    UtilesHelper.setValorCelda(sheet, i, "C", obj.ElementAt(2));
                    UtilesHelper.setValorCelda(sheet, i, "D", obj.ElementAt(3));
                    UtilesHelper.setValorCelda(sheet, i, "E", obj.ElementAt(4));
                    UtilesHelper.setValorCelda(sheet, i, "F", obj.ElementAt(5));
                    UtilesHelper.setValorCelda(sheet, i, "G", obj.ElementAt(6));
                    UtilesHelper.setValorCelda(sheet, i, "H", obj.ElementAt(7));
                    UtilesHelper.setValorCelda(sheet, i, "I", obj.ElementAt(8));
                    UtilesHelper.setValorCelda(sheet, i, "J", obj.ElementAt(9));
                    UtilesHelper.setValorCelda(sheet, i, "K", obj.ElementAt(10));
                    UtilesHelper.setValorCelda(sheet, i, "L", obj.ElementAt(11));
                    UtilesHelper.setValorCelda(sheet, i, "M", obj.ElementAt(12));
                    UtilesHelper.setValorCelda(sheet, i, "N", obj.ElementAt(13));
                    UtilesHelper.setValorCelda(sheet, i, "O", obj.ElementAt(14));
                    UtilesHelper.setValorCelda(sheet, i, "P", obj.ElementAt(15));
                    UtilesHelper.setValorCelda(sheet, i, "Q", obj.ElementAt(16));
                    UtilesHelper.setValorCelda(sheet, i, "R", obj.ElementAt(17));
                    UtilesHelper.setValorCelda(sheet, i, "S", obj.ElementAt(18));
                    UtilesHelper.setValorCelda(sheet, i, "T", obj.ElementAt(19));
                    UtilesHelper.setValorCelda(sheet, i, "U", obj.ElementAt(20));

                    UtilesHelper.setValorCelda(sheet, i, "V", int.Parse(obj.ElementAt(21)));
                    UtilesHelper.setValorCelda(sheet, i, "W", double.Parse(obj.ElementAt(22)), fourDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "X", double.Parse(obj.ElementAt(23)), twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "Y", double.Parse(obj.ElementAt(24)), fourDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "Z", double.Parse(obj.ElementAt(25)), fourDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "AA", double.Parse(obj.ElementAt(26)), fourDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "AB", double.Parse(obj.ElementAt(27)), fourDecCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "AC", obj.ElementAt(33));
                    UtilesHelper.setValorCelda(sheet, i, "AD", double.Parse(obj.ElementAt(34)), twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "AE", double.Parse(obj.ElementAt(35)), twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "AF", obj.ElementAt(36));
                    UtilesHelper.setValorCelda(sheet, i, "AG", double.Parse(obj.ElementAt(37)), twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "AH", double.Parse(obj.ElementAt(38)), twoDecCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "AI", obj.ElementAt(28));
                    UtilesHelper.setValorCelda(sheet, i, "AJ", obj.ElementAt(29));
                    UtilesHelper.setValorCelda(sheet, i, "AK", obj.ElementAt(30));
                    UtilesHelper.setValorCelda(sheet, i, "AL", obj.ElementAt(31));
                    UtilesHelper.setValorCelda(sheet, i, "AM", obj.ElementAt(32));
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

                    result.FileDownloadName = "ReporteDetallesSellOutVendedor_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }
    }
}