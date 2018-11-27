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
   
    public class ClienteSearch
    {
        public FileStreamResult generateExcel(List<Cliente> list)
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
                UtilesHelper.setValorCelda(sheet, 1, "B", "Razón Social", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "C", "Nombre", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "D", "Tipo Doc.", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "E", "N° Doc.", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "F", "Sede MP", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "G", "Grupo", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "H", "Asesor Comercial", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "I", "Supervisor Comercial", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "J", "Asistente Servicio Cliente", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "K", "Plazo Crédito Aprobado", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "L", "Crédito Aprobado (S/)", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "L", "¿Bloqueado?", titleCellStyle);
                i = 2;

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (Cliente obj in list)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", obj.codigo);
                    UtilesHelper.setValorCelda(sheet, i, "B", obj.razonSocialSunat);
                    UtilesHelper.setValorCelda(sheet, i, "C", obj.nombreComercial);
                    UtilesHelper.setValorCelda(sheet, i, "D", obj.tipoDocumentoIdentidadToString);
                    UtilesHelper.setValorCelda(sheet, i, "E", obj.ruc);
                    UtilesHelper.setValorCelda(sheet, i, "F", obj.ciudad.nombre);
                    UtilesHelper.setValorCelda(sheet, i, "G", obj.grupoCliente.nombre);
                    UtilesHelper.setValorCelda(sheet, i, "H", obj.responsableComercial.descripcion);
                    UtilesHelper.setValorCelda(sheet, i, "I", obj.supervisorComercial.descripcion);
                    UtilesHelper.setValorCelda(sheet, i, "J", obj.asistenteServicioCliente.descripcion);
                    UtilesHelper.setValorCelda(sheet, i, "K", obj.tipoPagoFacturaToString);
                    UtilesHelper.setValorCelda(sheet, i, "L", (double) obj.creditoAprobado);
                    if (obj.bloqueado)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "M", "BLOQUEADO");
                    } else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "M", "");
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

                    result.FileDownloadName = "Clientes_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }
    }
}