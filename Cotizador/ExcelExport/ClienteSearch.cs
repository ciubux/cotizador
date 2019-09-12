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

                HSSFFont titleFontEditable = (HSSFFont)wb.CreateFont();
                titleFontEditable.FontHeightInPoints = (short)11;
                titleFontEditable.FontName = "Arial";
                titleFontEditable.Color = IndexedColors.Green.Index;
                titleFontEditable.IsBold = true;

                //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
                //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
                HSSFCellStyle titleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleCellStyle.SetFont(titleFont);
                titleCellStyle.FillPattern = FillPattern.SolidForeground;
                titleCellStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
                
                HSSFCellStyle titleCellStyleEditable = (HSSFCellStyle)wb.CreateCellStyle();
                titleCellStyleEditable.SetFont(titleFontEditable);
                titleCellStyleEditable.FillPattern = FillPattern.SolidForeground;
                titleCellStyleEditable.FillForegroundColor = HSSFColor.Grey25Percent.Index;

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
                int cTotal = 30+ 2;

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


                UtilesHelper.setValorCelda(sheet, 1, "I", "Origen", titleCellStyle);
                UtilesHelper.combinarCeldas(sheet, 1, 1, "I", "J");

                UtilesHelper.setValorCelda(sheet, 1, "U", "Canales", titleCellStyle);
                UtilesHelper.combinarCeldas(sheet, 1, 1, "U", "X");

                UtilesHelper.setValorCelda(sheet, 1, "Y", "Negociación Multiregional", titleCellStyle);
                UtilesHelper.combinarCeldas(sheet, 1, 1, "Y", "Z");

                UtilesHelper.setValorCelda(sheet, 1, "AA", "SubDistribución", titleCellStyle);
                UtilesHelper.combinarCeldas(sheet, 1, 1, "AA", "AC");


                UtilesHelper.setValorCelda(sheet, 2, "A", "Código", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "B", "Razón Social", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "C", "Nombre", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "D", "Tipo Doc.", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "E", "N° Doc.", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "F", "Sede MP", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "G", "Cod. Grupo", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "H", "Grupo", titleCellStyle);

                UtilesHelper.setValorCelda(sheet, 2, "I", "Código", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "J", "Descripción", titleCellStyle);

                UtilesHelper.setValorCelda(sheet, 2, "K", "Asesor Validado", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "L", "VE_C", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "M", "Asesor Comercial", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "N", "SUP", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "O", "Supervisor Comercial", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "P", "VE_A", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "Q", "Asistente Servicio Cliente", titleCellStyle);

                UtilesHelper.setValorCelda(sheet, 2, "R", "Plazo Crédito Aprobado", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "S", "Crédito Aprobado (S/)", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "T", "Liberación Crediticia", titleCellStyle);

                UtilesHelper.setValorCelda(sheet, 2, "U", "Lima", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "V", "Provincias", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "W", "PCP", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "X", "Multiregional", titleCellStyleEditable);
                
                UtilesHelper.setValorCelda(sheet, 2, "Y", "RUC Habilitado", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "Z", "Registra Cotizaciones", titleCellStyleEditable);
                
                UtilesHelper.setValorCelda(sheet, 2, "AA", "Es SubDistribuidor", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "AB", "Código", titleCellStyleEditable);
                UtilesHelper.setValorCelda(sheet, 2, "AC", "Categoría", titleCellStyle);

                i = 3;

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
                    UtilesHelper.setValorCelda(sheet, i, "G", obj.grupoCliente.codigo);
                    UtilesHelper.setValorCelda(sheet, i, "H", obj.grupoCliente.nombre);

                    UtilesHelper.setValorCelda(sheet, i, "I", obj.origen.codigo);
                    UtilesHelper.setValorCelda(sheet, i, "J", obj.origen.nombre);

                    if (obj.vendedoresAsignados)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "K", "SI");
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "K", "");
                    }
                    UtilesHelper.setValorCelda(sheet, i, "L", obj.responsableComercial.codigo);
                    UtilesHelper.setValorCelda(sheet, i, "M", obj.responsableComercial.descripcion);
                    UtilesHelper.setValorCelda(sheet, i, "N", obj.supervisorComercial.codigo);
                    UtilesHelper.setValorCelda(sheet, i, "O", obj.supervisorComercial.descripcion);
                    UtilesHelper.setValorCelda(sheet, i, "P", obj.asistenteServicioCliente.codigo);
                    UtilesHelper.setValorCelda(sheet, i, "Q", obj.asistenteServicioCliente.descripcion);
                    UtilesHelper.setValorCelda(sheet, i, "R", obj.tipoPagoFacturaToString);
                    UtilesHelper.setValorCelda(sheet, i, "S", (double) obj.creditoAprobado);

                    UtilesHelper.setValorCelda(sheet, i, "T", obj.tipoLiberacionCrediticiaString);


                    if (obj.perteneceCanalLima)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "U", "SI");
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "U", "");
                    }

                    if (obj.perteneceCanalProvincias)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "V", "SI");
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "V", "");
                    }

                    if (obj.perteneceCanalPCP)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "W", "SI");
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "W", "");
                    }
                    if (obj.perteneceCanalMultiregional)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "X", "SI");
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "X", "");
                    }

                    

                    if (obj.negociacionMultiregional)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "Y", "SI");
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "Y", "");
                    }

                    if (obj.sedePrincipal)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "Z", "SI");
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "Z", "");
                    }

                    

                    

                    if (obj.esSubDistribuidor)
                    {
                        UtilesHelper.setValorCelda(sheet, i, "AA", "SI");
                        UtilesHelper.setValorCelda(sheet, i, "AB", obj.subDistribuidor.codigo);
                        UtilesHelper.setValorCelda(sheet, i, "AC", obj.subDistribuidor.nombre);
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, i, "AA", "");
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