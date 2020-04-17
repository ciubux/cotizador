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
    public class GrupoClienteSearch
    {
        public FileStreamResult generateExcel(List<GrupoCliente> list)
        {
            List<GrupoCliente> total = new List<GrupoCliente>(list);
            GrupoCliente cabezera = new GrupoCliente();
            total.Add(cabezera);

            HSSFWorkbook wb;
            //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);
            HSSFSheet sheet;
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());

                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Grupos");


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

                HSSFCellStyle titleDataStaticCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleDataStaticCellStyle.CloneStyleFrom(titleDataCellStyle);
                titleDataStaticCellStyle.FillForegroundColor = HSSFColor.Grey50Percent.Index;


                HSSFCellStyle PrincipalCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                PrincipalCellStyle.FillPattern = FillPattern.SolidForeground;
                PrincipalCellStyle.FillForegroundColor = HSSFColor.Yellow.Index;

                HSSFCellStyle Principal2CellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                Principal2CellStyle.FillPattern = FillPattern.SolidForeground;
                Principal2CellStyle.FillForegroundColor = HSSFColor.SkyBlue.Index;


                /*Se crean todas las celdas*/
                for (int r = 0; r < total.Count; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < 5; c++)
                    {
                        row.CreateCell(c);
                    }

                }

                UtilesHelper.setValorCelda(sheet, 1, "A", "código", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "B", "nombre", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "C", "sede MP (Principal)", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "D", "plazo crédito aprobado", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 1, "E", "crédito Aprobado (S/)", titleCellStyle);

                UtilesHelper.setColumnWidth(sheet, "A", 4000);
                UtilesHelper.setColumnWidth(sheet, "B", 6000);
                UtilesHelper.setColumnWidth(sheet, "C", 6000);
                UtilesHelper.setColumnWidth(sheet, "D", 6000);
                UtilesHelper.setColumnWidth(sheet, "E", 6000);


                for (int r = 0; r < list.Count; r++)
                {

                    UtilesHelper.setValorCelda(sheet, r + 2, "A", list[r].codigo);
                    UtilesHelper.setValorCelda(sheet, r + 2, "B", list[r].nombre);
                    UtilesHelper.setValorCelda(sheet, r + 2, "C", list[r].ciudad.nombre);
                    UtilesHelper.setValorCelda(sheet, r + 2, "D", list[r].plazoCreditoAprobadoToString);
                    UtilesHelper.setValorCelda(sheet, r + 2, "E", (list[r].creditoAprobado).ToString("C"));
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

                    result.FileDownloadName = "Grupo_Cliente_Busqueda" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                    return result;
                }
            }
        }


        public FileStreamResult mienbrosGruposExcel(List<GrupoCliente> list)
        {            
            List<GrupoCliente> mienbros = new List<GrupoCliente>();
            List<GrupoCliente> gruposinMienbros = new List<GrupoCliente>();
            GrupoCliente obj = new GrupoCliente();
            for (int a = 0; a < list.Count; a++)
            {
                List<Cliente> mienbrosGrupo = new List<Cliente>();
                mienbrosGrupo = list[a].miembros;
                if (mienbrosGrupo[0].codigo== null && mienbrosGrupo[0].razonSocialSunat == null)
                {
                    gruposinMienbros.Add(list[a]);                    
                }
                else
                    mienbros.Add(list[a]);                    
            }
            int totalTablaMienbros = (mienbros.Count) + 1;
            int totalTablaSinMienbros = (gruposinMienbros.Count) + 1;

            HSSFWorkbook wb;
            //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);

            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());

                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");                            

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

                HSSFCellStyle titleDataStaticCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleDataStaticCellStyle.CloneStyleFrom(titleDataCellStyle);
                titleDataStaticCellStyle.FillForegroundColor = HSSFColor.Grey50Percent.Index;


                HSSFCellStyle PrincipalCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                PrincipalCellStyle.FillPattern = FillPattern.SolidForeground;
                PrincipalCellStyle.FillForegroundColor = HSSFColor.Yellow.Index;

                HSSFCellStyle Principal2CellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                Principal2CellStyle.FillPattern = FillPattern.SolidForeground;
                Principal2CellStyle.FillForegroundColor = HSSFColor.SkyBlue.Index;


                if (totalTablaMienbros != 1)
                {
                    HSSFSheet sheet;
                    // create sheet
                    sheet = (HSSFSheet)wb.CreateSheet("mienbros de grupos");

                    for (int r = 0; r < totalTablaMienbros; r++)
                    {
                        var row = sheet.CreateRow(r);
                        for (int c = 0; c < 5; c++)
                        {
                            row.CreateCell(c);
                        }

                    }

                    UtilesHelper.setValorCelda(sheet, 1, "A", "código grupo", titleCellStyle);
                    UtilesHelper.setValorCelda(sheet, 1, "B", "grupo", titleCellStyle);
                    UtilesHelper.setValorCelda(sheet, 1, "C", "código cliente", titleCellStyle);
                    UtilesHelper.setValorCelda(sheet, 1, "D", "razón social", titleCellStyle);


                    UtilesHelper.setColumnWidth(sheet, "A", 4000);
                    UtilesHelper.setColumnWidth(sheet, "B", 8000);
                    UtilesHelper.setColumnWidth(sheet, "C", 4000);
                    UtilesHelper.setColumnWidth(sheet, "D", 10000);



                    int celda = 0;
                    for (int r = 0; r < mienbros.Count; r++)
                    {
                        List<Cliente> mienbrosGrupo = new List<Cliente>();
                        mienbrosGrupo = mienbros[r].miembros;
                        for (int i = 0; i < mienbrosGrupo.Count; i++)
                        {
                            UtilesHelper.setValorCelda(sheet, celda + 2, "A", mienbros[r].codigo);
                            UtilesHelper.setValorCelda(sheet, celda + 2, "B", mienbros[r].nombre);
                            UtilesHelper.setValorCelda(sheet, celda + 2, "C", mienbrosGrupo[i].codigo);
                            UtilesHelper.setValorCelda(sheet, celda + 2, "D", mienbrosGrupo[i].razonSocialSunat);
                            celda += 1;
                        }
                    }
                }

                if (totalTablaSinMienbros != 1)
                {
                    HSSFSheet sinMienbros;
                    sinMienbros = (HSSFSheet)wb.CreateSheet("grupos sin mienbros");



                    for (int r = 0; r < totalTablaSinMienbros; r++)
                    {
                        var row = sinMienbros.CreateRow(r);
                        for (int c = 0; c < 2; c++)
                        {
                            row.CreateCell(c);
                        }

                    }

                    UtilesHelper.setValorCelda(sinMienbros, 1, "A", "código grupo", titleCellStyle);
                    UtilesHelper.setValorCelda(sinMienbros, 1, "B", "grupo", titleCellStyle);
                    UtilesHelper.setColumnWidth(sinMienbros, "A", 4000);
                    UtilesHelper.setColumnWidth(sinMienbros, "B", 8000);


                    for (int r = 0; r < gruposinMienbros.Count; r++)
                    {
                        UtilesHelper.setValorCelda(sinMienbros, r + 2, "A", gruposinMienbros[r].codigo);
                        UtilesHelper.setValorCelda(sinMienbros, r + 2, "B", gruposinMienbros[r].nombre);

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

                        result.FileDownloadName = "Grupo_Cliente_Mienbros_Busqueda" + DateTime.Now.ToString("yyyyMMddHHmmss") + " .xls";

                        return result;
                    }
                }
            }
        }
}
