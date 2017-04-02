using System;
using System.Configuration;

namespace Framework.WebCommon
{
    public static class ConfigSettings
    {
        public static string GetSetting(string settingName)
        {
            var setting = ConfigurationManager.AppSettings[settingName];

            if (string.IsNullOrEmpty(setting))
                throw new Exception(string.Format("{0} - no está presente en el archivo de configuración.", settingName));

            return setting;
        }
    }
}
