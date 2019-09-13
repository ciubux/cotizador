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
   
    public class DocumentoCompraSearch
    {
        public FileStreamResult generateExcel(List<DocumentoCompra> list)
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
                int cTotal = 15 + 2;

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

             
                UtilesHelper.setValorCelda(sheet, 1, "A", "N° Factura", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "B", "N° Pedido", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "C", "Movimiento Almacén", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "D", "Creado por", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "E", "Fecha Emisión", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "F", "Fecha Vencimiento", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "G", "Cód. Cliente", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "H", "Razón Social", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "I", "RUC", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "J", "Sede MP", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "K", "Total", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "L", "Estado", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "M", "Comentario Solicitud Anulación", titleCellStyle);
                

                i = 2;

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (DocumentoCompra obj in list)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", obj.serieNumero);
                    UtilesHelper.setValorCelda(sheet, i, "B", obj.pedido.numeroPedidoString);
                    string movimientoAlmacen = "";
                    if (obj.guiaRemision != null)
                    {
                        movimientoAlmacen = obj.guiaRemision.serieNumeroGuia;
                    }
                    else if (obj.notaIngreso != null)
                    {
                        movimientoAlmacen = obj.notaIngreso.serieNumeroNotaIngreso;
                    }

                    UtilesHelper.setValorCelda(sheet, i, "C", movimientoAlmacen);
                    UtilesHelper.setValorCelda(sheet, i, "D", obj.usuario.nombre);
                    UtilesHelper.setValorCelda(sheet, i, "E", ((DateTime) obj.fechaEmision).ToString("dd/MM/yyyy"));
                    UtilesHelper.setValorCelda(sheet, i, "F", ((DateTime)obj.fechaVencimiento).ToString("dd/MM/yyyy"));
                    UtilesHelper.setValorCelda(sheet, i, "G", obj.proveedor.codigo);
                    UtilesHelper.setValorCelda(sheet, i, "H", obj.proveedor.razonSocial);
                    UtilesHelper.setValorCelda(sheet, i, "I", obj.proveedor.ruc);
                    UtilesHelper.setValorCelda(sheet, i, "J", obj.ciudad.nombre);
                    UtilesHelper.setValorCelda(sheet, i, "K", (double) obj.total);
                    UtilesHelper.setValorCelda(sheet, i, "L", obj.estadoDocumentoSunatString);
                    UtilesHelper.setValorCelda(sheet, i, "M", obj.comentarioSolicitudAnulacion);
                    
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

                    result.FileDownloadName = "Facturas_Compra_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }
    }
}