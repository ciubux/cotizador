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
   
    public class PedidoSearch
    {
        public FileStreamResult generateExcel(List<Pedido> list)
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

                UtilesHelper.setValorCelda(sheet, 1, "A", "N°", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "B", "Sede MP", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "C", "Cod.Cliente", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "D", "Razón Social", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "E", "O/C N°", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "F", "Creado por", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "G", "Fecha Registro", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "H", "Rango Fecha Entrega ", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "I", "Horarios Entrega", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "J", "Total(Incl.IGV)", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "K", "Distrito Entrega", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "L", "Estado Atención", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "M", "Estado Crediticio", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "N", "Obs. Uso Intern", titleCellStyle);
                
                    

                i = 2;

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (Pedido obj in list)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", obj.numeroPedidoString);
                    UtilesHelper.setValorCelda(sheet, i, "B", obj.ciudad.nombre);
                    UtilesHelper.setValorCelda(sheet, i, "C", obj.cliente.codigo);
                    UtilesHelper.setValorCelda(sheet, i, "D", obj.cliente.razonSocial);
                    UtilesHelper.setValorCelda(sheet, i, "E", obj.numeroReferenciaCliente);
                    UtilesHelper.setValorCelda(sheet, i, "F", obj.usuario.nombre);
                    UtilesHelper.setValorCelda(sheet, i, "G", obj.fechaHoraRegistro);
                    UtilesHelper.setValorCelda(sheet, i, "H", obj.rangoFechasEntrega);
                    UtilesHelper.setValorCelda(sheet, i, "I", obj.rangoHoraEntrega);
                    UtilesHelper.setValorCelda(sheet, i, "J", (double) obj.montoTotal);
                    UtilesHelper.setValorCelda(sheet, i, "K", obj.ubigeoEntrega.Distrito);
                    UtilesHelper.setValorCelda(sheet, i, "L", obj.seguimientoPedido.estadoString);
                    UtilesHelper.setValorCelda(sheet, i, "M", obj.seguimientoCrediticioPedido.estadoString);
                    UtilesHelper.setValorCelda(sheet, i, "N", obj.observaciones);

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

                    result.FileDownloadName = "Pedidos_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }
    }
}