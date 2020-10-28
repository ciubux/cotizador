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


                string[] tiposVentaRestringida = Enum.GetNames(typeof(Producto.TipoVentaRestringida));

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
                int cTotal = 39 + 2;

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


                var markConstraint = DVConstraint.CreateExplicitListConstraint(tiposVentaRestringida);
                var markColumn = new CellRangeAddressList(1, 1 + list.Count, 33, 33) ;
                var markdv = new HSSFDataValidation(markColumn, markConstraint);
                markdv.EmptyCellAllowed = true;
                markdv.CreateErrorBox("Valor Incorrecto", "Por favor seleccione un tipo de la lista");
                sheet.AddValidationData(markdv);

                setDataValidationSINO(sheet, 1 + list.Count, 28);
                setDataValidationSINO(sheet, 1 + list.Count, 29);
                setDataValidationSINO(sheet, 1 + list.Count, 32);
                setDataValidationSINO(sheet, 1 + list.Count, 37);

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
                    UtilesHelper.setValorCelda(sheet, i, "I", obj.unidadPedidoProveedor);
                    UtilesHelper.setValorCelda(sheet, i, "J", obj.equivalenciaUnidadPedidoProveedor);
                    UtilesHelper.setValorCelda(sheet, i, "K", obj.unidad_alternativa);
                    UtilesHelper.setValorCelda(sheet, i, "L", obj.equivalenciaAlternativa);

                    UtilesHelper.setValorCelda(sheet, i, "M", obj.monedaProveedor);
                    UtilesHelper.setValorCelda(sheet, i, "N", (double)obj.costoOriginal);
                    UtilesHelper.setValorCelda(sheet, i, "O", (double)obj.costoSinIgv, blockedCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "P", obj.monedaMP);
                    UtilesHelper.setValorCelda(sheet, i, "Q", (double)obj.precioOriginal);
                    UtilesHelper.setValorCelda(sheet, i, "R", (double)obj.precioSinIgv, blockedCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "S", (double)obj.precioProvinciasOriginal);
                    UtilesHelper.setValorCelda(sheet, i, "T", (double)obj.precioProvinciaSinIgv, blockedCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "U", obj.unidadConteo);
                    UtilesHelper.setValorCelda(sheet, i, "V", obj.unidadEstandarInternacional);
                    UtilesHelper.setValorCelda(sheet, i, "W", obj.equivalenciaUnidadEstandarUnidadConteo);
                    UtilesHelper.setValorCelda(sheet, i, "X", obj.unidadProveedorInternacional);
                    UtilesHelper.setValorCelda(sheet, i, "Y", obj.equivalenciaUnidadProveedorUnidadConteo);
                    UtilesHelper.setValorCelda(sheet, i, "Z", obj.unidadAlternativaInternacional);
                    UtilesHelper.setValorCelda(sheet, i, "AA", obj.equivalenciaUnidadAlternativaUnidadConteo);

                    UtilesHelper.setValorCelda(sheet, i, "AB", obj.codigoSunat);
                    UtilesHelper.setValorCelda(sheet, i, "AC", obj.exoneradoIgv ? "SI" : "NO");
                    UtilesHelper.setValorCelda(sheet, i, "AD", obj.inafecto ? "SI" : "NO");
                    UtilesHelper.setValorCelda(sheet, i, "AE", obj.tipoProducto.ToString());
                    UtilesHelper.setValorCelda(sheet, i, "AF", (double)obj.tipoCambio);
                    UtilesHelper.setValorCelda(sheet, i, "AG", obj.Estado == 1 ? "SI" : "NO");
                    UtilesHelper.setValorCelda(sheet, i, "AH", Enum.GetName(typeof(Producto.TipoVentaRestringida), obj.ventaRestringida));

                    UtilesHelper.setValorCelda(sheet, i, "AI", obj.motivoRestriccion == null || obj.motivoRestriccion.Trim().Equals("") ? "" : obj.motivoRestriccion);

                    UtilesHelper.setValorCelda(sheet, i, "AJ", obj.cantidadMaximaPedidoRestringido);
                    UtilesHelper.setValorCelda(sheet, i, "AK", obj.descripcionLarga);
                    UtilesHelper.setValorCelda(sheet, i, "AL", obj.agregarDescripcionCotizacion == 1 ? "SI" : "NO");
                    UtilesHelper.setValorCelda(sheet, i, "AM", (double)obj.topeDescuento);
                    i++;
                }

                UtilesHelper.setValorCelda(sheet, 1, "A", "SKU MP", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "B", "SKU Prov.", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "C", "Proveedor", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "D", "Familia", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "E", "Descripción", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "F", "Unidad de Venta", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "G", "Unidad Proveedor", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "H", "Equiva. Und.Prov.", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "I", "Unidad Facturación Proveedor", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "J", "Equivalencia UP/UFP", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "K", "Unidad Alternativa", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "L", "Equiva. Und. Alt.", titleCellStyle);
               

                UtilesHelper.setValorCelda(sheet, 1, "M", "Moneda Proveedor", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "N", "Costo Und Prov", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "O", "Costo Und MP", titleCellStyle);
                UtilesHelper.setColumnDefaultStyle(sheet, "O", blockedCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "P", "Moneda Venta MP", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "Q", "Precio Lista MP Lima", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "R", "Precio Lima", titleCellStyle);
                UtilesHelper.setColumnDefaultStyle(sheet, "R", blockedCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "S", "Precio Lista MP Provincias", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "T", "Precio Provincias", titleCellStyle);
                UtilesHelper.setColumnDefaultStyle(sheet, "T", blockedCellStyle);

                UtilesHelper.setValorCelda(sheet, 1, "U", "Unidad de Conteo", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "V", "Unidad SUNAT", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "W", "Equiva. Und.Est. Und.Conteo", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "X", "Unidad Proveedor Internacional", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "Y", "Equivalencia Und.Prov. Und.Conteo", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "Z", "Unidad Alternativa SUNAT", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AA", "Equivalencia Und.Alt. Und.Conteo", titleCellStyle);

                UtilesHelper.setValorCelda(sheet, 1, "AB", "Código SUNAT", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AC", "Exonerado IGV", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AD", "Inafecto", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AE", "Tipo de Producto", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AF", "Tipo Cambio", titleCellStyle);
                UtilesHelper.setColumnDefaultStyle(sheet, "AF", blockedCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AG", "Activo", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AH", "Venta Restringida", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AI", "Motivo Venta Restringida", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AJ", "Cantidad Máxima Para No Restringir Pedido", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AK", "Descripción Detallada", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AL", "Agregar Descripción Detallada a Cotización", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "AM", "% Tope Descuento", titleCellStyle);

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