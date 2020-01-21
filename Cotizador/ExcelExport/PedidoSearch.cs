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
        public FileStreamResult generateExcel(List<Pedido> list, Requerimiento requerimiento)
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
                sheet = (HSSFSheet)wb.CreateSheet("Pedidos");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (list.Count) + 8;
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

                                int i = 1; 
                UtilesHelper.setValorCelda(sheet, i, "A", "N°", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "Sede MP", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "Cod.Cliente", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "Razón Social", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "O/C N°", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "Creado por", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "Fecha Registro", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "Rango Fecha Entrega ", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", "Horarios Entrega", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", "Total(No Incl.IGV)", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "K", "Distrito Entrega", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "L", "Estado Atención", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "M", "Estado Crediticio", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "N", "Obs. Uso Intern", titleCellStyle);
 



                i++;

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


                sheet.GetRow(i-1).GetCell(9).CellFormula = "SUM(" + UtilesHelper.columnas[9] + "2:" + UtilesHelper.columnas[9] + (i-1) + ")";
           

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

        public FileStreamResult generateExcelRequerimientos(List<Requerimiento> list, Requerimiento requerimiento)
        {

            HSSFWorkbook wb;
            //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);
            HSSFSheet sheet;
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());


                HSSFFont titleFont = (HSSFFont)wb.CreateFont();
                titleFont.FontHeightInPoints = (short)11;
                titleFont.FontName = "Arial";
                titleFont.Color = IndexedColors.White.Index;
                titleFont.IsBold = true;
                //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
                //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
                HSSFCellStyle titleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleCellStyle.SetFont(titleFont);
                titleCellStyle.Alignment = HorizontalAlignment.Center;
                titleCellStyle.VerticalAlignment = VerticalAlignment.Center;
                titleCellStyle.FillPattern = FillPattern.SolidForeground;
                titleCellStyle.FillForegroundColor = HSSFColor.RoyalBlue.Index;
                titleCellStyle.WrapText = true;

                HSSFFont titleMasterFont = (HSSFFont)wb.CreateFont();
                titleMasterFont.FontHeightInPoints = (short)11;
                titleMasterFont.FontName = "Arial";
                titleMasterFont.Color = IndexedColors.RoyalBlue.Index;
                titleMasterFont.IsBold = true;
                //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
                //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
                HSSFCellStyle titleMasterCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleMasterCellStyle.SetFont(titleMasterFont);
                titleMasterCellStyle.Alignment = HorizontalAlignment.Center;
                titleMasterCellStyle.VerticalAlignment = VerticalAlignment.Center;
                titleMasterCellStyle.FillPattern = FillPattern.SolidForeground;
                titleMasterCellStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
                titleMasterCellStyle.BorderLeft = BorderStyle.Thin;
                titleMasterCellStyle.BorderRight = BorderStyle.Thin;
                titleMasterCellStyle.BorderTop = BorderStyle.Thin;
                titleMasterCellStyle.BorderBottom = BorderStyle.Thin;
                titleMasterCellStyle.BottomBorderColor = HSSFColor.RoyalBlue.Index;
                titleMasterCellStyle.LeftBorderColor = HSSFColor.RoyalBlue.Index;
                titleMasterCellStyle.TopBorderColor = HSSFColor.RoyalBlue.Index;
                titleMasterCellStyle.RightBorderColor = HSSFColor.RoyalBlue.Index;


                var avgCellFormate = wb.CreateDataFormat();
                var twoDecFormat = avgCellFormate.GetFormat("0.00");
                HSSFCellStyle twoDecCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                twoDecCellStyle.DataFormat = twoDecFormat;
                twoDecCellStyle.VerticalAlignment = VerticalAlignment.Center;


                HSSFCellStyle dataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                dataCellStyle.VerticalAlignment = VerticalAlignment.Center;
                dataCellStyle.WrapText = true;

                HSSFCellStyle dataCenterCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                dataCenterCellStyle.CloneStyleFrom(dataCellStyle);
                dataCenterCellStyle.Alignment = HorizontalAlignment.Center;


                HSSFFont fontErrorStyle = (HSSFFont)wb.CreateFont();
                fontErrorStyle.FontHeightInPoints = (short)10;
                fontErrorStyle.Color = IndexedColors.Red.Index;
                fontErrorStyle.IsBold = true;
                HSSFCellStyle twoDecErrorCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                twoDecErrorCellStyle.CloneStyleFrom(twoDecCellStyle);
                twoDecErrorCellStyle.SetFont(fontErrorStyle);


                HSSFFont fontSuccessStyle = (HSSFFont)wb.CreateFont();
                fontSuccessStyle.FontHeightInPoints = (short)10;
                fontSuccessStyle.Color = IndexedColors.Green.Index;
                HSSFCellStyle twoDecSuccessCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                twoDecSuccessCellStyle.CloneStyleFrom(twoDecCellStyle);
                twoDecSuccessCellStyle.SetFont(fontSuccessStyle);


                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");



                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Requerimientos");


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (list.Count) + 20;
                int cTotal = 14 + 3;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c);
                    }
                }



                UtilesHelper.setRowHeight(sheet, 1, 350);
                UtilesHelper.combinarCeldas(sheet, 1, 1, "A", "E");
                UtilesHelper.setValorCelda(sheet, 1, "A", "PRODUCTOS", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "B", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "C", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "D", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "E", "", titleMasterCellStyle);

                UtilesHelper.setRowHeight(sheet, 2, 540);
                UtilesHelper.setValorCelda(sheet, 2, "A", "Productos", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "B", "SKU", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "C", "SKU Prov", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "D", "Descripción", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "E", "Unidad", titleCellStyle);

                int i = 3;
                int contadorCanasta = 1;
                foreach (DocumentoDetalle requerimientoDetalle in requerimiento.documentoDetalle)
                {
                    if (requerimientoDetalle.producto.precioClienteProducto.estadoCanasta)
                    {
                       
                        UtilesHelper.setRowHeight(sheet, i, 540);
                        UtilesHelper.setValorCelda(sheet, i, "A", "Prod "+ contadorCanasta, dataCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "B", requerimientoDetalle.producto.sku, dataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "C", requerimientoDetalle.producto.skuProveedor, dataCenterCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "D", requerimientoDetalle.producto.descripcion, dataCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "E", requerimientoDetalle.unidad, dataCellStyle);
                        i++;
                        contadorCanasta++;
                    }
                }


                contadorCanasta = contadorCanasta - 1;




                UtilesHelper.setColumnWidth(sheet, "A", 2800);
                UtilesHelper.setColumnWidth(sheet, "B", 5000);
                UtilesHelper.setColumnWidth(sheet, "C", 3200);
                UtilesHelper.setColumnWidth(sheet, "D", 12000);
                UtilesHelper.setColumnWidth(sheet, "E", 10000);
                int columna = contadorCanasta;
                int cc = 0;
                for (cc = 0; cc < columna; cc++)
                {
                    UtilesHelper.setColumnWidth(sheet, UtilesHelper.columnas[contadorCanasta + cc], 2200);
                    //UtilesHelper.setValorCelda(sheet, i - 1, UtilesHelper.columnas[contadorCanasta + cc], "", titleMasterCellStyle);
                }
                columna = cc + contadorCanasta;

            
                UtilesHelper.setColumnWidth(sheet, UtilesHelper.columnas[columna], 2200);
                UtilesHelper.setColumnWidth(sheet, UtilesHelper.columnas[columna+1], 7200);
                UtilesHelper.setColumnWidth(sheet, UtilesHelper.columnas[columna+2], 3000);
                UtilesHelper.setColumnWidth(sheet, UtilesHelper.columnas[columna+3], 1500);
                

                i = i+4;
                UtilesHelper.setRowHeight(sheet, i-1, 350);
                UtilesHelper.combinarCeldas(sheet, i - 1, i - 1, "A", "M");
                UtilesHelper.setValorCelda(sheet, i - 1, "A", "REQUERIMIENTOS", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "B", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "C", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "D", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "E", "", titleMasterCellStyle);

                columna = contadorCanasta;
                cc = 0;
                for (cc=0; cc < columna; cc++)
                {
                    UtilesHelper.setValorCelda(sheet, i - 1, UtilesHelper.columnas[contadorCanasta + cc], "", titleMasterCellStyle);
                }
                columna = cc + contadorCanasta;


                UtilesHelper.setValorCelda(sheet, i - 1, UtilesHelper.columnas[columna], "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, UtilesHelper.columnas[columna + 1], "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, UtilesHelper.columnas[columna + 2], "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, UtilesHelper.columnas[columna + 3], "", titleMasterCellStyle);
                //UtilesHelper.setValorCelda(sheet, i - 1, UtilesHelper.columnas[columna + 5], "", titleMasterCellStyle);
                /*  UtilesHelper.setValorCelda(sheet, i - 1, "K", "", titleMasterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i - 1, "L", "", titleMasterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i - 1, "M", "", titleMasterCellStyle);*/








                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "A", "N° Req", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "Creado por", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "Fecha Registro", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "Distrito", titleCellStyle);
                //UtilesHelper.setValorCelda(sheet, i, "E", "Dirección", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "Obs. Uso interno", titleCellStyle);


                columna = contadorCanasta;
                cc = 0;
                for (cc = 0; cc < columna; cc++)
                {
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[contadorCanasta + cc], "Cant. Prod "+(cc+1), titleCellStyle);
                }
                columna = cc + contadorCanasta;
                



                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[columna], "Total (No Incl. IGV)", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[columna + 1], "Estado", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[columna + 2], "Tope Ppto.", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[columna + 3], "Excedió Presupuesto.", titleCellStyle);


                i++;

                /*  for (int iii = 0; iii<50;iii++)
                  { */
                string desf = "{0} ({1}) {2} - {3} - {4}";
                string desdr = "{0}  {1} - {2} - {3}";
                String des;
                foreach (Requerimiento obj in list)
                {
                    UtilesHelper.setRowHeight(sheet, i, 540);
                    UtilesHelper.setValorCelda(sheet, i, "A", obj.numeroRequerimientoString, dataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", obj.usuario.nombre, dataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "C", obj.fechaHoraRegistro, dataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "D", obj.ubigeoEntrega.Distrito, dataCellStyle);
                    /*      DireccionEntrega de = obj.direccionEntrega.direccionEntregaAlmacen;
                          des = string.Format(desf, de.descripcion, de.nombre,
                                              de.ubigeo.Departamento,
                                              de.ubigeo.Provincia,
                                              de.ubigeo.Distrito);*/




            /*        UtilesHelper.setValorCelda(sheet, i, "E", obj.observaciones, dataCellStyle);
                    de = obj.direccionEntrega;

                    des = string.Format(desdr, de.descripcion, 
                                        obj.ubigeoEntrega.Departamento,
                                        obj.ubigeoEntrega.Provincia,
                                        obj.ubigeoEntrega.Distrito);

                    UtilesHelper.setValorCelda(sheet, i, "E",des, dataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "F", obj.observaciones, dataCenterCellStyle);*/
               /*     foreach (RequerimientoDetalle pedidoDetalle in obj.requerimientoDetalleList)
                    {
                        switch (pedidoDetalle.producto.sku)
                        {




                            case "HJ1F20": UtilesHelper.setValorCelda(sheet, i, "F", pedidoDetalle.cantidad, dataCenterCellStyle); break;
                            case "HH2S81": UtilesHelper.setValorCelda(sheet, i, "G", pedidoDetalle.cantidad, dataCenterCellStyle); break;
                //            case "MPXX01": UtilesHelper.setValorCelda(sheet, i, "I", pedidoDetalle.cantidad, dataCenterCellStyle); break;
                     //       default: UtilesHelper.setValorCelda(sheet, i, "I", pedidoDetalle.cantidad, dataCenterCellStyle); break;
                        }
                    }
                    */



                    columna = contadorCanasta;
                    cc = 0;
                    for (cc = 0; cc < columna; cc++)
                    {
                        UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[contadorCanasta + cc], obj.requerimientoDetalleList[cc].cantidad, dataCenterCellStyle);
                    }
                    columna = cc + contadorCanasta;




                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[columna], (double)obj.montoTotal, twoDecCellStyle);
           // 
                  

                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[columna + 1], obj.estadoRequerimientoString, dataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[columna + 2], (double)obj.topePresupuesto, twoDecCellStyle);
                    if (obj.excedioPresupuesto)
                    {
                        sheet.GetRow(i - 1).GetCell(columna).CellStyle = twoDecErrorCellStyle;
                        UtilesHelper.setValorCelda(sheet, i, UtilesHelper.columnas[columna + 3], "X", dataCenterCellStyle);
                    }
                    else
                    {
                        sheet.GetRow(i - 1).GetCell(9).CellStyle = twoDecSuccessCellStyle;
                    }
                    i++;
                }

               UtilesHelper.setRowHeight(sheet, i, 540);
                sheet.GetRow(i - 1).GetCell(columna).CellFormula = "SUM(" + UtilesHelper.columnas[columna ] + "2:" + UtilesHelper.columnas[columna] + (i - 1) + ")";
                sheet.GetRow(i - 1).GetCell(columna ).CellStyle = dataCenterCellStyle;
                
           ///     sheet.GetRow(i - 1).GetCell(columna+1).CellFormula = "SUM(" + UtilesHelper.columnas[columna + 1] + "2:" + UtilesHelper.columnas[columna + 1] + (i - 1) + ")";
           ///     sheet.GetRow(i - 1).GetCell(columna + 1).CellStyle = dataCenterCellStyle;

                sheet.GetRow(i - 1).GetCell(columna + 2).CellFormula = "SUM(" + UtilesHelper.columnas[columna  + 2] + "2:" + UtilesHelper.columnas[columna + 2] + (i - 1) + ")";
                sheet.GetRow(i - 1).GetCell(columna + 2).CellStyle = dataCenterCellStyle;

          /*      sheet.GetRow(i - 1).GetCell(9).CellFormula = "SUM(" + UtilesHelper.columnas[9] + "2:" + UtilesHelper.columnas[9] + (i - 1) + ")";
                sheet.GetRow(i - 1).GetCell(9).CellStyle = twoDecCellStyle;
                */
                MemoryStream ms = new MemoryStream();
                using (MemoryStream tempStream = new MemoryStream())
                {
                    wb.Write(tempStream);
                    var byteArray = tempStream.ToArray();
                    ms.Write(byteArray, 0, byteArray.Length);
                    ms.Flush();
                    ms.Position = 0;
                    FileStreamResult result = new FileStreamResult(ms, "application/vnd.ms-excel");

                    result.FileDownloadName = "Requerimientos_" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }

    }
}