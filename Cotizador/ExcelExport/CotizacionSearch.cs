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
   
    public class CotizacionSearch
    {
        public FileStreamResult generateExcel(List<Cotizacion> list)
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
                int cTotal = 12 + 2;

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
                

                UtilesHelper.setValorCelda(sheet, 1, "A", "N°", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "B", "Creado por", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "C", "Fecha Última Edición", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "D", "Creado Para", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "E", "Razón Social", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "F", "Grupo", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "G", "Ciudad", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "H", "Total(No Inc. IGV)", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "I", "Desc % (max)", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "J", "Estado", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "K", "Total(Incl.IGV)", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "L", "Modificado última vez por", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "M", "Comentario Estado", titleCellStyle);
                
                    

                i = 2;

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (Cotizacion obj in list)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", obj.codigo);
                    UtilesHelper.setValorCelda(sheet, i, "B", obj.usuario.nombre);
                    UtilesHelper.setValorCelda(sheet, i, "C", obj.fecha.ToString("dd/MM/yyyy HH:mm"));
                    if (obj.cliente.idCliente == Guid.Empty)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "D", "Grupo");
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "D", "Cliente");
                    }
                    
                    UtilesHelper.setValorCelda(sheet, i, "E", obj.cliente.razonSocial);
                    UtilesHelper.setValorCelda(sheet, i, "F", obj.grupo.nombre);
                    UtilesHelper.setValorCelda(sheet, i, "G", obj.ciudad.nombre);
                    UtilesHelper.setValorCelda(sheet, i, "H", (double) obj.montoSubTotal);
                    UtilesHelper.setValorCelda(sheet, i, "I", (double) obj.maximoPorcentajeDescuentoPermitido);
                    UtilesHelper.setValorCelda(sheet, i, "J", obj.seguimientoCotizacion.estadoString);
                    UtilesHelper.setValorCelda(sheet, i, "K", (double)obj.montoTotal);
                    UtilesHelper.setValorCelda(sheet, i, "L", obj.seguimientoCotizacion.usuario.nombre);
                    UtilesHelper.setValorCelda(sheet, i, "M", obj.seguimientoCotizacion.observacion);

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

                    result.FileDownloadName = "Cotizaciones_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }
    }
}