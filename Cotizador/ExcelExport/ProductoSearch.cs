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

                UtilesHelper.setValorCelda(sheet, 1, "A", "Código", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "B", "Código Proveedor", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "C", "Proveedor", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "D", "Familia", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "E", "Descripcion", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "F", "Unidad", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "G", "Unidad Alternativa", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "H", "Equivalencia ", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "I", "Unidad Proveedor", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "J", "Equivalencia Proveedor", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "K", "Precio", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "L", "Precio Provincia", titleCellStyle, true);
                if (usuario.visualizaCostos) { 
                    UtilesHelper.setValorCelda(sheet, 1, "M", "Costo", titleCellStyle, true);
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
                    UtilesHelper.setValorCelda(sheet, i, "H", obj.equivalenciaAlternativa);
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

        public FileStreamResult generateUploadExcel(List<Producto> list, Usuario usuario)
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

                HSSFCellStyle blockedCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                blockedCellStyle.FillPattern = FillPattern.SolidForeground;
                blockedCellStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;

                //titleCellStyle.FillBackgroundColor = HSSFColor.BlueGrey.Index;

                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");



                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Productos");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (list.Count) + 1;
                int cTotal = 34 + 2;

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
                
                UtilesHelper.setValorCelda(sheet, 1, "A", "SKU MP", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "B", "SKU Prov.", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "C", "Proveedor", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "D", "Familia", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "E", "Descripción", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "F", "Unidad de Venta", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "G", "Unidad Proveedor", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "H", "Equiva. Und.Prov.", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "I", "Unidad Alternativa", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "J", "Equivalencia Und.Alt.", titleCellStyle, true);

                UtilesHelper.setValorCelda(sheet, 1, "K", "Moneda Proveedor", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "L", "Costo Und Prov", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "M", "Costo Und MP", titleCellStyle, true);
                UtilesHelper.setColumnDefaultStyle(sheet, "M", blockedCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "N", "Moneda Venta MP", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "O", "Precio Lista MP Lima", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "P", "Precio Lima", titleCellStyle, true);
                UtilesHelper.setColumnDefaultStyle(sheet, "P", blockedCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "Q", "Precio Lista MP Provincias", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "R", "Precio Provincias", titleCellStyle, true);
                UtilesHelper.setColumnDefaultStyle(sheet, "R", blockedCellStyle);

                UtilesHelper.setValorCelda(sheet, 1, "S", "Unidad de Conteo", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "T", "Unidad SUNAT", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "U", "Equiva. Und.Est. Und.Conteo", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "V", "Unidad Proveedor Internacional", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "W", "Equivalencia Und.Prov. Und.Conteo", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "X", "Unidad Alternativa SUNAT", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "Y", "Equivalencia Und.Alt. Und.Conteo", titleCellStyle, true);
                
                UtilesHelper.setValorCelda(sheet, 1, "Z", "Código SUNAT", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "AA", "Exonerado IGV", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "AB", "Inafecto", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "AC", "Tipo de Producto", titleCellStyle, true);
                UtilesHelper.setValorCelda(sheet, 1, "AD", "Tipo Cambio", titleCellStyle, true);
                UtilesHelper.setColumnDefaultStyle(sheet, "AD", blockedCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AE", "Activo", titleCellStyle, true);


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
                    UtilesHelper.setValorCelda(sheet, i, "G", obj.unidadProveedor);
                    UtilesHelper.setValorCelda(sheet, i, "H", obj.equivalenciaProveedor);
                    UtilesHelper.setValorCelda(sheet, i, "I", obj.unidad_alternativa);
                    UtilesHelper.setValorCelda(sheet, i, "J", obj.equivalenciaAlternativa);
                    
                    UtilesHelper.setValorCelda(sheet, i, "K", obj.monedaProveedor);
                    UtilesHelper.setValorCelda(sheet, i, "L", (double)obj.costoOriginal);
                    UtilesHelper.setValorCelda(sheet, i, "M", (double)obj.costoSinIgv, blockedCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "N", obj.monedaMP);
                    UtilesHelper.setValorCelda(sheet, i, "O", (double)obj.precioOriginal);
                    UtilesHelper.setValorCelda(sheet, i, "P", (double)obj.precioSinIgv, blockedCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "Q", (double)obj.precioProvinciasOriginal);
                    UtilesHelper.setValorCelda(sheet, i, "R", (double)obj.precioProvinciaSinIgv, blockedCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "S", obj.unidadConteo);
                    UtilesHelper.setValorCelda(sheet, i, "T", obj.unidadEstandarInternacional);
                    UtilesHelper.setValorCelda(sheet, i, "U", obj.equivalenciaUnidadEstandarUnidadConteo);
                    UtilesHelper.setValorCelda(sheet, i, "V", obj.unidadProveedorInternacional);
                    UtilesHelper.setValorCelda(sheet, i, "W", obj.equivalenciaUnidadProveedorUnidadConteo);
                    UtilesHelper.setValorCelda(sheet, i, "X", obj.unidadAlternativaInternacional);
                    UtilesHelper.setValorCelda(sheet, i, "Y", obj.equivalenciaUnidadAlternativaUnidadConteo);

                    UtilesHelper.setValorCelda(sheet, i, "Z", obj.codigoSunat);
                    UtilesHelper.setValorCelda(sheet, i, "AA", obj.exoneradoIgv ? "SI" : "NO");
                    UtilesHelper.setValorCelda(sheet, i, "AB", obj.inafecto ? "SI" : "NO");
                    UtilesHelper.setValorCelda(sheet, i, "AC", obj.tipoProducto.ToString());
                    UtilesHelper.setValorCelda(sheet, i, "AD", (double)obj.tipoCambio);
                    UtilesHelper.setValorCelda(sheet, i, "AE", obj.Estado == 1 ? "SI" : "NO");

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