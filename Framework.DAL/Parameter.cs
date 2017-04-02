using System;
using System.Data;
using System.Data.SqlClient;

namespace Framework.DAL
{
    public static class Parameter
    {
        public static SqlParameter CreateParam(string name, SqlDbType type, Int32 size, ParameterDirection direction, object value)
        {
            return new SqlParameter(name, type, size) { Direction = direction, Value = value };
        }

        public static SqlParameter CreateParam(string name, SqlDbType type, Int32 size, ParameterDirection direction)
        {
            return new SqlParameter(name, type, size) { Direction = direction };
        }
    }
}
