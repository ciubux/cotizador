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
using Newtonsoft.Json;
using NPOI.HSSF.Model;
using NPOI.HSSF.Util;
using NPOI.SS.Util;
using BusinessLayer;

using System.Web.Mvc;

namespace Cotizador.ExcelExport
{
   
    public class ReporteStock
    {
        private HSSFCellStyle defaulCellStyle;

        private HSSFFont formLabelFont;
        private HSSFCellStyle formLabelCellStyle;
        private HSSFCellStyle boldTextCenterCellStyle;
        private HSSFCellStyle formDataCenterCellStyle;
        private HSSFCellStyle tableDataCellStyle;
        private HSSFFont titleFont;
        private HSSFFont negativeStockFont;
        private HSSFCellStyle titleCellStyle;
        private HSSFCellStyle blockedDataCellStyle;
        private HSSFCellStyle titleDataCellStyle;
        private HSSFCellStyle familiaCellStyle;
        private HSSFCellStyle tableDataLastCellStyle;
        private HSSFCellStyle tableDataCenterCellStyle;
        private HSSFCellStyle tableDataCenterCellStyleNegative;
        private HSSFCellStyle tableDataCellStyleB;
        private HSSFCellStyle tableDataCenterCellStyleB;
        private HSSFCellStyle tableDataCenterCellStyleNegativeB;
        private HSSFCellStyle tableDataLastCenterCellStyle;
        private IDataFormat format;

        public FileStreamResult generateExcel(List<RegistroCargaStock> list, Usuario usuario, int tipoUnidad, DateTime fechaReporte)
        {
            
            HSSFWorkbook wb;
            //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);
            HSSFSheet sheet = null;
            
            wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());

            defaulCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
            defaulCellStyle.FillPattern = FillPattern.SolidForeground;
            defaulCellStyle.FillForegroundColor = HSSFColor.White.Index;

            formLabelFont = (HSSFFont)wb.CreateFont();
            formLabelFont.FontHeightInPoints = (short)11;
            formLabelFont.FontName = "Arial";
            formLabelFont.Color = IndexedColors.Black.Index;
            formLabelFont.IsBold = true;
            //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
            //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
            formLabelCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
            formLabelCellStyle.SetFont(formLabelFont);
            formLabelCellStyle.Alignment = HorizontalAlignment.Right;
            formLabelCellStyle.FillPattern = FillPattern.SolidForeground;
            formLabelCellStyle.FillForegroundColor = HSSFColor.White.Index;

            boldTextCenterCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
            boldTextCenterCellStyle.SetFont(formLabelFont);
            boldTextCenterCellStyle.Alignment = HorizontalAlignment.Center;

            formDataCenterCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
            formDataCenterCellStyle.Alignment = HorizontalAlignment.Center;
                
            tableDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
            tableDataCellStyle.WrapText = true;
            tableDataCellStyle.VerticalAlignment = VerticalAlignment.Center;
            tableDataCellStyle.BorderLeft = BorderStyle.Thin;
            tableDataCellStyle.BorderTop = BorderStyle.Thin;
            tableDataCellStyle.BorderRight = BorderStyle.Thin;
            tableDataCellStyle.BorderBottom = BorderStyle.Thin;


            negativeStockFont = (HSSFFont)wb.CreateFont();
            negativeStockFont.FontHeightInPoints = (short)11;
            negativeStockFont.FontName = "Arial";
            negativeStockFont.Color = IndexedColors.Red.Index;
            negativeStockFont.IsBold = true;

            titleFont = (HSSFFont)wb.CreateFont();
            titleFont.FontHeightInPoints = (short)11;
            titleFont.FontName = "Arial";
            titleFont.Color = IndexedColors.White.Index;
            titleFont.IsBold = true;
            //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
            //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
            titleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
            titleCellStyle.SetFont(titleFont);
            titleCellStyle.Alignment = HorizontalAlignment.Center;
            titleCellStyle.FillPattern = FillPattern.SolidForeground;
            titleCellStyle.FillForegroundColor = HSSFColor.RoyalBlue.Index;

            blockedDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
            blockedDataCellStyle.Alignment = HorizontalAlignment.Center;
            blockedDataCellStyle.FillPattern = FillPattern.SolidForeground;
            blockedDataCellStyle.FillForegroundColor = HSSFColor.Black.Index;


            titleDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
            titleDataCellStyle.CloneStyleFrom(titleCellStyle);
            titleDataCellStyle.VerticalAlignment = VerticalAlignment.Center;
            titleDataCellStyle.BorderLeft = BorderStyle.Thin;
            titleDataCellStyle.BorderTop = BorderStyle.Thin;
            titleDataCellStyle.BorderRight = BorderStyle.Thin;
            titleDataCellStyle.BorderBottom = BorderStyle.Thin;

            familiaCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
            familiaCellStyle.SetFont(formLabelFont);
            familiaCellStyle.VerticalAlignment = VerticalAlignment.Center;
            familiaCellStyle.Indention = 1;

            tableDataLastCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
            tableDataLastCellStyle.CloneStyleFrom(tableDataCellStyle);
            tableDataLastCellStyle.WrapText = true;
            tableDataLastCellStyle.BorderBottom = BorderStyle.Thin;

            tableDataCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
            tableDataCenterCellStyle.Alignment = HorizontalAlignment.Center;

            tableDataCenterCellStyleNegative = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCenterCellStyle);
            tableDataCenterCellStyleNegative.SetFont(negativeStockFont);

            tableDataCellStyleB = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
            tableDataCellStyleB.Alignment = HorizontalAlignment.Left;
            tableDataCellStyleB.FillPattern = FillPattern.SolidForeground;
            tableDataCellStyleB.FillForegroundColor = HSSFColor.Grey25Percent.Index;

            tableDataCenterCellStyleB = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCenterCellStyle);
            tableDataCenterCellStyleB.FillPattern = FillPattern.SolidForeground;
            tableDataCenterCellStyleB.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            

            tableDataCenterCellStyleNegativeB = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCenterCellStyleB);
            tableDataCenterCellStyleNegativeB.SetFont(negativeStockFont);

            tableDataLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataLastCellStyle);
            tableDataLastCenterCellStyle.Alignment = HorizontalAlignment.Center;

            format = wb.CreateDataFormat();
            ICellStyle dateFormatStyle = wb.CreateCellStyle();
            dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");


            int i = 0;
            // create sheet

            

            string tituloUnidad = "";
            switch (tipoUnidad)
            {
                case 1: tituloUnidad = "UNIDAD MP"; break;
                case 2: tituloUnidad = "UNIDAD ALTERNATIVA"; break;
                case 3: tituloUnidad = "UNIDAD PROVEEDOR"; break;
                case 99: tituloUnidad = "UNIDAD CONTEO"; break;
            }

            i = 2;

            /*  for (int iii = 0; iii<50;iii++)
                { */

            bool bStyle = false;
            Guid idSedeInicial = Guid.Empty;

            if (list.Count > 0) idSedeInicial = list[0].ciudad.idCiudad;

            sheet = NuevaHoja(wb, "STOCK", list.Count + 50, tituloUnidad);

            int col = 0;
            decimal total = 0;

            string[] colLetters = new string[13] { "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T" };
            foreach (RegistroCargaStock obj in list)
            {
                if (idSedeInicial.Equals(obj.ciudad.idCiudad))
                {
                    if (i>=3 && col > 0)
                    {
                        if (i==3)
                        {
                            UtilesHelper.combinarCeldas(sheet, 1, 1, "H", colLetters[col + 1]);
                            UtilesHelper.setColumnWidth(sheet, colLetters[col + 1], 3500);
                            UtilesHelper.setValorCelda(sheet, 2, colLetters[col + 1], "TOTAL", titleDataCellStyle);
                        }

                        if (total < 0)
                        {
                            UtilesHelper.setValorCelda(sheet, i, colLetters[col + 1], String.Format(Constantes.formatoDosDecimales, total), bStyle ? tableDataCenterCellStyleNegativeB : tableDataCenterCellStyleNegative);
                        }
                        else
                        {
                            UtilesHelper.setValorCelda(sheet, i, colLetters[col + 1], String.Format(Constantes.formatoDosDecimales, total), bStyle ? tableDataCenterCellStyleB : tableDataCenterCellStyle);
                        }

                        if (bStyle) { bStyle = false; } else { bStyle = true; }
                    }

                    i++;

                    total = 0;
                    col = 0;
                    string unidad = "";

                    switch (tipoUnidad)
                    {
                        case 1:
                            unidad = obj.producto.unidad;
                            break;
                        case 2:
                            unidad = obj.producto.unidad_alternativa;
                            break;
                        case 3:
                            unidad = obj.producto.unidadProveedor;
                            break;
                        case 99:
                            unidad = obj.producto.unidadConteo;
                            break;
                    }

                    if (bStyle)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "A", obj.producto.familia, tableDataCenterCellStyleB);
                        UtilesHelper.setValorCelda(sheet, i, "B", obj.producto.sku, tableDataCenterCellStyleB);
                        UtilesHelper.setValorCelda(sheet, i, "C", obj.producto.proveedor, tableDataCenterCellStyleB);
                        UtilesHelper.setValorCelda(sheet, i, "D", obj.producto.skuProveedor, tableDataCellStyleB);
                        UtilesHelper.setValorCelda(sheet, i, "E", obj.producto.descripcion, tableDataCellStyleB);

                        UtilesHelper.setValorCelda(sheet, i, "G", unidad, tableDataCellStyleB);
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "A", obj.producto.familia, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "B", obj.producto.sku, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "C", obj.producto.proveedor, tableDataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "D", obj.producto.skuProveedor, tableDataCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "E", obj.producto.descripcion, tableDataCellStyle);

                        UtilesHelper.setValorCelda(sheet, i, "G", unidad, tableDataCellStyle);
                    }
                } else
                {
                    col++;
                }
                
                
                if (i == 3)
                {
                    UtilesHelper.setColumnWidth(sheet, colLetters[col], 3500);
                    UtilesHelper.setValorCelda(sheet, 2, colLetters[col], obj.ciudad.nombre, titleDataCellStyle);
                }

                decimal stock = 0;
                
                switch (tipoUnidad)
                {
                    case 1:
                        //stock = ((decimal)obj.cantidadConteo) / ((decimal)(obj.producto.equivalenciaUnidadEstandarUnidadConteo));
                        stock = ((decimal)obj.cantidadConTrasladosConteo) / ((decimal)(obj.producto.equivalenciaUnidadEstandarUnidadConteo));
                        break;
                    case 2:
                        //stock = (((decimal)obj.cantidadConteo) / ((decimal)obj.producto.equivalenciaUnidadEstandarUnidadConteo)) * ((decimal)obj.producto.equivalenciaAlternativa);
                        stock = (((decimal)obj.cantidadConTrasladosConteo) / ((decimal)obj.producto.equivalenciaUnidadEstandarUnidadConteo)) * ((decimal)obj.producto.equivalenciaAlternativa);
                        break;
                    case 3:
                        //stock = ((decimal)obj.cantidadConteo) / ((decimal)(obj.producto.equivalenciaProveedor * obj.producto.equivalenciaUnidadEstandarUnidadConteo));
                        stock = ((decimal)obj.cantidadConTrasladosConteo) / ((decimal)(obj.producto.equivalenciaProveedor * obj.producto.equivalenciaUnidadEstandarUnidadConteo));
                        break;
                    case 99:
                        //stock = (decimal)obj.cantidadConteo;
                        stock = (decimal)obj.cantidadConTrasladosConteo;
                        break;
                }

                total += stock;

                if (obj.stockNoDisponible)
                {
                    UtilesHelper.setValorCelda(sheet, i, colLetters[col], "No Disponible", bStyle ? tableDataCenterCellStyleB : tableDataCenterCellStyle);
                }
                else
                {
                    if (stock < 0)
                    {
                        UtilesHelper.setValorCelda(sheet, i, colLetters[col], double.Parse(String.Format(Constantes.formatoDosDecimales, stock)), bStyle ? tableDataCenterCellStyleNegativeB : tableDataCenterCellStyleNegative);
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, colLetters[col], double.Parse(String.Format(Constantes.formatoDosDecimales, stock)), bStyle ? tableDataCenterCellStyleB : tableDataCenterCellStyle);
                    }
                    UtilesHelper.defineCellType(sheet, i, colLetters[col], CellType.Numeric);
                }
            }

            if (i >= 3 && col > 0)
            {
                if (i == 3)
                {
                    UtilesHelper.combinarCeldas(sheet, 1, 1, "H", colLetters[col + 1]);
                    UtilesHelper.setColumnWidth(sheet, colLetters[col + 1], 3500);
                    UtilesHelper.setValorCelda(sheet, 2, colLetters[col + 1], "TOTAL", titleDataCellStyle);
                }

                if (total < 0)
                {
                    UtilesHelper.setValorCelda(sheet, i, colLetters[col + 1], String.Format(Constantes.formatoDosDecimales, total), bStyle ? tableDataCenterCellStyleNegativeB : tableDataCenterCellStyleNegative);
                }
                else
                {
                    UtilesHelper.setValorCelda(sheet, i, colLetters[col + 1], String.Format(Constantes.formatoDosDecimales, total), bStyle ? tableDataCenterCellStyleB : tableDataCenterCellStyle);
                }

                if (bStyle) { bStyle = false; } else { bStyle = true; }
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

                result.FileDownloadName = "ReporteStockAl" + fechaReporte.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                return result;
            }
        }

        private HSSFSheet NuevaHoja(HSSFWorkbook wb, string titulo, int rows, string unidad)
        {
            HSSFSheet sheet = (HSSFSheet)wb.CreateSheet(titulo);


            /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                * ub.provincia,  ub.departamento, ma.observaciones,*/

            /*Cabecera, Sub total*/
            int cTotal = 14 + 10;

            /*Se crean todas las celdas*/
            for (int r = 0; r < rows; r++)
            {
                var row = sheet.CreateRow(r);
                for (int c = 0; c < cTotal; c++)
                {
                    row.CreateCell(c);
                }
            }

            int i = 1;

            UtilesHelper.combinarCeldas(sheet, i, i + 1, "A", "A");
            UtilesHelper.combinarCeldas(sheet, i, i + 1, "B", "B");
            UtilesHelper.combinarCeldas(sheet, i, i + 1, "C", "C");
            UtilesHelper.combinarCeldas(sheet, i, i + 1, "D", "D");
            UtilesHelper.combinarCeldas(sheet, i, i + 1, "E", "E");

            
            //UtilesHelper.combinarCeldas(sheet, i, i, "I", "J");
            //UtilesHelper.combinarCeldas(sheet, i, i, "L", "M");

            UtilesHelper.setValorCelda(sheet, i, "A", "Categoría", titleDataCellStyle);
            UtilesHelper.setValorCelda(sheet, i, "B", "SKU", titleDataCellStyle);
            UtilesHelper.setValorCelda(sheet, i, "C", "Prov.", titleDataCellStyle);
            UtilesHelper.setValorCelda(sheet, i, "D", "Cod. Proveedor", titleDataCellStyle);
            UtilesHelper.setValorCelda(sheet, i, "E", "Descripcion", titleDataCellStyle);

            UtilesHelper.setValorCelda(sheet, i, "G", unidad, titleDataCellStyle);
            UtilesHelper.combinarCeldas(sheet, i, i + 1, "G", "G");
            //UtilesHelper.setValorCelda(sheet, i + 1, "G", "Unidad", titleDataCellStyle);

            UtilesHelper.setValorCelda(sheet, i, "H", "STOCK EN SEDE", titleDataCellStyle);


            //UtilesHelper.setValorCelda(sheet, i, "I", "UNIDAD MP", titleDataCellStyle);
            //UtilesHelper.setValorCelda(sheet, i + 1, "I", "Unidad", titleDataCellStyle);
            //UtilesHelper.setValorCelda(sheet, i + 1, "J", "Stock", titleDataCellStyle);

            //UtilesHelper.setValorCelda(sheet, i, "L", "UNIDAD MENOR", titleDataCellStyle);
            //UtilesHelper.setValorCelda(sheet, i + 1, "L", "Unidad", titleDataCellStyle);
            //UtilesHelper.setValorCelda(sheet, i + 1, "M", "Stock", titleDataCellStyle);



            UtilesHelper.setColumnWidth(sheet, "A", 6500);
            UtilesHelper.setColumnWidth(sheet, "B", 2700);
            UtilesHelper.setColumnWidth(sheet, "C", 1800);
            UtilesHelper.setColumnWidth(sheet, "D", 4200);
            UtilesHelper.setColumnWidth(sheet, "E", 15000);

            UtilesHelper.setColumnWidth(sheet, "F", 500);
            UtilesHelper.setColumnWidth(sheet, "G", 6000);
           

            //UtilesHelper.setColumnWidth(sheet, "H", 500);
            //UtilesHelper.setColumnWidth(sheet, "I", 6000);
            //UtilesHelper.setColumnWidth(sheet, "J", 2000);

            //UtilesHelper.setColumnWidth(sheet, "K", 500);
            //UtilesHelper.setColumnWidth(sheet, "L", 6000);
            //UtilesHelper.setColumnWidth(sheet, "M", 2000);

            return sheet;
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