﻿using NPOI.SS.UserModel;
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

        public FileStreamResult generateExcelRequerimientos(List<Pedido> list)
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
                titleMasterCellStyle.BottomBorderColor = HSSFColor.RoyalBlue.Index;
                titleMasterCellStyle.LeftBorderColor = HSSFColor.RoyalBlue.Index;
                titleMasterCellStyle.TopBorderColor = HSSFColor.RoyalBlue.Index;
                titleMasterCellStyle.RightBorderColor = HSSFColor.RoyalBlue.Index;
                titleMasterCellStyle.BorderLeft = BorderStyle.Thin;
                titleMasterCellStyle.BorderRight = BorderStyle.Thin;
                titleMasterCellStyle.BorderTop = BorderStyle.Thin;
                titleMasterCellStyle.BorderBottom = BorderStyle.Thin;


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


                UtilesHelper.setColumnWidth(sheet, "A", 2800);
                UtilesHelper.setColumnWidth(sheet, "B", 5000);
                UtilesHelper.setColumnWidth(sheet, "C", 3200);
                UtilesHelper.setColumnWidth(sheet, "D", 12000);
                UtilesHelper.setColumnWidth(sheet, "E", 10000);

                UtilesHelper.setColumnWidth(sheet, "F", 7000);
                UtilesHelper.setColumnWidth(sheet, "G", 2200);
                UtilesHelper.setColumnWidth(sheet, "H", 2200);
                UtilesHelper.setColumnWidth(sheet, "I", 2200);
                UtilesHelper.setColumnWidth(sheet, "J", 2200);
                UtilesHelper.setColumnWidth(sheet, "K", 3000);

                UtilesHelper.setRowHeight(sheet, 1, 350);
                UtilesHelper.setValorCelda(sheet, 1, "A", "PRODUCTOS", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "B", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "C", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "D", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "E", "", titleMasterCellStyle);
                UtilesHelper.combinarCeldas(sheet, 1, 1, "A", "E");
                
               

                UtilesHelper.setRowHeight(sheet, 2, 540);
                UtilesHelper.setValorCelda(sheet, 2, "A", "Productos", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "B", "SKU", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "C", "SKU Prov", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "D", "Descripción", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "E", "Unidad", titleCellStyle);

                UtilesHelper.setRowHeight(sheet, 3, 540);
                UtilesHelper.setValorCelda(sheet, 3, "A", "Prod 1", dataCellStyle);
                UtilesHelper.setValorCelda(sheet, 3, "B", "HJ1F20", dataCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, 3, "C", "30197006", dataCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, 3, "D", "Jabón Espuma Kleenex Dermo Supreme", dataCellStyle);
                UtilesHelper.setValorCelda(sheet, 3, "E", "Sachet x 800 ml", dataCellStyle);

                UtilesHelper.setRowHeight(sheet, 4, 540);
                UtilesHelper.setValorCelda(sheet, 4, "A", "Prod 2", dataCellStyle);
                UtilesHelper.setValorCelda(sheet, 4, "B", "HH2S81", dataCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, 4, "C", "30228044", dataCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, 4, "D", "Papel higiénico Scott JRT Super Económico Ahorramax con pre-corte (rollo x 550m)", dataCellStyle);
                UtilesHelper.setValorCelda(sheet, 4, "E", "Bls x 4 x 550 mts", dataCellStyle);


                

                int i = 7;
                UtilesHelper.setRowHeight(sheet, i-1, 350);
                UtilesHelper.setValorCelda(sheet, i-1, "A", "REQUERIMIENTOS", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "B", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "C", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "D", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "E", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "F", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "G", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "H", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "I", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "J", "", titleMasterCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "K", "", titleMasterCellStyle);
                UtilesHelper.combinarCeldas(sheet, i-1, i-1, "A", "K");


                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "A", "N° Req", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "Creado por", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "Fecha Registro", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "Dirección Acopio", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "Dirección Establecimiento", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "Nombre Sede", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "Cant. Prod 1", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "Cant. Prod 2", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", "Tope Ppto.", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", "Total", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "K", "Estado", titleCellStyle);


                i++;

                /*  for (int iii = 0; iii<50;iii++)
                  { */
                string desf = "{0} ({1}) {2} - {3} - {4}";
                string desdr = "{0}  {1} - {2} - {3}";
                String des;
                foreach (Pedido obj in list)
                {
                    UtilesHelper.setRowHeight(sheet, i, 540);
                    UtilesHelper.setValorCelda(sheet, i, "A", obj.numeroPedidoString, dataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", obj.usuario.nombre, dataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "C", obj.fechaHoraRegistro, dataCenterCellStyle);

                    DireccionEntrega de = obj.direccionEntrega.direccionEntregaAlmacen;
                    des = string.Format(desf, de.descripcion, de.nombre,
                                        de.ubigeo.Departamento,
                                        de.ubigeo.Provincia,
                                        de.ubigeo.Distrito);

                   
                    

                    UtilesHelper.setValorCelda(sheet, i, "D", des, dataCellStyle);
                    de = obj.direccionEntrega;

                    des = string.Format(desdr, de.descripcion, 
                                        obj.ubigeoEntrega.Departamento,
                                        obj.ubigeoEntrega.Provincia,
                                        obj.ubigeoEntrega.Distrito);

                    UtilesHelper.setValorCelda(sheet, i, "E",des, dataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "F", obj.observaciones, dataCenterCellStyle);
                    foreach (PedidoDetalle pedidoDetalle in obj.pedidoDetalleList)
                    {
                        if (pedidoDetalle.producto.sku.Equals("HJ1F20"))
                        {
                            UtilesHelper.setValorCelda(sheet, i, "G", pedidoDetalle.cantidad, dataCenterCellStyle);
                        }
                        else
                        {
                            UtilesHelper.setValorCelda(sheet, i, "H", pedidoDetalle.cantidad, dataCenterCellStyle);
                        }

                    }

                    UtilesHelper.setValorCelda(sheet, i, "I", (double)obj.topePresupuesto, twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "J", (double)obj.montoTotal, twoDecCellStyle);
                    if (obj.excedioPresupuesto)
                    {
                        sheet.GetRow(i - 1).GetCell(9).CellStyle = twoDecErrorCellStyle;
                    } else
                    {
                        sheet.GetRow(i - 1).GetCell(9).CellStyle = twoDecSuccessCellStyle;
                    }

                    UtilesHelper.setValorCelda(sheet, i, "K", obj.estadoRequerimientoString, dataCenterCellStyle);

                    i++;
                }

                UtilesHelper.setRowHeight(sheet, i, 540);
                sheet.GetRow(i - 1).GetCell(6).CellFormula = "SUM(" + UtilesHelper.columnas[6] + "2:" + UtilesHelper.columnas[6] + (i - 1) + ")";
                sheet.GetRow(i - 1).GetCell(6).CellStyle = dataCenterCellStyle;
                
                sheet.GetRow(i - 1).GetCell(7).CellFormula = "SUM(" + UtilesHelper.columnas[7] + "2:" + UtilesHelper.columnas[7] + (i - 1) + ")";
                sheet.GetRow(i - 1).GetCell(7).CellStyle = dataCenterCellStyle;

                sheet.GetRow(i - 1).GetCell(9).CellFormula = "SUM(" + UtilesHelper.columnas[8] + "2:" + UtilesHelper.columnas[8] + (i - 1) + ")";
                sheet.GetRow(i - 1).GetCell(9).CellStyle = twoDecCellStyle;


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