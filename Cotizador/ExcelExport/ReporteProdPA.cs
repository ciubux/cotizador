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
   
    public class ReporteProdPA 
    {
        public FileStreamResult generateExcel(List<FilaProductoPendienteAtencion> list, ReportePendientesAtencionFiltro filtro, Usuario usuario)
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


                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");



                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Reporte");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (list.Count) + 12;
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


                UtilesHelper.setColumnWidth(sheet, "A", 3000);
                UtilesHelper.setColumnWidth(sheet, "B", 2700);
                UtilesHelper.setColumnWidth(sheet, "C", 15000);
                UtilesHelper.setColumnWidth(sheet, "D", 4200);
                UtilesHelper.setColumnWidth(sheet, "E", 1800);
                UtilesHelper.setColumnWidth(sheet, "F", 6500);
                UtilesHelper.setColumnWidth(sheet, "G", 3000);


                UtilesHelper.combinarCeldas(sheet, i, i, "A", "B");
                UtilesHelper.setValorCelda(sheet, i, "A", "SEDE:", titleCellStyle);
                if(filtro.ciudad == null)
                {
                    UtilesHelper.setValorCelda(sheet, i, "C", "TODAS");
                } else
                {
                    UtilesHelper.setValorCelda(sheet, i, "C", filtro.ciudad.nombre);
                }
                
                UtilesHelper.setValorCelda(sheet, i, "D", "UNIDAD:", titleCellStyle);
                switch (filtro.idProductoPresentacion)
                {
                    case 0: UtilesHelper.setValorCelda(sheet, i, "E", "MP"); break;
                    case 1: UtilesHelper.setValorCelda(sheet, i, "E", "ALTERNATIVA"); break;
                    case 2: UtilesHelper.setValorCelda(sheet, i, "E", "PROVEEDOR"); break;
                    case 3: UtilesHelper.setValorCelda(sheet, i, "E", "CONTEO"); break;
                    
                }

                i = i + 2;

                UtilesHelper.combinarCeldas(sheet, i, i, "A", "B");
                UtilesHelper.setValorCelda(sheet, i, "A", "FECHA ENTREGA:", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", filtro.fechaEntregaInicio.ToString("dd/MM/yyyy") + " - " + filtro.fechaEntregaFin.ToString("dd/MM/yyyy"));

                i = i + 2;

                UtilesHelper.setValorCelda(sheet, i, "A", "SEDE", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "SKU", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "NOMBRE PRODUCTO", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "FAMILIA", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "PROV.", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "UNIDAD", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "CANTIDAD PENDIENTE ATENCIÓN", titleCellStyle);

                i = i + 1;

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (FilaProductoPendienteAtencion obj in list)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", obj.sede);
                    UtilesHelper.setValorCelda(sheet, i, "B", obj.sku);
                    UtilesHelper.setValorCelda(sheet, i, "C", obj.nombreProducto);
                    UtilesHelper.setValorCelda(sheet, i, "D", obj.familia);
                    UtilesHelper.setValorCelda(sheet, i, "E", obj.proveedor);
                    switch(filtro.idProductoPresentacion)
                    {
                        case 0: UtilesHelper.setValorCelda(sheet, i, "F", obj.unidad);
                                UtilesHelper.setValorCelda(sheet, i, "G", (double) obj.cpMp, twoDecCellStyle);
                                break;
                        case 1: UtilesHelper.setValorCelda(sheet, i, "F", obj.unidadAlternativa);
                                UtilesHelper.setValorCelda(sheet, i, "G", (double) obj.cpAlternativa, twoDecCellStyle);
                                break;
                        case 2: UtilesHelper.setValorCelda(sheet, i, "F", obj.unidadProveedor);
                                UtilesHelper.setValorCelda(sheet, i, "G", (double) obj.cpProveedor, twoDecCellStyle);
                                break;
                        case 3: UtilesHelper.setValorCelda(sheet, i, "F", obj.unidadConteo);
                                UtilesHelper.setValorCelda(sheet, i, "G", (double) obj.cpConteo, twoDecCellStyle);
                                break;
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

                    result.FileDownloadName = "ReporteProductosPendienteAtencion_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }
    }
}