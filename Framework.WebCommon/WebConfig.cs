using System;
using System.Configuration;

namespace Framework.WebCommon
{
    public abstract class WebConfig
    {
        public static T GetSetting<T>(string settingName)
        {
            var setting = ConfigurationManager.AppSettings[settingName];

            if (string.IsNullOrEmpty(setting))
                throw new Exception($"{settingName} - no está presente en el archivo de configuración.");

            return (T)Convert.ChangeType(setting, typeof(T));
        }
    }
}
