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
using System.Globalization;
using NPOI.SS.Util;

namespace Cotizador.ExcelExport
{
    public class OrdenCompraClienteExcel
    {
        public static int filaInicioDatos { get { return 10; } }

        public FileStreamResult generateExcel(OrdenCompraCliente obj)
        {
            
            HSSFWorkbook wb;
            //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);
            HSSFSheet sheet;
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());

                HSSFCellStyle defaulCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                defaulCellStyle.FillPattern = FillPattern.SolidForeground;
                defaulCellStyle.FillForegroundColor = HSSFColor.White.Index;

                HSSFFont formLabelFont = (HSSFFont)wb.CreateFont();
                formLabelFont.FontHeightInPoints = (short)11;
                formLabelFont.FontName = "Arial";
                formLabelFont.Color = IndexedColors.Black.Index;
                formLabelFont.IsBold = true;
                //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
                //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
                HSSFCellStyle formLabelCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                formLabelCellStyle.SetFont(formLabelFont);
                formLabelCellStyle.Alignment = HorizontalAlignment.Right;
                formLabelCellStyle.FillPattern = FillPattern.SolidForeground;
                formLabelCellStyle.FillForegroundColor = HSSFColor.White.Index;

                HSSFCellStyle boldTextCenterCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                boldTextCenterCellStyle.SetFont(formLabelFont);
                boldTextCenterCellStyle.Alignment = HorizontalAlignment.Center;

                HSSFCellStyle formDataCenterCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                formDataCenterCellStyle.Alignment = HorizontalAlignment.Center;
                formDataCenterCellStyle.BorderLeft = BorderStyle.Thin;
                formDataCenterCellStyle.BorderTop = BorderStyle.Thin;
                formDataCenterCellStyle.BorderRight = BorderStyle.Thin;
                formDataCenterCellStyle.BorderBottom = BorderStyle.Thin;



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
                titleCellStyle.FillPattern = FillPattern.SolidForeground;
                titleCellStyle.FillForegroundColor = HSSFColor.RoyalBlue.Index;
               

                HSSFCellStyle titleDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleDataCellStyle.CloneStyleFrom(titleCellStyle);
                titleDataCellStyle.VerticalAlignment = VerticalAlignment.Center;
                titleDataCellStyle.BorderLeft = BorderStyle.Thin;
                titleDataCellStyle.BorderTop = BorderStyle.Thin;
                titleDataCellStyle.BorderRight = BorderStyle.Thin;

                HSSFCellStyle addressTextCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                addressTextCellStyle.CloneStyleFrom(defaulCellStyle);
                addressTextCellStyle.VerticalAlignment = VerticalAlignment.Top;
                addressTextCellStyle.WrapText = true;

                HSSFCellStyle ocTitleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                HSSFFont ocTitleFont = (HSSFFont)wb.CreateFont();
                ocTitleFont.FontHeightInPoints = (short)28;
                ocTitleFont.FontName = "Arial";
                ocTitleFont.Color = IndexedColors.RoyalBlue.Index;
                ocTitleFont.IsBold = true;
                ocTitleCellStyle.SetFont(ocTitleFont);
                ocTitleCellStyle.Alignment = HorizontalAlignment.Right;
                ocTitleCellStyle.VerticalAlignment = VerticalAlignment.Center;

                HSSFCellStyle footerTextCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                footerTextCellStyle.Alignment = HorizontalAlignment.Center;

                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                var avgCellFormate = wb.CreateDataFormat();
                var twoDecFormat = avgCellFormate.GetFormat("0.00");
                HSSFCellStyle twoDecCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                twoDecCellStyle.DataFormat = twoDecFormat;
                twoDecCellStyle.VerticalAlignment = VerticalAlignment.Center;
                twoDecCellStyle.BorderLeft = BorderStyle.Thin;
                twoDecCellStyle.BorderRight = BorderStyle.Thin;


                HSSFCellStyle totalsCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsCellStyle.DataFormat = twoDecFormat;
                totalsCellStyle.WrapText = true;
                totalsCellStyle.BorderLeft = BorderStyle.Thin;
                totalsCellStyle.BorderRight = BorderStyle.Thin;
                totalsCellStyle.BorderTop = BorderStyle.Thin;
                totalsCellStyle.BorderBottom = BorderStyle.Thin;
                totalsCellStyle.Indention = 1;


                HSSFCellStyle totalsTotalCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsTotalCellStyle.CloneStyleFrom(totalsCellStyle);
                totalsTotalCellStyle.BorderTop = BorderStyle.Double;
                HSSFFont totalFont = (HSSFFont)wb.CreateFont();
                totalFont.FontHeightInPoints = (short)11;
                totalFont.FontName = "Arial";
                totalFont.IsBold = true;
                totalsTotalCellStyle.SetFont(totalFont);


                HSSFCellStyle totalsTotalLabelCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsTotalLabelCellStyle.SetFont(totalFont);
                totalsTotalLabelCellStyle.BorderTop = BorderStyle.Double;

                HSSFCellStyle tableDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                tableDataCellStyle.WrapText = true;
                tableDataCellStyle.VerticalAlignment = VerticalAlignment.Center;
                tableDataCellStyle.BorderLeft = BorderStyle.Thin;
                tableDataCellStyle.BorderRight = BorderStyle.Thin;

                HSSFCellStyle tableDataLastCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                tableDataLastCellStyle.CloneStyleFrom(tableDataCellStyle);
                tableDataLastCellStyle.WrapText = true;
                tableDataLastCellStyle.BorderBottom = BorderStyle.Thin;

                HSSFCellStyle twoDecLastCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                twoDecLastCellStyle.CloneStyleFrom(twoDecCellStyle);
                twoDecLastCellStyle.BorderBottom = BorderStyle.Thin;


                HSSFCellStyle observacionesLabelCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                observacionesLabelCellStyle.WrapText = true;
                observacionesLabelCellStyle.VerticalAlignment = VerticalAlignment.Center;
                observacionesLabelCellStyle.Alignment = HorizontalAlignment.Center;
                HSSFFont observacionesLabelCellStyleFont = (HSSFFont)wb.CreateFont();
                observacionesLabelCellStyleFont.IsBold = true;
                observacionesLabelCellStyle.SetFont(observacionesLabelCellStyleFont);

                HSSFCellStyle observacionesTextCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                observacionesTextCellStyle.WrapText = true;
                observacionesTextCellStyle.VerticalAlignment = VerticalAlignment.Top;
                observacionesTextCellStyle.Alignment = HorizontalAlignment.Left;

                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("OCC_" + obj.numeroOrdenCompraCliente);


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (obj.detalleList.Count) + 20;
                int cTotal = 10;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c).CellStyle = defaulCellStyle;
                    }
                }

                UtilesHelper.combinarCeldas(sheet, 1, 1, "A", "B");

                IWorkbook newWorkbook = wb;

                byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\images\\logo.png");
                int picInd = newWorkbook.AddPicture(data, PictureType.PNG);
                HSSFCreationHelper helper = newWorkbook.GetCreationHelper() as HSSFCreationHelper;
                IDrawing drawing = sheet.CreateDrawingPatriarch();
                HSSFClientAnchor anchor = helper.CreateClientAnchor() as HSSFClientAnchor;
                anchor.Col1 = 0;
                anchor.Row1 = 0;
                anchor.Col2 = 3;

                anchor.AnchorType = AnchorType.DontMoveAndResize;

                HSSFPicture pict = drawing.CreatePicture(anchor, picInd) as HSSFPicture;
                pict.Resize(1);

                UtilesHelper.combinarCeldas(sheet, 1, 1, "D", "J");
                UtilesHelper.setValorCelda(sheet, 1, "D", "ORDEN COMPRA CLIENTE", ocTitleCellStyle);

                UtilesHelper.setRowHeight(sheet, 1, 1200);


                UtilesHelper.setValorCelda(sheet, 3, "A", "Solicitado Por");
                UtilesHelper.setValorCelda(sheet, 4, "A", obj.solicitante.nombre);
                UtilesHelper.setValorCelda(sheet, 5, "A", obj.solicitante.telefono);
                UtilesHelper.setValorCelda(sheet, 6, "A", obj.solicitante.correo);

                UtilesHelper.setValorCelda(sheet, 2, "H", "N° Registro:", formLabelCellStyle);
                UtilesHelper.combinarCeldas(sheet, 2, 2, "I", "J");
                UtilesHelper.setValorCelda(sheet, 2, "I", obj.numeroOrdenCompraClienteString, formDataCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "J", "", formDataCenterCellStyle);

                UtilesHelper.setValorCelda(sheet, 3, "H", "N° Orden Compra:", formLabelCellStyle);
                UtilesHelper.combinarCeldas(sheet, 3, 3, "I", "J");
                UtilesHelper.setValorCelda(sheet, 3, "I", obj.numeroReferenciaCliente, formDataCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, 3, "J", "", formDataCenterCellStyle);

                int i = filaInicioDatos - 1;

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "A", "SKU", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "SKU PROV" , titleDataCellStyle);
                UtilesHelper.combinarCeldas(sheet, i, i, "C", "D");
                UtilesHelper.setValorCelda(sheet, i, "C", "DESCRIPCIÓN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "TIPO UNIDAD", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "UNIDAD", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "CANT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "P. UNIT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", "FLETE", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", "TOTAL", titleDataCellStyle);

                UtilesHelper.setColumnWidth(sheet, "A", 2200);
                UtilesHelper.setColumnWidth(sheet, "B", 2900);
                UtilesHelper.setColumnWidth(sheet, "C", 4200);
                UtilesHelper.setColumnWidth(sheet, "D", 8000);
                UtilesHelper.setColumnWidth(sheet, "E", 4000);
                UtilesHelper.setColumnWidth(sheet, "F", 5000);

                UtilesHelper.setColumnWidth(sheet, "H", 2900);
                UtilesHelper.setColumnWidth(sheet, "I", 2900);
                UtilesHelper.setColumnWidth(sheet, "J", 3500);

                i++;

                HSSFCellStyle tableDataCenterCellStyle = (HSSFCellStyle) UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                HSSFCellStyle tableDataLastCenterCellStyle = (HSSFCellStyle) UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataLastCellStyle);

                HSSFCellStyle twoDecCenterCellStyle = (HSSFCellStyle) UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecCellStyle);
                HSSFCellStyle twoDecLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecLastCellStyle);

                HSSFCellStyle twoDecIdentCellStyle = (HSSFCellStyle) UtilesHelper.GetCloneStyleWithIndent(wb, twoDecCellStyle, 1);
                HSSFCellStyle twoDecIdentLastCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithIndent(wb, twoDecLastCellStyle, 1);

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (OrdenCompraClienteDetalle det in obj.detalleList)
                {
                    UtilesHelper.setRowHeight(sheet, i, 540);
                    UtilesHelper.setValorCelda(sheet, i, "A", "", tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", "", tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "C", "", tableDataCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "D", "", tableDataCellStyle);

                    definirListaUnidades(sheet, i - 1, 4);

                    String unidadDesc = "MP";

                    if (det.ProductoPresentacion != null)
                    {
                        switch (det.ProductoPresentacion.IdProductoPresentacion)
                        {
                            case 1: unidadDesc = "Alternativa"; break;
                            case 2: unidadDesc = "Proveedor"; break;
                        }
                    }

                    UtilesHelper.setValorCelda(sheet, i, "E", "", tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "F", "", tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "G", "", tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "H", "", tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "I", "", tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "J", "", tableDataCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "A", det.producto.sku);
                    UtilesHelper.setValorCelda(sheet, i, "B", det.producto.skuProveedor);
                    UtilesHelper.combinarCeldas(sheet, i, i, "C", "D");
                    UtilesHelper.setValorCelda(sheet, i, "C", det.producto.descripcion);
                    
                    UtilesHelper.setValorCelda(sheet, i, "E", unidadDesc);
                    UtilesHelper.setValorCelda(sheet, i, "F", det.unidad);
                    UtilesHelper.setValorCelda(sheet, i, "G", det.cantidad);
                    
                    
                    UtilesHelper.setValorCelda(sheet, i, "H", (double)det.precioUnitario, twoDecCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "I", (double)det.flete, twoDecIdentCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "J", (double)det.subTotal, twoDecIdentCellStyle);

                    i++;
                }
                /*
                UtilesHelper.combinarCeldas(sheet, i, i, "A", "B");
                UtilesHelper.setValorCelda(sheet, i, "A", "Observaciones:", observacionesLabelCellStyle);

                UtilesHelper.combinarCeldas(sheet, i + 1, i + 2, "B", "E");
                UtilesHelper.setValorCelda(sheet, i + 1, "B", obj.observaciones, observacionesTextCellStyle);
                */

                UtilesHelper.setValorCelda(sheet, i - 1, "A", UtilesHelper.getValorCelda(sheet, i - 1, "A"), tableDataLastCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "B", UtilesHelper.getValorCelda(sheet, i - 1, "B"), tableDataLastCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "C", UtilesHelper.getValorCelda(sheet, i - 1, "C"), tableDataLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "D", UtilesHelper.getValorCelda(sheet, i - 1, "D"), tableDataLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "E", UtilesHelper.getValorCelda(sheet, i - 1, "E"), tableDataLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "F", UtilesHelper.getValorCelda(sheet, i - 1, "F"), tableDataLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "G", int.Parse(UtilesHelper.getValorCelda(sheet, i - 1, "G")), tableDataLastCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "H", double.Parse(UtilesHelper.getValorCelda(sheet, i - 1, "H")), twoDecLastCenterCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "I", double.Parse(UtilesHelper.getValorCelda(sheet, i - 1, "I")), twoDecIdentLastCellStyle);
                UtilesHelper.setValorCelda(sheet, i - 1, "J", double.Parse(UtilesHelper.getValorCelda(sheet, i - 1, "J")), twoDecIdentLastCellStyle);

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "I", "SUB TOTAL");
                UtilesHelper.setValorCelda(sheet, i, "J", (double)obj.montoSubTotal, totalsCellStyle);
                i++;

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "I", "IGV");
                UtilesHelper.setValorCelda(sheet, i, "J", (double)obj.montoIGV, totalsCellStyle);
                i++;

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "I", "TOTAL", totalsTotalLabelCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", (double)obj.montoTotal, totalsTotalCellStyle);
                i++;

                /*
                UtilesHelper.combinarCeldas(sheet, i, i, "A", "F");
                UtilesHelper.setValorCelda(sheet, i, "A", "* Condición de Pago: " + obj.textoCondicionesPago);

                if (obj.usuario.firmaImagen != null)
                {
                    i = i + 2;
                    UtilesHelper.combinarCeldas(sheet, i, i, "A", "D");
                    UtilesHelper.setRowHeight(sheet, i, 900);

                    int imgSign = newWorkbook.AddPicture(obj.usuario.firmaImagen, PictureType.PNG);
                    HSSFClientAnchor signAnchor = helper.CreateClientAnchor() as HSSFClientAnchor;
                    signAnchor.Col1 = 2;
                    signAnchor.Row1 = i - 1;
                    signAnchor.AnchorType = AnchorType.MoveDontResize;


                    HSSFPicture picSign = drawing.CreatePicture(signAnchor, imgSign) as HSSFPicture;
                    picSign.Resize(1);
                    i++;

                    UtilesHelper.combinarCeldas(sheet, i, i, "A", "D");
                    UtilesHelper.setValorCelda(sheet, i, "A", obj.usuario.nombre + " - " + obj.usuario.cargo, boldTextCenterCellStyle);

                    i = i + 2;
                } else
                {
                    i = i + 3;
                }

                UtilesHelper.combinarCeldas(sheet, i, i, "A", "H");
                UtilesHelper.setValorCelda(sheet, i, "A", "Si usted tiene alguna pregunta sobre esta orden de compra, por favor, póngase en contacto con", footerTextCellStyle);

                i++;
                UtilesHelper.combinarCeldas(sheet, i, i, "A", "H");
                UtilesHelper.setValorCelda(sheet, i, "A", obj.usuario.nombre + " | " + obj.usuario.contacto + " | E-Mail: " + obj.usuario.email, footerTextCellStyle);
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

                    result.FileDownloadName = "OCC_" + obj.numeroOrdenCompraClienteString + " .xls";

                    return result;
                }
            }
        }

        protected void definirListaUnidades(HSSFSheet sheet, int row, int col)
        {
            var markConstraint = DVConstraint.CreateExplicitListConstraint(new string[] { "MP", "Alternativa", "Proveedor" });
            var markColumn = new CellRangeAddressList(row, row, col, col);
            var markdv = new HSSFDataValidation(markColumn, markConstraint);
            markdv.EmptyCellAllowed = true;
            markdv.CreateErrorBox("Valor Incorrecto", "Por favor seleccione un tipo de unidad de la lista");
            sheet.AddValidationData(markdv);
        }
    }
}