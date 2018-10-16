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


        public static String getValorCelda(ISheet sheet, int fila, string columna)
        {
            List<String> columnas = new List<string>();
            columnas.Add("A");
            columnas.Add("B");
            columnas.Add("C");
            columnas.Add("D");
            columnas.Add("E");
            columnas.Add("F");
            columnas.Add("G");
            columnas.Add("H");
            columnas.Add("I");
            columnas.Add("J");
            columnas.Add("K");
            columnas.Add("L");
            columnas.Add("M");
            columnas.Add("N");
            columnas.Add("O");
            columnas.Add("P");
            columnas.Add("Q");
            columnas.Add("R");
            columnas.Add("S");
            columnas.Add("T");
            columnas.Add("U");
            columnas.Add("V");
            columnas.Add("W");
            columnas.Add("X");
            columnas.Add("Y");
            columnas.Add("Z");

            String valorCelda = sheet.GetRow(fila - 1).GetCell(columnas.FindIndex(x => x.StartsWith(columna))).ToString();
            return valorCelda;
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
