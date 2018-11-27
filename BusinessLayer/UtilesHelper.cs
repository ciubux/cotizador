using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
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
        "F","G","H","I","J","K","L","M","N","O","P","R","S","T","U","V","W","X","Y","Z"};

        public static String getValorCelda(ISheet sheet, int fila, string columna)
        {
            String valorCelda = String.Empty;
            if (sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))) != null)
            {
                valorCelda = sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).ToString();
            }
            
            return valorCelda;
        }

        public static void setValorCelda(ISheet sheet, int fila, string columna, string valor)
        {
            sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).SetCellValue(valor);
        }

        public static void setValorCelda(ISheet sheet, int fila, string columna, double valor)
        {
            sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).SetCellValue(valor);
        }

        public static void setValorCelda(ISheet sheet, int fila, string columna, int valor)
        {
            sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).SetCellValue(valor);
        }

        public static void setValorCelda(ISheet sheet, int fila, string columna, DateTime valor)
        {
            sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).SetCellValue(valor);
        }

        public static void setValorCelda(ISheet sheet, int fila, int columna, string valor, HSSFCellStyle cellStyle = null)
        {
            sheet.GetRow(fila - 1).GetCell(columna - 1).SetCellValue(valor);
            if (cellStyle != null)
            { sheet.GetRow(fila - 1).GetCell(columna - 1).CellStyle = cellStyle; }
        }

        public static void setValorCelda(ISheet sheet, int fila, string columna, string valor, HSSFCellStyle cellStyle = null)
        {
            sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).SetCellValue(valor);
            if (cellStyle != null)
            { sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).CellStyle = cellStyle; }
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

    }
}

