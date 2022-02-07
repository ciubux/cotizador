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
using NPOI.SS.Util;
using BusinessLayer;

using System.Web.Mvc;


namespace Cotizador.ExcelExport
{
    public class CotizacionDetalleExcel
    {
        protected int cantFilasRegistrosAdicionales = 200;
        public static int filaInicioDatos { get { return 11; } }

        public FileStreamResult generateExcel(Cotizacion obj)
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


                HSSFCellStyle formDataCenterCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                formDataCenterCellStyle.Alignment = HorizontalAlignment.Center;
                formDataCenterCellStyle.BorderLeft = BorderStyle.Thin;
                formDataCenterCellStyle.BorderTop = BorderStyle.Thin;
                formDataCenterCellStyle.BorderRight = BorderStyle.Thin;
                formDataCenterCellStyle.BorderBottom = BorderStyle.Thin;

                HSSFCellStyle lastDataCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                lastDataCellStyle.BorderTop = BorderStyle.Thin;
                lastDataCellStyle.FillPattern = FillPattern.SolidForeground;
                lastDataCellStyle.FillForegroundColor = HSSFColor.White.Index;


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
                titleDataCellStyle.WrapText = true;
                titleDataCellStyle.VerticalAlignment = VerticalAlignment.Center;
                titleDataCellStyle.BorderLeft = BorderStyle.Thin;
                titleDataCellStyle.BorderTop = BorderStyle.Thin;
                titleDataCellStyle.BorderRight = BorderStyle.Thin;

                HSSFCellStyle ocTitleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                HSSFFont ocTitleFont = (HSSFFont)wb.CreateFont();
                ocTitleFont.FontHeightInPoints = (short)28;
                ocTitleFont.FontName = "Arial";
                ocTitleFont.Color = IndexedColors.RoyalBlue.Index;
                ocTitleFont.IsBold = true;
                ocTitleCellStyle.SetFont(ocTitleFont);
                ocTitleCellStyle.Alignment = HorizontalAlignment.Center;
                ocTitleCellStyle.VerticalAlignment = VerticalAlignment.Center;

                HSSFCellStyle footerTextCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                footerTextCellStyle.Alignment = HorizontalAlignment.Center;

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
                twoDecCellStyle.Indention = 1;


                HSSFCellStyle twoDecPercentageCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecCellStyle);
                twoDecPercentageCellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                twoDecPercentageCellStyle.Alignment = HorizontalAlignment.Right;

                HSSFCellStyle totalsCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsCellStyle.DataFormat = twoDecFormat;
                totalsCellStyle.WrapText = true;
                totalsCellStyle.VerticalAlignment = VerticalAlignment.Center;
                totalsCellStyle.BorderLeft = BorderStyle.Thin;
                totalsCellStyle.BorderRight = BorderStyle.Thin;
                totalsCellStyle.BorderTop = BorderStyle.Thin;
                totalsCellStyle.BorderBottom = BorderStyle.Thin;
                totalsCellStyle.Indention = 1;


                HSSFCellStyle totalsTotalCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsTotalCellStyle.CloneStyleFrom(totalsCellStyle);
                totalsTotalCellStyle.BorderTop = BorderStyle.Double;
                totalsTotalCellStyle.VerticalAlignment = VerticalAlignment.Center;
                HSSFFont totalFont = (HSSFFont)wb.CreateFont();
                totalFont.FontHeightInPoints = (short)11;
                totalFont.FontName = "Arial";
                totalFont.IsBold = true;
                totalsTotalCellStyle.SetFont(totalFont);


                HSSFCellStyle totalsLabelCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsLabelCellStyle.BorderRight = BorderStyle.Thin;
                totalsLabelCellStyle.BorderLeft = BorderStyle.Thin;
                totalsLabelCellStyle.BorderBottom = BorderStyle.Thin;
                totalsLabelCellStyle.BorderTop = BorderStyle.Thin;
                totalsLabelCellStyle.VerticalAlignment = VerticalAlignment.Center;
                totalsLabelCellStyle.Alignment = HorizontalAlignment.Right;
                totalsLabelCellStyle.Indention = 1;

                HSSFCellStyle totalsTotalLabelCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                totalsTotalLabelCellStyle.CloneStyleFrom(totalsLabelCellStyle);
                totalsTotalLabelCellStyle.SetFont(totalFont);
                totalsTotalLabelCellStyle.BorderTop = BorderStyle.Double;
                totalsTotalLabelCellStyle.VerticalAlignment = VerticalAlignment.Center;
                totalsTotalLabelCellStyle.Alignment = HorizontalAlignment.Right;

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

                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("COT-" + obj.codigo);


                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (obj.cotizacionDetalleList.Count) + cantFilasRegistrosAdicionales;
                int cTotal = 24;

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c).CellStyle = defaulCellStyle;
                    }
                }

                UtilesHelper.combinarCeldas(sheet, 1, 1, "A", "D");

                IWorkbook newWorkbook = wb;

                byte[] data = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\images\\logos\\logo_" + obj.usuario.codigoEmpresa + ".png");
                int picInd = newWorkbook.AddPicture(data, PictureType.PNG);
                HSSFCreationHelper helper = newWorkbook.GetCreationHelper() as HSSFCreationHelper;
                IDrawing drawing = sheet.CreateDrawingPatriarch();
                HSSFClientAnchor anchor = helper.CreateClientAnchor() as HSSFClientAnchor;
                anchor.Col1 = 0;
                anchor.Row1 = 0;
                anchor.Col2 = 4;

                anchor.AnchorType = AnchorType.DontMoveAndResize;

                HSSFPicture pict = drawing.CreatePicture(anchor, picInd) as HSSFPicture;
                pict.Resize(1);

                UtilesHelper.combinarCeldas(sheet, 1, 1, "M", "N");
                UtilesHelper.setValorCelda(sheet, 1, "M", "COTIZACIÓN", ocTitleCellStyle);

                UtilesHelper.setRowHeight(sheet, 1, 1300);


                if (obj.cliente == null || obj.cliente.idCliente == Guid.Empty)
                {
                    UtilesHelper.setValorCelda(sheet, 2, "B", "Grupo:", formLabelCellStyle);
                    UtilesHelper.setValorCelda(sheet, 2, "C", obj.grupo.codigoNombre);
                }
                else
                {
                    UtilesHelper.setValorCelda(sheet, 2, "B", "Cliente:", formLabelCellStyle);
                    UtilesHelper.setValorCelda(sheet, 2, "C", obj.cliente.codigoRazonSocial);
                }

                UtilesHelper.setValorCelda(sheet, 3, "B", "Contacto:", formLabelCellStyle);
                UtilesHelper.setValorCelda(sheet, 3, "C", obj.contacto);

                UtilesHelper.setValorCelda(sheet, 4, "B", "Validez Oferta:", formLabelCellStyle);
                UtilesHelper.setValorCelda(sheet, 4, "C", obj.fechaLimiteValidezOferta.ToString("dd/MM/yyyy"));

                UtilesHelper.setValorCelda(sheet, 5, "B", "Tipo:", formLabelCellStyle);
                switch (obj.tipoCotizacion)
                {
                    case Cotizacion.TiposCotizacion.Normal: UtilesHelper.setValorCelda(sheet, 5, "C", "Normal"); break;
                    case Cotizacion.TiposCotizacion.Transitoria: UtilesHelper.setValorCelda(sheet, 5, "C", "Transitoria"); break;
                    case Cotizacion.TiposCotizacion.Trivial: UtilesHelper.setValorCelda(sheet, 5, "C", "Trivial"); break;
                }

                if (obj.tipoCotizacion != Cotizacion.TiposCotizacion.Trivial)
                    UtilesHelper.setValorCelda(sheet, 6, "B", "Vigencia:", formLabelCellStyle);

                string fechaVigencia = "";
                if (obj.fechaInicioVigenciaPrecios == null)
                {
                    fechaVigencia = "No definida";
                }
                else
                {
                    fechaVigencia = obj.fechaInicioVigenciaPrecios.Value.ToString("dd/MM/yyyy");
                }

                fechaVigencia = fechaVigencia + " - ";
                if (obj.fechaFinVigenciaPrecios == null)
                {
                    fechaVigencia = fechaVigencia + "No definida";
                }
                else
                {
                    fechaVigencia = fechaVigencia + obj.fechaFinVigenciaPrecios.Value.ToString("dd/MM/yyyy");
                }

                UtilesHelper.setValorCelda(sheet, 6, "C", fechaVigencia);



                UtilesHelper.setValorCelda(sheet, 2, "M", "Número:", formLabelCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "N", obj.numeroCotizacionString, formDataCenterCellStyle);

                UtilesHelper.setValorCelda(sheet, 3, "M", "Sede:", formLabelCellStyle);
                UtilesHelper.setValorCelda(sheet, 3, "N", obj.ciudad.nombre, formDataCenterCellStyle);


                int i = filaInicioDatos - 1; 

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "A", "PROV.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "SKU PROV", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "*SKU", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "IMG.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "E", "DESCRIPCIÓN", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "*UNIDAD", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "G", "*CANT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "P. LISTA", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", "% DSCTO.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", "*P. NETO", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "K", "*FLETE", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "L", "P. UNIT.", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "M", "TOTAL", titleDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "N", "*OBSERVACIONES", titleDataCellStyle);
                if (obj.usuarioBusqueda.visualizaCostos)
                {
                    UtilesHelper.setValorCelda(sheet, i, "O", "COSTO", titleDataCellStyle);
                }
                if (obj.usuarioBusqueda.visualizaMargen)
                {
                    UtilesHelper.setValorCelda(sheet, i, "P", "% MARGEN", titleDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "Q", "Precio Neto Ant.", titleDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "R", "Var % Precio Neto", titleDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "S", "Var % Precio Lista", titleDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "T", "Var % Costo", titleDataCellStyle);
                }

                UtilesHelper.setColumnWidth(sheet, "A", 2300);
                UtilesHelper.setColumnWidth(sheet, "B", 3500);
                UtilesHelper.setColumnWidth(sheet, "C", 2500);
                UtilesHelper.setColumnWidth(sheet, "D", 1800);
                UtilesHelper.setColumnWidth(sheet, "E", 12000);
                UtilesHelper.setColumnWidth(sheet, "F", 8000);
                UtilesHelper.setColumnWidth(sheet, "G", 2000);

                UtilesHelper.setColumnWidth(sheet, "H", 3000);
                UtilesHelper.setColumnWidth(sheet, "I", 3000);
                UtilesHelper.setColumnWidth(sheet, "J", 3000);
                UtilesHelper.setColumnWidth(sheet, "K", 3000);
                UtilesHelper.setColumnWidth(sheet, "L", 3500);
                UtilesHelper.setColumnWidth(sheet, "M", 3500);

                UtilesHelper.setColumnWidth(sheet, "N", 8000);

                if (obj.usuarioBusqueda.visualizaCostos)
                {
                    UtilesHelper.setColumnWidth(sheet, "O", 3000);
                }
                if (obj.usuarioBusqueda.visualizaMargen)
                {
                    UtilesHelper.setColumnWidth(sheet, "P", 3500);
                    UtilesHelper.setColumnWidth(sheet, "Q", 3500);
                    UtilesHelper.setColumnWidth(sheet, "R", 3500);
                    UtilesHelper.setColumnWidth(sheet, "S", 3500);
                    UtilesHelper.setColumnWidth(sheet, "T", 3000);
                }


                i = filaInicioDatos;

                HSSFCellStyle tableDataCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataCellStyle);
                HSSFCellStyle tableDataLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, tableDataLastCellStyle);

                HSSFCellStyle twoDecCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecCellStyle);
                HSSFCellStyle twoDecLastCenterCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithHCenter(wb, twoDecLastCellStyle);

                HSSFCellStyle twoDecIdentCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithIndent(wb, twoDecCellStyle, 1);
                HSSFCellStyle twoDecIdentLastCellStyle = (HSSFCellStyle)UtilesHelper.GetCloneStyleWithIndent(wb, twoDecLastCellStyle, 1);

                /*  for (int iii = 0; iii<50;iii++)
                  { */

                foreach (CotizacionDetalle det in obj.cotizacionDetalleList)
                {
                    UtilesHelper.setRowHeight(sheet, i, 810);

                    int picDet = newWorkbook.AddPicture(det.producto.image, PictureType.PNG);
                    HSSFClientAnchor detAnchor = helper.CreateClientAnchor() as HSSFClientAnchor;
                    detAnchor.Col1 = 3;
                    detAnchor.Row1 = i - 1;
                    detAnchor.AnchorType = AnchorType.DontMoveAndResize;

                    HSSFPicture pictdet = drawing.CreatePicture(detAnchor, picDet) as HSSFPicture;
                    pictdet.Resize(1);

                    definirListaUnidades(sheet, obj, det, i - 1, 5);

                    String unidadDesc = "";

                    if (det.ProductoPresentacion == null)
                    {
                        unidadDesc = "MP - " + det.unidad;
                    } else
                    {
                        switch(det.ProductoPresentacion.IdProductoPresentacion)
                        {
                            case 1: unidadDesc = "Alternativa - " + det.unidad; break;
                            case 2: unidadDesc = "Proveedor - " + det.unidad; break;
                        }
                    }


                    UtilesHelper.setValorCelda(sheet, i, "A", det.producto.proveedor, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "B", det.producto.skuProveedor, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "C", det.producto.sku, tableDataCenterCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "E", det.producto.descripcion, tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "F", unidadDesc, tableDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "G", det.cantidad, tableDataCenterCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "H", (double)det.precioLista, twoDecCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "I", (double)(det.porcentajeDescuento/100), twoDecPercentageCellStyle);
                    UtilesHelper.setFormulaCelda(sheet, i, "J", "H" + i.ToString() + "*(1-I" + i.ToString() + ")", twoDecCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "K", (double)det.flete, twoDecCellStyle);
                    UtilesHelper.setFormulaCelda(sheet, i, "L", "J" + i.ToString() + "+K" + i.ToString() + "", twoDecCellStyle);
                    UtilesHelper.setFormulaCelda(sheet, i, "M", "L" + i.ToString() + "*G" + i.ToString() + "", twoDecCellStyle);

                    UtilesHelper.setValorCelda(sheet, i, "N", det.observacion, tableDataCellStyle);

                    if (obj.usuarioBusqueda.visualizaCostos)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "O", (double)det.costoListaVisible, twoDecCellStyle);
                    }
                    if (obj.usuarioBusqueda.visualizaMargen)
                    {
                        UtilesHelper.setFormulaCelda(sheet, i, "P", "1 - (O" + i.ToString() + "/J" + i.ToString() + ")", twoDecPercentageCellStyle);

                        if (det.esPrecioAlternativo)
                        {
                            UtilesHelper.setValorCelda(sheet, i, "Q", (double)det.producto.precioClienteProducto.precioNetoAlternativo, twoDecCellStyle);
                        }
                        else
                        {
                            UtilesHelper.setValorCelda(sheet, i, "Q", (double)det.producto.precioClienteProducto.precioNeto, twoDecCellStyle);
                        }
                        UtilesHelper.setValorCelda(sheet, i, "R", (double)det.variacionPrecioAnterior, twoDecCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "S", (double)det.variacionPrecioListaAnterior, twoDecCellStyle);
                        UtilesHelper.setValorCelda(sheet, i, "T", (double)det.variacionCosto, twoDecCellStyle);
                    }

                    i++;
                }

                UtilesHelper.setValorCelda(sheet, i, "A", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "B", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "C", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "D", "", lastDataCellStyle);

                UtilesHelper.setValorCelda(sheet, i, "E", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "F", "", lastDataCellStyle);

                UtilesHelper.setValorCelda(sheet, i, "G", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "H", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "I", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "J", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "K", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "L", "", lastDataCellStyle);

                UtilesHelper.setValorCelda(sheet, i, "M", "", lastDataCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "N", "", lastDataCellStyle);

                marcarUnidadesNuevosRegistros(sheet, i - 1, cantFilasRegistrosAdicionales, 5);

                if (obj.usuarioBusqueda.visualizaCostos)
                {
                    UtilesHelper.setValorCelda(sheet, i, "O", "", lastDataCellStyle);
                }
                if (obj.usuarioBusqueda.visualizaMargen)
                {
                    UtilesHelper.setValorCelda(sheet, i, "P", "", lastDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "Q", "", lastDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "R", "", lastDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "S", "", lastDataCellStyle);
                    UtilesHelper.setValorCelda(sheet, i, "T", "", lastDataCellStyle);
                }

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "L", "Subtotal", totalsLabelCellStyle);
                UtilesHelper.setFormulaCelda(sheet, i, "M", "SUM(M" + filaInicioDatos.ToString() + ":M" + (i-1).ToString() + ")", twoDecCellStyle);
                //UtilesHelper.setValorCelda(sheet, i, "M", (double)obj.montoSubTotal, totalsCellStyle);
                i++;

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "L", "IGV 18%", totalsLabelCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "M", (double)obj.montoIGV, totalsCellStyle);
                i++;

                UtilesHelper.setRowHeight(sheet, i, 540);
                UtilesHelper.setValorCelda(sheet, i, "L", "TOTAL", totalsTotalLabelCellStyle);
                UtilesHelper.setValorCelda(sheet, i, "M", (double)obj.montoTotal, totalsTotalCellStyle);
                i++;


                MemoryStream ms = new MemoryStream();
                using (MemoryStream tempStream = new MemoryStream())
                {
                    wb.Write(tempStream);
                    var byteArray = tempStream.ToArray();
                    ms.Write(byteArray, 0, byteArray.Length);
                    ms.Flush();
                    ms.Position = 0;
                    FileStreamResult result = new FileStreamResult(ms, "application/vnd.ms-excel");

                    result.FileDownloadName = "COT_" + obj.codigo + " .xls";

                    return result;
                }
            }
        }

        protected void definirListaUnidades(HSSFSheet sheet, Cotizacion obj, CotizacionDetalle det, int row, int col)
        {
            List<String> unidades = new List<String>();
            ProductoBL productoBL = new ProductoBL();
            Producto prod = productoBL.getProducto(det.producto.idProducto, obj.ciudad.esProvincia, obj.incluidoIGV, obj.cliente.idCliente);

            unidades.Add("MP - " + prod.unidad);

            String nombreAlt = "";
            String nombreProv = "";

            foreach (ProductoPresentacion pres in prod.ProductoPresentacionList)
            {
                switch (pres.IdProductoPresentacion) {
                    case 1: nombreAlt = "Alternativa - " + pres.Presentacion; break;
                    case 2: nombreProv = "Proveedor - " + pres.Presentacion; break;
                }
            }

            if (!nombreAlt.Equals(""))
            {
                unidades.Add(nombreAlt);
            }

            if (!nombreProv.Equals(""))
            {
                unidades.Add(nombreProv);
            }

            var markConstraint = DVConstraint.CreateExplicitListConstraint(unidades.ToArray());
            var markColumn = new CellRangeAddressList(row, row, col, col);
            var markdv = new HSSFDataValidation(markColumn, markConstraint);
            markdv.EmptyCellAllowed = true;
            markdv.CreateErrorBox("Valor Incorrecto", "Por favor seleccione un tipo de unidad de la lista");
            sheet.AddValidationData(markdv);
        }

        protected void marcarUnidadesNuevosRegistros(HSSFSheet sheet, int rowInit, int rows, int col)
        {
            var markConstraint = DVConstraint.CreateExplicitListConstraint(new string[] { "MP", "Alternativa", "Proveedor" });
            var markColumn = new CellRangeAddressList(rowInit, rowInit + rows, col, col);
            var markdv = new HSSFDataValidation(markColumn, markConstraint);
            markdv.EmptyCellAllowed = true;
            markdv.CreateErrorBox("Valor Incorrecto", "Por favor seleccione un tipo de unidad de la lista");
            sheet.AddValidationData(markdv);
        }
    }
}