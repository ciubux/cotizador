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
using NPOI.SS.Util;

namespace Cotizador.ExcelExport
{
   
    public class ProductoWebCarga 
    {
        public FileStreamResult generateExcel(List<ProductoWeb> list, Usuario usuario)
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

                HSSFCellStyle blockedCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                blockedCellStyle.FillPattern = FillPattern.SolidForeground;
                blockedCellStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;


                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Reporte");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (list.Count) + 20;
                int cTotal = 30 + 2;

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
                UtilesHelper.setColumnWidth(sheet, "B", 8000);
                UtilesHelper.setColumnWidth(sheet, "C", 5000);
                UtilesHelper.setColumnWidth(sheet, "D", 7000);
                UtilesHelper.setColumnWidth(sheet, "E", 3000);
                UtilesHelper.setColumnWidth(sheet, "F", 3000);



                UtilesHelper.setValorCelda(sheet, i, "A", "SKU", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "TITULO", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "TIPO UNIDAD WEB", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "UNIDAD WEB", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "CUOTA WEB", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "ACTIVO", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "DESCRIPCIÓN CATÁLOGO", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "DESCRIPCIÓN CORTA", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", "POSICIÓN", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", "CATEGORÍA", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "K", "SUBCATEGORÍA", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "L", "ATRIBUTO 1 TITULO", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "M", "ATRIBUTO 1 VALOR", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "N", "ATRIBUTO 2 TITULO", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "O", "ATRIBUTO 2 VALOR", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "P", "ATRIBUTO 3 TITULO", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "Q", "ATRIBUTO 3 VALOR", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "R", "ATRIBUTO 4 TITULO", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "S", "ATRIBUTO 4 VALOR", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "T", "ATRIBUTO 5 TITULO", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "U", "ATRIBUTO 5 VALOR", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "V", "TAGS BÚSQUEDA", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "W", "TAGS PROMOCIONES", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "X", "SEO TÍTULO", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "Y", "SEO PALABRAS CLAVE", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "Z", "SEO DESCRIPCIÓN", titleCellStyle);
                
                i = i + 1;

                /*  for (int iii = 0; iii<50;iii++)
                  { */
                marcarUnidades(sheet, i - 1, i + list.Count + 10, 2);
                setDataValidationSINO(sheet, i - 1, i + list.Count + 10, 5);

                foreach (ProductoWeb obj in list)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", obj.producto.sku);
                    UtilesHelper.setValorCelda(sheet, i, "B", obj.nombre);

                    
                    switch(obj.presentacion.IdProductoPresentacion)
                    {
                        case 0: UtilesHelper.setValorCelda(sheet, i, "C", "MP"); break;
                        case 1: UtilesHelper.setValorCelda(sheet, i, "C", "Alternativa"); break;
                        case 2: UtilesHelper.setValorCelda(sheet, i, "C", "Proveedor"); break;
                        case 3: UtilesHelper.setValorCelda(sheet, i, "C", "Conteo"); break;
                    }


                    UtilesHelper.setValorCelda(sheet, i, "D", obj.presentacion.Presentacion, blockedCellStyle);

                    if (obj.cuotaWeb >= 0)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "E", obj.cuotaWeb);
                    } else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "E", "");
                    }

                    if(obj.Estado == 1)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "F", "SI");
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "F", "NO");
                    }

                    UtilesHelper.setValorCelda(sheet, i, "G", obj.descripcionCatalogo);
                    UtilesHelper.setValorCelda(sheet, i, "H", obj.descripcionCorta);
                    UtilesHelper.setValorCelda(sheet, i, "I", obj.itemOrder);
                    UtilesHelper.setValorCelda(sheet, i, "J", obj.categoria);
                    UtilesHelper.setValorCelda(sheet, i, "K", obj.subCategoria);
                    UtilesHelper.setValorCelda(sheet, i, "L", obj.atributoTitulo1);
                    UtilesHelper.setValorCelda(sheet, i, "M", obj.atributoValor1);
                    UtilesHelper.setValorCelda(sheet, i, "N", obj.atributoTitulo2);
                    UtilesHelper.setValorCelda(sheet, i, "O", obj.atributoValor2);
                    UtilesHelper.setValorCelda(sheet, i, "P", obj.atributoTitulo3);
                    UtilesHelper.setValorCelda(sheet, i, "Q", obj.atributoValor3);
                    UtilesHelper.setValorCelda(sheet, i, "R", obj.atributoTitulo4);
                    UtilesHelper.setValorCelda(sheet, i, "S", obj.atributoValor4);
                    UtilesHelper.setValorCelda(sheet, i, "T", obj.atributoTitulo5);
                    UtilesHelper.setValorCelda(sheet, i, "U", obj.atributoValor5);
                    UtilesHelper.setValorCelda(sheet, i, "V", obj.tagBusqueda);
                    UtilesHelper.setValorCelda(sheet, i, "W", obj.tagPromociones);
                    UtilesHelper.setValorCelda(sheet, i, "X", obj.seoTitulo);
                    UtilesHelper.setValorCelda(sheet, i, "Y", obj.seoPalabrasClave);
                    UtilesHelper.setValorCelda(sheet, i, "Z", obj.seoDescripcion);

                    i++;
                }

                for (int j = i; j < (i + 10); j++)
                {
                    UtilesHelper.setValorCelda(sheet, j, "D", "", blockedCellStyle);
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

                    result.FileDownloadName = "ProductosWeb_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

                    return result;
                }
            }
        }

        protected void marcarUnidades(HSSFSheet sheet, int rowInit, int rows, int col)
        {
            var markConstraint = DVConstraint.CreateExplicitListConstraint(new string[] { "MP", "Alternativa", "Proveedor", "Conteo" });
            var markColumn = new CellRangeAddressList(rowInit, rowInit + rows, col, col);
            var markdv = new HSSFDataValidation(markColumn, markConstraint);
            markdv.EmptyCellAllowed = true;
            markdv.CreateErrorBox("Valor Incorrecto", "Por favor seleccione un tipo de unidad de la lista");
            sheet.AddValidationData(markdv);
        }

        protected void setDataValidationSINO(HSSFSheet sheet, int rowInit, int rows, int col)
        {
            string[] options = { "SI", "NO" };
            var markConstraintA = DVConstraint.CreateExplicitListConstraint(options);
            var markColumnA = new CellRangeAddressList(rowInit, rowInit + rows, col, col);
            var markdvA = new HSSFDataValidation(markColumnA, markConstraintA);
            markdvA.EmptyCellAllowed = true;
            markdvA.CreateErrorBox("Valor Incorrecto", "Por favor seleccione un valor de la lista");
            sheet.AddValidationData(markdvA);
        }
    }
}