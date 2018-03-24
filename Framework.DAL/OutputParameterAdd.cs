using System.Data;
using System.Data.SqlClient;

namespace Framework.DAL
{
    public static class OutputParameterAdd
    {
        public static void Int(SqlCommand objCommand, string name)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Int, 0, ParameterDirection.Output));
        }

        public static void UniqueIdentifier(SqlCommand objCommand, string name)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.UniqueIdentifier, 0, ParameterDirection.Output));
        }
        public static void VarBinary(SqlCommand objCommand, string name, int size)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.VarBinary, size, ParameterDirection.Output));
        }



        public static void Varchar(SqlCommand objCommand, string name, int size)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.VarChar, size, ParameterDirection.Output));
        }
        

        public static void Bit(SqlCommand objCommand, string name)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.Bit, 0, ParameterDirection.Output));
        }

        public static void DateTime(SqlCommand objCommand, string name)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.DateTime, 0, ParameterDirection.Output));
        }

        public static void BigInt(SqlCommand objCommand, string name)
        {
            objCommand.Parameters.Add(Parameter.CreateParam("@" + name, SqlDbType.BigInt, 0, ParameterDirection.Output));
        }
    }
}
