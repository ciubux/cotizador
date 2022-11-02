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
   
    public class ProductoWebSendData
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
                int rTotal = (list.Count * 8) + 20;
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
                UtilesHelper.setColumnWidth(sheet, "B", 3000);
                UtilesHelper.setColumnWidth(sheet, "C", 3000);



                UtilesHelper.setValorCelda(sheet, i, "A", "SKU", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "PRECIO", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "STOCK", titleCellStyle);
                
                i = i + 1;

                /*  for (int iii = 0; iii<50;iii++)
                  { */
                marcarUnidades(sheet, i - 1, i + list.Count + 10, 2);
                setDataValidationSINO(sheet, i - 1, i + list.Count + 10, 5);

                foreach (ProductoWeb obj in list)
                {
                    for(int j = 0; j < obj.stocks.Count; j++)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "A", obj.codigoSedes.ElementAt(j) + obj.sku);
                        if (obj.codigoSedes.ElementAt(j).Equals("LI"))
                        {
                            UtilesHelper.setValorCelda(sheet, i, "B", (double) obj.precio, twoDecCellStyle);
                        } else
                        {
                            UtilesHelper.setValorCelda(sheet, i, "B", (double) obj.precioProvincia, twoDecCellStyle);
                        }

                        UtilesHelper.setValorCelda(sheet, i, "C", obj.stocks.ElementAt(j));

                        i++;
                    }
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

                    result.FileDownloadName = "ProductosWebInventario_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

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