using System;
using System.Data.SqlClient;

namespace Framework.DAL
{
    public static class OutputParameterGet
    {
        public static T Value<T>(SqlCommand objCommand, string resultcode)
        {
            var value = objCommand.Parameters["@" + resultcode].Value;

            if (value is DBNull)
                return default(T);

            return (T)value;
        }
    }
}
