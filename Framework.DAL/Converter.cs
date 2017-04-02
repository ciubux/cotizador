using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Framework.DAL
{
    public class Converter
    {
        public static DateTime GetDateTime(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return new DateTime();

            DateTime returnValue;
            var value = row[fieldName].ToString();

            if (DateTime.TryParse(value, out returnValue))
                return returnValue;

            return new DateTime();
        }

        public static DateTime? GetDateTimeNullable(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return null;

            DateTime returnValue;
            var value = row[fieldName].ToString();

            if (DateTime.TryParse(value, out returnValue))
                return returnValue;

            return null;
        }

        public static DateTime? GetUtcDateTimeNullable(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return null;

            DateTime returnValue;
            var value = row[fieldName].ToString();

            if (DateTime.TryParse(value, out returnValue))
                return DateTime.SpecifyKind(returnValue, DateTimeKind.Utc);

            return null;
        }

        public static decimal GetDecimal(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return 0;

            decimal returnValue;
            var value = row[fieldName].ToString();

            if (decimal.TryParse(value, out returnValue))
                return returnValue;

            return 0;
        }

        public static decimal? GetDecimalNullable(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return 0;

            if (row[fieldName] == DBNull.Value)
                return null;

            decimal returnValue;
            var value = row[fieldName].ToString();

            if (decimal.TryParse(value, out returnValue))
                return returnValue;

            return 0;
        }

        public static bool GetBool(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return false;

            bool returnValue;

            if (row[fieldName] == DBNull.Value)
                return false;

            var value = row[fieldName].ToString();

            const string one = "1";

            if (value == one)
                return true;

            if (Boolean.TryParse(value, out returnValue))
                return returnValue;

            return false;
        }

        public static bool? GetBoolNullable(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return null;

            if (row[fieldName] == DBNull.Value)
                return null;

            bool returnValue;
            var value = row[fieldName].ToString();

            if (Boolean.TryParse(value, out returnValue))
                return returnValue;

            return false;
        }

        public static long GetLong(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return 0;

            long returnValue;
            var value = row[fieldName].ToString();

            if (long.TryParse(value, out returnValue))
                return returnValue;

            return 0;
        }

        public static short GetShort(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return 0;

            short returnValue;
            var value = row[fieldName].ToString();

            if (short.TryParse(value, out returnValue))
                return returnValue;

            return 0;
        }

        public static short? GetShortNullable(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return null;

            if (row[fieldName] == DBNull.Value)
                return null;

            short returnValue;

            var value = row[fieldName].ToString();

            if (short.TryParse(value, out returnValue))
                return returnValue;

            return 0;
        }

        public static double GetDouble(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return 0;

            double returnValue;
            var value = row[fieldName].ToString();

            if (double.TryParse(value, out returnValue))
                return returnValue;

            return 0;
        }

        public static string GetString(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return null;

            var value = row[fieldName];

            if (value != DBNull.Value)
                return value.ToString();

            return null;
        }

        public static int GetInt(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return 0;

            int returnValue;
            var value = row[fieldName].ToString();

            if (int.TryParse(value, out returnValue))
                return returnValue;

            return 0;
        }

        public static byte GetByte(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return 0;

            byte returnValue;
            var value = row[fieldName].ToString();

            if (byte.TryParse(value, out returnValue))
                return returnValue;

            return 0;
        }

        public static byte[] GetBytes(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return null;

            var value = row[fieldName];

            if (value != DBNull.Value)
                return (byte[])value;

            return null;
        }

        public static int? GetIntNullable(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return null;

            if (row.IsNull(fieldName))
                return null;

            int returnValue;
            string value = row[fieldName].ToString();

            if (int.TryParse(value, out returnValue))
                return returnValue;

            return null;
        }

        public static string RemoveNullChars(object o)
        {
            byte[] asciiChars = Encoding.ASCII.GetBytes(o.ToString()).Where(y => y != 0).ToArray();
            return Encoding.ASCII.GetString(asciiChars);
        }
        public static Guid GetGuid(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return new Guid();

            Guid returnValue;
            var value = row[fieldName].ToString();

            if (Guid.TryParse(value, out returnValue))
                return returnValue;

            return new Guid();
        }
        public static Guid? GetGuidNullable(DataRow row, string fieldName)
        {
            if (!row.Table.Columns.Contains(fieldName))
                return null;

            Guid returnValue;
            var value = row[fieldName].ToString();

            if (Guid.TryParse(value, out returnValue))
                return returnValue;

            return null;
        }

        public static List<string> GetListaCadenas(DataTable dataTable, string fieldName)
        {
            List<string> lista = new List<string>();

            if (dataTable.Rows.Count == 0)
                return null;

            int i = 0;
            int max = dataTable.Rows.Count;

            while (i < max)
            {
                var row = dataTable.Rows[i];

                string item = row[fieldName].ToString();

                lista.Add(item);

                i++;
            }

            return lista;
        }

        public static DateTime GetDateTimeFromDateAndTime(DataRow row, string fecha, string hora)
        {
            var date = GetDateTime(row, fecha);

            var timeString = GetString(row, hora);

            return timeString == null ? date : date.Add(TimeSpan.Parse(GetString(row, hora)));
        }
    }
}