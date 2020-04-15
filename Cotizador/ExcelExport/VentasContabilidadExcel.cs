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


using BusinessLayer;
using Model.ServiceReferencePSE;
using System.IO.Compression;

using NPOI.HSSF.UserModel;
using Cotizador.ExcelExport;
using NPOI.HSSF.Model;



namespace Cotizador.ExcelExport
{
    public class VentasContabilidadExcel
    {
        string MesSiguiente(string mes)
        {
            string mesSiguiente = "";
            switch (mes)
            {
                case "ENERO": mesSiguiente = "FEBRERO"; break;
                case "FEBRERO": mesSiguiente = "MARZO"; break;
                case "MARZO": mesSiguiente = "ABRIL"; break;
                case "ABRIL": mesSiguiente = "MAYO"; break;
                case "MAYO": mesSiguiente = "JUNIO"; break;
                case "JUNIO": mesSiguiente = "JULIO"; break;
                case "JULIO": mesSiguiente = "AGOSTO"; break;
                case "AGOSTO": mesSiguiente = "SEPTIEMBRE"; break;
                case "SEPTIEMBRE": mesSiguiente = "OCTUBRE"; break;
                case "OCTUBRE": mesSiguiente = "NOVIEMBRE"; break;
                case "NOVIEMBRE": mesSiguiente = "DICIEMBRE"; break;
                case "DICIEMBRE": mesSiguiente = "ENERO"; break;
            }
            return mesSiguiente;
        }

        public FileStreamResult generateExcel(List<List<CPE_CABECERA_BE>> ListExcel, string mes)
        {
            List<CPE_CABECERA_BE> sheet1 = new List<CPE_CABECERA_BE>(ListExcel[0]);
            List<CPE_CABECERA_BE> sheet2 = new List<CPE_CABECERA_BE>(ListExcel[1]);
            List<CPE_CABECERA_BE> sheet3 = new List<CPE_CABECERA_BE>(ListExcel[2]);
            List<CPE_CABECERA_BE> sheet4 = new List<CPE_CABECERA_BE>(ListExcel[3]);
            List<CPE_CABECERA_BE> sheet5 = new List<CPE_CABECERA_BE>(ListExcel[4]);

            CPE_CABECERA_BE cabezera = new CPE_CABECERA_BE();
            sheet1.Add(cabezera);
            sheet2.Add(cabezera);
            sheet3.Add(cabezera);
            sheet4.Add(cabezera);
            sheet5.Add(cabezera);

            HSSFWorkbook wb;
            HSSFSheet shMontosVenta;
            HSSFSheet shCpes;
            HSSFSheet shVentasMesFacturadas;
            HSSFSheet shVentaNoFacturadaMes;
            HSSFSheet shVentasAntFacturadasMes;
            HSSFSheet shVentasExtornadasMes;

            // create xls if not exists
            //  if (!File.Exists("test.xls"))

            wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());
            HSSFCellStyle defaulCellStyle = (HSSFCellStyle)wb.CreateCellStyle();

            // create sheet
            
            shMontosVenta = (HSSFSheet)wb.CreateSheet("Resumen Montos Ventas");
            shCpes = (HSSFSheet)wb.CreateSheet("CPEs");
            shVentasMesFacturadas = (HSSFSheet)wb.CreateSheet("Vts " + mes.Substring(0, 3) + " Facturadas " + MesSiguiente(mes).Substring(0, 3) + "");
            shVentaNoFacturadaMes = (HSSFSheet)wb.CreateSheet("Vts " + mes.Substring(0, 3) + " aun no facturada");
            shVentasAntFacturadasMes = (HSSFSheet)wb.CreateSheet("Vtas ant facturadas " + mes.Substring(0, 3) + "");
            shVentasExtornadasMes = (HSSFSheet)wb.CreateSheet("Ventas Extornadas en " + mes.Substring(0, 3) + "");


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

            for (int r = 0; r < 8; r++)
                    {                        
                        var row = shMontosVenta.CreateRow(r);
                        for (int c = 0; c < 7; c++)
                        {
                            row.CreateCell(c);
                        }
                    }
            UtilesHelper.setValorCelda(shMontosVenta, 1, "A", "", titleDataCellStyle);
            UtilesHelper.setValorCelda(shMontosVenta, 1, "B", "SUB TOTAL GRABADA", titleDataCellStyle);
            UtilesHelper.setValorCelda(shMontosVenta, 1, "C", "SUB TOTAL INAFECTA", titleDataCellStyle);
            UtilesHelper.setValorCelda(shMontosVenta, 1, "D", "SUB TOTAL EXONERADA", titleDataCellStyle);
            UtilesHelper.setValorCelda(shMontosVenta, 1, "E", "SUB TOTAL GRATUITA", titleDataCellStyle);
            
            UtilesHelper.setValorCelda(shMontosVenta, 2, "A", "COMPROBANTES DE PAGO ELECTRONICOS EMITIDOS EN "+mes+" ", PrincipalCellStyle);
            UtilesHelper.setValorCelda(shMontosVenta, 3, "A", "VENTAS DE " + mes + " FACTURADAS EN " + MesSiguiente(mes)+".");
            UtilesHelper.setValorCelda(shMontosVenta, 4, "A", "VENTAS DE " + mes + " AUN NO FACTURADAS");
            UtilesHelper.setValorCelda(shMontosVenta, 5, "A", "VENTAS ANTERIORES A " + mes + " Y FACTURADAS EN " + mes + "");
            UtilesHelper.setValorCelda(shMontosVenta, 6, "A", "VENTAS EXTORNADAS EN " + mes + "");


            UtilesHelper.setColumnWidth(shMontosVenta, "A", 16200);
            UtilesHelper.setColumnWidth(shMontosVenta, "B", 6000);
            UtilesHelper.setColumnWidth(shMontosVenta, "C", 6000);
            UtilesHelper.setColumnWidth(shMontosVenta, "D", 7000);
            UtilesHelper.setColumnWidth(shMontosVenta, "E", 7000);
          
            // 5 rows, 8 columns
            for (int r = 0; r < sheet1.Count; r++)
            {
                                var row = shCpes.CreateRow(r);
                                for (int c = 0; c < 9; c++)
                                {
                                    row.CreateCell(c);
                                }

            }
            UtilesHelper.setValorCelda(shCpes, 1, "A", "CPE", titleDataCellStyle);
            UtilesHelper.setValorCelda(shCpes, 1, "B", "FECHA_EMISION_CPE", titleDataCellStyle);
            UtilesHelper.setValorCelda(shCpes, 1, "C", "SUB_TOTAL_GRABADA", titleDataCellStyle);
            UtilesHelper.setValorCelda(shCpes, 1, "D", "SUB_TOTAL_INAFECTA", titleDataCellStyle);
            UtilesHelper.setValorCelda(shCpes, 1, "E", "SUB_TOTAL_EXONERADA", titleDataCellStyle);
            UtilesHelper.setValorCelda(shCpes, 1, "F", "SUB_TOTAL_GRATUITA", titleDataCellStyle);
            UtilesHelper.setValorCelda(shCpes, 1, "G", "IGV_CPE", titleDataCellStyle);
            UtilesHelper.setValorCelda(shCpes, 1, "H", "TOTAL_CPE", titleDataCellStyle);

            UtilesHelper.setColumnWidth(shCpes, "A", 6000);
            UtilesHelper.setColumnWidth(shCpes, "B", 6000);
            UtilesHelper.setColumnWidth(shCpes, "C", 6000);
            UtilesHelper.setColumnWidth(shCpes, "D", 6000);
            UtilesHelper.setColumnWidth(shCpes, "E", 6000);
            UtilesHelper.setColumnWidth(shCpes, "F", 6000);
            UtilesHelper.setColumnWidth(shCpes, "G", 6000);
            UtilesHelper.setColumnWidth(shCpes, "H", 6000);
           
            for (int r = 0; r < ListExcel[0].Count; r++)
            {
                
                UtilesHelper.setValorCelda(shCpes, r + 2, "A", ListExcel[0][r].SERIE);
                UtilesHelper.setValorCelda(shCpes, r + 2, "B", ListExcel[0][r].FEC_EMI);
                UtilesHelper.setValorCelda(shCpes, r + 2, "C", Double.Parse(ListExcel[0][r].MNT_TOT_GRV));
                UtilesHelper.setValorCelda(shCpes, r + 2, "D", Double.Parse(ListExcel[0][r].MNT_TOT_INF));
                UtilesHelper.setValorCelda(shCpes, r + 2, "E", Double.Parse(ListExcel[0][r].MNT_TOT_EXR));
                UtilesHelper.setValorCelda(shCpes, r + 2, "F", Double.Parse(ListExcel[0][r].MNT_TOT_GRT));
                UtilesHelper.setValorCelda(shCpes, r + 2, "G", Double.Parse(ListExcel[0][r].MNT_TOT_TRB_IGV));
                UtilesHelper.setValorCelda(shCpes, r + 2, "H", Double.Parse(ListExcel[0][r].MNT_TOT));
            }

                    for (int r = 0; r < sheet2.Count; r++)
                    {
                        var row = shVentasMesFacturadas.CreateRow(r);
                        for (int c = 0; c < 16; c++)
                        {
                            row.CreateCell(c);
                        }
                    }
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "A", "GUIA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "B", "FECHA_EMISION_GUIA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "C", "CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "D", "FECHA_EMISION_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "E", "SUB_TOTAL_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "F", "IGV_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "G", "TOTAL_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "H", "TIPO_TRANSACCION", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "I", "MOTIVO_TRANSACCION", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "J", "FECHA_TRANSACCION", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "K", "SUB_TOTAL_VENTA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "L", "CLIENTE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "M", "DOCUMENTO", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "N", "CODIGO", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "O", "GUIA_EXTORNADA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasMesFacturadas, 1, "P", "FECHA_EMISION_GUIA_EXTORNADA", titleDataCellStyle);

            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "A", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "B", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "C", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "D", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "E", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "F", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "G", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "H", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "I", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "J", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "K", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "L", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "M", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "N", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "O", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "P", 6000);
            UtilesHelper.setColumnWidth(shVentasMesFacturadas, "Q", 6000);

            for (int r = 0; r < ListExcel[1].Count; r++)
            {
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "A", ListExcel[1][r].CORRELATIVO);
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "B", ListExcel[1][r].FEC_EMI);
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "C", ListExcel[1][r].SERIE);
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "D", ListExcel[1][r].FEC_VCTO);
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "E", Double.Parse(ListExcel[1][r].MNT_TOT_ANTCP));
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "F", Double.Parse(ListExcel[1][r].MNT_TOT_TRB_IGV));
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "G", Double.Parse(ListExcel[1][r].MNT_TOT));
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "H", ListExcel[1][r].COD_TIP_OPE);
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "I", ListExcel[1][r].COD_OPC);
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "J", ListExcel[1][r].DES_REF_CLT);
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "K", Double.Parse(ListExcel[1][r].MNT_REF));
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "L", ListExcel[1][r].DIR_DES_ENT);
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "M", ListExcel[1][r].NRO_DOC_EMI);
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "N", ListExcel[1][r].COD_GPO);
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "O", ListExcel[1][r].ID_EXT_RZN);
                        UtilesHelper.setValorCelda(shVentasMesFacturadas, r + 2, "P", ListExcel[1][r].MNT_TOT_EXP);
            }

                    for (int r = 0; r < sheet3.Count; r++)
                    {
                        var row = shVentaNoFacturadaMes.CreateRow(r);
                        for (int c = 0; c < 16; c++)
                        {
                            row.CreateCell(c);
                        }
                    }
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "A", "GUIA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "B", "FECHA_EMISION_GUIA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "C", "CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "D", "FECHA_EMISION_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "E", "SUB_TOTAL_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "F", "IGV_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "G", "TOTAL_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "H", "TIPO_TRANSACCION", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "I", "MOTIVO_TRANSACCION", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "J", "FECHA_TRANSACCION", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "K", "SUB_TOTAL_VENTA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "L", "CLIENTE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "M", "DOCUMENTO", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "N", "CODIGO", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "O", "GUIA_EXTORNADA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, 1, "P", "FECHA_EMISION_GUIA_EXTORNADA", titleDataCellStyle);

            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "A", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "B", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "C", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "D", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "E", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "F", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "G", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "H", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "I", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "J", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "K", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "L", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "M", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "N", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "O", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "P", 6000);
            UtilesHelper.setColumnWidth(shVentaNoFacturadaMes, "Q", 6000);

            for (int r = 0; r < ListExcel[2].Count; r++)

            {
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "A", ListExcel[2][r].CORRELATIVO);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "B", ListExcel[2][r].FEC_EMI);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "C", ListExcel[2][r].SERIE);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "D", ListExcel[2][r].FEC_VCTO);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "E", ListExcel[2][r].MNT_TOT_ANTCP == null ? 0:Double.Parse(sheet3[r].MNT_TOT_ANTCP));
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "F", ListExcel[2][r].MNT_TOT_TRB_IGV==null ? 0:Double.Parse(sheet3[r].MNT_TOT_TRB_IGV));
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "G", ListExcel[2][r].MNT_TOT== null ? 0:Double.Parse(sheet3[r].MNT_TOT));
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "H", ListExcel[2][r].COD_TIP_OPE);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "I", ListExcel[2][r].COD_OPC);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "J", ListExcel[2][r].DES_REF_CLT);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "K", ListExcel[2][r].MNT_REF == null ? 0:Double.Parse(sheet3[r].MNT_REF));
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "L", ListExcel[2][r].DIR_DES_ENT);            
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "M", ListExcel[2][r].NRO_DOC_EMI);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "N", ListExcel[2][r].COD_GPO);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "O", ListExcel[2][r].ID_EXT_RZN);
                    UtilesHelper.setValorCelda(shVentaNoFacturadaMes, r + 2, "P", ListExcel[2][r].MNT_TOT_EXP);
            }                 
                    for (int r = 0; r < sheet4.Count; r++)
                    {
                        var row = shVentasAntFacturadasMes.CreateRow(r);
                        for (int c = 0; c < 16; c++)
                        {
                            row.CreateCell(c);
                        }
                    }
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "A", "GUIA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "B", "FECHA_EMISION_GUIA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "C", "CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "D", "FECHA_EMISION_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "E", "SUB_TOTAL_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "F", "IGV_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "G", "TOTAL_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "H", "TIPO_TRANSACCION", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "I", "MOTIVO_TRANSACCION", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "J", "FECHA_TRANSACCION", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "K", "SUB_TOTAL_VENTA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "L", "CLIENTE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "M", "DOCUMENTO", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "N", "CODIGO", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "O", "GUIA_EXTORNADA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasAntFacturadasMes, 1, "P", "FECHA_EMISION_GUIA_EXTORNADA", titleDataCellStyle);


            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "A", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "B", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "C", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "D", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "E", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "F", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "G", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "H", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "I", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "J", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "K", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "L", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "M", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "N", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "O", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "P", 6000);
            UtilesHelper.setColumnWidth(shVentasAntFacturadasMes, "Q", 6000);

            for (int r = 0; r < ListExcel[3].Count; r++)
                    {
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "A", ListExcel[3][r].CORRELATIVO);
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "B", ListExcel[3][r].FEC_EMI);
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "C", ListExcel[3][r].SERIE);
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "D", ListExcel[3][r].FEC_VCTO);
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "E", Double.Parse(ListExcel[3][r].MNT_TOT_ANTCP));
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "F", Double.Parse(ListExcel[3][r].MNT_TOT_TRB_IGV));
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "G", Double.Parse(ListExcel[3][r].MNT_TOT));
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "H", ListExcel[3][r].COD_TIP_OPE);
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "I", ListExcel[3][r].COD_OPC);
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "J", ListExcel[3][r].DES_REF_CLT);
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "K", Double.Parse(ListExcel[3][r].MNT_REF));
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "L", ListExcel[3][r].DIR_DES_ENT);
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "M", ListExcel[3][r].NRO_DOC_EMI);
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "N", ListExcel[3][r].COD_GPO);
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "O", ListExcel[3][r].ID_EXT_RZN);
                        UtilesHelper.setValorCelda(shVentasAntFacturadasMes, r + 2, "P", ListExcel[3][r].MNT_TOT_EXP);
            }

                    for (int r = 0; r < sheet5.Count; r++)
                    {
                        var row = shVentasExtornadasMes.CreateRow(r);
                        for (int c = 0; c < 16; c++)
                        {
                            row.CreateCell(c);
                        }
                    }
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "A", "GUIA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "B", "FECHA_EMISION_GUIA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "C", "CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "D", "FECHA_EMISION_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "E", "SUB_TOTAL_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "F", "IGV_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "G", "TOTAL_CPE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "H", "TIPO_TRANSACCION", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "I", "MOTIVO_TRANSACCION", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "J", "FECHA_TRANSACCION", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "K", "SUB_TOTAL_VENTA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "L", "CLIENTE", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "M", "DOCUMENTO", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "N", "CODIGO", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "O", "GUIA_EXTORNADA", titleDataCellStyle);
                    UtilesHelper.setValorCelda(shVentasExtornadasMes, 1, "P", "FECHA_EMISION_GUIA_EXTORNADA", titleDataCellStyle);


            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "A", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "B", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "C", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "D", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "E", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "F", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "G", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "H", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "I", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "J", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "K", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "L", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "M", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "N", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "O", 6000);
            UtilesHelper.setColumnWidth(shVentasExtornadasMes, "P", 6000);
      

            for (int r = 0; r < ListExcel[4].Count; r++)
                    {
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "A", ListExcel[4][r].CORRELATIVO);
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "B", ListExcel[4][r].FEC_EMI);
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "C", ListExcel[4][r].SERIE);
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "D", ListExcel[4][r].FEC_VCTO);
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "E", ListExcel[4][r].MNT_TOT_ANTCP==null ?0:Double.Parse(sheet5[r].MNT_TOT_ANTCP));
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "F", ListExcel[4][r].MNT_TOT_TRB_IGV==null ? 0:Double.Parse(sheet5[r].MNT_TOT_TRB_IGV));
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "G", ListExcel[4][r].MNT_TOT==null ? 0:Double.Parse(sheet5[r].MNT_TOT));
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "H", ListExcel[4][r].COD_TIP_OPE);
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "I", ListExcel[4][r].COD_OPC);
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "J", ListExcel[4][r].DES_REF_CLT);
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "K", ListExcel[4][r].MNT_REF==null ?0:Double.Parse(sheet5[r].MNT_REF));
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "L", ListExcel[4][r].DIR_DES_ENT);
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "M", ListExcel[4][r].NRO_DOC_EMI);
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "N", ListExcel[4][r].COD_GPO);
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "O", ListExcel[4][r].ID_EXT_RZN);
                        UtilesHelper.setValorCelda(shVentasExtornadasMes, r + 2, "P", ListExcel[4][r].MNT_TOT_EXP);
            }           

            UtilesHelper.setFormulaCelda(shMontosVenta, 2, "B", "SUM('CPEs'!C2:C"+ sheet1.Count.ToString() + ")", PrincipalCellStyle);
            UtilesHelper.setFormulaCelda(shMontosVenta, 3, "B", "SUM('Vts " + mes.Substring(0, 3) + " Facturadas " + MesSiguiente(mes).Substring(0, 3) + "'!K2:K"+ sheet2.Count.ToString() + ")");
            UtilesHelper.setFormulaCelda(shMontosVenta, 4, "B", "SUM('Vts " + mes.Substring(0, 3) + " aun no facturada'!K2:K"+ sheet3.Count.ToString() + ")");
            UtilesHelper.setFormulaCelda(shMontosVenta, 5, "B", "-SUM('Vtas ant facturadas " + mes.Substring(0, 3) + "'!K2:K"+ sheet4.Count.ToString() + ")");
            UtilesHelper.setFormulaCelda(shMontosVenta, 6, "B", "-SUM('Ventas Extornadas en " + mes.Substring(0, 3) + "'!K2:K"+ sheet5.Count.ToString() + ")");
            UtilesHelper.setFormulaCelda(shMontosVenta, 7, "B", "SUM(B2:B6)");
            
            UtilesHelper.setFormulaCelda(shMontosVenta, 2, "C", "SUM(CPEs!D2:D" + sheet1.Count.ToString() + ")", PrincipalCellStyle);            
            UtilesHelper.setFormulaCelda(shMontosVenta, 7, "C", "SUM(C2:C5)");

            UtilesHelper.setFormulaCelda(shMontosVenta, 2, "D", "SUM(CPEs!E2:E" + sheet1.Count.ToString() + ")", PrincipalCellStyle);           
            UtilesHelper.setFormulaCelda(shMontosVenta, 7, "D", "SUM(D2:D5)");

            UtilesHelper.setFormulaCelda(shMontosVenta, 2, "E", "SUM(CPEs!F2:F" + sheet1.Count.ToString() + ")", PrincipalCellStyle);         
            UtilesHelper.setFormulaCelda(shMontosVenta, 7, "E", "SUM(E2:E5)");
            
            MemoryStream ms = new MemoryStream();
                    using (MemoryStream tempStream = new MemoryStream())
                    {
                        wb.Write(tempStream);
                        var byteArray = tempStream.ToArray();
                        ms.Write(byteArray, 0, byteArray.Length);
                        ms.Flush();
                        ms.Position = 0;
                        FileStreamResult result = new FileStreamResult(ms, "application/vnd.ms-excel");

                        result.FileDownloadName = "Reporte de Ventas Contabilidad " + mes + ".xls";

                        return result;
                    }


                
            }
    }

}

