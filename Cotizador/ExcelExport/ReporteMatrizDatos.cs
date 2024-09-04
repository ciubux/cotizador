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
using NLog.Filters;
using static System.Net.WebRequestMethods;
using NPOI.SS.Formula.Functions;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace Cotizador.ExcelExport
{

    public class ReporteMatrizDatos
    {
        public FileStreamResult generateExcel(ReporteMatriz matriz)
        {

            HSSFWorkbook wb;
            //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);
            HSSFSheet sheet;
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());

                HSSFFont titleFont = (HSSFFont)wb.CreateFont();
                HSSFCellStyle datoCS = (HSSFCellStyle)wb.CreateCellStyle();
                HSSFCellStyle titleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                HSSFCellStyle titleDataCellStyle;

                datoCS = (HSSFCellStyle)wb.CreateCellStyle();
                datoCS.Alignment = HorizontalAlignment.Center;

                titleCellStyle.Alignment = HorizontalAlignment.Center;
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


                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet(matriz.etiquetaColumnas + " " + matriz.etiquetaFilas);


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = matriz.filas.Count + 12;
                int cTotal = matriz.columnas.Count + 5;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c);
                    }
                }

                List<String> codigosFilas = new List<String>();
                List<String> codigosColumnas = new List<String>();

                int iniFil = 2;
                int iniCol = 2;

                if (matriz.agrupaColumnas) { iniFil++; }
                if (matriz.tieneDescripcionColumnas) { iniFil++; }

                int i = 0;


                foreach (ReporteMatrizCabecera fila in matriz.filas)
                {
                    codigosFilas.Add(fila.codigo);
                    UtilesHelper.setValorCelda(sheet, iniFil + i, iniCol - 1, fila.nombre, titleDataCellStyle);
                    i++;
                }
                //sheet.AutoSizeColumn(iniCol - 2);

                int tmpIniAgrupador = 0;
                string tmpAgrupador = "";
                
                i = 0;
                foreach (ReporteMatrizCabecera col in matriz.columnas)
                {
                    codigosColumnas.Add(col.codigo);

                    UtilesHelper.setValorCelda(sheet, iniFil - 1, iniCol + i, col.nombre, titleDataCellStyle);
                    //sheet.AutoSizeColumn(iniCol + i - 1);

                    if (matriz.agrupaColumnas)
                    {
                        if(tmpAgrupador.Equals("")) { tmpAgrupador = col.nombreAgrupador; }
                        
                        if(!tmpAgrupador.Equals(col.nombreAgrupador)) {

                            UtilesHelper.combinarCeldas(sheet, iniFil - 2, iniFil - 2, iniCol + tmpIniAgrupador, iniCol + i - 1);
                            UtilesHelper.setValorCelda(sheet, iniFil - 2, iniCol + tmpIniAgrupador, tmpAgrupador, titleDataCellStyle);

                            tmpAgrupador = col.nombreAgrupador;
                            tmpIniAgrupador = i;
                        }
                    }

                    if(matriz.tieneDescripcionColumnas)
                    {
                        UtilesHelper.setValorCelda(sheet, iniFil - 3, iniCol + i, col.descripcion);
                    }

                    i++;
                }

                if (matriz.agrupaColumnas)
                {
                    UtilesHelper.combinarCeldas(sheet, iniFil - 2, iniFil - 2, iniCol + tmpIniAgrupador, iniCol + i - 1);
                    UtilesHelper.setValorCelda(sheet, iniFil - 2, iniCol + tmpIniAgrupador, tmpAgrupador, titleDataCellStyle);
                }

                int posFila = 0;
                int posCol = 0;
                string valorCelda = "";
                foreach (ReporteMatrizDato dato in matriz.datos) {
                    posFila = codigosFilas.IndexOf(dato.codigoFila);
                    posCol = codigosColumnas.IndexOf(dato.codigoColumna);
                    if (posFila >= 0 && posCol >= 0)
                    {
                        valorCelda = "";
                        if (matriz.concatenaValores && sheet.GetRow(iniFil + posFila - 1).GetCell(iniCol + posCol - 1) != null)
                        {
                            valorCelda = sheet.GetRow(iniFil + posFila - 1).GetCell(iniCol + posCol - 1).ToString();
                            valorCelda = valorCelda + matriz.concatenador + dato.valor;  
                        } else
                        {
                            valorCelda = dato.valor;
                        }

                        
                        UtilesHelper.setValorCelda(sheet, iniFil + posFila, iniCol + posCol, valorCelda, datoCS);
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

                    result.FileDownloadName = "Reporte" + matriz.etiquetaColumnas + matriz.etiquetaFilas + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }
    }
}