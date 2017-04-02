using System;
using System.Data;
using System.Data.SqlClient;

namespace Framework.DAL
{
    public abstract class DaoBase : IDisposable
    {
        private bool _commited;
        private readonly int _commandTimeout;
        readonly SqlConnection _connection;
        SqlTransaction _transaction;

        protected DaoBase(IDalSettings settings)
        {
            if (string.IsNullOrEmpty(settings.ConnectionString))
                throw new InvalidOperationException("No se puede inicializar el acceso a datos sin una cadena de conexión válida.");
            _connection = new SqlConnection(settings.ConnectionString);
            _connection.Open();
            _commandTimeout = settings.CommandTimeout;
        }

        public bool Commited
        {
            get { return _commited; }
        }

        public int CommandTimeout
        {
            get { return _commandTimeout; }
        }

        public SqlConnection Connection
        {
            get { return _connection; }
        }

        public SqlTransaction Transaction
        {
            get { return _transaction; }
        }

        public SqlCommand GetSqlCommand(string storProcName)
        {
            var objCommand = new SqlCommand(storProcName, _connection)
            {
                CommandTimeout = CommandTimeout,
                CommandType = CommandType.StoredProcedure
            };

            objCommand.Parameters.Add(Parameter.CreateParam("@RETURN_VALUE", SqlDbType.Int, 0, ParameterDirection.ReturnValue, 0));

            return objCommand;
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            _transaction = _connection.BeginTransaction(isolationLevel);
            _commited = false;
        }

        public void ExecuteNonQuery(SqlCommand pCommand)
        {
            try
            {
                if (_transaction != null)
                {
                    pCommand.Transaction = _transaction;
                }



                pCommand.ExecuteNonQuery();
            }
            finally
            {
            }
        }

        public string ExecuteXmlReader(SqlCommand pCommand)
        {
            string xml;

            try
            {
                var xmlReader = pCommand.ExecuteXmlReader();
                xmlReader.MoveToContent();
                xml = xmlReader.ReadOuterXml();
            }
            finally
            {
            }

            return xml;
        }

        public DataTable Execute(SqlCommand pCommand)
        {
            var dataTable = new DataTable();
            try
            {
                if (_transaction != null)
                {
                    pCommand.Transaction = _transaction;
                }
                var adapter = new SqlDataAdapter(pCommand);
                adapter.Fill(dataTable);
            }
            catch (Exception e)
            {
                
            }
            finally
            {
            }

            return dataTable;
        }


        public DataSet ExecuteDataSet(SqlCommand pCommand)
        {
            var dataSet = new DataSet();
            try
            {
                if (_transaction != null)
                {
                    pCommand.Transaction = _transaction;
                }
                var adapter = new SqlDataAdapter(pCommand);
                adapter.Fill(dataSet);
            }
            finally
            {
            }

            return dataSet;
        }



        public void Commit()
        {
            if (_commited)
            {
                throw new Exception("No ejecutar Commit múltiples veces");
            }
            if (_transaction != null)
            {
                _transaction.Commit();
            }
            _commited = true;
        }

        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _commited = true;
            }
        }

        public void Dispose()
        {
            if (!_commited)
            {
                Rollback();
            }
            if (_transaction != null)
            {
                _transaction.Dispose();
            }
            _connection.Dispose();
        }
    }
}
