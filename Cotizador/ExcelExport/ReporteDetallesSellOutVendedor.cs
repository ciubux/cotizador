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
                int cTotal = 56 + 2;

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

                int indexColumn = 0;

                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "id_venta_detalle", titleCellStyle); /* 0 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "CÓDIGO RESPONSABLE COMERCIAL", titleCellStyle); /* 1 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "RESPONSABLE COMERCIAL", titleCellStyle); /* 2 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "CIUDAD", titleCellStyle); /* 3 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "CÓDIGO GRUPO", titleCellStyle); /* 4 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "GRUPO", titleCellStyle); /* 5 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "N° DOC CLIENTE", titleCellStyle); /* 6 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "CLIENTE", titleCellStyle); /* 7 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "CÓDIGO CLIENTE", titleCellStyle); /* 8 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "TIPO MOVIMIENTO", titleCellStyle); /* 9 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "FECHA TRANSACCIÓN", titleCellStyle); /* 10 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "N° PEDIDO", titleCellStyle); /* 11 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "N° GRUPO PEDIDO", titleCellStyle); /* 12 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "GUÍA", titleCellStyle); /* 13 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "FECHA EMISIÓN GUÍA", titleCellStyle); /* 14 */ indexColumn++;

                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "TIPO CPE", titleCellStyle); /* 43 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "N° CPE", titleCellStyle); /* 44 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "FECHA EMISIÓN CPE", titleCellStyle); /* 45 */ indexColumn++;

                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "FAMILIA PROD", titleCellStyle); /* 15 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "PROVEEDOR", titleCellStyle); /* 16 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "SKU MP", titleCellStyle); /* 17 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "SKU PROV", titleCellStyle); /* 18 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "DESCRIPCIÓN PROD", titleCellStyle); /* 19 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "UNIDAD VENTA", titleCellStyle); /* 20 */ indexColumn++;

                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "CANTIDAD", titleCellStyle); /* 21 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "VALOR UNIT", titleCellStyle); /* 22 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "SUBTOTAL", titleCellStyle); /* 23 */ indexColumn++;
                
                if(usuario.visualizaCostos)
                {
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "COSTO UNIT", titleCellStyle); /* 24 */ indexColumn++;
                }
                
                if (usuario.visualizaMargen)
                {
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "MK UP%", titleCellStyle); /* 25 */ indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "GP S/", titleCellStyle); /* 26 */ indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "GP %", titleCellStyle); /* 27 */ indexColumn++;
                }

                if (usuario.visualizaCostos)
                {
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "COSTO ESPECIAL", titleCellStyle); /* 46 */ indexColumn++;
                }
                

                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "UNIDAD MP", titleCellStyle); /* 33 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "EQUIVALENCIA MP", titleCellStyle); /* 34 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "CANTIDAD MP", titleCellStyle); /* 35 */ indexColumn++;

                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "UNIDAD PROV", titleCellStyle); /* 36 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "EQUIVALENCIA PROV", titleCellStyle); /* 37 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "CANTIDAD PROV", titleCellStyle); /* 38 */ indexColumn++;

                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "CÓDIGO SUPERVISOR COMERCIAL", titleCellStyle); /* 28 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "SUPERVISOR COMERCIAL", titleCellStyle); /* 29 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "CÓDIGO ASISTENTE COMERCIAL", titleCellStyle); /* 30 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "ASISTENTE COMERCIAL", titleCellStyle); /* 31 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "PEDIDO CREADO POR", titleCellStyle); /* 32 */ indexColumn++;

                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "VENTA EXCLUIDA", titleCellStyle); /* 39 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "EMPRESA", titleCellStyle); /* 41 */ indexColumn++;
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], "N° PEDIDO RELACIONADO", titleCellStyle); /* 42 */ indexColumn++;

                i = i + 1;

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (List<String> obj in list)
                {
                    indexColumn = 0;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(0)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(1)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(2)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(3)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(4)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(5)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(6)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(7)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(8)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(9)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(10)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(11)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(12)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(13)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(14)); indexColumn++;

                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(43)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(44)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(45)); indexColumn++;


                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(15)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(16)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(17)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(18)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(19)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(20)); indexColumn++;

                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], int.Parse(obj.ElementAt(21))); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], double.Parse(obj.ElementAt(22)), fourDecCellStyle); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], double.Parse(obj.ElementAt(23)), twoDecCellStyle); indexColumn++;

                    if (usuario.visualizaCostos)
                    {
                        UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], double.Parse(obj.ElementAt(24)), fourDecCellStyle); indexColumn++;
                    }

                    if (usuario.visualizaMargen)
                    {
                        UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], double.Parse(obj.ElementAt(25)), fourDecCellStyle); indexColumn++;
                        UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], double.Parse(obj.ElementAt(26)), fourDecCellStyle); indexColumn++;
                        UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], double.Parse(obj.ElementAt(27)), fourDecCellStyle); indexColumn++;
                    }

                    if (usuario.visualizaCostos)
                    {
                        UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], double.Parse(obj.ElementAt(46)), fourDecCellStyle); indexColumn++;
                    }

                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(33)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], double.Parse(obj.ElementAt(34)), twoDecCellStyle); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], double.Parse(obj.ElementAt(35)), twoDecCellStyle); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(36)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], double.Parse(obj.ElementAt(37)), twoDecCellStyle); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], double.Parse(obj.ElementAt(38)), twoDecCellStyle); indexColumn++;

                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(28)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(29)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(30)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(31)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(32)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(39)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(41)); indexColumn++;
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[indexColumn], obj.ElementAt(42)); indexColumn++;

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