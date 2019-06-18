using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public static class UtilesHelper
    {
        // Gets the first character of a string.

        public static List<String> columnas = new List<string>{ "A", "B","C", "D", "E",
        "F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z","AA","AB","AC","AD","AE","AF","AG","AH"};

        public static String getValorCelda(ISheet sheet, int fila, string columna)
        {
            String valorCelda = String.Empty;
            if (sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))) != null)
            {
                valorCelda = sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).ToString();
            }
            
            return valorCelda;
        }


        public static void setValorCelda(ISheet sheet, int fila, int columna, string valor, ICellStyle cellStyle = null)
        {
            sheet.GetRow(fila - 1).GetCell(columna-1).SetCellValue(valor);
            if (cellStyle != null)
            { sheet.GetRow(fila - 1).GetCell(columna - 1).CellStyle = cellStyle; }
        }


        public static void combinarCeldas(ISheet sheet, int filaInicio, int filaFin, string columnaInicio, string columnaFin)
        {
            NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(filaInicio - 1, filaFin - 1, columnas.FindIndex(x => x.StartsWith(columnaInicio)), columnas.FindIndex(x => x.StartsWith(columnaFin)));
            sheet.AddMergedRegion(cra);
        }
        
        public static void setValorCelda(ISheet sheet, int fila, string columna, string valor, ICellStyle cellStyle = null, bool autoSizeColumn = false)
        {
            sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).SetCellValue(valor);
            if (cellStyle != null)
            { sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).CellStyle = cellStyle; }

            if (autoSizeColumn)
            {
                sheet.AutoSizeColumn(columnas.FindIndex(x => x.StartsWith(columna)));
            }
        }

        public static void setValorCelda(ISheet sheet, int fila, string columna, double valor, ICellStyle cellStyle = null)
        {
            sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).SetCellValue(valor);
            if (cellStyle != null)
            { sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).CellStyle = cellStyle; }
        }

        public static void setValorCelda(ISheet sheet, int fila, int columna, double valor)
        {
            sheet.GetRow(fila - 1).GetCell(columna -1).SetCellValue(valor);
        }

        public static void setValorCelda(ISheet sheet, int fila, string columna, int valor)
        {
            sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).SetCellValue(valor);
        }

        public static void setValorCelda(ISheet sheet, int fila, int columna, int valor)
        {
            sheet.GetRow(fila - 1).GetCell(columna-1).SetCellValue(valor);
        }


        public static void setValorCelda(ISheet sheet, int fila, string columna, DateTime valor, ICellStyle cellStyle)
        {
            ICell cell = sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna)));
            sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).SetCellValue(valor);
            sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).CellStyle = cellStyle;
        }

        public static void setValorCelda(ISheet sheet, int fila, int columna, DateTime valor)
        {
            sheet.GetRow(fila - 1).GetCell(columna-1).SetCellValue(valor);
        }

        
        public static int getValorCeldaInt(ISheet sheet, int fila, string columna)
        {
            String valorCelda = UtilesHelper.getValorCelda(sheet, fila, columna);
            int valorCeldaInt = 0;
            try
            {
                valorCeldaInt  = int.Parse(valorCelda);
            }
            catch (Exception)
            {
                valorCeldaInt = 0;
            }

            return valorCeldaInt;
        }
        
        public static void setColumnDefaultStyle(ISheet sheet, string columna, ICellStyle cellStyle)
        {
            sheet.SetDefaultColumnStyle(columnas.FindIndex(x => x.StartsWith(columna)), cellStyle);
        }

        public static DateTime? getValorCeldaDate(ISheet sheet, int fila, string columna)
        {
            DateTime? valorCelda = null;
            if (sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))) != null)
            {
                valorCelda = sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).DateCellValue;
            }

            return valorCelda;
        }

        public static decimal getValorCeldaDecimal(ISheet sheet, int fila, string columna)
        {
            String valorCelda = UtilesHelper.getValorCelda(sheet, fila, columna);
            decimal valorCeldaDecimal = 0;
            try
            {
                valorCeldaDecimal = decimal.Parse(valorCelda);
            }
            catch (Exception)
            {
                valorCeldaDecimal = 0;
            }

            return valorCeldaDecimal;
        }

        public static void setColumnWidth(ISheet sheet, string columna, int width = -1)
        {
            sheet.SetColumnWidth(columnas.FindIndex(x => x.StartsWith(columna)), width);
            if (width < 0)
            {
                sheet.AutoSizeColumn(columnas.FindIndex(x => x.StartsWith(columna)));
            }
        }

        public static void setRowHeight(ISheet sheet, int fila, int height)
        {
            sheet.GetRow(fila - 1).Height = (short) height;
        }

        public static ICellStyle GetCloneStyleWithIndent(IWorkbook wb, ICellStyle original, short indention)
        {
            ICellStyle temp = wb.CreateCellStyle();
            temp.CloneStyleFrom(original);
            temp.Indention = indention;

            return temp;
        }

        public static ICellStyle GetCloneStyleWithHCenter(IWorkbook wb, ICellStyle original)
        {
            ICellStyle temp = wb.CreateCellStyle();
            temp.CloneStyleFrom(original);
            temp.Alignment = HorizontalAlignment.Center;

            return temp;
        }

        public static ICellStyle GetCloneStyle(IWorkbook wb, ICellStyle original)
        {
            ICellStyle temp = wb.CreateCellStyle();
            temp.CloneStyleFrom(original);

            return temp;
        }
    }
}

