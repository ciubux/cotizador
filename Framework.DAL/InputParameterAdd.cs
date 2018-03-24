using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Framework.DAL
{
    public static class InputParameterAdd
    {
        public static void Varchar(SqlCommand objCommand, string name, int size, string value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.VarChar, size, ParameterDirection.Input, new SqlString(value)));
        }

        public static void TinyInt(SqlCommand objCommand, string name, byte value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.TinyInt, 0, ParameterDirection.Input, new SqlByte(value)));
        }

        public static void TinyInt(SqlCommand objCommand, string name, byte? value)
        {
            if (value.HasValue)
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.TinyInt, 0, ParameterDirection.Input, new SqlByte(value.Value)));
            else
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.TinyInt, 0, ParameterDirection.Input, DBNull.Value));
            }
        }

        public static void Bit(SqlCommand objCommand, string name, bool value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Bit, 0, ParameterDirection.Input, new SqlBoolean(value)));
        }
        public static void Bit(SqlCommand objCommand, string name, bool? value)
        {
            if (value.HasValue)
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Bit, 0, ParameterDirection.Input, new SqlBoolean(value.Value)));
            else
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Bit, 0, ParameterDirection.Input, DBNull.Value));
            }
        }

        public static void Int(SqlCommand objCommand, string name, int value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Int, 0, ParameterDirection.Input, new SqlInt32(value)));
        }

        public static void BigInt(SqlCommand objCommand, string name, long value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.BigInt, 0, ParameterDirection.Input, new SqlInt64(value)));
        }

        public static void DateTime(SqlCommand objCommand, string name, DateTime value)
        {
            if (value != null)
                if (value.Year != 1)
                    objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.DateTime, 0, ParameterDirection.Input, new SqlDateTime(value)));
                else
                    objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.DateTime, 0, ParameterDirection.Input, DBNull.Value));
            else
            {
                return;
            }

            return;
        }

        public static void Date(SqlCommand objCommand, string name, DateTime value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Date, 0, ParameterDirection.Input, new SqlDateTime(value.Year, value.Month, value.Day)));
        }

        public static void SmallDateTime(SqlCommand objCommand, string name, DateTime value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.SmallDateTime, 0, ParameterDirection.Input, new SqlDateTime(value)));
        }

        public static void Time(SqlCommand objCommand, string name, DateTime value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Time, 0, ParameterDirection.Input, new SqlDateTime(value)));
        }

        public static void SmallInt(SqlCommand objCommand, string name, short value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.SmallInt, 0, ParameterDirection.Input, new SqlInt16(value)));
        }

        public static void SmallInt(SqlCommand objCommand, string name, short? value)
        {
            if (value.HasValue)
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.SmallInt, 0, ParameterDirection.Input, new SqlInt16(value.Value)));
            else
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.SmallInt, 0, ParameterDirection.Input, DBNull.Value));
            }
        }

        public static void Decimal(SqlCommand objCommand, string name, decimal value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Decimal, 0, ParameterDirection.Input, new SqlDecimal(value)));
        }

        public static void Decimal(SqlCommand objCommand, string name, decimal? value)
        {
            if (value != null && value.HasValue)
            {
                Decimal(objCommand, name, value.Value);
            }
            else
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Decimal, 0, ParameterDirection.Input, DBNull.Value));
            }
        }

        public static void DateTime(SqlCommand objCommand, string name, DateTime? value)
        {
            if (value.HasValue)
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.DateTime, 0, ParameterDirection.Input, new SqlDateTime(value.Value)));
            else
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.DateTime, 0, ParameterDirection.Input, DBNull.Value));
        }

        public static void SmallDateTime(SqlCommand objCommand, string name, DateTime? value)
        {
            if (value.HasValue)
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.SmallDateTime, 0, ParameterDirection.Input, new SqlDateTime(value.Value)));
            else
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.SmallDateTime, 0, ParameterDirection.Input, DBNull.Value));
        }

        public static void BigInt(SqlCommand objCommand, string name, long? value)
        {
            if (value.HasValue)
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.BigInt, 0, ParameterDirection.Input, new SqlInt64(value.Value)));
            else
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.BigInt, 0, ParameterDirection.Input, DBNull.Value));
            }
        }

        public static void Int(SqlCommand objCommand, string name, int? value)
        {

            if (value.HasValue)
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Int, 0, ParameterDirection.Input, new SqlInt32(value.Value)));
            else
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Int, 0, ParameterDirection.Input, DBNull.Value));
            }
        }

        public static void Varchar(SqlCommand objCommand, string name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.VarChar, 0, ParameterDirection.Input, DBNull.Value));
            }
            else
            {
            //    value = value.ToUpper();
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.VarChar, value.Length, ParameterDirection.Input, new SqlString(value)));
            }
        }

        public static void Char(SqlCommand objCommand, string name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Char, 0, ParameterDirection.Input, DBNull.Value));
            }
            else
            {
         //       value = value.ToUpper();
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Char, value.Length, ParameterDirection.Input, new SqlString(value)));
            }
        }

        public static void Money(SqlCommand objCommand, string name, decimal value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Money, 0, ParameterDirection.Input, new SqlMoney(value)));
        }

        public static void Money(SqlCommand objCommand, string name, decimal? value)
        {
            if (!value.HasValue)
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Money, 0, ParameterDirection.Input, DBNull.Value));
            }
            else
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Money, 0, ParameterDirection.Input, new SqlMoney(value.Value)));
            }
        }

        public static void Guid(SqlCommand objCommand, string name, Guid value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam(name, SqlDbType.UniqueIdentifier, 0, ParameterDirection.Input, new SqlGuid(value)));
        }

        public static void Guid(SqlCommand objCommand, string name, Guid? value)
        {
            if (value != null && value.HasValue)
            {
                Guid(objCommand, name, value.Value);
            }
            else
            {
                objCommand.Parameters.Add(Parameter.CreateParam(name, SqlDbType.UniqueIdentifier, 0, ParameterDirection.Input, DBNull.Value));
            }
        }

        public static void Text(SqlCommand objCommand, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Text, value.Length, ParameterDirection.Input, new SqlString(value)));
            else
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Text, value.Length, ParameterDirection.Input, DBNull.Value));

        }

        public static void DateTime(SqlCommand objCommand, string name, DateTime? value, bool checkMinDate)
        {

            if (value.HasValue)
            {
                var updatedValue = checkMinDate ? value.Value == System.DateTime.MinValue
                       ? SqlDateTime.Null
                       : new SqlDateTime(value.Value) : new SqlDateTime(value.Value);


                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.DateTime, 0, ParameterDirection.Input, updatedValue));
            }

            else
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.DateTime, 0, ParameterDirection.Input, DBNull.Value));
        }

        public static void VarcharEmpty(SqlCommand objCommand, string name, string value)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.VarChar, value.Length, ParameterDirection.Input, new SqlString(value)));
        }

        public static void Binary(SqlCommand objCommand, string name, byte[] value)
        {
            if (value != null)
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Binary, value.Length, ParameterDirection.Input, new SqlBinary(value)));
            }
            else
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Binary, 0, ParameterDirection.Input, DBNull.Value));
            }
        }

        public static void VarBinary(SqlCommand objCommand, string name, byte[] value)
        {
            if (value != null)
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.VarBinary, value.Length, ParameterDirection.Input, new SqlBinary(value)));
            }
            else
            {
                objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.VarBinary, 0, ParameterDirection.Input, DBNull.Value));
            }
        }

    }
}
