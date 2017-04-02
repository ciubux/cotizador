using Framework.WebCommon;
using System;
using System.Configuration;

namespace Framework.DAL.Settings
{
    public abstract class DalSettingsBase : WebConfig, IDalSettings
    {
        private readonly string _connectionString;
        private readonly int _commandTimeoutInSeconds;
        private const int DefaultTimeOutInSeconds = 300;

        protected DalSettingsBase(string connectionStringAttributeName, string commandTimeoutAttributeName)
        {
            var section = (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");

            _connectionString = section.ConnectionStrings[connectionStringAttributeName].ConnectionString;
            try
            {
                _commandTimeoutInSeconds = GetSetting<int>(commandTimeoutAttributeName);
            }
            catch (Exception)
            {
                _commandTimeoutInSeconds = DefaultTimeOutInSeconds;
            }

        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public int CommandTimeout
        {
            get { return _commandTimeoutInSeconds; }
        }
    }
}
