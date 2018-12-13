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

namespace Cotizador.ExcelExport
{
   
    public class ProductoSearch
    {
        public FileStreamResult generateExcel(List<Producto> list, Usuario usuario)
        {
            
            HSSFWorkbook wb;
            //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);
            HSSFSheet sheet;
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());


                HSSFFont titleFont = (HSSFFont)wb.CreateFont();
                titleFont.FontHeightInPoints = (short)11;
                titleFont.FontName = "Arial";
                titleFont.Color = IndexedColors.Black.Index;
                titleFont.IsBold = true;
                //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
                //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
                HSSFCellStyle titleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleCellStyle.SetFont(titleFont);
                titleCellStyle.FillPattern = FillPattern.SolidForeground;
                titleCellStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;

                //titleCellStyle.FillBackgroundColor = HSSFColor.BlueGrey.Index;





                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");



                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Atenciones");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (list.Count) + 4;
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

                int i = 0;

                UtilesHelper.setValorCelda(sheet, 1, "A", "Código", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "B", "Código Proveedor", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "C", "Proveedor", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "D", "Familia", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "E", "Descripcion", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "F", "Unidad", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "G", "Unidad Alternativa", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "H", "Equivalencia ", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "I", "Unidad Proveedor", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "J", "Equivalencia Proveedor", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "K", "Precio", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "L", "Precio Provincia", titleCellStyle);
                if (usuario.visualizaCostos) { 
                    UtilesHelper.setValorCelda(sheet, 1, "M", "Costo", titleCellStyle);
                }
                i = 2;

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (Producto obj in list)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", obj.sku);
                    UtilesHelper.setValorCelda(sheet, i, "B", obj.skuProveedor);
                    UtilesHelper.setValorCelda(sheet, i, "C", obj.proveedor);
                    UtilesHelper.setValorCelda(sheet, i, "D", obj.familia);
                    UtilesHelper.setValorCelda(sheet, i, "E", obj.descripcion);
                    UtilesHelper.setValorCelda(sheet, i, "F", obj.unidad);
                    UtilesHelper.setValorCelda(sheet, i, "G", obj.unidad_alternativa);
                    UtilesHelper.setValorCelda(sheet, i, "H", obj.equivalencia);
                    UtilesHelper.setValorCelda(sheet, i, "I", obj.unidadProveedor);
                    UtilesHelper.setValorCelda(sheet, i, "J", obj.equivalenciaProveedor);
                    UtilesHelper.setValorCelda(sheet, i, "K", (double) obj.precioSinIgv);
                    UtilesHelper.setValorCelda(sheet, i, "L", (double) obj.precioProvinciaSinIgv);
                    if (usuario.visualizaCostos)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "M", (double) obj.costoSinIgv);
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

                    result.FileDownloadName = "Productos_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }
    }
}